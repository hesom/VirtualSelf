using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using UnityEngine;
using VirtualSelf;

namespace VirtualSelf
{
	
public class GraspIndicator : MonoBehaviour
{
	public InteractionHand Hand;
	public IndicatorColor IndicatorColor;
//	public IndicatorMaterial IndicatorMaterial;
	public Vector3 RiggedLocalPos;
	public Vector3 RiggedLocalRot;

	private bool _tracked = true;
	private Renderer _ren;
	private Light _light;
	private bool _rigged;
	private Quaternion _riggedLocalrot;
	private Vector3 _capsuleLocalPos;
	private Quaternion _capsuleLocalRot;
	
	// Use this for initialization
	void Start ()
	{
		_ren = GetComponent<Renderer>();
		_light = GetComponentInChildren<Light>();
		
		Hand.OnGraspBegin += GraspBegin;
		Hand.OnGraspEnd += GraspEnd;
		// alternatively: in update check Hand.isGraspingObject
		
		GraspEnd();

		_riggedLocalrot = Quaternion.Euler(RiggedLocalRot);
	}

	public void ToggleRigged()
	{
		if (_rigged)
		{
			transform.localRotation = _capsuleLocalRot;
			transform.localPosition = _capsuleLocalPos;
		}
		else
		{
			transform.localRotation = _riggedLocalrot;
			transform.localPosition = RiggedLocalPos;
		}

		_rigged = !_rigged;
	}
	
	void LateUpdate() // update in the same method as the hand
	{
		if (Hand.isTracked != _tracked)
		{
//			gameObject.SetActive(Hand.isTracked); // cannot disable this because update will not be called anymore
			_ren.enabled = Hand.isTracked;
			_tracked = Hand.isTracked;
		}
	}

	private void GraspBegin()
	{
		Debug.Log("grasp");
		IndicatorColor.State = IndicatorColor.States.Valid;
		
//		else if (IndicatorMaterial != null) IndicatorMaterial.State = States.Valid;

		_light.color = IndicatorColor.CurrentColor();
	}

	private void GraspEnd()
	{
		Debug.Log("ungrasp");
		IndicatorColor.State = IndicatorColor.States.Invalid;
//		else if (IndicatorMaterial != null) IndicatorMaterial.State = States.Valid;
		
		_light.color = IndicatorColor.CurrentColor();
	}
}
	
}
