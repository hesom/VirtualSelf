using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf
{

public class FireGesture : MonoBehaviour
{
	public UnityEvent OnWaitingForRelease;
	public UnityEvent OnActivate;
	public UnityEvent OnDeactivate;
	
	public PalmDirectionDetector PalmFacingAway;
	public ExtendedFingerDetector Open;
	public ExtendedFingerDetector Fist;

	private State _state;

	private enum State
	{
		WaitingForPalmOpen, WaitingForFistClose, WaitingForFlameRelease, WaitingForReset
	}
	private float _statechangetime;
	
	// Use this for initialization
	void Start ()
	{
//		Fist.OnActivate.AddListener(EvalStages);
//		Fist.OnDeactivate.AddListener(EvalStages);
//		Open.OnActivate.AddListener(EvalStages);
//		Open.OnDeactivate.AddListener(EvalStages);
//		PalmFacingAway.OnActivate.AddListener(EvalStages);
//		PalmFacingAway.OnDeactivate.AddListener(EvalStages);
	}

	void SetState(State s)
	{
		State prev = _state;
		_state = s;
		_statechangetime = Time.time;
		Debug.Log("Changed state from "+prev+" to "+s);
	}
	
	void EvalStages()
	{
		if (_state == State.WaitingForPalmOpen)
		{
			if (PalmFacingAway.IsActive && Open.IsActive)
			{
				SetState(State.WaitingForFistClose);
			}
			else if (Time.time > _statechangetime + 1 && Fist.IsActive)
			{
				OnDeactivate.Invoke();
				SetState(State.WaitingForReset);
			}
		}
		if (_state == State.WaitingForFistClose)
		{
			if (PalmFacingAway.IsActive && Fist.IsActive)
			{
				OnWaitingForRelease.Invoke();
				SetState(State.WaitingForFlameRelease);
			}
			else if (Time.time > _statechangetime + 2)
			{
				SetState(State.WaitingForPalmOpen);
			}
			else if (!PalmFacingAway.IsActive)
			{
				SetState(State.WaitingForPalmOpen);
			}
		}
		if (_state == State.WaitingForFlameRelease)
		{
			if (Open.IsActive)
			{
				OnActivate.Invoke();
				SetState(State.WaitingForReset);
			}
		}
		if (_state == State.WaitingForReset)
		{
			if (Fist.IsActive)
			{
				OnDeactivate.Invoke();
				SetState(State.WaitingForPalmOpen);
			}
			else if (Time.time > _statechangetime + 2)
			{
				SetState(State.WaitingForPalmOpen);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		EvalStages();
	}
	
	
}
	
}
