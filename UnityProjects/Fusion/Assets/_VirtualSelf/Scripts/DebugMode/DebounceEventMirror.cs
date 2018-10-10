using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf 
{
	
public class DebounceEventMirror : MonoBehaviour {

	public DebounceEvents DebounceEvents;
	public UnityEvent OutEvent;
	
	void Start () 
	{
		DebounceEvents.Event.AddListener(() => OutEvent.Invoke());
	}
	
}

}
