using System;
using Leap.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VirtualSelf
{

[RequireComponent(typeof(Rigidbody))]
public class LaunchRigidbody : MonoBehaviour {
    
    private Rigidbody _rb;
    private float _lastTargetDistance;
    private bool _isClosingIn;
    private Vector3 _target;
	private bool _isActive;
	private float _acceptableDistance;
	private Action _onArrival;
	private int _lowVelocityFires;
	private float _startTime;
	private float _timeoutTime;

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
	{
		if (_isActive)
		{
			float newdist = Vector3.Distance(_rb.position, _target);
			_isClosingIn = newdist < _lastTargetDistance && 
			               Mathf.Abs(_rb.velocity.y) > 1e-3; // do not accept very slow rolling along the ground  
			_lastTargetDistance = newdist;

			Debug.Log("distance: "+newdist+"/"+_acceptableDistance);//TODO remove
			if (newdist <= _acceptableDistance)
			{
				_isActive = false;
				_onArrival.Invoke();
			}
			else
			{
				TryFire();
			}
		}
	}

	private void TryFire()
	{
//		Debug.Log($"TryFire {(_rb.velocity.y > 1e-3)} {_isClosingIn}");
		if (_rb.velocity.y > 1e-3) return;
		if (_isClosingIn) return;
		_isClosingIn = true;
		
		Fire();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="target"></param>
	/// <param name="onArrival">this is not guaranteed to be run</param>
	/// <param name="acceptableDistance"></param>
	/// <param name="timeout"></param>
	/// <returns>true if this instance was still trying to reach a previous target</returns>
	public bool FireTo(Vector3 target, Action onArrival, float acceptableDistance = 0.17f, float timeout = 30)
	{
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (acceptableDistance == 0) Debug.LogWarning(name+": did you really mean to assign acceptable distance of 0? This will probably never solve");
		
		bool alreadyActive = _isActive;
		_target = target;
		_lowVelocityFires = 0;
		_startTime = Time.time;
		_timeoutTime = _startTime + timeout;
		_acceptableDistance = acceptableDistance;
		_onArrival = onArrival;
		_isActive = true;
		return alreadyActive;
	}

	public void SingleFire(Vector3 target)
	{
		_target = target;
		_lowVelocityFires = 0;
		_startTime = Time.time;
		Fire();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns>true if this instance was active</returns>
	public bool Cancel()
	{
		bool alreadyActive = _isActive;
		_isActive = false;
		return alreadyActive;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="newtarget"></param>
	/// <returns>true if this instance was active, if not this call has no effect</returns>
	public bool UpdateTarget(Vector3 newtarget)
	{
		_target = newtarget;
		_lowVelocityFires = 0;
		_startTime = Time.time;
		return  _isActive;
	}
	
    private void Fire()
    {
        float gravity = Physics.gravity.magnitude;
 
        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(_target.x, 0, _target.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);
 
        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = transform.position.y - _target.y;

        float angle = ArcAngle(Mathf.Atan(-yOffset / distance) * Mathf.Rad2Deg);
        angle = Mathf.Clamp(angle, .01f, 89.9f) * Mathf.Deg2Rad;
 
        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
 
        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
	    if (velocity.sqrMagnitude < 1e-3)
	    {
		    _lowVelocityFires++;
		    if (_lowVelocityFires > 10)
		    {
			    if (Time.time > _timeoutTime)
			    {
				    Debug.LogWarning($"{name}: timed out FireTo, triggering finish action anyways");
				    _onArrival.Invoke();
				    return;
			    }
				_rb.AddForce(new Vector3(Random.value, 0, Random.value).normalized * _rb.mass * 1, ForceMode.Impulse);
			    _lowVelocityFires--;
			    return;
		    }
//			Debug.Log($"{name}: ideal trajectory unlikely: angle: {angle * Mathf.Rad2Deg}, velocity: {velocity.ToString("F4")}, sqrm:{velocity.sqrMagnitude}, using random force");
//	        _rb.AddForce(new Vector3(Random.value, 0, Random.value).normalized * _rb.mass * 15, ForceMode.Impulse);
//		    return;
	    }
	    else
	    {
		    _lowVelocityFires = Mathf.Max(0, _lowVelocityFires - 1);
	    }
 
        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        angleBetweenObjects *= (_target.x > transform.position.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
 
        if (finalVelocity.ContainsNaN()) 
        {
	        Debug.LogWarning($"{name}: could not solve trajectory: {angle * Mathf.Rad2Deg}, using random force");
	        _rb.AddForce(new Vector3(Random.value, 0, Random.value).normalized * _rb.mass * 15, ForceMode.Impulse);
	        return;
		}
	    
	    Debug.Log($"{name}: angle: {angle * Mathf.Rad2Deg} vel: {finalVelocity}");
		
        _rb.AddForce(finalVelocity * _rb.mass - _rb.velocity, ForceMode.Impulse);
    }
	
	private float ArcAngle(float x)
	{
		return .45f * x + 50;
	}	
}

}
