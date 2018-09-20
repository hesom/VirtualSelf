using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace VirtualSelf
{
	
public class Spawner : MonoBehaviour
{

//	enum EnemyState
//	{
//		PathToIntermediate, PathToPlayer, RagdollPhysics
//	}

	public float SpawnInterval = 5;
	public LocationProvider LocationProvider;
	public GameObject EnemyPrefab;
	
	List<NavMeshAgent> activeAgents;
	List<NavMeshAgent> removals;
	float lastSpawn;
	int nextId;
	
	// Use this for initialization
	void Start ()
	{
		activeAgents = new List<NavMeshAgent>();
		removals = new List<NavMeshAgent>();
//		StartCoroutine(Spawn());
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach (var a in removals)
		{
//			Destroy(a.gameObject);
			activeAgents.Remove(a);
//			RemoveAgent(a.gameObject, false);
		}
		removals.Clear();
		
		if (Time.time > lastSpawn + SpawnInterval)
		{
			lastSpawn = Time.time;
			GameObject enemy = Instantiate(EnemyPrefab, LocationProvider.RandomSpawnPos(), Quaternion.identity);
//			NavMeshAgent agent = ;
//			EnemyCollider ec = ;
			enemy.name += " "+nextId++;
			enemy.GetComponent<EnemyCollider>().Init(this, LocationProvider);
//			ec.State = EnemyCollider.EnemyState.PathToIntermediate;
//			agent.SetDestination(LocationProvider.RandomTargetPos());
			activeAgents.Add(enemy.GetComponent<NavMeshAgent>());
		}
		

		
//		List<NavMeshAgent> removals = new List<NavMeshAgent>();
//		List<NavMeshAgent> setPathToPlayer = new List<NavMeshAgent>();
		foreach (NavMeshAgent a in activeAgents)
		{
			a.GetComponent<EnemyCollider>().UpdateAi();
		}


	}

//	public void PauseAgent(GameObject a)
//	{
////		a.GetComponent<NavMeshAgent>().isStopped = true;
//		a.GetComponent<NavMeshAgent>().enabled = false;
////		removals.Add(a.GetComponent<NavMeshAgent>());
////		activeAgents.Remove(a.GetComponent<NavMeshAgent>());
//	}
	
	

	public void RemoveAgent(GameObject a, bool violent)
	{
		removals.Add(a.GetComponent<NavMeshAgent>());
//		activeAgents.Remove(a.GetComponent<NavMeshAgent>());
//		a.GetComponent<Renderer>().material.color = violent ? Color.red : Color.blue;
//		StartCoroutine(a.GetComponent<EnemyCollider>().FadeDown());
//		a.GetComponent<NavMeshAgent>().isStopped = true;
//		Destroy(a, 2);
	}



//	IEnumerator Spawn()
//	{
//		while (true)
//		{
//			yield return new WaitForSeconds(SpawnInterval);
//			GameObject enemy = Instantiate(EnemyPrefab, LocationProvider.RandomSpawnPos(), Quaternion.identity);
//			NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
//			agent.SetDestination(LocationProvider.RandomTargetPos());
//			activeAgents.Add(agent, EnemyState.PathToIntermediate);
//		}
//	}
}
	
}
