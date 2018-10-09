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
	private Vector3[] _attackSlots;
	private Vector3[] _rangedAttackSlots;
	
	// Use this for initialization
	void Start ()
	{
		_max = Health;
		_core = transform.Find("cores").gameObject;
		
		Transform rings = transform.Find("rings");
		_rings = new GameObject[rings.childCount];
		for (int i = 0; i < _rings.Length; i++) _rings[i] = rings.GetChild(i).gameObject;

		_attackSlots = ChildPositions("attack slots");
		_rangedAttackSlots = ChildPositions("ranged attack slots");
		
		_postStart = true;
	}

	private Vector3[] ChildPositions(string cname)
	{
		Transform slots = transform.Find(cname);
		Vector3[] attackSlots = new Vector3[slots.childCount];
		for (int i = 0; i < attackSlots.Length; i++) attackSlots[i] = slots.GetChild(i).position;
		return attackSlots;
	}

	void OnValidate()
	{
		if (gameObject.activeInHierarchy && _postStart)
		{
			CheckHealth();
		}
	}

	public void TakeDamage(float amount)
	{
		Health -= amount;
		CheckHealth();
	}

	public Vector3 RandomAttackSlot()
	{
		return _attackSlots[Random.Range(0, _attackSlots.Length)];
	}

	public Vector3 RandomRangedAttackSlot()
	{
		return _rangedAttackSlots[Random.Range(0, _rangedAttackSlots.Length)];
	}

	private void CheckHealth()
	{
		if (!enabled) return;
		
		if (Health < 0) Health = 0;
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