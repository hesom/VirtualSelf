using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnPortal : MonoBehaviour {

    public Transform portalMesh;
    public float fadeDuration = 2.0f;
    private Vector3 initialPosition;
	// Use this for initialization
	void Start () {
        initialPosition = portalMesh.position;
	}

    IEnumerator PortalDown()
    {
        float t = 0;
        while(t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Lerp(0.0f, 2.0f, t);
            portalMesh.position = initialPosition - lerp * Vector3.down;
            yield return null;
        }

        Destroy(transform.parent.parent.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(PortalDown());
        }
    }
}
