using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf{
	public class RespawnOnFloor : MonoBehaviour {

		public BallDispenser spawnPoint;
		public MirrorColorChanger colorChanger;
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		private void OnCollisionEnter(Collision other) {
			if(other.transform.tag == "MirrorCube"){
				MirrorColor coloredObject = other.transform.GetComponent<MirrorColor>();
				if(coloredObject != null){
					var newCube = spawnPoint.SpawnCube(coloredObject.mirrorMaterial, coloredObject.visible);
					colorChanger.UpdateReference(coloredObject, newCube);
					Destroy(coloredObject.gameObject);
				}
			}
		}
	}
}

