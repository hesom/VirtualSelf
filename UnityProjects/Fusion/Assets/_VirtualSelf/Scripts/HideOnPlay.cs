using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{

public class HideOnPlay : MonoBehaviour {

	public enum Method
	{
		Start, Awake, Renderer
	}

	public Method HideMethod;

	void Awake()
	{
		if (HideMethod == Method.Awake) gameObject.SetActive(false);
	}

	void Start () 
	{
		if (HideMethod == Method.Start) gameObject.SetActive(false);
		else if (HideMethod == Method.Renderer)
		{
			var ren = GetComponent<Renderer>();
			if (ren != null) ren.enabled = false;
			foreach (var c in GetComponentsInChildren<Renderer>()) c.enabled = false;
		}
	}
}

}