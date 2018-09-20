using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using VirtualSelf;

namespace VirtualSelf
{
	
public class EnemyCollider : MonoBehaviour
{

	public enum EnemyState
	{
		PathToIntermediate, // initial state 
		PathToPlayer, // chasing player
		RagdollPhysics // AI is paused for a moment
	}

	private EnemyState State
	{
		get { return _state; }
		set { 
			_state = value;
			lastStateTransition = Time.time;
		}
	}
	public int Health = 10;
	public float CloseToPlayerDistance = 3;
	
	[SerializeField] private EnemyState _state;
	private float lastStateTransition;
	private LocationProvider locationProvider;
	private Spawner spawner;
	private NavMeshAgent a;
	private Rigidbody r;
	private NavMeshObstacle o;

//	private bool wasEverRigidbody;
	
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init(Spawner spawner, LocationProvider locationProvider)
	{
		this.spawner = spawner;
		this.locationProvider = locationProvider;
		a = GetComponent<NavMeshAgent>();
		r = GetComponent<Rigidbody>();
		o = GetComponent<NavMeshObstacle>();
		
		State = EnemyState.PathToIntermediate;
		a.SetDestination(locationProvider.RandomTargetPos());
		r.useGravity = false;
//		r.isKinematic = true;
	}

	public void UpdateAi()
	{
		// probably fell off map
		if (State != EnemyState.RagdollPhysics && !a.isOnNavMesh)
		{
			if (Time.time > lastStateTransition + 10)
			{
				Debug.LogWarning(name+" removed due to not being on mesh");
				spawner.RemoveAgent(gameObject, false);
				Destroy(gameObject);
			}
			return;
		}
		
		if (a.pathPending)
		{
//			Debug.Log(name+" path pending");
			return;
		}
		
		// TODO bugs
		// desync due to rigidbody/agent swap -> more testing required

		if (State == EnemyState.RagdollPhysics)
		{
			// TODO health system
			// iframes while being ragdolled? 1s or until active again? signal with color change?
			// support variable damage amounts? bullet types or randomness?
			if (Time.time > lastStateTransition + 1) 
			{
				//Debug.Log(name+" velocity "+r.velocity.magnitude+" ang "+r.angularVelocity.magnitude);
				
				if (Time.time > lastStateTransition + 2.5
				    || (r.velocity.magnitude < 0.7 && r.angularVelocity.magnitude < 1))
				{
					r.velocity = Vector3.zero;
					r.angularVelocity = Vector3.zero;
//					Debug.Log(name+" is switching from ragdoll to navmeshagent");
					State = EnemyState.PathToPlayer;
//					a.nextPosition = r.position;

//					r.position += Vector3.up * .5f;
//					r.position = new Vector3(r.position.x, 0.5393f, r.position.z);
//					r.rotation = Quaternion.identity;
					
//					a.updatePosition = true;
//					a.updateRotation = true;
//					a.updateUpAxis = true;
					r.useGravity = false;
					
					o.enabled = false;
					a.enabled = true;
//					a.nextPosition = r.position;

					a.SetDestination(locationProvider.PlayerPos());
				}
			}
		}
		else if (State == EnemyState.PathToIntermediate)
		{
//			bool arrived = a.remainingDistance <= a.stoppingDistance && !a.hasPath && a.velocity.sqrMagnitude < 1e-7;
			bool arrived = a.remainingDistance <= a.stoppingDistance && a.velocity.sqrMagnitude < 1e-7;
			
			if (arrived)
			{
				a.SetDestination(locationProvider.PlayerPos());
				State = EnemyState.PathToPlayer;
			}
			else if (Vector3.Distance(locationProvider.PlayerPos(), transform.position) < CloseToPlayerDistance)
			{
				Debug.Log(name+" is close to player, canceled intermediate target");
				a.SetDestination(locationProvider.PlayerPos());
				State = EnemyState.PathToPlayer;
			}
		}		
		else if (State == EnemyState.PathToPlayer)
		{
			bool arrived = a.remainingDistance <= a.stoppingDistance && a.velocity.sqrMagnitude < 1e-3;

			if (arrived)
			{
				if (Vector3.Distance(locationProvider.PlayerPos(), transform.position) < CloseToPlayerDistance)
				{
					PrepareRemove(true); // arrived at player => TODO deal damage
				}
				else
				{
					PrepareRemove(false); // no more path, but player not in range => give up
				}
			}
			else
			{
//				Debug.Log(name+" remaining distance "+a.remainingDistance+", dist to player "+Vector3.Distance(locationProvider.PlayerPos(), transform.position)+" path "+a.hasPath+" vel "+a.velocity.sqrMagnitude);
				a.SetDestination(locationProvider.PlayerPos());
			}
		}
	}

	void PrepareRemove(bool violent)
	{
		GetComponent<Renderer>().material.color = violent ? Color.red : Color.blue;
		StartCoroutine(FadeDown());
		spawner.RemoveAgent(gameObject, false);
	}
	

	void OnCollisionEnter(Collision other)
	{

		if (other.gameObject.CompareTag("Projectile"))
		{
			Debug.Log(name+" collision with "+other.gameObject.name);
			//Spawner.RemoveAgent(gameObject, true);
//			spawner.PauseAgent(gameObject);
//			a.updatePosition = false;
//			a.updateRotation = false;
//			a.updateUpAxis = false;

//			r.position = a.nextPosition;
//			r.isKinematic = false;
			a.enabled = false;
			o.enabled = true;
			State = EnemyState.RagdollPhysics;
			r.useGravity = true;
//			Debug.Log(name+" hit");
		}
		else
		{
			Debug.Log(name+" collision with "+other.gameObject.name);
//			a.nextPosition = r.position;
		}
	}
	
	IEnumerator FadeDown()
	{
		a.isStopped = true;
		GetComponent<Collider>().enabled = false;
		yield return new WaitForSeconds(.5f);
		for (float y=transform.position.y; y > -transform.localScale.y; y -= .006f)
		{
			transform.position = new Vector3(transform.position.x, y, transform.position.z);
			yield return null;
		}

		Destroy(gameObject);
	}
	
}

}
