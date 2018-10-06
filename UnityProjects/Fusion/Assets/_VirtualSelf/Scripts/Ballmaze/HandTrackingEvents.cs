using Leap.Unity;
using UnityEngine.Events;

namespace VirtualSelf.Ballmaze
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