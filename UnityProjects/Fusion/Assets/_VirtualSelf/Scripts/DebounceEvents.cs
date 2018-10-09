using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf 
{

public class DebounceEvents : MonoBehaviour {

	public UnityEvent Event;
	public float MinDelay = 0.1f;

	private float _lastTrigger;

	public void TryTigger() 
	{
		if (Time.time > _lastTrigger + MinDelay) {
			Event.Invoke();
			_lastTrigger = Time.time;
		}
	}
}

}