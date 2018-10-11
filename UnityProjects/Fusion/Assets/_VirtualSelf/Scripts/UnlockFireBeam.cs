using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockFireBeam : MonoBehaviour {

    private GlobalFireGestureState globalGameState;
    private void Start()
    {
        globalGameState = GameObject.FindGameObjectWithTag("GlobalGameState")?.GetComponent<GlobalFireGestureState>();
        if(globalGameState == null)
        {
            throw new System.Exception("No GlobalGameState object for fire gesture found");
        }
    }

    public void GetFire()
    {
        globalGameState.fireGestureDiscovered = true;
    }
}
