using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf
{

public class HandTrackingEvents : HandTransitionBehavior
{
	public UnityEvent OnFinish;
	public UnityEvent OnReset;

	protected override void HandReset()
	{
		OnReset.Invoke();
	}

	protected override void HandFinish()
	{
		OnFinish.Invoke();
	}
}

}