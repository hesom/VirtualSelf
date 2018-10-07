using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace VirtualSelf
{
	
public class GetRandomChildAttribute : MonoBehaviour
{
	public Transform RandomTransform()
	{
		return transform.GetChild(Random.Range(0, transform.childCount));
	}

	public T RandomInstantiate<T>(T t) where T : Object
	{
		Transform f = RandomTransform();
		Vector3 objPos = t is GameObject ? (t as GameObject).transform.localPosition : Vector3.zero;
		return Instantiate(t, f.position+objPos, f.rotation);
	}
	
	public Vector3 RandomPos()
	{
		return RandomTransform().position;
	}
	
	public Vector3 RandomLocalPos()
	{
		return RandomTransform().localPosition;
	}

	public Quaternion RandomRot()
	{
		return RandomTransform().rotation;
	}

	public Vector3 RandomScale()
	{
		return RandomTransform().localScale;
	}
}
	
}
