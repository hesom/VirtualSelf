using System;
using Leap.Unity;
using Leap.Unity.Attachments;
using Leap.Unity.Interaction;
using UnityEngine;
using VirtualSelf.CubeScripts.Interaction;

namespace VirtualSelf.CubeScripts
{
    public class CubeHandler : MonoBehaviour
    {
        public enum ScrambleConfig
        {
            NoScramble,
            EasyMoveScramble,
            FixedStartScramble
        }

        public AttachmentHand LeftHand;
        public AttachmentHand RightHand;
        public InteractionHand LeftInteraction;
        public InteractionHand RightInteraction;
        public ScrambleConfig InitialScramble;
        public GameObject GhostCube;

        public bool SkipUpdate = false;

        private HandManager handManager;
        private WinCondition winCondition;
        private InteractionBehaviour graspingCube;

        private void Awake()
        {
            graspingCube = transform.parent.GetComponent<InteractionBehaviour>();
            CheckAllPreconditions();
            var cube = new Cube2X2(gameObject);
            var ghostCube = new Cube2X2(GhostCube);
            var indicator = GetComponentInChildren<IndicatorRing>(true);

            var locator = new GridLocator(transform);
            var rotator = new SideRotator(cube, ghostCube, indicator);
            var builder = new GeneralBuilder(locator, rotator);
            var handSwitcher = new HandPositionInfo(LeftHand, RightHand, LeftInteraction, RightInteraction, locator);
            handManager = new HandManager(handSwitcher, builder, graspingCube);
            winCondition = new WinCondition(cube);

            switch (InitialScramble)
            {
                case ScrambleConfig.NoScramble:
                    break;
                case ScrambleConfig.EasyMoveScramble:
                    Scramble.Easy(rotator);
                    break;
                case ScrambleConfig.FixedStartScramble:
                    Scramble.FixedFull(rotator);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (SkipUpdate) return;

            handManager.Update();

            winCondition.CheckWhiteSideSolved();
        }

        private void CheckAllPreconditions()
        {
            if (LeftHand == null || RightHand == null)
            {
                throw new UnityException(
                   "You forgot to pass hands into the cubeHandler!");
            }

            if (LeftHand.chirality != Chirality.Left
                || RightHand.chirality != Chirality.Right)
            {
                throw new UnityException(
                    "Left and right hands are mixed up!");
            }

            if (graspingCube.allowMultiGrasp)
            {
                throw new UnityException(
                   "For the grasping cube parent multigrasp is allowed!");
            }

            if ((int)LeftHand.indexTip.attachmentPoint != (int)AttachmentPointFlags.IndexTip
                || (int)RightHand.indexTip.attachmentPoint != (int)AttachmentPointFlags.IndexTip
                || (int)LeftHand.palm.attachmentPoint != (int)AttachmentPointFlags.Palm
                || (int)RightHand.palm.attachmentPoint != (int)AttachmentPointFlags.Palm)
            {
                throw new UnityException("Left and right attachment hand have to have " +
                    "IndexTip and Palm enabled,otherwise this script will not work.");
            }
        }
    }
}
