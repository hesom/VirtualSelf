using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf
{
	
public class Checkpoint : MonoBehaviour
{
	public GameObject DebugDot;
//	public Vector3 Rotation;
	public bool DebugTriggerOnStart;
	public UnityEvent OnReset;

	void Start()
	{
		if (DebugTriggerOnStart) ResetObject(GameObject.Find("ring3"));
	}
	
	public void ResetObject(GameObject o)
	{
//		List<Vector3> vert = new List<Vector3>();
//		GetComponent<MeshFilter>().mesh.GetVertices(vert);
		
		Vector3[] vert = GetComponent<MeshFilter>().mesh.vertices;
		Vector3 pos = transform.TransformPoint(vert[1]);
		// Vector3 pos = transform.position;

		if (DebugDot != null) Instantiate(DebugDot, pos, Quaternion.identity).transform.localScale = Vector3.one * .005f;

		// Vector3 rotAxis = Quaternion.AngleAxis(-90, Vector3.right) * (pos - transform.TransformPoint(vert[0]));
		Vector3 rotAxis = Quaternion.Euler(-90, 0, 0) * (pos - transform.TransformPoint(vert[0]));
		// Vector3 rotAxis = Quaternion.Euler(-90, -90, 0) * (Vector3.up);
		Quaternion rot = Quaternion.LookRotation(rotAxis);
//		Quaternion rot = rb.rotation = Quaternion.Euler(Rotation);
		
		Rigidbody rb = o.GetComponent<Rigidbody>();
		if (rb != null)
		{
//			rb.position = pos;
//			rb.rotation = rot;
			rb.MovePosition(pos);
			rb.MoveRotation(rot);
			rb.useGravity = false;
//			foreach (var c in o.GetComponentsInChildren<Collider>()) c.enabled = false;
		}
		else
		{
			o.transform.position = pos;
			o.transform.rotation = rot;
		}

		OnReset.Invoke();
	}
}
	
}
