using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using Leap.Unity.Infix;
using UnityEngine;
using UnityEngine.AI;
using VirtualSelf.Utility;
using TransformUtil = VirtualSelf.Utility.TransformUtil;

namespace VirtualSelf {

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody), typeof(NavMeshObstacle))]
[RequireComponent(typeof(Collider))]
public class BaseAi : MonoBehaviour {

    /// <summary>
    /// Lifecycle description: <para></para>
    /// The AI can switch multiple times between Navigating and Ragdolled. <para></para>
    /// Once the DyingFall state is entered, there is no coming back; the AI will ragdoll until it collides
    /// with something and stops moving. <para></para>
    /// Once it stopped moving, it will enter DyingDecay and animate into the ground, and then be destroyed.<para></para>
    /// Staggered: Similar alternative to Ragdolled, except the AI stays active and walks back for a moment.<para></para>
    /// Lost: Error state when the AI fell too far away from any NavMesh after being ragdolled.<para></para>
    /// Idle: Nothing is active, intended to use for animating in the AI, though leaving this state has to be done externally.
    /// </summary>
    public enum AiState
    {
        Navigating, Ragdolled, Idle, Staggered, Lost, DyingFall, DyingDecay, PreNavigating
    }

    public enum DamageEffect
    {
        None, Ragdoll, Stagger
    }

    public enum LostBehavior
    {
        CannonballToMesh, Idle, Die
    }
    
    private const float InitialDespawnVelocity = 0.02f;
    private const float InitialDespawnAngularVelocity = 0.7f;

    [Tooltip("Only Navigating and Ragdolled are valid initial states")]
    public AiState State;
    public LostBehavior WhenLost;
    public bool LogTransitions = true;
    public float Health = 10;
    public float StaggerDuration = .3f;
    
    public Action OnDeath;
    public Action OnDyingFall;
    public Action OnDyingDecay;
    public Action OnLostIdle;
    public Action OnDamage;
    public Action OnArrive;
    public Action OnIdleExit;
    
    protected NavMeshAgent _navMeshAgent;
    protected Rigidbody _rigidbody;
    protected NavMeshObstacle _navMeshObstacle;
    protected Collider _collider;
    protected bool _postStart;
    protected LaunchRigidbody _launchRigidbody;

    private float _maxHealth;
    private AiState _builtState;
    private float _lastTransition;
    private float _despawnVelocity = InitialDespawnVelocity;
    private float _despawnAngularVelocity = InitialDespawnAngularVelocity;
    private float _minRagdollRecoveryTime = 1;
    private float _maxRagdollRecoveryTime = 3;
    private NavMeshHit _hit;
    private Vector3 _lastPosition;
    private Vector3 _velocity;
    private Vector3 _preStaggerDestination;
    private Vector3 _staggerDestination;
    private bool _postFirstTransition;
    private Vector3 _lastArrivePos;

    protected void Awake()
    {
        GetComponents();
    }

    protected void Start()
    {
        _maxHealth = Health;
        _postStart = true;
        SetState(State);
    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        if (!_postStart)
        {
            GetComponents();
            if (State != AiState.Navigating && State != AiState.Ragdolled && State != AiState.Idle) State = AiState.Navigating;
            if (_builtState != State) SetState(State);
            
            if (WhenLost == LostBehavior.CannonballToMesh)
            {
                if (_launchRigidbody == null)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        _launchRigidbody = gameObject.AddComponent<LaunchRigidbody>();
                    };
                }
            }
            else
            {
                if (_launchRigidbody != null)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(_launchRigidbody);
                    };
                }
            }
        }
        else
        {
            if (_builtState != State)
            {
                SetState(State);
            }
        }
    }
#endif

    void Reset()
    {
        GetComponents();
        SetState(State);
    }

    protected void OnCollisionStay(Collision other)
    {
        if (_builtState == AiState.DyingFall)
        {
//            Debug.Log(_rigidbody.velocity.sqrMagnitude + " / " + _despawnVelocity);
//            Debug.Log(_rigidbody.angularVelocity.sqrMagnitude+" / "+_despawnAngularVelocity);
            _despawnVelocity *= 1.01f;
            _despawnAngularVelocity *= 1.01f;
            if (_rigidbody.velocity.sqrMagnitude < _despawnVelocity && 
                _rigidbody.angularVelocity.sqrMagnitude < _despawnAngularVelocity)
            {
                StartCoroutine(FadeIntoGround());
            }
        }
        else 
            CheckRagdollExit(); 
    }

    private void CheckRagdollExit()
    {
        if (_builtState == AiState.Ragdolled)
        {
            _despawnVelocity *= 1.01f;
            _despawnAngularVelocity *= 1.01f;

            // exit ragdoll when we slowed down enough or enough time passed
            if (_rigidbody.velocity.sqrMagnitude < _despawnVelocity &&
                _rigidbody.angularVelocity.sqrMagnitude < _despawnAngularVelocity &&
                Time.time > _lastTransition + _minRagdollRecoveryTime
            )
            {
                ExitRagdoll();
            }
            else if (Time.time > _lastTransition + _maxRagdollRecoveryTime)
            {
                ExitRagdoll();
            }
        }
    }

    private void ExitRagdoll()
    {
        // if on mesh: return to navigating and done
        // otherwise:
        // cannonball: push rigidbody back to nearest mesh location
        // idle: disable navmeshagent
        // die: go into dyingdecay
        Debug.Log("exit ragdoll");
        
        if (!NavMesh.SamplePosition(_rigidbody.position, out _hit, 12f, NavMesh.AllAreas))
        {
            Death();
        }

        SetState(_hit.distance < .1f ? AiState.PreNavigating : AiState.Lost);
    }
    
//    protected void OnCollisionEnter(Collision other)
//    {
//        
//    }

    protected void FixedUpdate()
    {
        if (_rigidbody.velocity.y < -50) // probably falling indefinitely 
        {
            Death();
        }
        StopRigidbody();
        CheckRagdollExit();
    }

    private void StopRigidbody()
    {
        if (State == AiState.Navigating ||
            State == AiState.PreNavigating ||
            State == AiState.Staggered)
        {
            _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    protected void Update() {
//        StopRigidbody();
        
        // calculate a "velocity" purely for a backwards direction for staggering
        // mid-staggering values do not reflect the actual navigation direction
        if (_builtState == AiState.Staggered) {
            if (Time.time > _lastTransition + StaggerDuration) {
                Vector3 dest = _navMeshAgent.destination;
                dest.y = 0;
                _staggerDestination.y = 0;

                // set back the pre stagger destination, unless externally some newer one was given already
                // we check this by comparing if the current destination is still the stagger destination
                // the y value need not be compared (and is also different, i.e. corrected in some form to be on the mesh)
                if (dest.ApproxEquals(_staggerDestination))
                    _navMeshAgent.SetDestination(_preStaggerDestination);
                SetState(AiState.Navigating);
            }
        }
//        else if (_builtState == AiState.PreNavigating) {
//            if (Time.time > _lastTransition + 2f) {
//                SetState(AiState.Navigating);
//            }
//        } 
        else
        {
            Vector3 p = transform.position;
            _velocity = p - _lastPosition;
            _lastPosition = p;

            if (_navMeshAgent.enabled &&
                _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance &&
                !_navMeshAgent.hasPath &&
                _lastArrivePos != _navMeshAgent.nextPosition)
            {
                Debug.Log($"{name} arrived");
                _lastArrivePos = _navMeshAgent.nextPosition;
                OnArrive?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="effect"></param>
    /// <returns>true if health went to 0</returns>
    public bool TakeDamage(float amount, DamageEffect effect = DamageEffect.None)
    {
        Health = Mathf.Min(Health - amount, _maxHealth);
        if (Health <= 0)
        {
            Health = 0;
            SetState(AiState.DyingFall);
            OnDamage?.Invoke();
            return true;
        }

        if (effect == DamageEffect.Ragdoll) SetState(AiState.Ragdolled);
        else if (effect == DamageEffect.Stagger) SetState(AiState.Staggered);
        
        OnDamage?.Invoke();
        return false;
    }

    public void SetState(AiState state)
    {
        _launchRigidbody.Cancel();
        if (_builtState == AiState.DyingFall || _builtState == AiState.DyingDecay) return;
        
        if (state == AiState.Navigating)
        {
            // enable ai
            _rigidbody.useGravity = false;
            _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = false;
            _collider.enabled = true;
            _navMeshObstacle.enabled = false;
            _navMeshAgent.enabled = true;
            if (_builtState == AiState.Idle) OnIdleExit?.Invoke();
        }
        else if (state == AiState.PreNavigating) {
            _rigidbody.isKinematic = true;
            _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
            _navMeshAgent.enabled = false;
            _rigidbody.useGravity = false;
            const int ticks = 60;
            StartCoroutine(RotateUpwards(ticks));
            StartCoroutine(PositionAboveMeshpoint(ticks));
            _collider.enabled = false;
        }
        else if (state == AiState.Ragdolled || state == AiState.DyingFall)
        {
            // enable physics
            _navMeshAgent.enabled = false;
            _navMeshObstacle.enabled = true;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            
            _despawnVelocity = InitialDespawnVelocity;
            _despawnAngularVelocity = InitialDespawnAngularVelocity;
            if (state == AiState.DyingFall) OnDyingFall?.Invoke();
        }
        else if (state == AiState.Idle)
        {
            // disable everything
            _navMeshObstacle.enabled = false;
            _navMeshAgent.enabled = false;
            _rigidbody.isKinematic = true;
        }
        else if (state == AiState.Lost)
        {
            if (WhenLost == LostBehavior.Idle)
            {
                // disable everything
                _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
                _navMeshAgent.enabled = false;
                _navMeshObstacle.enabled = false;
                _rigidbody.isKinematic = true;
                OnLostIdle?.Invoke();
            } 
            else if (WhenLost == LostBehavior.Die)
            {
                StartCoroutine(FadeIntoGround());
            } 
            else if (WhenLost == LostBehavior.CannonballToMesh)
            {
                Invoke(nameof(InvokableFireToMesh), .5f);
            } 
            else throw new NotImplementedException(WhenLost.ToString()); 
        }
        else if (state == AiState.Staggered)
        {
            _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
            
            // save the current ai destination to restore later, send ai backwards a bit
            // careful: if we stack staggers, the read destination value is not the original one we want, but a 
            // temporary one from the currently active stagger still, so do not save in stagger
            if (_builtState != AiState.Staggered) _preStaggerDestination = _navMeshAgent.destination;
            
            Vector3 rawStaggerDest = transform.position + (-_velocity.normalized * 2f);
            NavMeshHit hit;
            NavMesh.SamplePosition(rawStaggerDest, out hit, 2, NavMesh.AllAreas);
            _staggerDestination = hit.position;
            _navMeshAgent.SetDestination(_staggerDestination);
            
            // from here on: we could check if we arrived at the target, or also just go by time
            // until we reset the previous destination again (unless the destination has become different
            // from the stagger target (externally), in which case we do nothing)
        }
        else throw new NotImplementedException(state.ToString());
        
        if (LogTransitions && _postFirstTransition) Debug.Log($"{name}: {_builtState} -> {state} {Time.time-_lastTransition}");
        if (_postStart) _postFirstTransition = true;

        _builtState = state;
        State = state;
        _lastTransition = Time.time;
    }

    private Coroutine _rotateUpwards;
    private void InvokableFireToMesh()
    {
        // rotate the rigidbody upwards
        if (_rotateUpwards != null) StopCoroutine(_rotateUpwards);
        _rotateUpwards = StartCoroutine(RotateUpwards(100));
        
        // cannonball the rigidbody towards the closest navmesh point
        float assumedHeight = _collider.bounds.size.CompMax();
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (assumedHeight == 0) throw new ArgumentException($"assumed height, collider disabled? {_collider.enabled}");
        _launchRigidbody.FireTo(_hit.position, () => {
                SetState(AiState.PreNavigating); // arrived, back to navigating
        }, assumedHeight/2f);
    }
    
    protected IEnumerator RotateUpwards(int physicsTicks)
    {
        Quaternion start = _rigidbody.rotation;
        Quaternion end = start.UprightRotation();

        for (float i = 0; i <= 1; i+=1f/physicsTicks)
        {
            _rigidbody.rotation = Quaternion.Slerp(start, end, i);
            yield return new WaitForFixedUpdate();
        }
    }
    
    protected IEnumerator PositionAboveMeshpoint(int physicsTicks)
    {
        Vector3 start = _rigidbody.position;
        Vector3 end = _hit.position;
        float y = _collider.bounds.extents.CompMax();
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (y == 0) throw new ArgumentException($"got no bounds, collider enabled? {_collider.enabled}");
        end.y += y;
        
        for (float i = 0; i <= 1; i+=1f/physicsTicks)
        {
            _rigidbody.position = Vector3.Slerp(start, end, i);
            yield return new WaitForFixedUpdate();
        }
        
        SetState(AiState.Navigating);
    }

    protected void Death()
    {
        Destroy(gameObject);
        OnDeath?.Invoke();
    }

    protected IEnumerator FadeIntoGround()
    {
        _builtState = AiState.DyingDecay;
        OnDyingDecay?.Invoke();
        float assumedHeight = _collider.bounds.size.CompMax();
        float end = _rigidbody.position.y - assumedHeight;
        _collider.enabled = false;
//        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _rigidbody.angularVelocity /= 2;
        _rigidbody.velocity /= 2;
        
        for (float y = _rigidbody.position.y; y > end; y -= 5e-3f)
        {
            Vector3 p = _rigidbody.position;
            p.y = y;
            _rigidbody.position = p;
            yield return new WaitForFixedUpdate();
        }

        Death();
        yield return null;
    }
    
    private void GetComponents()
    {
        _navMeshAgent =    GetComponent<NavMeshAgent>();
        _rigidbody =       GetComponent<Rigidbody>();
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _collider =        GetComponent<Collider>();
        _launchRigidbody = GetComponent<LaunchRigidbody>();
    }
}

}
