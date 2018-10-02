using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine.Events;
using VirtualSelf;

namespace VirtualSelf
{
	
public class PositionTracker : MonoBehaviour
{
	public UnityEvent OnActivate;
	public DetectorLogicGate DetectorLogicGate;
	public HandModelBase HandModel1 = null;
	public HandModelBase HandModel2 = null;
	public float DistanceRequired = .2f;
	public Direction DistanceAxis;
	public GameObject DebugDot;
	public bool debug;
	
	private bool _tracking;
	private Vector3 _startPos;
	private Vector3 _lastPos;
//	private Controller Controller;
	
	// Use this for initialization
	void Start ()
	{
//		Controller = new Controller();
	}
	
	// Update is called once per frame
	void Update () {
		Hand h1 = HandModel1.GetLeapHand();
		Hand h2 = HandModel2.GetLeapHand();
		
		
		if (h1 != null && h2 != null)
		{
//			Debug.Log(h1.TimeVisible+" "+h2.TimeVisible);
			_lastPos = PalmPos(h1, h2);
			
//			if (_tracking && !DetectorLogicGate.IsActive)
//			{
//				CheckEndPos();
//							
			if (_tracking)
			{
				CheckEndPos();
				
			} else if (DetectorLogicGate.IsActive)
			{
				if (debug) Debug.Log("tracking started");
				_tracking = true;
//				_startPos = (HandModel1.transform.position+h1.PalmPosition.ToVector3()
//				             -HandModel2.transform.position+h2.PalmPosition.ToVector3())/2;
				_startPos = PalmPos(h1, h2);
				if (debug) Instantiate(DebugDot, _startPos, Quaternion.identity);
			}
//			else if (_tracking && !DetectorLogicGate.IsActive)
//			{
//				Debug.Log("gesture inactive, tracking canceled");
////				_tracking = false;
//				CheckEndPos();
//			}
		}
		else if (_tracking)
		{
			if (debug) Debug.Log("leap hand not found, tracking canceled");
			CheckEndPos();
		}
	}

	private Vector3 BasePos(Hand h1, Hand h2)
	{
//		var hands = Controller.Frame().Hands;
//		return (hands[0].Basis.translation.ToVector3()+hands[1].Basis.translation.ToVector3())/2;
		return (h1.Basis.translation.ToVector3()+h2.Basis.translation.ToVector3())/2;
	}

	private Vector3 PalmPos(Hand h1, Hand h2)
	{
//		Vector3 palm = (h1.PalmPosition.ToVector3()+h2.PalmPosition.ToVector3())/2;
//		return BasePos(h1, h2) + palm;
		return (HandModel1.transform.position
		        +HandModel2.transform.position
		        +h1.PalmPosition.ToVector3()
		        +h2.PalmPosition.ToVector3()
		        )/2;
	}
	
	private void CheckEndPos()
	{
//		_tracking = false;
		Vector3 endPos = _lastPos;
		
//		Vector3 endPos = (HandModel1.transform.position+h1.PalmPosition.ToVector3()
//		                  -HandModel2.transform.position+h2.PalmPosition.ToVector3())/2;		
//		Vector3 endPos = BasePos();
//		Instantiate(DebugDot, endPos, Quaternion.identity);
				
		float diff;
		switch (DistanceAxis)
		{
			case Direction.X: diff = Mathf.Abs(_startPos.x-endPos.x);
				break;
			case Direction.Y: diff = Mathf.Abs(_startPos.y-endPos.y);
				break;
			case Direction.Z: diff = Mathf.Abs(_startPos.z-endPos.z);
				break;
			default: diff = float.MaxValue;
				break;
		}

		if (diff > DistanceRequired)
		{
			if (debug) Debug.Log("tracking completed with enough distance "+diff);
			OnActivate.Invoke();
			_tracking = false;
			if (debug) Instantiate(DebugDot, endPos, Quaternion.identity);
		}
		else if (!DetectorLogicGate.IsActive)
		{
			if (debug) Debug.Log("tracking completed without enough distance "+diff);
			_tracking = false;
			if (debug) Instantiate(DebugDot, endPos, Quaternion.identity);
		}
	}
	
	public enum Direction {X,Y,Z}
}
	
}
