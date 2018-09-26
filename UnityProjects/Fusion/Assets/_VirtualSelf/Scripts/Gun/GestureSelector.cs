using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
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

    private HandModelBase LeftHand;
    private HandModelBase RightHand;

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

    public void ActivateGesture(int index)
    {
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
        ActivateGesture(SelectedIndex);
    }

    private bool _postStart;

	// Use this for initialization
	void Start () {
		ActivateGesture(SelectedIndex);
	    _postStart = true;
	}

    public void UnloadUnscopeAll()
    {
        foreach (GestureObjects g in GestureObjectsArr)
        {
            GunGesture gg = g.PlayerGestures.GetComponent<GunGesture>();
            if (gg == null) g.PlayerGestures.GetComponent<GunGesturePinch>().UnloadUnscope();
            else gg.UnloadUnscope();
        }
    }

    void OnValidate()
    {
        if (!isActiveAndEnabled) return;
        
        ActivateGesture(SelectedIndex);
    }
}
