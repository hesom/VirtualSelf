using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
    public class RegisterRightCam : MonoBehaviour
    {
        public SceneSwitcher sceneSwitcher;

        void Awake()
        {
            sceneSwitcher.SetRightCamera(GetComponent<Camera>());
        }
    }
}