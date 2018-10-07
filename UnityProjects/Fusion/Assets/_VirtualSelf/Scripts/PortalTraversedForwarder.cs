using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VirtualSelf
{
    public class PortalTraversedForwarder : MonoBehaviour
    {
        public SceneSwitcher sceneSwitcher;
        public UnityEvent onPortalTraversed;
        // Use this for initialization
        void Start()
        {
            sceneSwitcher.onPortalTraversed.AddListener(ForwardEvent);
        }

        public void ForwardEvent()
        {
            onPortalTraversed.Invoke();
        }
    }
}