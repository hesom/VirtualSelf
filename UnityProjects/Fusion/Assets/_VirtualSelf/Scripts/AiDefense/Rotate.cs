using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{

public class Rotate : MonoBehaviour
{

	public float Speed = 0.1f;
	public Vector2 SpeedRange = new Vector2(0, 0.1f);
	public bool UseRange;
	
	void FixedUpdate()
	{
		if (UseRange) transform.Rotate(Vector3.up, Random.Range(SpeedRange.x, SpeedRange.y));
		else transform.Rotate(Vector3.up, Speed);
	}
}

}