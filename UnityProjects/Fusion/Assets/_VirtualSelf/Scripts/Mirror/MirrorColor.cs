using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf{
	[ExecuteInEditMode]
	[SelectionBase]
	public class MirrorColor : MonoBehaviour {

		public Material mirrorMaterial;
		public bool visible;

		private Vector3 startPosition;
		private Quaternion startRotation;
		private Rigidbody rb;

		void Awake()
		{
			startPosition = transform.position;
			startRotation = transform.rotation;
			rb = GetComponent<Rigidbody>();

			var visibleMesh = transform.Find("VisibleMesh");
			visibleMesh.GetComponent<Renderer>().enabled = true;

			if(!visible){
				visibleMesh.gameObject.layer = LayerMask.NameToLayer("NotMirror");
			}else{
				visibleMesh.gameObject.layer = LayerMask.NameToLayer("VisibleBeforeMirror");
			}

			var mirrorMesh = transform.Find("InvisibleMesh");
			var renderer = mirrorMesh.GetComponent<Renderer>();
			renderer.material = mirrorMaterial;
		}

		public void MoveToSpawn()
		{
			if(rb != null){
				rb.transform.position = startPosition;
				rb.transform.rotation = startRotation;
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				//rb.MovePosition(startPosition);
			}else{
				transform.position = startPosition;
				transform.rotation = startRotation;
			}
		}

		void OnValidate()
		{
			var mirrorMesh = transform.Find("InvisibleMesh");
			var renderer = mirrorMesh.GetComponent<Renderer>();
			renderer.material = mirrorMaterial;

			//var visibleMesh = transform.Find("VisibleMesh");
			//visibleMesh.gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}
}

