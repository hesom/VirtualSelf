using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf{
	public class MirrorColorChanger : MonoBehaviour {

		private List<Color> defaultColor;
		private List<MirrorColor> objectList;

		private void Start()
		{
			objectList = new List<MirrorColor>(FindObjectsOfType<MirrorColor>() as MirrorColor[]);
			defaultColor = new List<Color>();
			for(int i = 0; i < objectList.Count; i++){
				MirrorColor obj = objectList[i];
				var mirrorMesh = obj.transform.Find("VisibleMesh");
				defaultColor.Add(mirrorMesh.GetComponent<Renderer>().material.color);
			}
		}

		public void UpdateReference(MirrorColor mirrorColorOld, MirrorColor mirrorColorNew)
		{
			int i = objectList.IndexOf(mirrorColorOld);
			objectList[i] = mirrorColorNew;
		}
	}
}


