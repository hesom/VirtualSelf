using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearLaserAnimator : MonoBehaviour {

    public Vector3 fromOffset = new Vector3(0.0F, 45.0F, 0.0F);
    public Vector3 toOffset = new Vector3(0.0F, -45.0F, 0.0F);
    public float m_frequency = 1.0F;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * this.m_frequency));
        this.transform.position = initialPosition + Vector3.Lerp(fromOffset, toOffset, lerp);
    }
}
