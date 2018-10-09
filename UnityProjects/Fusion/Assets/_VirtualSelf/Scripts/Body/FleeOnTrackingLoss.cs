using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

namespace VirtualSelf
{

public class FleeOnTrackingLoss : MonoBehaviour
{
	public InteractionHand Hand;
	private bool _lastTracked = true;
	
//	void Start ()
//	{
//		HandModel.OnBegin += () => transform.localPosition = Vector3.zero;
//		HandModel.OnFinish += () => transform.localPosition = Vector3.one * 10000;
//	}

	void Update()
	{
		if (Hand.isTracked != _lastTracked)
		{
			transform.localPosition = Hand.isTracked ? Vector3.zero : Vector3.one * 10000;
			_lastTracked = Hand.isTracked;
		}
	}
}

}