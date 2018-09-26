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

	void Start()
	{
		if (HandModel == null) HandModel = GetComponent<HandModelBase>();
		if (HandModel != null)
		{
			HandModel.OnBegin += HandReset;
			HandModel.OnFinish += HandFinish;
		}
		else
		{
			throw new InvalidOperationException("hand not found");
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