using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf
{

public class CoreHealth : MonoBehaviour
{
	public float Health = 1;
	public bool InvertRings;
	public UnityEvent OnDeath;
	
	private float _max;
	private GameObject[] _rings;
	private GameObject _core;
	private bool _postStart;
	
	// Use this for initialization
	void Start ()
	{
		_max = Health;
		_core = transform.Find("cores").gameObject;
		Transform rings = transform.Find("rings");
		_rings = new GameObject[rings.childCount];
		for (int i = 0; i < _rings.Length; i++) _rings[i] = rings.GetChild(i).gameObject;
		_postStart = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnValidate()
	{
		if (gameObject.activeInHierarchy && _postStart)
		{
			CheckHealth();
		}
	}

	private void CheckHealth()
	{
		int rings = Mathf.CeilToInt((Health / _max) * _rings.Length);
		for (int i = 0; i < _rings.Length; i++)
		{
			_rings[InvertRings ? _rings.Length-i-1 : i].SetActive(i < rings);
		}

		if (rings == 0)
		{
			OnDeath.Invoke();
			enabled = false;
			_core.SetActive(false);
		}
	}

	public void Reset()
	{
		enabled = true;
		Health = _max;
		CheckHealth();
		_core.SetActive(true);
	}
}

}