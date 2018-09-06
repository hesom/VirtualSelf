using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKeypad : MonoBehaviour
{

    public InputLogic inputLogic;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1)) inputLogic.PressKey(Key.K1);
		if (Input.GetKeyDown(KeyCode.Alpha2)) inputLogic.PressKey(Key.K2);
		if (Input.GetKeyDown(KeyCode.Alpha3)) inputLogic.PressKey(Key.K3);
		if (Input.GetKeyDown(KeyCode.Alpha4)) inputLogic.PressKey(Key.K4);
		if (Input.GetKeyDown(KeyCode.Alpha5)) inputLogic.PressKey(Key.K5);
		if (Input.GetKeyDown(KeyCode.Alpha6)) inputLogic.PressKey(Key.K6);
		if (Input.GetKeyDown(KeyCode.Alpha7)) inputLogic.PressKey(Key.K7);
		if (Input.GetKeyDown(KeyCode.Alpha8)) inputLogic.PressKey(Key.K8);
		if (Input.GetKeyDown(KeyCode.Alpha9)) inputLogic.PressKey(Key.K9);
		if (Input.GetKeyDown(KeyCode.Alpha0)) inputLogic.PressKey(Key.K0);
		if (Input.GetKeyDown(KeyCode.Asterisk)) inputLogic.PressKey(Key.KStar);
		if (Input.GetKeyDown(KeyCode.Hash)) inputLogic.PressKey(Key.KHash);
	}

    public void Print(string s)
    {
        Debug.Log("keypad sequence: "+s);
    }
}
