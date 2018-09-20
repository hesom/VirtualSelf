using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

[RequireComponent(typeof(InteractionBehaviour))]
public class BodyMover : MonoBehaviour {

    //public GameObject handlesGroup;
    public Rigidbody handle;

    //private bool disabled = false;

    private InteractionBehaviour intObj;
    private Vector3 offsetT;
    private Quaternion offsetR;

    public bool Disabled { get; set; }

    void Start() {

        intObj = GetComponent<InteractionBehaviour>();

        intObj.OnContactBegin += ContactBegins;
        intObj.OnContactStay += ContactStays;
        intObj.OnContactEnd += ContactEnds;

        /*
        offsetT = new Vector3[handles.Length];
        offsetR = new Quaternion[handles.Length];
        for (int i=0;i<handles.Length;i++) {
            offsetT[i] = handles[i].transform.localPosition;
            offsetR[i] = handles[i].transform.localRotation;
        }*/
        offsetT = handle.transform.localPosition;
        offsetR = handle.transform.localRotation;
    }

    void FixedUpdate() {

        if (Disabled) {
            return;
        }
        

        /*
        //handlesGroup.transform.position = transform.position;
        //handlesGroup.transform.rotation = transform.rotation;
        for (int i = 0;i<handles.Length;i++) {
            handles[i].position = offsetT[i]+transform.position;
            handles[i].rotation = transform.rotation;
        }*/

        handle.position = offsetT + transform.position;
        handle.rotation = transform.rotation * offsetR;
    }

    void ContactBegins() {

        //Debug.Log(this.gameObject.name + ": Contact begins.");
    }

    void ContactStays() {

        // Debug.Log(this.gameObject.name + ": Is in contact...");
    }

    void ContactEnds() {

        //Debug.Log(this.gameObject.name + ": Contact ends.");
    }
}
