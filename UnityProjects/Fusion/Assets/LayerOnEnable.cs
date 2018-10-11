using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerOnEnable : MonoBehaviour {

	public string Layer;
	
	private void OnEnable() {
		int layer = LayerMask.NameToLayer(Layer);
		gameObject.layer = layer;
		foreach (Transform t in transform) {
			t.gameObject.layer = layer;
		}
	}

}
