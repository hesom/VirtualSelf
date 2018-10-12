using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualSelf.Utility;

public class RemoveBehindPortalLayer : MonoBehaviour {

	public void DoIt() {
		int behindPortal = LayerMask.NameToLayer("Behind Portal");
		int defaultt = LayerMask.NameToLayer("Default");
		foreach (var g in GameObjectsUtils.FindObjectsOfTypeButNoPrefabs<GameObject>()) {
			if (g.layer == behindPortal) g.layer = defaultt;
		}
	}
}
