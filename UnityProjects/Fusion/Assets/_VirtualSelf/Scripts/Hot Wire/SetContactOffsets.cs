using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
	
public class SetContactOffsets : MonoBehaviour
{

	public float ContactOffset = 0.001f;
	
	// Use this for initialization
	void Start () {
		foreach (Collider c in GetComponentsInChildren<Collider>()) c.contactOffset = ContactOffset;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

}