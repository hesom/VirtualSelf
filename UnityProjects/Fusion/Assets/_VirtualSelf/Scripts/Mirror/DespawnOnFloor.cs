using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnOnFloor : MonoBehaviour {
	
	void OnCollisionEnter(Collision other){
		if(other.transform.tag == "Floor"){
			Invoke("Despawn", 5);
		}
	}

	void OnCollisionExit(Collision other) {
		if(other.transform.tag == "Floor"){
			CancelInvoke("Despawn");
		}
	}

	void Despawn(){
		Destroy(gameObject);
	}
}
