using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
	
public class LinearPatrol : MonoBehaviour
{
	public enum SavePosition {None, A, B}
	
	public Vector3 PosA;
	public Vector3 PosB;
	public SavePosition Save;
	[Range(0,1)]
	public float Speed = 0.001f;
	public bool RandomStart;
	[Tooltip("If this object collides with another gameobject with any of these tags, this script is disabled")]
	public List<string> StopTags;
	
	private Rigidbody _rb;
	private float _lerpFrac;
	private bool _incrementing = true;
	
	void Start ()
	{
		Save = SavePosition.None;
		_rb = GetComponent<Rigidbody>();
		if (RandomStart) _lerpFrac = Random.value;
	}

	void FixedUpdate()
	{
		if (_rb.velocity.sqrMagnitude < .6 && _rb.angularVelocity.sqrMagnitude < 10)
		{
			_rb.velocity = _rb.angularVelocity = Vector3.zero;
			
			float speed = Speed / 60f;
			if (_incrementing)
			{
				_lerpFrac = Mathf.Min(1, _lerpFrac + speed);
				if (_lerpFrac == 1) _incrementing = false;
			}
			else
			{
				_lerpFrac = Mathf.Max(0, _lerpFrac - speed);
				if (_lerpFrac == 0) _incrementing = true;
			}
//			_rb.MovePosition(transform.TransformDirection(Vector3.Lerp(PosA,PosB,_lerpFrac)));
			_rb.MovePosition(Vector3.Lerp(PosA,PosB,_lerpFrac)); // using global position for now
		}
//		else
//		{
//			Debug.Log(_rb.velocity.sqrMagnitude+" "+_rb.angularVelocity.sqrMagnitude);
//		}
	}

	void OnCollisionEnter(Collision other)
	{
		if (StopTags.Contains(other.gameObject.tag)) enabled = false;
	}

	void OnValidate()
	{
		switch (Save)
		{
			case SavePosition.A:
				PosA = transform.position;
				Save = SavePosition.None;
				break;
			case SavePosition.B:
				PosB = transform.position;
				Save = SavePosition.None;
				break;
		}
	}
}

}