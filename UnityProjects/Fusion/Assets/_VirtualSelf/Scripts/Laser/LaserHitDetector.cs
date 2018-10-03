using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserHitDetector : MonoBehaviour {

    public UnityEvent OnPlayerHit;

    public void NotifyHit()
    {
        OnPlayerHit.Invoke();
    }
}
