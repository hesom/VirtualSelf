using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{

[RequireComponent(typeof(DisableSet))]
public class QueryDebugMode : MonoBehaviour
{
	public SwitchDelayed SwitchDelayed;
	
	// Use this for initialization
	void Start () {
		// by default, we assume debug mode is on, which means the DisableSet script should be active and thus also
		// the skip button. only if we are not in debug mode, we need to disable our disableset.
		// though there should be no harm in setting it both cases anyways
		GetComponent<DisableSet>().enabled = SwitchDelayed.IsDebug;
	}
}

}