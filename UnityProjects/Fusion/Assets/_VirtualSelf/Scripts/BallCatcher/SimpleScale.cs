using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HingeJoint))]
public class SimpleScale : MonoBehaviour
{

    private HingeJoint joint;

    public bool greaterThan = true;
    public float angle = 0f;

    private bool fired = false;

    public UnityEvent OnScaleTipped;

    void Awake()
    {
        joint = GetComponent<HingeJoint>();
    }


    void Update()
    {
        if (!fired)
        {
            if (greaterThan)
            {
                if (joint.angle > angle)
                {
                    Debug.Log("Joint angle greater than " + angle);
					OnScaleTipped.Invoke();
                    fired = true;
                }
            }
            else
            {
                if (joint.angle < angle)
                {
                    Debug.Log("Joint angle smaller than " + angle);
					OnScaleTipped.Invoke();
                    fired = true;
                }
            }
        }

    }
}
