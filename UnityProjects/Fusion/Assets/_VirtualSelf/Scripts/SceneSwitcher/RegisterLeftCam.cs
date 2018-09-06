using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
    public class RegisterLeftCam : MonoBehaviour
    {
        public SceneSwitcher sceneSwitcher;

        void Awake()
        {
            sceneSwitcher.SetLeftCamera(GetComponent<Camera>());
        }
    }
}