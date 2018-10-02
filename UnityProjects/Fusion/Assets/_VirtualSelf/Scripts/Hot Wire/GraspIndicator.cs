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

	private bool _tracked = true;
	private Renderer _ren;
	
	// Use this for initialization
	void Start ()
	{
		_ren = GetComponent<Renderer>();
		
		Hand.OnGraspBegin += GraspBegin;
		Hand.OnGraspEnd += GraspEnd;
		// alternatively: in update check Hand.isGraspingObject
		
		GraspEnd();
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
	}

	private void GraspEnd()
	{
		Debug.Log("ungrasp");
		IndicatorColor.State = IndicatorColor.States.Invalid;
//		else if (IndicatorMaterial != null) IndicatorMaterial.State = States.Valid;
	}
}
	
}
