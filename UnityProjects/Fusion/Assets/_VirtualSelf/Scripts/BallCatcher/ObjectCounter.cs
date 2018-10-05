using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectCounter : MonoBehaviour {

	public _7Segment digitOnes;
	public _7Segment digitTens;
	public _7Segment digitHundreds;

	public int countThreshold = 10;
	public UnityEvent OnObjectsCounted;

	private int counter = 0;
	private bool onObjectsCountedFired = false;	
	// Update is called once per frame
	void Update () {
		int counterDigits = counter;
		int ones = counterDigits % 10;
		counterDigits /= 10;
		int tens = counterDigits % 10;
		counterDigits /= 10;
		int hundreds = counterDigits % 10;

		digitOnes.SetCharacter(ones.ToString()[0]);
		digitTens.SetCharacter(tens.ToString()[0]);
		digitHundreds.SetCharacter(hundreds.ToString()[0]);
		
		if(!onObjectsCountedFired && counter >= countThreshold){
			OnObjectsCounted.Invoke();
			onObjectsCountedFired = true;
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.transform.tag == "Sphere"){
			counter++;
		}	
	}

	void OnTriggerExit(Collider other)
	{
		if(other.transform.tag == "Sphere"){
			counter--;
		}
	}
}
