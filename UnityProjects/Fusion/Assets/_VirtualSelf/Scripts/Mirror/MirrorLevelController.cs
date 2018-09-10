using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf{
	public class MirrorLevelController : MonoBehaviour
	{

		public UnityEvent onFinished;
		private List<MirrorColor> coloredObjects;
		private List<MirrorBucket> mirrorBuckets;
		private Dictionary<Color, int> counter;

		bool levelFinished = false;

		// Use this for initialization
		void Start () {
			MirrorColor[] coloredObjectsArray = FindObjectsOfType<MirrorColor>() as MirrorColor[];
			MirrorBucket[] mirrorBucketsArray = FindObjectsOfType<MirrorBucket>() as MirrorBucket[];
			counter = new Dictionary<Color, int>();
			coloredObjects = new List<MirrorColor>(coloredObjectsArray);
			mirrorBuckets = new List<MirrorBucket>(mirrorBucketsArray);
			foreach(var bucket in mirrorBuckets){
				counter[bucket.bucketMaterial.color] = 0;
			}
			foreach(var coloredObject in coloredObjects){
				Color objectColor = coloredObject.mirrorMaterial.color;
				bool hasBucket = false;
				foreach(var bucket in mirrorBuckets){
					if(objectColor == bucket.bucketMaterial.color){
						hasBucket = true;
						break;
					}
				}
				if(hasBucket){
					counter[objectColor]++;
				}
			}
		}
		
		// Update is called once per frame
		void Update () {
			bool allBucketsFilled = true;
			foreach(var bucket in mirrorBuckets){
				if(bucket.GetCount() < counter[bucket.bucketMaterial.color]){
					allBucketsFilled = false;
					break;
				}
			}

			if(!levelFinished && allBucketsFilled){
				Debug.Log("All objects placed correctly!");
				levelFinished = true;
				onFinished.Invoke();
			}
		}
	}
}

