using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;
using VirtualSelf;

namespace VirtualSelf
{
	

public class WireCollider : MonoBehaviour
{

	public IndicatorColor Indicator;
	public GameObject DebugDot;
	public bool DropOnCollision;
	public InteractionManager InteractionManager;
	public Checkpoints Checkpoints;
	public float PickupGracePeriod = 1;

	private Rigidbody _rb;
	private float _graspBegin;
	
	// Use this for initialization
	void Start () {
//		foreach (Collider c in GetComponentsInChildren<Collider>()) c.contactOffset = 0.001f;
		_rb = GetComponent<Rigidbody>();
		GetComponent<InteractionBehaviour>().OnGraspBegin += OnGrasp;
	}

	void OnGrasp()
	{
		_rb.useGravity = true;
		_graspBegin = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// gravity for the handle is disabled when it gets reset to a checkpoint
		// in case this handle is clipped into something it will get pushed out and fly continously
		// so the workaround here is to allow for the handle to still get pushed, but take away the velocity
		if (!_rb.useGravity)
		{
			_rb.velocity = _rb.angularVelocity = Vector3.zero;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Wire"))
		{
			
			Debug.Log("collision "+name+" with "+other.gameObject.name+" at "+other.contacts.Length+" points");
			foreach (ContactPoint c in other.contacts)
			{
				Destroy(Instantiate(DebugDot, c.point, Quaternion.identity), 10);
			}
			Indicator.State = IndicatorColor.States.Invalid;
//			Indicator.SetState(States.Invalid);
//			Indicator.State = States.Invalid;

			if (DropOnCollision && Time.time > _graspBegin+PickupGracePeriod)
			{
				foreach (var h in InteractionManager.GetComponentsInChildren<InteractionHand>())
				{
					IInteractionBehaviour ib;
					if (h.ReleaseGrasp(out ib))
					{
						// adding a force is easy, but doesn't work very well
//						ib.gameObject.GetComponent<Rigidbody>().AddRelativeForce(10,-200,10);
						
						// reset the handle to the nearest checkpoint
						Checkpoints.BackToNearestCheckpoint(other.gameObject);
						
//						
						Renderer r = ChildByName(ib.gameObject, "ring3_visual").GetComponent<Renderer>();
//						r.material.color = Color.blue;
						if (_ogColor == Color.magenta) _ogColor = r.material.color;
						StopCoroutine(ChangeMat(r));
						StartCoroutine(ChangeMat(r));
					}
				}
			}
		}
	}

	private Color _ogColor = Color.magenta;

	public static GameObject ChildByName(GameObject g, String name)
	{
		for (int i = 0; i < g.transform.childCount; i++)
		{
			Transform t = g.transform.GetChild(i);
			if (t.name.Equals(name)) return t.gameObject;
		}

		return null;
	}

	private IEnumerator ChangeMat(Renderer r)
	{
		for (int i = 0; i < 5; i++)
		{
			if (i % 2 == 0)
			{	
				r.material.color = new Color(1,0,0,.1f);
			}
			else
			{
				r.material.color = _ogColor;
			}
			yield return new WaitForSeconds(.1f);
		}
		r.material.color = _ogColor;
		yield return null;
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.CompareTag("Wire"))
		{
			Indicator.State = IndicatorColor.States.Valid;
//			Indicator.SetState(States.Valid);
//			Indicator.State = States.Valid;
		}
	}
}


}