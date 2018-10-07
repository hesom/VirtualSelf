using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using VirtualSelf;

namespace VirtualSelf
{
	
[RequireComponent(typeof(BaseAi))]
public class MeleeCoreAttacker : MonoBehaviour
{
	public float Damage = 1;
	public GameObject ExplisionPrefab;
	
	private CoreHealth _core;

	public void Init(CoreHealth core)
	{
		_core = core;
		BaseAi b = GetComponent<BaseAi>();
		b.OnIdleExit += () =>
		{
			GetComponent<NavMeshAgent>().SetDestination(core.RandomAttackSlot());
		};
		GetComponent<BaseAi>().OnArrive += DamageCore;
	}

	public void DamageCore()
	{
		Debug.Log(name+" damaged core");
		
		// explosion
		GameObject exp = Instantiate(ExplisionPrefab, transform.position, transform.rotation);
		Destroy(exp, exp.GetComponent<ParticleSystem>().main.duration);
		
		_core.TakeDamage(Damage);
		Destroy(gameObject);
	}
}

}