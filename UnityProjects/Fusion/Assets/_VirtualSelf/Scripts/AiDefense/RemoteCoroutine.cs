using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
	
public class RemoteCoroutine : MonoBehaviour {

	/// <summary>
	/// Start a coroutine on this GameObject.
	/// </summary>
	/// <param name="ie"></param>
	/// <returns></returns>
	public new Coroutine StartCoroutine(IEnumerator ie)
	{
		return base.StartCoroutine(ie);
	}
}

}