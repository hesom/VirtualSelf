using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualSelf;

public class GestureSelector : MonoBehaviour {

    [System.Serializable]
    public class GestureObjects
    {
        public GameObject PlayerGestures;
        public GameObject Scope;
        public GameObject BulletSpawn;
    }

    public GestureObjects[] GestureObjectsArr;
    /**public int SelectedIndex
    {
        get { return _selectedIndex; }
        set {
            _selectedIndex = value;
            ActivateGesture(_selectedIndex);
        }
    }**/


    [Range(-1,2)]
    [Tooltip("Activate the gesture with the selected index, -1 to disable all")]
    public int SelectedIndex;

    private int _lastActiveAboveZero;

    public void ActivateGesture(int index) {
        if (index > 0) _lastActiveAboveZero = index;
        
        for (int i=0;i<GestureObjectsArr.Length;i++)
        {
            bool active = i == index;
            
            // unload gestures
            var g = GestureObjectsArr[i].PlayerGestures.GetComponent<GunGesture>();
            if (g != null && _postStart) g.UnloadUnscope(true);
            var g2 = GestureObjectsArr[i].PlayerGestures.GetComponent<GunGesturePinch>();
            if (g2 != null && _postStart) g2.UnloadUnscope(true);
                
            GestureObjectsArr[i].BulletSpawn.SetActive(active);
            GestureObjectsArr[i].PlayerGestures.SetActive(active);
            GestureObjectsArr[i].Scope.SetActive(active);
        }
    }

    public void ReactivateGesture()
    {
        ActivateGesture(_lastActiveAboveZero);
    }

    public void UnloadUnscopeAll()
    {
        foreach (var t in GestureObjectsArr)
        {
            // unload gestures
            var g = t.PlayerGestures.GetComponent<GunGesture>();
            if (g != null && _postStart) g.UnloadUnscope(false);
            var g2 = t.PlayerGestures.GetComponent<GunGesturePinch>();
            if (g2 != null && _postStart) g2.UnloadUnscope(false);
        }
    }

    private bool _postStart;

	// Use this for initialization
	void Start () {
		ActivateGesture(SelectedIndex);
	    _postStart = true;
	}

    #if UNITY_EDITOR
    void OnValidate()
    {
        if (_postStart) {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                ActivateGesture(SelectedIndex);
            };
        }
    }
    #endif
}
