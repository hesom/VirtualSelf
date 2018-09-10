using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;


public class DrawLeapFrustum : MonoBehaviour {

	private Controller controller;
	private Device device;
	// Use this for initialization
	void Start () {
		controller = new Controller();
	}

	void OnDrawGizmos()
	{
		if(controller != null && controller.Devices.Count > 0){
			device = controller.Devices[0];
			var temp = Gizmos.matrix;
			float aspect = device.HorizontalViewAngle / device.VerticalViewAngle;
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation /** Quaternion.FromToRotation(Vector3.forward, Vector3.up)*/, Vector3.one);
			Gizmos.DrawFrustum(Vector3.zero, Mathf.Rad2Deg*device.VerticalViewAngle, device.Range/1000.0f, 0.0f, aspect);
			Gizmos.matrix = temp;
		}
	}
}
