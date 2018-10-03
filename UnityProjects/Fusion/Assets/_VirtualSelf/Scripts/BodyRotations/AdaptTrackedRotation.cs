using UnityEngine;

namespace VirtualSelf.BodyRotations
{
    public class AdaptTrackedRotation : MonoBehaviour
    {
        public BodyRotationData Rotation;

        private Transform childMeshTransform;

        // Use this for initialization
        private void Start()
        {
            childMeshTransform = transform.GetChild(0);
            if (childMeshTransform == null)
                Debug.LogError("The tracked gameobject has to have its real mesh as a child.");
            childMeshTransform.localRotation = Rotation.RelativeInverseRotation;
        }

        // Update is called once per frame
        private void Update()
        {
            //Debug.Log(transform.localRotation);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SaveInverseRotation();
            }
        }

        /// <summary>
        /// Sets the local rotation of the child to the inverse of the tracked gameobject.
        /// Acts nearly like a "freeze transformations" for the childmesh.
        /// Inverse rotation is saved in ScriptableObject to preserve state at startup.
        /// </summary>
        private void SaveInverseRotation()
        {
            var inverse = Quaternion.Inverse(transform.localRotation);
            childMeshTransform.localRotation = inverse;
            Rotation.RelativeInverseRotation = inverse;
        }
    }
}