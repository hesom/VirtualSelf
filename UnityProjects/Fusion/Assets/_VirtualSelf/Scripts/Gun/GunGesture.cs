using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using Leap.Unity.Attachments;
using UnityEngine;

namespace VirtualSelf 
{

public class GunGesture : MonoBehaviour
{

	public ExtendedFingerDetector ThumbExtended;
	public ExtendedFingerDetector ThumbRetracted;
	public PalmDirectionDetector PalmFacingAway;
	public FingerDirectionDetector ThumbUp;
	public GameObject Bullet;
	public GameObject LoadedParticles;
	public AttachmentPointBehaviour IndexTip;
	public Transform BulletSpawnPos;
	public AttachmentPointBehaviour Palm;
//	public AttachmentPointBehaviour Wrist;
	public float BulletSpeed = 10;
	public GameObject MuzzleFlash;
	public GameObject CartridgeEject;
	public Vector3 BulletDir = Vector3.forward;
	public ScopeFade ScopeFade;
	
	private bool _reloaded;
	private GameObject _particleInstance;
	private bool _fading;
	private bool _unloadTimer;
	private float _unloadTimerStart;
	
	// Use this for initialization
	void Start () {
		UnloadUnscope(true);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.V)) ForcedFire();
		else if (ThumbUp.IsActive && PalmFacingAway.IsActive && ThumbExtended.IsActive) Load();
//		else if (ThumbExtended.IsActive) Load(); // for testing only
		else if (ThumbRetracted.IsActive) Fire();
		else if (_reloaded && !ThumbRetracted.IsActive && !ThumbExtended.IsActive)
		{
            Debug.Log("No thumb gesture active");
			// if neither load nor fire gesture is active for a while, unload again
			if (_unloadTimer && Time.time > _unloadTimerStart + .4f)
			{
//                Debug.LogError("unload timer completed");
				Unload();
				_unloadTimer = false;
			}
			else if (!_unloadTimer)
			{
//                Debug.LogWarning("Unload timer started");
				_unloadTimer = true;
				_unloadTimerStart = Time.time;
			}
		}
	}

	/// <summary>
	/// Load, Fire and clear particles all at once.
	/// For debugging only. 
	/// </summary>
	void ForcedFire()
	{
		Load();
		Fire();
		CancelInvoke("DestroyParticles");
		DestroyParticles();
	}

	void Load()
	{
		if (_reloaded) return;
		if (_fading) return;
		
		Debug.Log("Load");
		_reloaded = true;
		_particleInstance = Instantiate(LoadedParticles, IndexTip.transform);
		ScopeFade.StartFadeIn();
	}

	void FadeoutParticles()
	{
		if (_particleInstance != null)
		{
			_fading = true;
			ParticleSystem ps = _particleInstance.GetComponent<ParticleSystem>();
			if (ps != null) ps.Stop();
			if (_particleInstance.transform.childCount > 0)
			{
				ps = _particleInstance.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
				if (ps != null) ps.Stop();	
			}
			Invoke("DestroyParticles", 1.5f);
		}
	}

	void DestroyParticles()
	{
		if (_particleInstance != null)
		{
			Destroy(_particleInstance);
			_fading = false;
		}
	}

	void Unload()
	{
		if (!_reloaded) return;
		
		Debug.Log("Unload");
		_reloaded = false;
		_unloadTimer = false;
		FadeoutParticles();
	}
	
	public void UnloadUnscope()
	{
		UnloadUnscope(false);
	}
	
	public void UnloadUnscope(bool instant)
	{
		Unload();
		if (instant) ScopeFade.Hide(); else ScopeFade.StartFadeOut();
	}

	void Fire()
	{
		if (!_reloaded) return;
		
		Debug.Log("Fire");
		Unload();
		
		// # Projectile
		// For the projectile there are 2 possible approaches: a physically based bullet
		// and a simulated one via raycast. High velocity objects have some issues and 
		// for simulated real life weapons the bullet drop is negligable over short distances,
		// even for small bullets.
		// I'm not sure if we really want high velocity bullets, or which method fits better here,
		// For now, the physically based approach is used, though there are some issues with 
		// getting the exact intersection point between the bullet and objects, as it cannot
		// be stopped before it either deflects a bit or penetrates the object. 
		// Maybe the bullet can keep track of its previous position of each frame and then use a
		// raycast between that and the object it collided with to get the exact position.
		
		// # Trajectory
		// There are many conceivable configurations of bullet spawn position and direction.
		// Intuitively, the bullet should come out of the index finger, as it represents the barrel,
		// though this makes alignment with the scope difficult. I believe a real gun has the same
		// problem of different ray origin for a scope and the barrel, though in our case the offset
		// between them is even different in 2 dimensions. I also believe the only solution to this 
		// is to configure the intersection between the 2 rays at a fixed distance.
		// Though in the 1 dimension of a real gun this is mandatory anyways due to gravity.
		// Setting the spawn position behind the scope seems to work well though, since the bullet
		// path is not really visible anyways due to the high velocity.
		// For the direction, a real gun would not use the wrist, but rather the palm.
		// Using the wrist makes for the easiest aim, since every joint adds in one more possible rotation.
		// It would also be possible to use maybe the last finger joint as the direction, which would be
		// the hardest single joint direction, influenced by 2 finger joints, palm and wrist.
		// Even more difficult would be to use multiple joints and average between them.
		// For now, using the real world approach of using the palm will be used, especially since
		// it would be very straining to keep the index finger perfectly straight.
		GameObject bullet = Instantiate(Bullet, BulletSpawnPos.position, BulletSpawnPos.rotation);
		
//		Vector3 dir = Vector3.Normalize(Palm.transform.position - Wrist.transform.position);
		Vector3 dir = Palm.transform.rotation * BulletDir;
		bullet.GetComponent<Rigidbody>().velocity = dir * BulletSpeed;

		MuzzleFlash.GetComponent<ParticleSystem>().Play();
		CartridgeEject.GetComponent<ParticleSystem>().Play();
//		Instantiate(MuzzleFlash, IndexTip.transform);
//		Instantiate(CartridgeEject, IndexTip.transform);
	}
}
	
}
