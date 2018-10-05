using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateKeypadInTutorial : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var keypad = GameObject.FindWithTag("Keypad");
		keypad.SetActive(false);
	}
}
