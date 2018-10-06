using UnityEditor;
using UnityEngine;

namespace VirtualSelf.BodyRotations
{
    public class RotationAdapter : MonoBehaviour
    {
        public BodyRotationData Rotation;

        public GameObject MeshGameObject => childMeshTransform.gameObject;

        private Transform childMeshTransform;

        // Use this for initialization
        private void Start()
        {
            childMeshTransform = transform.GetComponentInChildren<MeshRenderer>()?.transform;
            if (childMeshTransform == null)
                throw new UnityException("The tracked gameobject has to have its real mesh as a child.");
            childMeshTransform.localRotation = Rotation.RelativeInverseRotation;
        }

        /// <summary>
        /// Sets the local rotation of the child to the inverse of the tracked gameobject.
        /// Acts nearly like a "freeze transformations" for the childmesh.
        /// Inverse rotation is saved in ScriptableObject to preserve state at startup.
        /// </summary>
        public void SaveInverseRotation()
        {
            var inverse = Quaternion.Inverse(transform.localRotation);
            childMeshTransform.localRotation = inverse;
            Rotation.RelativeInverseRotation = inverse;
        }
    }

    [CustomEditor(typeof(RotationAdapter))]
    public class SaveRotationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        
            var myScript = (RotationAdapter)target;
            if (GUILayout.Button("Save Inverse Rotation"))
            {
                myScript.SaveInverseRotation();
            }
        }
    }
}