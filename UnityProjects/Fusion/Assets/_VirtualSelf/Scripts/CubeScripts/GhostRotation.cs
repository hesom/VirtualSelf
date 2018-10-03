using UnityEngine;

namespace VirtualSelf.CubeScripts {
	
	public class GhostRotation : MonoBehaviour {

		public Transform OriginalCube;
	
		// Update is called once per frame
		void Update () {
			transform.rotation = OriginalCube.rotation;
		}
	}
}


