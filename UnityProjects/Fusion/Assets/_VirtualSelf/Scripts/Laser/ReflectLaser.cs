using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReflectLaser : MonoBehaviour
{

    public ParticleSystem hitEffectParticles;
    public ParticleSystem reflectParticles;

    public UnityEvent OnTargetHit;
    private bool targetEverHit = false;

    private LineRenderer lr;

    // Use this for initialization
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit))
        {
            if (hit.collider)
            {
                lr.SetPosition(1, hit.point);
                hitEffectParticles.gameObject.SetActive(true);
                hitEffectParticles.transform.position = hit.point;
                hitEffectParticles.transform.forward = hit.normal;

                if(hit.collider.tag == "LaserReflector")
                {
                    lr.positionCount = 3;
                    Vector3 prevHitPos = hit.point;
                    Vector3 reflectedHitDirection = Vector3.Reflect(transform.right, hit.normal).normalized;
                    if (Physics.Raycast(hit.point, reflectedHitDirection, out hit))
                    {
                        lr.SetPosition(2, hit.point);
                        hitEffectParticles.transform.position = hit.point;
                        hitEffectParticles.transform.forward = hit.normal;

                        if(!targetEverHit && hit.collider.tag == "LaserTarget")
                        {
                            OnTargetHit.Invoke();
                            Debug.Log("Target Hit");
                            targetEverHit = true;
                        }
                    }
                    else
                    {
                        lr.SetPosition(2, reflectedHitDirection * 5000);
                    }
                    reflectParticles.transform.position = prevHitPos + 0.1f*reflectedHitDirection;
                    reflectParticles.transform.forward = reflectedHitDirection;
                    reflectParticles.gameObject.SetActive(true);
                }
                else
                {
                    lr.positionCount = 2;
                    reflectParticles.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            lr.SetPosition(1, transform.right * 5000);
            hitEffectParticles.gameObject.SetActive(false);
        }
    }
}
