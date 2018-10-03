using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Leap.Unity.Interaction;

public class KeypadCompleted : MonoBehaviour {

    public UnityEvent OnKeypadCompleted;

    private int missingKeyCount;

	// Use this for initialization
	void Start () {
        var missingKeys = GetComponentsInChildren<AnchorableBehaviour>();
        missingKeyCount = missingKeys.Length;
	}

    public void KeySetIn()
    {
        missingKeyCount--;
        if(missingKeyCount == 0)
        {
            OnKeypadCompleted.Invoke();
        }
        if(missingKeyCount < 0)
        {
            throw new System.Exception("More keys set into tutorial keypad than there should have been in the scene!");
        }
    }
}
