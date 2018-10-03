using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinLaserRoom : MonoBehaviour {

    public UnityEvent OnRoomWon;

    private bool recentlyHit = false;
    private bool winConditionCleared = false;
    private bool everWon = false;

	// Use this for initialization
	void Start () {
	    	
	}
	
	// Update is called once per frame
	void Update () {
		
        if(!everWon && !recentlyHit && winConditionCleared)
        {
            OnRoomWon.Invoke();
        }
	}

    public void RecentlyHit(bool hit)
    {
        recentlyHit = hit;
    }

    public void WinConditionCleared(bool cleared)
    {
        winConditionCleared = cleared;
    }
    
}
