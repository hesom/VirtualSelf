using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity.Infix;
using UnityEngine;

public class Test : MonoBehaviour
{
	private Quaternion start;
	private Quaternion end;
	private float step = 0;
	
	void Start ()
	{
		start = transform.rotation;
		
		DrawVector(transform.forward, "forward");
		DrawVector(transform.right, "right");
		DrawVector(transform.up, "up");

		Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
		DrawVector(forward, "my forward");

		Vector3 right = forward.Cross(transform.forward);
		float ra = right.Angle(transform.right);
		if (ra > 90) right *= -1;
		DrawVector(right, "my right");

		Vector3 up = forward.Cross(right);
		float ua = up.Angle(transform.up);
		if (ua > 90) up *= -1;
		DrawVector(up, "my up");

		if (up.y < 0)
		{
			Debug.Log(up);
			up *= -1;
		}
		end = Quaternion.LookRotation(forward, up);

		transform.rotation = start;
	}
	
	void Update ()
	{
		float speed = 0.02f;
		if (Input.GetKey(KeyCode.PageUp))
		{
			transform.rotation = Quaternion.Slerp(start, end, step += speed);
		}
		else if (Input.GetKey(KeyCode.PageDown))
		{
			transform.rotation = Quaternion.Slerp(start, end, step -= speed);
		}

		step = Mathf.Clamp01(step);
	}

	void DrawVector(Vector3 v, string name)
	{
		SpawnCylinder(transform.position, v, 2).name = name;
	}

	private static void VisualizeVector(Vector3 start, Vector3 dir, float length, float spheres)
	{
		Vector3 end = start + dir.normalized * length;
//		Debug.Log($"spheres: start {start} end {end} dir {dir} len {length}");
		
		for (float i = 0; i <= 1; i+=1f/spheres)
		{
			GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			o.transform.position = Vector3.Lerp(start, end, i);
			o.transform.localScale = Vector3.one * 0.03f;
			o.GetComponent<Renderer>().material.color = new Color(dir.x, dir.y, dir.z);
		}
	}
	
	private static GameObject SpawnCylinder(Vector3 start, Vector3 dir, float flength)
	{
		Vector3 end = start + dir.normalized * flength;
//		Debug.Log($"cylinder: start {start} end {end} dir {dir} len {flength}");
		Vector3 center = Vector3.Lerp(start, end, .5f);
		Vector3 objectVector = end-start;
		float length = objectVector.magnitude;
		Quaternion rot = Quaternion.FromToRotation(Vector3.up, objectVector.normalized);
		GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cyl.transform.position = center;
		cyl.transform.rotation = rot;
		Vector3 scale = cyl.transform.localScale;
		scale.x = scale.z = .03f;
		scale.y = length/2f;
		cyl.transform.localScale = scale;
		objectVector.Normalize();
		cyl.GetComponent<Renderer>().material.color = new Color(objectVector.x, objectVector.y, objectVector.z);
		return cyl;
	}
}
