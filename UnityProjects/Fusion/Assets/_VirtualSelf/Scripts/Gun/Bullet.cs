using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VirtualSelf
{


public class Bullet : MonoBehaviour
{
	private static readonly Bullet[] Bullets = new Bullet[20];
	private static int _i;

	[Tooltip("Visualize the ray for collision detection")]
	public bool DebugHit;
	public bool KeepBulletAfterHit;
	public GameObject metalHitEffect;
	public GameObject sandHitEffect;
	public GameObject stoneHitEffect;
	public GameObject waterLeakEffect;
	public GameObject[] fleshHitEffects;
	public GameObject woodHitEffect;
	
	private Rigidbody rb;
	private Vector3 lastPos;
	private Vector3 preLastPos;
	private bool doneColliding;
	
	// Use this for initialization
	void Start () {
		if (Bullets[_i] != null) Destroy(Bullets[_i].gameObject);
		Bullets[_i] = this;
		_i = (_i+1) % Bullets.Length;

		rb = GetComponent<Rigidbody>();
		Invoke("Destroy", 30); // destroy if no collision
	}

	void Destroy()
	{
		Destroy(gameObject);
	}

	void FixedUpdate()
	{
		if (!rb.isKinematic)
		{
			preLastPos = lastPos;
			lastPos = rb.position;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision other)
	{
		if (doneColliding) return;
		doneColliding = true;
		
//		GetComponent<Renderer>().enabled = false;
		
		CancelInvoke("Destroy");
		if (DebugHit) Debug.Log(name+" collision with "+other.gameObject.name);
		
		if (other.rigidbody != null) gameObject.transform.SetParent(other.transform);
		
		// collision position might not be on surface
		
		// determine last air position of the bullet: either the last or second to last one
		Bounds bl = new Bounds {center = lastPos};
		Vector3 lastAirPos = other.collider.bounds.Intersects(bl) ? preLastPos : lastPos;
		
		// determine surface hit position with ray from last air position towards collision position 
		RaycastHit hit = new RaycastHit();
		bool h = Physics.Raycast(lastAirPos, (other.contacts[0].point-lastAirPos), out hit, 10);
		// if the ray hit this bullet itself, the bullet was already perfectly placed and we can ignore the ray
		bool fallback = !h || hit.collider.gameObject == gameObject;
		Vector3 correctedHitPos = fallback ? other.contacts[0].point : hit.point;
		
		CloneDebug(lastAirPos, Color.yellow);
		CloneDebug(other.contacts[0].point, Color.red);
		
		// spawn a decal if a material was found for the surface, otherwise just keep the bullet in place
		bool decal = fallback
			? SpawnDecals(other.collider, other.contacts[0].point, other.contacts[0].normal)
			: SpawnDecals(hit.collider, hit.point, hit.normal);
		if (decal)
		{
			if (DebugHit) Debug.Log("Decal spawned");
			// decal was spawned, let bullet deflect
			if (KeepBulletAfterHit)
			{
//				float s = .1f;
//				transform.localScale = new Vector3(s,s,s);
				GetComponent<Renderer>().enabled = true;
				rb.angularDrag = .1f;
//				rb.drag = .1f;
			}
			else
			{
//				rb.isKinematic = true;
//				GetComponent<Collider>().enabled = false;
				Destroy();
			}
		}
		else
		{
			// freeze bullet in place
			rb.isKinematic = true;
			GetComponent<Collider>().enabled = false;
			rb.position = correctedHitPos;
			GetComponent<Renderer>().enabled = true;
		}
	}

	void CloneDebug(Vector3 pos, Color c)
	{
		if (DebugHit) {
			GameObject clone = Instantiate(gameObject, pos, transform.rotation, transform.parent);
			clone.GetComponent<Renderer>().material.color = c;
			clone.GetComponent<Bullet>().doneColliding = true;
			clone.GetComponent<Rigidbody>().isKinematic = true;
			clone.GetComponent<Collider>().enabled = false;
		}
		
	}

	bool SpawnDecals(Collider col, Vector3 pos, Vector3 normal)
	{
		if (col.sharedMaterial == null) return false;
		
		switch(col.sharedMaterial.name)
		{
			case "Metal":
				SpawnDecal(col, pos, normal, metalHitEffect);
				break;
			case "Sand":
				SpawnDecal(col, pos, normal, sandHitEffect);
				break;
			case  "Stone":
				SpawnDecal(col, pos, normal, stoneHitEffect);
				break;
			case "WaterFilled":
				SpawnDecal(col, pos, normal, waterLeakEffect);
				SpawnDecal(col, pos, normal, metalHitEffect);
				break;
			case "Wood":
				SpawnDecal(col, pos, normal, woodHitEffect);
				break;
			case "Meat":
				SpawnDecal(col, pos, normal, fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]);
				break;
			default:
				return false;
		}

		return true;
	}
	
	void SpawnDecal(Collider col, Vector3 pos, Vector3 normal, GameObject prefab)
	{
		GameObject spawnedDecal = GameObject.Instantiate(prefab, pos, Quaternion.LookRotation(normal));
		spawnedDecal.transform.SetParent(col.transform);
	}
}
	
}
