using UnityEngine;

namespace VirtualSelf.BodyRotations
{
    [CreateAssetMenu]
    public class BodyRotationData : ScriptableObject
    {
        public Quaternion RelativeInverseRotation;

        public Vector3 PositionOffset;

    }
}