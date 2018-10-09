using UnityEditor;
using UnityEngine;

namespace VirtualSelf.BodyRotations
{
    public class RootSettings : MonoBehaviour
    {
        public void ToggleAllRenderer()
        {
            var allAdapters = GetComponentsInChildren<RotationAdapter>();

            foreach (var adapter in allAdapters)
            {
                var meshRenderer = adapter.GetComponentInChildren<MeshRenderer>();
                if (meshRenderer != null)
                    meshRenderer.enabled = !meshRenderer.enabled;
            }
        }
    }

    [CustomEditor(typeof(RootSettings))]
    public class RootSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = (RootSettings) target;
            if (GUILayout.Button("Toggle all mesh renderer"))
            {
                myScript.ToggleAllRenderer();
            }
        }
    }
}