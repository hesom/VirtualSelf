using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
	
public class ScopeFade : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private float _speed = .04f;

	public void StartFadeIn()
	{
		gameObject.SetActive(true);
		StopAllCoroutines();
		StartCoroutine(FadeIn());
	}
	
	public void StartFadeOut()
	{
		StopAllCoroutines();
		if (gameObject.activeSelf) StartCoroutine(FadeOut());
	}

	public IEnumerator FadeOut()
	{
		float initialScale = transform.localScale.x;
		for (float scale = initialScale; scale > 1e-2; scale /= 2)
		{
			transform.localScale = Vector3.one * scale;
			yield return new WaitForSeconds(_speed);
		}

		Hide();
	}

	public void Hide()
	{
		transform.localScale = Vector3.one * 1e-2f;
		gameObject.SetActive(false);
	}
	public void Show()
	{
		transform.localScale = Vector3.one;
		gameObject.SetActive(true);
	}
	
	public IEnumerator FadeIn()
	{
		float initialScale = transform.localScale.x;
		for (float scale = initialScale; scale < 1; scale *= 2)
		{
			transform.localScale = Vector3.one * Mathf.Max(1, scale);
			yield return new WaitForSeconds(_speed);
		}
	}
}
	
}
