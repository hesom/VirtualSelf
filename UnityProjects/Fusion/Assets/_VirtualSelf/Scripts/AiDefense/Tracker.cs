using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UVRPN.Core;

public class Tracker : MonoBehaviour {

	public VRPN_Manager manager;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Debug.Log(manager.GetPosition("dtrack", 0).ToString("F4"));
	}
}
