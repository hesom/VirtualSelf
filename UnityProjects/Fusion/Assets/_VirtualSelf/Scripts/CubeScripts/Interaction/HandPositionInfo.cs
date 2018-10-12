using System.Collections;
using Leap.Unity;
using Leap.Unity.Attachments;
using Leap.Unity.Interaction;
using UnityEngine;
using System.Linq;

namespace VirtualSelf.CubeScripts.Interaction
{
    /// <summary>
    /// This class stores the attachment hands and determines the closest/focused one.
    /// </summary>
    public sealed class HandPositionInfo
    {
        private readonly AttachmentHand leftHand;
        private readonly AttachmentHand rightHand;
        private readonly InteractionHand leftInteraction;
        private readonly InteractionHand rightInteraction;
        private readonly GridLocator locator;

        private bool leftHandIsGrasped;
        private bool rightHandIsGrasped;
        private bool lastFocusWasLeft;

        public HandPositionInfo(
            AttachmentHand leftAttachmentHand,
            AttachmentHand rightAttachmentHand,
            InteractionHand leftInteractionHand,
            InteractionHand rightInteractionHand,
            GridLocator gridLocator)
        {
            leftHand = leftAttachmentHand;
            rightHand = rightAttachmentHand;
            leftInteraction = leftInteractionHand;
            rightInteraction = rightInteractionHand;
            locator = gridLocator;
        }

        /// <summary>Helper struct, cause we cannot use Tupels *sadface*...</summary>
        private struct Distance
        {
            public readonly float Left;
            public readonly float Right;

            public Distance(float left, float right)
            {
                Left = left;
                Right = right;
            }
        }

        /// <summary>Distances of both Hands to the cube center</summary>
        private Distance DistancesToCenter()
        {
            return new Distance(
                locator.DistanceToCenter(leftHand.palm.transform.position),
                locator.DistanceToCenter(rightHand.palm.transform.position));
        }

        /// <summary>
        /// Returns the closest valid hand.
        /// </summary>
        public AttachmentHand ClosestHand()
        {
            lastFocusWasLeft = LeftIsFocusedHand();
            return lastFocusWasLeft ? leftHand : rightHand;
        }

        /// <summary>
        /// Checks if the output of <see cref="ClosestHand"/> differs since the last call.
        /// </summary>
        public bool ClosestHandChanged()
        {
            return lastFocusWasLeft != LeftIsFocusedHand();
        }

        /// <summary>Distance of the last returned closest hand to the cube center.</summary>
        public float ClosestHandDistance()
        {
            return lastFocusWasLeft
                ? locator.DistanceToCenter(leftHand.palm.transform.position)
                : locator.DistanceToCenter(rightHand.palm.transform.position);
        }

        /// <summary>
        /// If a hand is not tracked it loses focus. The other hand is then returned.
        /// If both hand are untracked the right hand will be returned.
        /// If a hand is currently grasping the cube, the other hand is returned.
        /// If no hand is grasping the cube and both hands are tracked,
        /// the focused hand is the one closest to the cube.
        /// </summary>
        private bool LeftIsFocusedHand()
        {
            if (leftHandIsGrasped || !leftHand.isTracked)
                return false;
            if (rightHandIsGrasped || !rightHand.isTracked)
                return true;

            var distances = DistancesToCenter();
            return distances.Left < distances.Right;
        }

        /// <summary>Signal that a grasp has started.</summary>
        public void GraspStart(InteractionHand hand)
        {
            bool isLeft = hand.isLeft;
            leftHandIsGrasped = isLeft;
            rightHandIsGrasped = !isLeft;

            if (isLeft)
                rightInteraction.graspingEnabled = false;
            else
                leftInteraction.graspingEnabled = false;
        }

        /// <summary>Signal that a grasp has stopped.</summary>
        public IEnumerator GraspStop()
        {
            //yield return new WaitForSeconds(0.25f);
            leftHandIsGrasped = false;
            rightHandIsGrasped = false;
            leftInteraction.graspingEnabled = true;
            rightInteraction.graspingEnabled = true;
            yield return null;
        }
    }
}
