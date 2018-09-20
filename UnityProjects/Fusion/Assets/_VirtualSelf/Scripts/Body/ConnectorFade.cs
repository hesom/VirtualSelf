using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
	
public class ConnectorFade : MonoBehaviour
{
	public enum DebugCommand {Nothing, FadeOut, FadeIn}
	
	public CollisionConnector CollisionConnector;
	public float WaitForSeconds = .01f;
	public float StepMultiplicative = .1f;
	public float StepAdditive = .01f;
	public DebugCommand Debug;

	private bool _postStart;
	private DebugCommand _usedCommand;
	private const float Min = 0.001f;

	// Use this for initialization
	void Start ()
	{
		_postStart = true;
		CollisionConnector.ScaleBias = Min;
		CollisionConnector.enabled = false;
	}
	
	public void FadeOut()
	{
		StopAllCoroutines();
		StartCoroutine(StartFadeOut());
	}
	
	public void FadeIn()
	{
		StopAllCoroutines();
		StartCoroutine(StartFadeIn());
	}

	private IEnumerator StartFadeOut()
	{
		for (float s = CollisionConnector.ScaleBias; s >= Min; s = s*(1-StepMultiplicative) - StepAdditive)
		{
			s = Mathf.Max(Min, s);
			CollisionConnector.ScaleBias = s;
			yield return new WaitForSeconds(WaitForSeconds);
		}

		CollisionConnector.enabled = false;
	}
	
	private IEnumerator StartFadeIn()
	{
		CollisionConnector.enabled = true;
		
		for (float s = CollisionConnector.ScaleBias; s <= 1; s = s* (1+StepMultiplicative) + StepAdditive)
		{
			s = Mathf.Min(1, s);
			CollisionConnector.ScaleBias = s;
			yield return new WaitForSeconds(WaitForSeconds);
		}
	}

	void OnValidate()
	{
		if (_postStart)
		{
			if (_usedCommand != Debug)
			{
				if (Debug == DebugCommand.FadeIn) FadeIn();
				else if (Debug == DebugCommand.FadeOut) FadeOut();
				
				_usedCommand = Debug;
			}
		}
	}
}
	
}
