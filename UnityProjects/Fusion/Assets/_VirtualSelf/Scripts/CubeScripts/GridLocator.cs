using System;
using UnityEngine;

namespace VirtualSelf.CubeScripts
{
    /// <summary>
    /// This class provides location information for a given 2x2 cube object.
    /// Mostly used to determine the gridlike positioning near sides or subcubes.
    /// </summary>
    public sealed class GridLocator
    {
        /// <summary>
        /// The parent transform of the rubis cube.
        /// Used for the center position and world to local conversions.
        /// </summary>
        private readonly Transform center;

        /// <summary>
        /// Requires the parent transform of the cubes.
        /// </summary>
        public GridLocator(Transform cubeParentTransform)
        {
            center = cubeParentTransform;
        }

        /// <summary>
        /// Checks which cube location is the closest.
        /// </summary>
        /// <param name="position">in world coordinates</param>
        /// <returns>Nearest <see cref="Cube2X2.Position"/></returns>
        public Cube2X2.Position NearestCube(Vector3 position)
        {
            var localPos = center.worldToLocalMatrix.MultiplyPoint3x4(position);
            int index = 0;
            index += localPos.x > 0 ? 1 : 0;
            index += localPos.y <= 0 ? 2 : 0;
            index += localPos.z > 0 ? 4 : 0;
            return (Cube2X2.Position) index;
        }

        /// <summary>
        /// Checks for the given position, which cube side is the nearest one.
        /// </summary>
        /// <param name="position">in world coordinates</param>
        /// <returns>Nearest <see cref="Cube2X2.Side"/></returns>
        public Cube2X2.Side NearestSide(Vector3 position)
        {
            var localPos = center.worldToLocalMatrix.MultiplyPoint3x4(position);

            if (Mathf.Abs(localPos.x) > Mathf.Abs(localPos.y))
            {
                return Mathf.Abs(localPos.x) > Mathf.Abs(localPos.z)
                    ? (localPos.x > 0 ? Cube2X2.Side.Right : Cube2X2.Side.Left)
                    : (localPos.z > 0 ? Cube2X2.Side.Back : Cube2X2.Side.Front);
            }

            return Mathf.Abs(localPos.y) > Mathf.Abs(localPos.z)
                ? (localPos.y > 0 ? Cube2X2.Side.Top : Cube2X2.Side.Bottom)
                : (localPos.z > 0 ? Cube2X2.Side.Back : Cube2X2.Side.Front);
        }

        /// <summary>
        /// Distance to the center for given position along the axis specified by the Side Normal.
        /// </summary>
        /// <param name="pos">In world coordinates</param>
        /// <param name="side">Defines the axis direction on which the distance is calculated</param>
        public float SideAxisDistanceToCenter(Vector3 pos, Cube2X2.Side side)
        {
            var localPos = center.worldToLocalMatrix.MultiplyPoint3x4(pos);
            switch (side)
            {
                case Cube2X2.Side.Left:
                case Cube2X2.Side.Right:
                    return Mathf.Abs(localPos.x);
                case Cube2X2.Side.Top:
                case Cube2X2.Side.Bottom:
                    return Mathf.Abs(localPos.y);
                case Cube2X2.Side.Front:
                case Cube2X2.Side.Back:
                    return Mathf.Abs(localPos.z);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gives back the distance to the middle of the side on the primary axis along the side face normal.
        /// </summary>
        /// <param name="pos">In world coordinates</param>
        /// <param name="side">Defines axis direction and center point for distance calculation</param>
        public float DistanceToSideCenterOnAxis(Vector3 pos, Cube2X2.Side side)
        {
            var localPos = center.worldToLocalMatrix.MultiplyPoint3x4(pos);
            switch (side)
            {
                case Cube2X2.Side.Left:
                    return Mathf.Abs(localPos.x + Cube2X2.HalfCubeWidth);
                case Cube2X2.Side.Right:
                    return Mathf.Abs(localPos.x - Cube2X2.HalfCubeWidth);
                case Cube2X2.Side.Top:
                    return Mathf.Abs(localPos.y - Cube2X2.HalfCubeWidth);
                case Cube2X2.Side.Bottom:
                    return Mathf.Abs(localPos.y + Cube2X2.HalfCubeWidth);
                case Cube2X2.Side.Front:
                    return Mathf.Abs(localPos.z + Cube2X2.HalfCubeWidth);
                case Cube2X2.Side.Back:
                    return Mathf.Abs(localPos.z - Cube2X2.HalfCubeWidth);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <param name="pos">In world coordinates</param>
        /// <returns>Distance in meters</returns>
        public float DistanceToCenter(Vector3 pos)
        {
            return (pos - center.position).magnitude;
        }

        /// <summary>
        /// Maps the position to the 2D side space by throwing away the axis of the side normal.
        /// </summary>
        /// <param name="pos">In world coordinates</param>
        /// <param name="side">Defines through the side normal, which axis to throw away</param>
        public Vector2 MapToSideSpace(Vector3 pos, Cube2X2.Side side)
        {
            var localPos = center.worldToLocalMatrix.MultiplyPoint3x4(pos);
            switch (side)
            {
                case Cube2X2.Side.Front:
                    return new Vector2(localPos.x, localPos.y);
                case Cube2X2.Side.Back:
                    return new Vector2(localPos.y, localPos.x);
                case Cube2X2.Side.Top:
                    return new Vector2(localPos.x, localPos.z);
                case Cube2X2.Side.Bottom:
                    return new Vector2(localPos.z, localPos.x);
                case Cube2X2.Side.Left:
                    return new Vector2(localPos.y, localPos.z);
                case Cube2X2.Side.Right:
                    return new Vector2(localPos.z, localPos.y);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}