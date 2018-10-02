using System;
using Leap.Unity.Attachments;
using UnityEngine;

namespace VirtualSelf.CubeScripts.Interaction
{
    public sealed class FingerMoveDetector
    {
        private readonly Transform finger;
        private readonly Transform palm;
        private readonly GridLocator locator;
        private readonly SideRotator rotator;
        private Vector2 last2DPosition;

        // TODO Maybe insert here hard coded additional size of finger tip thickness, instead of multiplier
        private const float StartMultiplier = 1.05f;
        private const float StopMultiplier = 1.10f;

        public FingerMoveDetector(
            AttachmentPointBehaviour fingerPoint,
            AttachmentPointBehaviour palmPoint,
            GridLocator gridLocator,
            SideRotator sideRotator)
        {
            finger = fingerPoint.gameObject.transform;
            palm = palmPoint.gameObject.transform;
            locator = gridLocator;
            rotator = sideRotator;
        }

        /// <summary>
        /// Updates the cubes according to the rotation state.
        /// </summary>
        public void UpdateMove()
        {
            rotator.Indicator.Active = true;
            switch (rotator.CurrentState)
            {
                case SideRotator.State.Idle:

                    var weightedPosition = (finger.position * 0.4f) + (palm.position * 0.6f);
                    var nearestSide = locator.NearestSide(weightedPosition);
                    rotator.Indicator.TransformToSide(nearestSide);
                    if (ValidPosition(nearestSide, StartMultiplier))
                    {
                        StartRotation(nearestSide);
                    }

                    break;
                case SideRotator.State.Rotating:
                    if (ValidPosition(rotator.RotatingSide, StopMultiplier))
                    {
                        UpdateRotation();
                    }
                    else
                    {
                        rotator.StopRotation();
                    }

                    break;
                case SideRotator.State.Correcting:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool ValidPosition(Cube2X2.Side side, float multiplier)
        {
            float hoverDistance = locator.DistanceToSideCenterOnAxis(finger.position, side);
            bool hoversAboveSide = hoverDistance < Cube2X2.HalfCubeWidth * multiplier;
            var pos = locator.MapToSideSpace(finger.position, side);
            // (Un-)Commenting this switches between touching the exact cube or inside the indicator range

#pragma warning disable CS0162 // Unreachable code detected
            const bool ExactCubeDistance = true;
            if (ExactCubeDistance)
            {
                if (rotator.CurrentState == SideRotator.State.Rotating)
                { // Rotate vector to get correct result for distance
                    float sin = Mathf.Sin(rotator.CurrentAngle * Mathf.Deg2Rad);
                    float cos = Mathf.Cos(rotator.CurrentAngle * Mathf.Deg2Rad);

                    pos = new Vector2(
                        (cos * pos.x) - (sin * pos.y),
                        (sin * pos.x) + (cos * pos.y));
                }

                float widthMargin = Cube2X2.HalfCubeWidth * 2 * multiplier;
                bool closeToIndicator = pos.x < widthMargin && pos.y < widthMargin;
                return hoversAboveSide && closeToIndicator;
            }
            else
            {
                float cubeDiagonal = Cube2X2.HalfCubeWidth * 2 * Mathf.Sqrt(2);
                bool closeToIndicator = pos.magnitude < cubeDiagonal * multiplier;
                return hoversAboveSide && closeToIndicator;
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        /// <summary>
        /// Starts rotation
        /// </summary>
        /// <param name="nearestSide"></param>
        private void StartRotation(Cube2X2.Side nearestSide)
        {
            last2DPosition = locator.MapToSideSpace(finger.position, nearestSide);
            if (last2DPosition.magnitude < 0.000001f)
            {
                last2DPosition = Vector2.up;
            }

            rotator.StartRotation(nearestSide);
        }

        /// <summary>
        /// Stops the current cube rotation if necessary.
        /// </summary>
        public void StopMoving()
        {
            rotator.StopRotation();
        }

        private void UpdateRotation()
        {
            var current2DPosition = locator.MapToSideSpace(finger.position, rotator.RotatingSide);
            if (current2DPosition.magnitude < 0.000001f)
            {
                current2DPosition = Vector2.up;
            }

            if (Mathf.Abs(current2DPosition.x - last2DPosition.x) < 0.001f
                && Mathf.Abs(current2DPosition.y - last2DPosition.y) < 0.001f)
            {
                return;
            }

            float angle = Vector2.SignedAngle(current2DPosition, last2DPosition);
            // hundertstel von nem grad sollten reichen als genauigkeit
            if (Mathf.Abs(angle) < 0.01f) return;

            rotator.RotateCubes(angle);
            last2DPosition = current2DPosition;
        }
    }
}
