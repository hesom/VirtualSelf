using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Measure : MonoBehaviour {

	public _7Segment digit1;
	public _7Segment digit2;

	public int weightThreshold = 10;
	public UnityEvent OnWeightPassed;

	private SpringJoint spring;
	private float gravity;
	private bool onWeightPassedFired = false;
	void Awake() {
		spring = GetComponent<SpringJoint>();
		gravity = -Physics.gravity.y;
	}
	
	// Update is called once per frame
	void Update () {
		float weight = spring.currentForce.y / gravity;
		int weightInt = (int)weight;
		int ones = weightInt % 10;
		weightInt /= 10;
		int tens = weightInt % 10;
		digit1.SetCharacter(tens.ToString()[0]);
		digit2.SetCharacter(ones.ToString()[0]);
		if(weight > weightThreshold && !onWeightPassedFired){
			OnWeightPassed.Invoke();
			onWeightPassedFired = true;
		}
	}
}
