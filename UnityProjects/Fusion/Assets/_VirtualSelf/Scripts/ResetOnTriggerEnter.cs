using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResetOnTriggerEnter : MonoBehaviour
{
	public UnityEvent OnReset;
	
	private Vector3 _pos;
	
	// Use this for initialization
	void Start ()
	{
		_pos = transform.position;
	}

	void OnTriggerEnter(Collider other)
	{
		Rigidbody r = GetComponent<Rigidbody>();
		if (r != null) r.position = _pos;
		else transform.position = _pos;
		OnReset.Invoke();
	}
}
