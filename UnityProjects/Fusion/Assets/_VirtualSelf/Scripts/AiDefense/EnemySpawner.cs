using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Attributes;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace VirtualSelf
{

[RequireComponent(typeof(GetRandomChildAttribute))]
public class EnemySpawner : MonoBehaviour
{
	[Serializable]
	public class SpawnClass
	{
		public GameObject Prefab;
		[MinMax(0.1f, 60f)]
		public Vector2 SpawnInterval;
	}
	
	[Serializable]
	public class KillThresholds
	{
		public int KillCount;
		public UnityEvent OnEquals;
		[NonSerialized] public bool _cleared;
	}
	
	[Disable] public int SpawnCount;
	[Disable] public int KillCount;
	public CoreHealth Core;
	public GetRandomChildAttribute SniperLocations;
	public bool DebugMode;
	public SpawnClass[] Classes;
	public KillThresholds[] KillEvents;
	
	private GetRandomChildAttribute _random;
	private List<BaseAi> _activeEnemies;
	private bool _stopWasCalled;
	
	// Use this for initialization
	void Start ()
	{
		_random = GetComponent<GetRandomChildAttribute>();
		_activeEnemies = new List<BaseAi>();
		
		if (!DebugMode) StartAll();
		else Debug.Log($"{nameof(EnemySpawner)} debug mode is active, use keys U and to the right to spawn enemies manually");
	}

	// only used in debug mode
	void Update()
	{
		if (!DebugMode) return;

		int clazz = 0;
		if (Input.GetKeyDown(KeyCode.U)) clazz = 0;
		else if (Input.GetKeyDown(KeyCode.I)) clazz = 1;
		else if (Input.GetKeyDown(KeyCode.O)) clazz = 2;
		else if (Input.GetKeyDown(KeyCode.P)) clazz = 3;
		else
		{
			return;
		}
		
		SpawnClass c = Classes?[clazz];
		Debug.Log($"Debug Spawn {clazz} {c?.Prefab.name}");
		if (c == null) return;
		
		GameObject enemy = _random.RandomInstantiate(c.Prefab);
		
		BaseAi b = enemy.GetComponent<BaseAi>();
		AddEnemy(b);
		if (b != null) b.OnDeath += () => IncrementKills(b);
		enemy.GetComponent<MeleeCoreAttacker>()?.Init(Core);
		enemy.GetComponent<RangedCoreAttacker>()?.Init(Core, SniperLocations);
	}

	public void StartAll()
	{
		StopAll();
		foreach (var c in Classes)
		{
			StartCoroutine(Spawner(c));
		}
	}

	public void StopAll()
	{
		_stopWasCalled = true;
		StopAllCoroutines();
		foreach (BaseAi activeEnemy in _activeEnemies)
		{
			if (activeEnemy == null) continue;
			activeEnemy.SetState(BaseAi.AiState.Idle);
		}
		Invoke(nameof(KillRemaining), 2.5f);
	}

	private void KillRemaining()
	{
		foreach (BaseAi activeEnemy in _activeEnemies)
		{
			if (activeEnemy == null) continue;
			activeEnemy.SetState(BaseAi.AiState.DyingFall);
		}
	}

	private void IncrementKills(BaseAi victim)
	{
		if (_stopWasCalled) return; // stop counting kills after we lost
		
		KillCount++;
		_activeEnemies.Remove(victim);
		foreach (var ke in KillEvents)
		{
			if (!ke._cleared && KillCount >= ke.KillCount)
			{
				ke._cleared = true;
				ke.OnEquals.Invoke();
			}
		}
	}

	private void AddEnemy(BaseAi enemy)
	{
		SpawnCount++;
		_activeEnemies.Add(enemy);
	}
	
	private IEnumerator Spawner(SpawnClass c)
	{
		Debug.Log("Starting spawner for "+c.Prefab.name);
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(c.SpawnInterval.x, c.SpawnInterval.y));
			GameObject enemy = _random.RandomInstantiate(c.Prefab);
			
			BaseAi b = enemy.GetComponent<BaseAi>();
			AddEnemy(b);
			if (b != null) b.OnDeath += () => IncrementKills(b);
			enemy.GetComponent<MeleeCoreAttacker>()?.Init(Core);
			enemy.GetComponent<RangedCoreAttacker>()?.Init(Core, SniperLocations);
		}
		// ReSharper disable once IteratorNeverReturns
	}
}

}