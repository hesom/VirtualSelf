using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
    [RequireComponent(typeof(Camera))]
    public class BehindPortalCullingMask : MonoBehaviour
    {
        public LayerMask cullingMaskBehindPortal;
    }
}

