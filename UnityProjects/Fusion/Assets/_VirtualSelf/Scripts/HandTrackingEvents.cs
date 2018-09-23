using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf
{

public class HandTrackingEvents : HandTransitionBehavior
{
	[Tooltip("This can be null if this script is on a HandModel")]
	public HandModelBase HandModel;
	public UnityEvent OnFinish;
	public UnityEvent OnReset;

	void Start()
	{
		if (HandModel != null)
		{
			HandModel.OnBegin += HandReset;
			HandModel.OnFinish += HandFinish;
		}
	}

	protected override void HandReset()
	{
		OnReset.Invoke();
	}

	protected override void HandFinish()
	{
		OnFinish.Invoke();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (HandModel != null)
		{
			HandModel.OnBegin -= HandReset;
			HandModel.OnFinish -= HandFinish;
		}
	}
}

}