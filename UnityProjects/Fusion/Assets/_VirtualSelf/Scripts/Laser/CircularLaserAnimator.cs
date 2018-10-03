using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularLaserAnimator : MonoBehaviour {

    public Vector3 fromEuler = new Vector3(0.0F, 45.0F, 0.0F);
    public Vector3 toEuler = new Vector3(0.0F, -45.0F, 0.0F);
    public float frequency = 1.0F;

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        Quaternion from = Quaternion.Euler(fromEuler);
        Quaternion to = Quaternion.Euler(toEuler);

        float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * frequency));
        this.transform.localRotation = initialRotation * Quaternion.Lerp(from, to, lerp);
    }
}
