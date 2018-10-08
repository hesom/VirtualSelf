using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
    public class LayerUtils
    {
        /// <summary>
        /// Traverses all child objects of root and sets their layer to layerName
        /// </summary>
        /// <param name="root">The parent object</param>
        /// <param name="layerName">The name of the layer</param>
        public static void SetLayersRecursive(GameObject root, string layerName)
        {
            string layer = root.GetComponent<BehindPortalSpecialLayerFix>()?.layerBehindPortal ?? layerName;
            root.layer = LayerMask.NameToLayer(layer);

            if (root.transform.childCount == 0) return;

            foreach (Transform child in root.transform)
            {
                SetLayersRecursive(child.gameObject, layerName);
            }
        }

        public static void ProcessLightRecursive(GameObject root, string layerName)
        {
            Light light = root.GetComponent<Light>();

            if(light != null)
            {
                light.cullingMask = 1 << LayerMask.NameToLayer(layerName);
                light.cullingMask |= 1 << LayerMask.NameToLayer("Master");
            }

            if(root.transform.childCount == 0) return;

            foreach(Transform child in root.transform)
            {
                ProcessLightRecursive(child.gameObject, layerName);
            }
        }
        
        public static void DeactivateLightCullingLayer(Light light, string layerName)
        {
            light.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));
        }

        /// <summary>
        /// Traverses all child objects of root and sets Components of type T to enabled
        /// </summary>
        /// <param name="root">The parent object</param>
        /// <param name="active">If the Component should be enabled or disabled</param>
        public static void SetEnabledRecursive<T>(GameObject root, bool enabled) where T : MonoBehaviour
        {
            T o = root.GetComponent<T>();
            if (o != null)
            {
                o.enabled = enabled;
            }

            if (root.transform.childCount == 0) return;

            foreach (Transform child in root.transform)
            {
                SetEnabledRecursive<T>(child.gameObject, enabled);
            }
        }
    }
}

