using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

public class CopyAndMirror : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		GameObject copy = Instantiate(gameObject, gameObject.transform.parent);
		Quaternion rot = copy.transform.rotation;
		copy.transform.rotation = Quaternion.Euler(rot.x, rot.y-180, rot.z);
		Destroy(copy.GetComponent<Collider>());
		Destroy(copy.GetComponent<CopyAndMirror>());
		foreach (Transform child in copy.transform.GetChildren())
		{
			Destroy(child.gameObject);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
