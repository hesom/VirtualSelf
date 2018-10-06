using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualSelf.Ballmaze
{
    public class RigSwitch : MonoBehaviour {

        /* The list of all rigs that can be switched between. This is not type-safe, it is up to the
     * user to only actually add rigs here, and not other game objects. */
        public List<GameObject> rigs;

        /* The UI text element that will be used to display the message denoting the currently active
     * rig.
     * The text will only be shown for the amount of time specified by "rigSwitchTextDisplayTime". */
        public Text rigSwitchText;

        /* The base text of the "rigSwitchText" text element. This can be something like "Current Rig: ".
     * The id and name of the current rig will be appended to this text. */
        public String rigSwitchTextBase = "Current Rig: ";

        /* The duration (in seconds) that the "rigSwitchText" text element will be displayed after the
     * current rig has been switched.
     * If this is 0, the text element will not be displayed at all. */
        [Range(0.0f, 10.0f)]
        public float rigSwitchTextDisplayTime = 5.0f;

        /* The id (within the "rigs" list) of the rig that is currently active. */
        private int currentRigId;

        void Start () {

            /* Disable all rigs (because we don't know how many are currently enabled within the Unity
         * editor, and we only ever want to have a single one enabled), then active the first rig in
         * the list by default. */

            rigs.ForEach(x => x.SetActive(false));
            currentRigId = 0;
            ActivateRig(currentRigId);
        }

        void Update () {

            if (Input.GetKeyDown(KeyCode.R) == true) {

                /* If the rig switch key is pressed multiple times in fast succession, we want to reset
             * the time for displaying the switch text element every time, so it's the full
             * duration. */
                CancelInvoke("DisableRigSwitchText");

                /* Cycle through the rigs, one at a time. */
                int dir = Input.GetKey(KeyCode.LeftShift) ? -1 : 1;
            
                int newRigId = (currentRigId + dir + rigs.Count) % rigs.Count;
                ActivateRig(newRigId);
            }
        
        }

        /* Activates the rig with the id denoted by "newRigId". This includes disabling the current (old)
     * one, setting the new one as the current one, and showing the rig switch text element. */
        void ActivateRig(int newRigId) {

            rigs[currentRigId].SetActive(false);
            rigs[newRigId].SetActive(true);

            currentRigId = newRigId;

            rigSwitchText.text = (rigSwitchTextBase + "[" + currentRigId.ToString() + "] " +
                                  rigs[currentRigId].gameObject.name);

            /* If the display time is 0, we will not display the message at all. */
            if (rigSwitchTextDisplayTime < 0.001f) { return; }

            rigSwitchText.gameObject.SetActive(true);

            /* After "rigSwitchTextDisplayTime" has passed, the method to disable the rig switch text
         * element again will be invoked. */
            Invoke("DisableRigSwitchText", rigSwitchTextDisplayTime);
        }

        /* Disables the rig switch text element. This method is never called directly, only invoked
     * through Unity, to be ran after "rigSwitchTextDisplayTime" has passed. */
        void DisableRigSwitchText() {

            rigSwitchText.gameObject.SetActive(false);
        }
    }
}