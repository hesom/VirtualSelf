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
	public Vector2 TimeInterval = new Vector2(0.1f, 0.2f);
	public Vector3 Axis = Vector3.up;

	private float _nextPick = -1;
	private float _currentValue;
	
	void FixedUpdate()
	{
		if (Time.time > _nextPick)
		{
			_currentValue = UseRange ? Random.Range(SpeedRange.x, SpeedRange.y) : Speed;
			_nextPick = Time.time + Random.Range(TimeInterval.x, TimeInterval.y);
		}
		
		transform.Rotate(Axis, _currentValue);
	}
}

}