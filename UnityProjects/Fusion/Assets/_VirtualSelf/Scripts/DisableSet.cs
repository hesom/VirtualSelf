using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{

public class DisableSet : MonoBehaviour
{
	[Tooltip("Set the enable state of these objects to the same as this component")]
	public GameObject[] Set;
	
	private void OnEnable()
	{
		foreach (var o in Set)
		{
			o.SetActive(true);
		}
	}

	private void OnDisable()
	{
		foreach (var o in Set)
		{
			o.SetActive(false);
		}
	}

	public void Toggle() 
	{
		enabled = !enabled;
	}
}
	
}
