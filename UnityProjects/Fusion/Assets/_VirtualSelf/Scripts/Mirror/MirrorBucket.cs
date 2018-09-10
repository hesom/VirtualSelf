using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VirtualSelf{
	[ExecuteInEditMode]
	[SelectionBase]
	public class MirrorBucket : MonoBehaviour {

		public Material bucketMaterial;
		public BallDispenser spawnPoint;
		public MirrorColorChanger colorChanger;
		[Tooltip("Max number of cubes that can go in the wrong bucket before scene restarts")]
		public int cheatProtection = 10;

		private int counter = 0;
		private int cheatCounter = 0;
		
		void OnValidate()
		{
			var children = GetComponentsInChildren<Renderer>();
			foreach(Renderer rend in children){
				rend.material = bucketMaterial;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if(other.transform.tag=="MirrorCube"){
				MirrorColor coloredObject = other.GetComponent<MirrorColor>();
				if(coloredObject != null){
					Material mirrorMaterial = coloredObject.mirrorMaterial;
					if(mirrorMaterial.color == bucketMaterial.color){
						counter++;
					}else{
						// teleport object back to start
						var mirrorColorNew = spawnPoint.SpawnCube(mirrorMaterial, coloredObject.visible);
						colorChanger.UpdateReference(coloredObject, mirrorColorNew);
						Destroy(coloredObject.gameObject);
						cheatCounter++;
						if(cheatCounter > cheatProtection){
							SceneManager.LoadScene(SceneManager.GetActiveScene().name);
						}
					}
				}
			}
			
		}

		private void OnTriggerExit(Collider other)
		{
			if(other.transform.tag == "MirrorCube"){
				MirrorColor coloredObject = other.GetComponent<MirrorColor>();
				if(coloredObject != null){
					Material mirrorMaterial = coloredObject.mirrorMaterial;
					if(mirrorMaterial.color == bucketMaterial.color){
						counter--;
					}
				}
			}
		}

		public int GetCount()
		{
			return counter;
		}
	}
}

