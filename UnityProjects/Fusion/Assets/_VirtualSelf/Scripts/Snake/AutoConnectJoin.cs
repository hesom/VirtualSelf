using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoConnectJoin : MonoBehaviour {

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		GetComponent<CharacterJoint>().connectedBody = transform.parent.GetComponent<Rigidbody>();
	}
}
