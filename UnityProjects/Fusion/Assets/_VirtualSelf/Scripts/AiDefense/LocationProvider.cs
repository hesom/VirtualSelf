using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VirtualSelf
{
	
public class LocationProvider : MonoBehaviour
{

	public GameObject SpawnLocations;
	public GameObject IntermediateTargetLocations;
	public GameObject Player;
	
	Vector3[] spawnPos;
	Vector3[] targetPos;
	
	// Use this for initialization
	void Start ()
	{
		spawnPos = new Vector3[SpawnLocations.transform.childCount];
		for (int i = 0; i < spawnPos.Length; i++) 
			spawnPos[i] = SpawnLocations.transform.GetChild(i).position;
		
		targetPos = new Vector3[IntermediateTargetLocations.transform.childCount];
		for (int i = 0; i < targetPos.Length; i++) 
			targetPos[i] = IntermediateTargetLocations.transform.GetChild(i).position;
	}

	public Vector3 RandomSpawnPos()
	{
		return spawnPos[(int) (Random.value * spawnPos.Length)];
	}

	public Vector3 RandomTargetPos()
	{
		return targetPos[(int) (Random.value * targetPos.Length)];
	}

	public Vector3 PlayerPos()
	{
		return Player.transform.position;
	}
	
}
	
}
