using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempReset : MonoBehaviour {

	private Rigidbody rb;
	private Vector3 orig;
	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		orig = rb.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (rb.position.y < orig.y - 1) {
			rb.position = orig;
			rb.velocity = Vector3.zero;
			rb.rotation = Quaternion.identity;
		}
		
	}
}
