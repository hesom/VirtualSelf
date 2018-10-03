using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    public ParticleSystem hitEffectParticles;

    private LineRenderer lr;

	// Use this for initialization
	void Start () {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
	}
	
	// Update is called once per frame
	void Update () {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.right, out hit))
        {
            if (hit.collider)
            {
                lr.SetPosition(1, hit.point);
                hitEffectParticles.gameObject.SetActive(true);
                hitEffectParticles.transform.position = hit.point;
                hitEffectParticles.transform.forward = hit.normal;
            }
        }
        else
        {
            lr.SetPosition(1, transform.right * 5000);
            hitEffectParticles.gameObject.SetActive(false);
        }
	}
}
