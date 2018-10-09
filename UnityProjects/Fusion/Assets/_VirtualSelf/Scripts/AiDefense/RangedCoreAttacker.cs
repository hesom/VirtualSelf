using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap.Unity;
using Leap.Unity.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace VirtualSelf
{

[RequireComponent(typeof(BaseAi))]
public class RangedCoreAttacker : MonoBehaviour
{
    private static readonly Vector3[] RecentSniperPositions = new Vector3[5];
    private static int _recentSniperPositionsI;
    
    public float Damage = 1;
    public float Velocity = 1;
    [MinMax(0.1f, 20)]
    public Vector2 ShootInterval;
    public Transform BulletSpawn;
    public ParticleSystem[] FireEffects;
    public GameObject BulletPathPrefab;
    public GameObject Bullet;
    public int BulletPathFadeInTicks = 100;
    public int BulletPathFadeoutTicks = 100;
    
    private CoreHealth _core;
    private NavMeshAgent _navMeshAgent;

    public void Init(CoreHealth core, GetRandomChildAttribute sniperLocations)
    {
        _core = core;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        BaseAi b = GetComponent<BaseAi>();
        b.OnIdleExit += () =>
        {
            // move to a random sniper position, unless it is in the list of recent positions
            // this history is not cleared up, unless it completely fills up
            Vector3 p = Vector3.zero;
            bool match = false;
            for (int i = 0; i < RecentSniperPositions.Length; i++)
            {
                p = sniperLocations.RandomPos();
                if (!RecentSniperPositions.Contains(p))
                {
                    RecentSniperPositions[_recentSniperPositionsI] = p;
                    _recentSniperPositionsI = (_recentSniperPositionsI + 1) % RecentSniperPositions.Length;
                    match = true;
                    break;
                }
            }
            
            if (!match) RecentSniperPositions.ClearWithDefaults();

            _navMeshAgent.SetDestination(p);
        };
        b.OnArrive += () => StartCoroutine(StartFiring());
        b.OnDyingFall += StopAllCoroutines;
    }

    private IEnumerator StartFiring()
    {
        while (true)
        {
            // get random target
            Vector3 target = _core.RandomRangedAttackSlot();
            Vector3 lookDir = (target - BulletSpawn.position).normalized;
        
            // rotate ai towards shooting direction
            _navMeshAgent.enabled = false;
            TryStop(_rotateXZTowards);
            _rotateXZTowards = StartCoroutine(RotateXZTowards(target, lookDir));
            
            yield return new WaitForSeconds(Random.Range(ShootInterval.x, ShootInterval.y));
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private void Fire(Vector3 target) 
    {
        // position cylinder to visualize bullet path
        GameObject bulletPath = SpawnCylinder(BulletPathPrefab, BulletSpawn.position, target);
        StartCoroutine(FadeAlpha(bulletPath, true, BulletPathFadeInTicks));
        
        // spawn bullet and set velocity towards target
        Vector3 lookDir = (target - BulletSpawn.position).normalized;
        Quaternion rot = Quaternion.LookRotation(lookDir) * Bullet.transform.rotation;
        GameObject bullet = Instantiate(Bullet, BulletSpawn.position, rot);
        bullet.GetComponent<AiBullet>().Init(this, _core, bulletPath);
        bullet.GetComponent<Rigidbody>().velocity = lookDir.normalized * Velocity;
        Destroy(bullet, 30);
        
        PlayEffects(FireEffects);
    }

    private void TryStop(Coroutine r)
    {
        if (r != null) StopCoroutine(r);
    }

    public static IEnumerator FadeAlpha(GameObject g, bool fadeIn, int physicsTicks)
    {
        Renderer r = g.GetComponent<Renderer>();
        Color start = r.material.color;
        
        for (float i = fadeIn ? 0 : start.a; 
            fadeIn ? i <= start.a : i >= 0; 
            i += (fadeIn ? 1f : -1f) / physicsTicks)
        {
            r.material.color = start.WithAlpha(i);
            yield return new WaitForFixedUpdate();
        }

        r.material.color = start;
        if (!fadeIn) Destroy(g);
    }
    
    private Coroutine _rotateXZTowards;
    private IEnumerator RotateXZTowards(Vector3 target, Vector3 direction) 
    {
        Quaternion start = transform.rotation;
        direction.y = 0;
        Quaternion end = Quaternion.LookRotation(direction);
        Debug.Log($"rotate from {start.eulerAngles} to {end.eulerAngles}");
        int physicsTicks = 60;

        for (float i = 0; i <= 1; i += 1f / physicsTicks) {
            transform.rotation = Quaternion.Slerp(start, end, i);
            yield return new WaitForFixedUpdate();
        }
        
        // this is where the magic happens
        Fire(target);
    }

    private void PlayEffects(IEnumerable<ParticleSystem> arr)
    {
        foreach (ParticleSystem effect in arr)
        {
            effect.Play();
        }
    }

    private static GameObject SpawnCylinder(GameObject cylinder, Vector3 start, Vector3 end)
    {
        Vector3 center = Vector3.Lerp(start, end, .5f);
        Vector3 objectVector = end-start;
        float length = objectVector.magnitude;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, objectVector.normalized);
        GameObject cyl = Instantiate(cylinder, center, rot);
        Vector3 scale = cyl.transform.localScale;
        scale.y = length/2f;
        cyl.transform.localScale = scale;
        return cyl;
    }
}

}