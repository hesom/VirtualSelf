using System.CodeDom;
using UnityEngine;
using UnityEngine.XR;
using VirtualSelf.GameSystems;

namespace VirtualSelf {

/// <summary>
/// TODO: Fill out this class description: TrackingCamera
/// </summary>
public sealed class TrackingCamera {

    /* ---------- Variables & Properties ---------- */

    public ulong UniqueId { get; }
    
    public string Name { get; }
   
    public Vector3 Position {
        get { return (position); }
        private set { position = value; }
    }
    
    public Quaternion Rotation {
        get { return (rotation); }
        private set { rotation = value; }
    }

    public int LastUpdatedFrame { get; private set; }
    
    private Vector3 position = Vector3.zero;

    private Quaternion rotation = Quaternion.identity;


    /* ---------- Constructors ---------- */

    private TrackingCamera(ulong uniqueId, string name) {

        UniqueId = uniqueId;
        Name = name;
    }
    

    /* ---------- Methods ---------- */

    public static TrackingCamera CreateFromNodeState(XRNodeState nodeState) {

        if (IsTrackingCameraNodeState(nodeState) == false) {
            
            throw new VrHardwareSystemException(
                "A new tracking camera hardware device could not be created from the given XR " +
                "node state, because the node state does not correspond to a tracking camera " +
                "device."
            );
        }

        return (
            new TrackingCamera(nodeState.uniqueID, InputTracking.GetNodeName(nodeState.uniqueID))
        );
    }

    /// <summary>
    /// Returns whether the given node state belongs to a tracking camera hardware device, or not.
    /// </summary>
    /// <remarks>
    /// Notice that this method is static: It does <i>not</i> check if the node state belongs to a
    /// <i>specific</i> device, only if it generally belongs to a tracking camera device.
    /// </remarks>
    /// <param name="nodeState">The node state to be checked.</param>
    /// <returns>
    /// <c>true</c> if the given node state belongs to a tracking camera hardware device, and
    /// <c>false</c> otherwise.
    /// </returns>
    public static bool IsTrackingCameraNodeState(XRNodeState nodeState) {
       
        return (nodeState.nodeType == XRNode.TrackingReference);
    }

    public bool BelongsToThisDevice(XRNodeState nodeState) {

        return (nodeState.uniqueID == UniqueId);
    }

    public void UpdateHardwareState(XRNodeState nodeState) {

        if (IsTrackingCameraNodeState(nodeState) == false) {
            
            throw new VrHardwareSystemException(
                "The XR Node type of the node state given to update this tracking camera " +
                "hardware device was of the wrong type - it does not seem to belong to a " +
                "tracking camera. The hardware state could not be updated."
            );
        }

        if (BelongsToThisDevice(nodeState) == false) {
            
            throw new VrHardwareSystemException(
                "The node state given to update this tracking camera hardware device does not " + 
                "seem to belong to it - it belongs to some other device. The hardware state " +
                "could not be updated."
            );
        }

        Vector3 tempPosition;
        Quaternion tempRotation;
        
        if (nodeState.TryGetPosition(out tempPosition) == false) {
            
            throw new VrHardwareSystemException(
                "Attempting to update the position of this tracking camera hardware device " + 
                "failed, because it was not possible to acquire that value from Unity's XR system."
            );
        }
        
        if (nodeState.TryGetRotation(out tempRotation) == false) {
            
            throw new VrHardwareSystemException(
                "Attempting to update the rotation of this tracking camera hardware device " + 
                "failed, because it was not possible to acquire that value from Unity's XR system."
            );
        }

        position = tempPosition;
        rotation = tempRotation;

        LastUpdatedFrame = Time.frameCount;
    }



    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}