using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf{
	[SelectionBase]
	public class BallDispenser : MonoBehaviour {

		public MirrorColor prefab;

		[Range(0.0f, 1.0f)]
		public float spawnTimeGap = 0.5f;

		private Queue<MirrorColor> spawnQueue = new Queue<MirrorColor>();
		private float timeSinceLastSpawn = 0.0f;

		void Update()
		{
			if(spawnQueue.Count > 0 && timeSinceLastSpawn > spawnTimeGap){
				var cube = spawnQueue.Dequeue();
				cube.gameObject.SetActive(true);
				timeSinceLastSpawn = 0.0f;
			}

			timeSinceLastSpawn += Time.deltaTime;
		}

		public MirrorColor SpawnCube(Material material, bool visible)
		{
			var cube = Instantiate(prefab, transform.position, transform.rotation);
			var visibleMesh = cube.transform.Find("VisibleMesh");
			if(!cube.visible){
				visibleMesh.gameObject.layer = LayerMask.NameToLayer("NotMirror");
			}else{
				visibleMesh.gameObject.layer = LayerMask.NameToLayer("VisibleBeforeMirror");
			}
			cube.mirrorMaterial = material;

			var invisibleMesh = cube.transform.Find("InvisibleMesh");
			invisibleMesh.GetComponent<Renderer>().material = material;

			var velocity = new Vector3(Random.Range(-0.1f, 0.1f),0.0f, Random.Range(-0.1f, 0.1f));
			var rb = cube.GetComponent<Rigidbody>();
			rb.velocity = velocity;
			
			cube.gameObject.SetActive(false);
			cube.visible = visible;
			spawnQueue.Enqueue(cube);
			return cube;
		}
	}
}


