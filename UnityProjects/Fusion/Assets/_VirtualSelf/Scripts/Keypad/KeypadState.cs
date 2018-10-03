using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class KeypadState : MonoBehaviour {

    public GameObject brokenKey;
    public GameObject unbrokenKey;
    public Anchor anchor;
    public AnchorableBehaviour anchorable;

    private static bool isComplete = false;

	// Use this for initialization
	void Start () {
        if (isComplete)
        {
            brokenKey.SetActive(false);
            unbrokenKey.SetActive(true);
            anchor.gameObject.SetActive(false);
            anchorable.gameObject.SetActive(false);
        }
	}
	

    public void MakeComplete()
    {
        isComplete = true;
    }
}
