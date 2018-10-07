using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{

[RequireComponent(typeof(Rigidbody))]
public class ProjectileFire : MonoBehaviour {

	public Transform target;
//	[Range(0,90)]
//	public float initialAngle;
	[Range(.1f,1.5f)]
	public float a = 0.45f;
	[Range(0,89)]
	public float b = 50;
	
	[Range(0,1)]
	public float KeepVelocity = .5f;

	public KeyCode ResetKey = KeyCode.J;
	public KeyCode FireKey = KeyCode.K;
	public KeyCode SmartFireKey = KeyCode.L;

	private Vector3 _initialPos;
	private Quaternion _initialRot;
	private Rigidbody _rb;
	private float _lastTargetDistance;
	private bool _isClosingIn;
//	private float _closest = float.MaxValue;
//	private float _lastClosestUpdate;
 
	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
		_initialPos = _rb.position;
		_initialRot = _rb.rotation;
	}

	/// <summary>
	/// Most of the code is pieced together from here:
	/// <see cref="https://forum.unity.com/threads/how-to-calculate-force-needed-to-jump-towards-target-point.372288/"></see>.
	/// <para></para>
	/// Modifications were to automatically calculate the initial angle, configurable by a linear function f(x) = ax+b.
	/// This should be such that when the target is very high above, the angle should be pretty far into the sky, whereas
	/// when on the same elevation or above, it should be a shallow angle. Ideally, this avoids most obstacles, while
	/// still keeping the trajectory reasonably low.
	/// This change is also accompanied by clamping the angle to a valid range, and also silently returning if no
	/// solution was found. <para></para>
	/// The second change was to not set the velocity, but rather add some. The fraction for which the old velocity is kept is
	/// another parameter.
	/// </summary>
	private void Fire()
	{
		Vector3 p = target.position;
 
		float gravity = Physics.gravity.magnitude;
		// Selected angle in radians
//		float angle = initialAngle * Mathf.Deg2Rad;
 
		// Positions of this object and the target on the same plane
		Vector3 planarTarget = new Vector3(p.x, 0, p.z);
		Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);
 
		// Planar distance between objects
		float distance = Vector3.Distance(planarTarget, planarPostion);
		// Distance along the y axis between objects
		float yOffset = transform.position.y - p.y;

		float angle = ArcAngle(Mathf.Atan(-yOffset / distance) * Mathf.Rad2Deg);
		angle = Mathf.Clamp(angle, .1f, 89) * Mathf.Deg2Rad;
//		Debug.Log(name+" "+angle);
//		angle = angle * Mathf.Deg2Rad;
 
		float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
 
		Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
 
		// Rotate our velocity to match the direction between the two objects
		float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
		angleBetweenObjects *= (p.x > transform.position.x ? 1 : -1);
		Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
 
		// Fire!
//		_rb.velocity = finalVelocity;
 
		// Alternative way:
//		_rb.AddForce(finalVelocity * _rb.mass, ForceMode.Impulse);

		if (float.IsNaN(finalVelocity.x)) return; // at least we tried...
		
		_rb.AddForce(finalVelocity * _rb.mass - _rb.velocity*KeepVelocity, ForceMode.Impulse);
	}

	void FixedUpdate()
	{
		float newdist = Vector3.Distance(_rb.position, target.transform.position);
		_isClosingIn = newdist < _lastTargetDistance;
//		if (newdist < _closest-1e-5)
//		{
//			_closest = newdist;
//			_lastClosestUpdate = Time.time;
//		}
		_lastTargetDistance = newdist;
	}

	private void Update()
	{
		if (Input.GetKeyDown(FireKey))
		{
			Fire();
		} 
		else if (Input.GetKeyDown(ResetKey))
		{
			_rb.position = _initialPos;
			_rb.rotation = _initialRot;
			_rb.velocity = _rb.angularVelocity = Vector3.zero;
			GetComponent<ParticleSystem>()?.Clear();
			
			_isClosingIn = false;
//			_closest = float.MaxValue;
//			_lastClosestUpdate = Time.time;
		} 
		else if (Input.GetKey(SmartFireKey))
		{
			// only fire when we're falling down
			if (_rb.velocity.y > 0) return;
			
			// and we're not getting closer to the target
			if (_isClosingIn) return;
			_isClosingIn = true;

			// if we ever get stuck, we could try to pick a (random) non direct path in order to get a new chance
			// uncomment all places that use _closet to enable such a method, but for now this is beyond the scope of this simple class
//			if (Time.time - _lastClosestUpdate > 3)
//			{
//				_rb.AddForce(Random.insideUnitSphere * _rb.mass * 300, ForceMode.Impulse);
//				_closest = float.MaxValue;
//				_lastClosestUpdate = Time.time;
//				return;
//			}

			Fire();
		}
	}

	private float ArcAngle(float x)
	{
		return a * x + b;
	}
}

}