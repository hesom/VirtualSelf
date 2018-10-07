using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using VirtualSelf;

namespace VirtualSelf 
{

[RequireComponent(typeof(BaseAi))]
public class RiseFromGround : MonoBehaviour
{
	public float Speed = 1e-2f;
//	public Action OnFinish;
	
	void Start ()
	{
		StartCoroutine(FadeAboveGround());
	}
	
	private IEnumerator FadeAboveGround()
	{
		float assumedHeight = GetComponent<Collider>().bounds.size.CompMax();
		Rigidbody rb = GetComponent<Rigidbody>();
		Vector3 pos = rb.position;
		float end = pos.y;
		pos.y -= assumedHeight;
		
		rb.position = pos;	
        transform.position = pos;
        
		for (float y = pos.y; y <= end; y += Speed)
		{
			Vector3 p = rb.position;
			p.y = y;
			rb.position = p;
			yield return new WaitForFixedUpdate();
        }

		GetComponent<BaseAi>().SetState(BaseAi.AiState.Navigating);
//		OnFinish.Invoke();
		enabled = false;
	}
}

}