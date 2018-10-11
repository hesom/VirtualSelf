using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf
{

public class HandTrackingEvents : MonoBehaviour
{
	[Tooltip("This can be null if this script is on a HandModel")]
	public HandModelBase HandModel;
	public UnityEvent OnFinish;
	public UnityEvent OnReset;

	private int _handId;
	private bool _addedNewHand;

	void Start()
	{
		if (HandModel == null) HandModel = GetComponent<HandModelBase>();
		if (HandModel != null)
		{
			HandModel.OnBegin += HandReset;
			HandModel.OnFinish += HandFinish;
			_handId = HandModel.GetHashCode();
		}
		else
		{
			throw new InvalidOperationException("hand not found");
		}
	}

	void Update() {
		if (!_addedNewHand && HandModel.GetHashCode() != _handId) {
			HandModel.OnBegin += HandReset;
			HandModel.OnFinish += HandFinish;
			_addedNewHand = true;
		}
	}

	void HandReset()
	{
		OnReset.Invoke();
	}

	void HandFinish()
	{
		OnFinish.Invoke();
	}

	void OnDestroy()
	{
		if (HandModel != null)
		{
			HandModel.OnBegin -= HandReset;
			HandModel.OnFinish -= HandFinish;
		}
	}
}

}