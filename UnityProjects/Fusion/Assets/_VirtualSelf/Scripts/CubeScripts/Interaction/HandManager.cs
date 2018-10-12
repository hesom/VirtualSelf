using Leap.Unity.Attachments;
using Leap.Unity.Interaction;

namespace VirtualSelf.CubeScripts.Interaction {

    /// <summary>
    /// TODO: Fill out this class description: HandManager
    /// </summary>
    public sealed class HandManager
    {
        private readonly HandPositionInfo handPosInfo;
        private readonly GeneralBuilder builder;
        private readonly InteractionBehaviour graspingCube;
        private AttachmentHand activeHand;
        private FingerMoveDetector moveDetector;

        private bool handLocked;
        private const float LockDistance = Cube2X2.HalfCubeWidth * 0.4f; // TODO vielleicht noch etwas später die hände festlegen?

        public HandManager(
            HandPositionInfo handPositionInfo,
            GeneralBuilder generalBuilder,
            InteractionBehaviour graspingCubeBehaviour)
        {
            handPosInfo = handPositionInfo;
            builder = generalBuilder;
            graspingCube = graspingCubeBehaviour;
            activeHand = handPosInfo.ClosestHand();
            moveDetector = builder.CreateFingerMoveDetector(activeHand);

            // Subscribe to grasp events
            graspingCube.OnGraspBegin += GraspBegin;
            graspingCube.OnGraspEnd += GraspEnd;
        }

        /// <summary>
        /// If an update is needed, the hand will be locked/unlocked accordingly
        /// </summary>
        private void UpdateLock()
        {
            if (!handLocked && handPosInfo.ClosestHandChanged())
            {
                SwitchActiveHand();
            }

            if (activeHand.isTracked)
            {
                bool shouldBeLocked = handPosInfo.ClosestHandDistance() < LockDistance;
                if (!shouldBeLocked) // Stop moving, if hands should not be locked
                    moveDetector.StopMoving();

                handLocked = shouldBeLocked;
            }
            else
            {
                moveDetector.StopMoving();
                handLocked = false;
            }
        }

        /// <summary>
        /// Update activeHand and related moveDetector
        /// </summary>
        private void SwitchActiveHand()
        {
            activeHand = handPosInfo.ClosestHand();
            moveDetector = builder.CreateFingerMoveDetector(activeHand);
        }

        public void Update()
        {
            UpdateLock();

            if (handLocked)
            {
                moveDetector.UpdateMove();
            }
            else
            {
                builder.Rotator.Indicator.Active = false;
            }
        }

        public void GraspBegin()
        {
            // As multigrasp is not allowed, graspingHands will return 
            // only a single value. Looping over this collection is the
            // easiest access. But it is still only a single iteration.
            foreach (var hand in graspingCube.graspingHands)
            {
                handPosInfo.GraspStart(hand);
            }
            moveDetector.StopMoving();
            handLocked = false;
            if (handPosInfo.ClosestHandChanged())
                SwitchActiveHand();
        }

        public void GraspEnd() 
        {
            graspingCube.StartCoroutine(handPosInfo.GraspStop());
        }
    }
}
