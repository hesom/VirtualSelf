using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Leap.Unity.Attachments;
using Leap.Unity.Attributes;
using UnityEngine;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class PalmAttachPointStabilized : MonoBehaviour
{
//  [Tooltip("The AttachmentHand associated with this AttachmentPointBehaviour. AttachmentPointBehaviours "
//           + "should be beneath their AttachmentHand object in the hierarchy.")]
//  [Disable]
//  public AttachmentHand attachmentHand;

  public HandModelBase HandModel;

//  [Tooltip("To change which attachment points are available on an AttachmentHand, refer to the "
//           + "inspector for the parent AttachmentHands object.")]
//  [Disable]
//  public AttachmentPointFlags attachmentPoint;

//  void OnValidate()
//  {
//    if (!attachmentPoint.IsSinglePoint() && attachmentPoint != AttachmentPointFlags.None)
//    {
//      Debug.LogError("AttachmentPointBehaviours should refer to a single attachmentPoint flag.", this.gameObject);
//      attachmentPoint = AttachmentPointFlags.None;
//    }
//  }

//  void OnDestroy()
//  {
//    if (attachmentHand != null)
//    {
//      attachmentHand.notifyPointBehaviourDeleted(this);
//    }
//  }

//  public static implicit operator AttachmentPointFlags(PalmAttachPointStabilized p)
//  {
//    if (p == null) return AttachmentPointFlags.None;
//    return p.attachmentPoint;
//  }

  private void Update()
  {
    Hand h = HandModel.GetLeapHand();
    if (h != null) SetTransformUsingHand(h);
  }

  public void SetTransformUsingHand(Leap.Hand hand)
  {
    if (hand == null)
    {
      //Debug.LogError("Unable to set transform with a null hand.", this.gameObject);
      return;
    }

    Vector3 position = Vector3.zero;
    Quaternion rotation = Quaternion.identity;

    GetLeapHandPointData(hand, out position, out rotation);

    this.transform.position = position;
    this.transform.rotation = rotation;
  }

  public static void GetLeapHandPointData(Leap.Hand hand, out Vector3 position,
    out Quaternion rotation)
  {
    
    position = hand.StabilizedPalmPosition.ToVector3();
//    position = hand.PalmPosition.ToVector3();
//    position = hand.Basis.translation.ToVector3();
    rotation = hand.Basis.rotation.ToQuaternion();
  }


}
