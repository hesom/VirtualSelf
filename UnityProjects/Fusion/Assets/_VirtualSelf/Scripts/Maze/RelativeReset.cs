using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf.Ballmaze
{
    public class RelativeReset : MonoBehaviour {
    
        public GameObject baseObject;
        public GameObject[] resetObjects;

        private Vector3[] posDiffs;
        private Quaternion[] rotDiffs;

        // Use this for initialization
        void Start () {
            posDiffs = new Vector3[resetObjects.Length];
            rotDiffs = new Quaternion[resetObjects.Length];
            for (int i=0;i<resetObjects.Length;i++)
            {
                GameObject go = resetObjects[i];
                posDiffs[i] = baseObject.transform.position - go.transform.position;
                rotDiffs[i] = Quaternion.Inverse(baseObject.transform.rotation) * go.transform.rotation;
            }
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        public void ResetAll()
        {
            for (int i = 0; i < resetObjects.Length; i++)
            {
                GameObject go = resetObjects[i];
                Rigidbody rb = go.GetComponent<Rigidbody>();

//                Vector3 pos = rb == null ? go.transform.position : rb.position;
                Vector3 newPos = baseObject.transform.position - (baseObject.transform.rotation * posDiffs[i]);

                if (rb == null)
                {
                    go.transform.position = newPos;
                }
                else
                {
                    rb.position = newPos;
                    rb.velocity = rb.angularVelocity = Vector3.zero;
                }
            
                //TODO this assumes there is no initial difference in the rotations

                /*
            Quaternion rot = rb == null ? go.transform.rotation : rb.rotation;
            Quaternion newRot = baseObject.transform.rotation * rotDiffs[i];
            if (rb == null) go.transform.rotation = newRot; else rb.transform.rotation = newRot;
            */
            }
        
        }
    }
}