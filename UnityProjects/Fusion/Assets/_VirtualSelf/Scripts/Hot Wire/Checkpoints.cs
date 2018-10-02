using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
	
public class Checkpoints : MonoBehaviour
{

	public GameObject CheckpointObject;
	public int NameSubstring = 1;
	
	private readonly List<Checkpoint> _checkpoints = new List<Checkpoint>();
	private int _securedCheckPoint;
	
	// Use this for initialization
	void Start () {
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform t = transform.GetChild(i);
			Checkpoint p = t.GetComponent<Checkpoint>();
			if (p != null)
			{
				_checkpoints.Add(p);
			}
		}

//		foreach (var p in _checkpoints)
//		{
//			Debug.Log("Checkpoint "+p.name+" "+NumOfWire(p.gameObject));
//		}
	}
	
	private Checkpoint HighestBelow(int n)
	{
		int current = -1;
		Checkpoint highest = null;
		foreach (Checkpoint p in _checkpoints)
		{
			int i = NumOfWire(p.gameObject);
			if (i > current && i <= n)
			{
				highest = p;
				current = i;
			}
		}
		
		Debug.Log("collision with wire piece "+n+", resetting to checkpoint "+highest+" "+NumOfWire(highest.gameObject));
		_securedCheckPoint = current;
		
		return highest;
	}

	public void BackToNearestCheckpoint(GameObject wire)
	{
		if (!wire.CompareTag("Wire")) throw new ArgumentException(wire+" does not have the Wire tag");

		HighestBelow(Math.Max(NumOfWire(wire), _securedCheckPoint)).ResetObject(CheckpointObject);

	}

	public void ResetProgression()
	{
		_securedCheckPoint = 0;
	}

	private int NumOfWire(GameObject wire)
	{
		Debug.Log(wire.name+" wire name, num: "+wire.name.Substring(NameSubstring));
		return int.Parse(wire.name.Substring(NameSubstring));
	}
}

}
