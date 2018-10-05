using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VirtualSelf.GameSystems;

public class ActivateKeypad : MonoBehaviour {

	public Keycode code;
	
	// Use this for initialization
	void Start() {
		var obj = SceneManager.GetSceneByName("MasterScene").GetRootGameObjects();
		foreach (var o in obj) {
			if (o.tag == "Keypad") {
				o.SetActive(true);
			}
		}

		code.IsDiscovered = true;
	}

}
