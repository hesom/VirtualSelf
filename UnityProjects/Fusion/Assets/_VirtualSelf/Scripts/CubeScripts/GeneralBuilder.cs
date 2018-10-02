using Leap.Unity.Attachments;
using VirtualSelf.CubeScripts.Interaction;

namespace VirtualSelf.CubeScripts
{
    public sealed class GeneralBuilder
    {
        public readonly GridLocator Locator;
        public readonly SideRotator Rotator;

        public GeneralBuilder(
            GridLocator locator,
            SideRotator rotator)
        {
            Locator = locator;
            Rotator = rotator;
        }

        public FingerMoveDetector CreateFingerMoveDetector(AttachmentHand attachmentHand) {
            return new FingerMoveDetector(
                attachmentHand.indexTip,
                attachmentHand.palm,
                Locator,
                Rotator);
        }
    }
}