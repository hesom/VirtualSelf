using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour {

	private void Update() {
		if (Input.GetKeyDown(KeyCode.K)) {
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

	private void OnCollisionStay(Collision other) {
		Debug.LogWarning("collision stay "+other.gameObject.name);
	}

	private void OnCollisionEnter(Collision other) {
		Debug.LogWarning("collision enter "+other.gameObject.name);
	}

}
