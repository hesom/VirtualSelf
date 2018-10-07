using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

namespace VirtualSelf
{
    public class KeypadState : MonoBehaviour
    {

        public GameObject brokenKey;
        public GameObject unbrokenKey;
        public Anchor anchor;
        public AnchorableBehaviour anchorable;

        private GlobalKeypadState globalState;

        // Use this for initialization
        void Start()
        {
            globalState = GameObject.FindGameObjectWithTag("GlobalGameState").GetComponent<GlobalKeypadState>();
            if (globalState == null)
            {
                throw new System.Exception("Global keypad state could not be found. Is a GlobalGameState object in the MasterScene?");
            }

            if (globalState.isKey7Repaired)
            {
                brokenKey.SetActive(false);
                unbrokenKey.SetActive(true);
                anchor.gameObject.SetActive(false);
                anchorable.gameObject.SetActive(false);
            }
        }


        public void MakeComplete()
        {
            globalState.isKey7Repaired = true;
        }
    }
}

