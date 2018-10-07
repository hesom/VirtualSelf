using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Leap;
using Leap.Unity.Attributes;
using UnityEngine;
using VirtualSelf;

namespace VirtualSelf
{
	
[RequireComponent(typeof(BaseAi))]
public class DamageSetup : MonoBehaviour {

	[Serializable]
	public class DamageClass
	{
		public string Tag;
		public float Amount;
		public BaseAi.DamageEffect Effect;
		public float InvulnerableTime = .25f;
		[Disable]
		public float DPS;
		[NonSerialized] public float _lastDamageTime = -1;
	}

	public DamageClass[] Classes;

	private BaseAi _base;
	
	// Use this for initialization
	void Start ()
	{
		_base = GetComponent<BaseAi>();
	}

	void OnCollisionEnter(Collision other)
	{
		OnAllCollisions(other.gameObject, other);
	}

	void OnParticleCollision(GameObject other) {
		OnAllCollisions(other, null);
	}

	void OnValidate() {
		foreach (var c in Classes) {
			c.DPS = c.Amount / c.InvulnerableTime;
		}
	}

	private void OnAllCollisions(GameObject other, Collision col) {
		GameObject o = other.gameObject;
		foreach (var c in Classes)
		{
			if (o.CompareTag(c.Tag)) {
				if (Time.time > c._lastDamageTime + c.InvulnerableTime) {
					Debug.Log($"{name} taking damage from {c.Tag} with strength {col?.relativeVelocity.magnitude}");
					_base.TakeDamage(c.Amount, c.Effect);
					c._lastDamageTime = Time.time;
				}
				return;
			}
		}
	}
}

}