using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualSelf;

[RequireComponent(typeof(DebounceEvents))]
public class SwitchDelayed : MonoBehaviour
{
	public bool IsDebug { get; private set; }

	// Use this for initialization
	void Start ()
	{
		IsDebug = true;
		GetComponent<DebounceEvents>().Event.AddListener(() => IsDebug = !IsDebug);
		Invoke(nameof(Stuff), 1);
	}

	private void Stuff() {
//		GetComponent<ModelSwitcher>().CycleGroup();
		GetComponent<DebounceEvents>().TryTigger();
		IsDebug = false;
	}
}
