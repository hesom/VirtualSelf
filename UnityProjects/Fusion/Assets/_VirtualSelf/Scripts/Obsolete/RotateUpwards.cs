using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUpwards : MonoBehaviour
{

	private Rigidbody _rb;
	
	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
//		StartCoroutine(Rotate());
		Invoke(nameof(Temp), 3);
		Invoke(nameof(Temp2), 3.5f);
	}
	
	private void Temp()
	{
		StartCoroutine(Rotate(100));
	}

	private void Temp2()
	{
		StopCoroutine("Rotate");
	}

	IEnumerator Rotate(int physicsTicks)
	{
		Quaternion start = _rb.rotation;
		Quaternion end = Quaternion.FromToRotation(Vector3.up, Vector3.up);
		for (float i = 0; i <= 1; i+=1f/physicsTicks)
		{
			_rb.rotation = Quaternion.Slerp(start, end, i);
			yield return new WaitForFixedUpdate();
		}
	}
}
