using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualSelf;

public class AiBullet : MonoBehaviour
{
	public ParticleSystem Explosion;
	
	private float _dmg;
	private CoreHealth _core;
	private string _sourceName;
	private GameObject _path;
	private int _fadeTicks;

	public void Init(RangedCoreAttacker attacker, CoreHealth core, GameObject path)
	{
		// take everything we need out of the attacker, since it may be destroyed before the bullet hits anything
		_dmg = attacker.Damage;
		_core = core;
		_sourceName = attacker.name;
		_path = path;
		_fadeTicks = attacker.BulletPathFadeoutTicks;
	}

	void OnCollisionEnter(Collision other)
	{
		GameObject o = other.gameObject;
		Debug.Log($"{_sourceName}'s bullet collided with {o.name}");
	
		// bullet hits core -> damage
		if (o.CompareTag("Core"))
		{
			_core.TakeDamage(_dmg);
		}

		Destroy(Explosion.gameObject, Explosion.main.duration);
		Explosion.transform.parent = null;
		Explosion.Play();
		
		// destroy the path visualizer
		_path.transform.parent = null;
		_path.GetComponent<RemoteCoroutine>().StartCoroutine(RangedCoreAttacker.FadeAlpha(_path, false, _fadeTicks));
		
		// bullet hits player, shield, some enemy, etc. -> despawn after a moment
		Destroy(gameObject, .05f);
	}
}
