using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToStartPositionOnFloor : MonoBehaviour {

    private Vector3 startPos;
    private Quaternion startRot;
    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
        startRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Floor")
        {
            transform.position = startPos;
            transform.rotation = startRot;
            rb.velocity = Vector3.zero;
        }
    }
}
