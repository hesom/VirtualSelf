using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour {

    public float maxIntensity = 1.0f;
    public float minIntensity = 0.0f;
    public float pulseSpeed = 1.0f;

    private float targetIntensity;
    private float currentIntensity;

    private Light alarmLight;

    private bool alarmOn = false;
	// Use this for initialization
	void Start () {
        alarmLight = GetComponent<Light>();
        targetIntensity = maxIntensity;
	}
	
	// Update is called once per frame
	void Update () {
        if (alarmOn)
        {
            currentIntensity = Mathf.MoveTowards(alarmLight.intensity, targetIntensity, Time.deltaTime * pulseSpeed);
            if (currentIntensity >= maxIntensity)
            {
                currentIntensity = maxIntensity;
                targetIntensity = minIntensity;
            }
            else if (currentIntensity <= minIntensity)
            {
                currentIntensity = minIntensity;
                targetIntensity = maxIntensity;
            }
            alarmLight.intensity = currentIntensity;
        }
	}

    public void AlarmOn()
    {
        alarmOn = true;
    }

    public void AlarmOff()
    {
        alarmOn = false;
        alarmLight.intensity = minIntensity;
    }
}
