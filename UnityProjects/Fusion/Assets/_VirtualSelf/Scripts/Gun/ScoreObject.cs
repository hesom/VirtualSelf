using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
	
public class ScoreObject : MonoBehaviour
{
	public Gamescore Gamescore;
	public int ScoreValue = 1;
	public bool EnableAllScriptsOnReset;
	
	private bool _done;
	private Vector3 _initialPos;
	private Quaternion _initialRot;
//	private Rigidbody _rb;
	
	void Start ()
	{
		_initialPos = transform.localPosition;
		_initialRot = transform.localRotation;
//		_rb = GetComponent<Rigidbody>();
		// setting rigidbody would be more ideal, but position is only changed once per scene reset anyways
	}
	
	void OnCollisionEnter(Collision other)
	{
		if (_done) return;
		
		if (other.gameObject.CompareTag("Projectile") || other.gameObject.CompareTag("Floor"))
		{
			_done = true;
			Gamescore.Increment(this, ScoreValue);
		}
	}

	public void Reset()
	{
		_done = false;
//		_rb.position = _initialPos;
		transform.localPosition = _initialPos;
		transform.localRotation = _initialRot;
		if (EnableAllScriptsOnReset)
		{
			foreach (MonoBehaviour b in GetComponents<MonoBehaviour>()) b.enabled = true;
		}
	}
}
	
}