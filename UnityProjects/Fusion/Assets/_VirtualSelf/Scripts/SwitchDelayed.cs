using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualSelf;

[RequireComponent(typeof(DebounceEvents))]
public class SwitchDelayed : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		Invoke(nameof(Stuff), 1);
	}

	private void Stuff() {
//		GetComponent<ModelSwitcher>().CycleGroup();
		GetComponent<DebounceEvents>().TryTigger();
	}
}
