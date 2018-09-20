using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{



public class ChangeCamFov : MonoBehaviour
{

	public Camera Camera;
	public Vector2 Range;
	public float Increment;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CycleFov()
	{
		Camera.fieldOfView = ((Camera.fieldOfView-Range.x + Increment) % (Range.y-Range.x)) + Range.x;
		Debug.Log("Set fov to "+Camera.fieldOfView);
	}
}
	
}
