using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaybeLoadFireGesture : MonoBehaviour {

    private void Start()
    {
        var globalGameState = GameObject.FindGameObjectWithTag("GlobalGameState")?.GetComponent<GlobalFireGestureState>();
        if(globalGameState != null)
        {
            if (!globalGameState.fireGestureDiscovered)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            throw new System.Exception("No GlobalGameState object for fire gesture found");
        }
    }
}
