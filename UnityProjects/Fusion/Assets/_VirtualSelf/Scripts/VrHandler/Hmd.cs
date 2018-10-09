using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;


namespace VirtualSelf.GameSystems {

    
/// <summary>
/// TODO: Fill out this class description: Hmd
/// </summary>
public sealed class Hmd {
    
    /* ---------- Enumerations ---------- */
    
    /// <summary>
    /// The different tracking states that can exist for an HMD.<br/>
    /// For detailed information on this, see <see cref="CurrentTrackingState"/>.
    /// </summary>
    public enum TrackingState {

        /// <summary>
        /// The HMD is currently being tracked.
        /// </summary>
        Tracking,
        /// <summary>
        /// The HMD is currently not being tracked (e.g. it is out of range or there is a problem
        /// with the tracking camera(s)).
        /// </summary>
        Untracked,
        /// <summary>
        /// The tracking state is unknown. This is true if, for example, no VR SDK is even running.
        /// </summary>
        Unknown
    }
    

    /* ---------- Variables & Properties ---------- */
    
    public const int RequiredNodeStatesForCreation = 4;

    public ulong UniqueIdHead { get; }
    public ulong UniqueIdCenterEye { get; }
    public ulong UniqueIdLeftEye { get; }
    public ulong UniqueIdRightEye { get; }
       
    public string Model { get; }
    
    /// <summary>
    /// Denotes the current "tracking state" of the HMD.<br/>
    /// The tracking state describes whether or not the HMD is currently tracking within its
    /// confined space, meaning that its position and rotation within that space are known to it.
    /// <br/>
    /// This may describe vastly different things for different kinds of HMDs. Some, like the HTC
    /// Vive and the Oculus Rift, have externally placed tracking cameras, which create a small
    /// confined space and provide an actual position in space to the HMD. Others, like the
    /// Microsoft Hololens, can track themselves and do not need external cameras. There's also some
    /// which are made for a seating experience, and only provide rotational data for the head.<br/>
    /// <br/>
    /// Our game is concerned with the first kind described above. This means that the HMD can go
    /// outside of its tracking space, and lose positional tracking if all the tracking cameras
    /// cannot see it anymore. As long as it does not have tracking, it cannot be used and the data
    /// it delivers is essentially useless.<br/>
    /// Note that as long as "VR Mode" is not active, or had just been started up, the tracking
    /// state of the HMD will be <see cref="TrackingState.Unknown"/>.
    /// </summary>
    public TrackingState CurrentTrackingState { get; private set; } = TrackingState.Unknown;
    
    /// <summary>
    /// Denotes whether the HMD is currently tracking (and thus delivers positional and rotational
    /// data), or not.<br/>
    /// For details, see <see cref="CurrentTrackingState"/>.
    /// </summary>
    public bool IsTracking => (CurrentTrackingState == TrackingState.Tracking);
   
    /// <summary>
    /// Denotes the current "user presence" state of the HMD.<br/>
    /// The user presence refers to whether the HMD is currently being worn by someone, or not.<br/>
    /// What exactly "being worn" means, seems to be dependent on the HMD model in question, and
    /// probably also on the specific VR SDK. In general, if the user presence is not
    /// <see cref="UserPresenceState.Present"/>, the HMD can be assumed to currently not being used,
    /// e.g. just lying around somewhere.<br/>
    /// It should be noted that some HMDs might not support this feature at all. In this case,
    /// the state should be <see cref="UserPresenceState.Unsupported"/>.<br/>
    /// <br/>
    /// Also, the user presence can only be determined while the HMD currently has tracking (at
    /// least in a general way - there might be HMDs out there with more sophisticated systems).
    /// While the HMD is not being tracked, the presence state will be
    /// <see cref="UserPresenceState.Unknown"/>.
    /// <remarks>
    /// From our brief experiments with this, the HTC Vive supports this, but only in a very
    /// rudimentary way. It does not have special sensors for this. Instead, it merely detects
    /// whether it is currently moving around or not.<br/>
    /// While the switch from <see cref="UserPresenceState.NotPresent"/> to
    /// <see cref="UserPresenceState.Present"/> seems to be pretty much instant, the switch
    /// in the opposite direction has a delay of something between 5 and 10 seconds. This is likely
    /// on purpose to prevent the HMD from accidentally shutting down just because the user might
    /// have been very still for a moment.
    /// </remarks>
    /// </summary>
    public UserPresenceState CurrentUserPresenceState { get; private set; } =
           UserPresenceState.Unknown;
      
    /// <summary>
    /// Denotes whether there is currently a user being present for the HMD, or not. If this is
    /// <c>true</c>, the HMD can be assumed to be currently worn and in use.<br/>
    /// For details, see <see cref="CurrentUserPresenceState"/>.
    /// </summary>
    public bool IsUserPresent => (CurrentUserPresenceState == UserPresenceState.Present);
    
    public Vector3 HeadPosition {
        get { return (headPosition); }
        private set { headPosition = value; }
    }
    public Quaternion HeadRotation {
        get { return (headRotation); }
        private set { headRotation = value; }
    }
    
    public Vector3 CenterEyePosition {
        get { return (centerEyePosition); }
        private set { centerEyePosition = value; }
    }
    public Quaternion CenterEyeRotation {
        get { return (centerEyeRotation); }
        private set { centerEyeRotation = value; }
    }
    
    public Vector3 LeftEyePosition {
        get { return (leftEyePosition); }
        private set { leftEyePosition = value; }
    }
    public Quaternion LeftEyeRotation {
        get { return (leftEyeRotation); }
        private set { leftEyeRotation = value; }
    }
    
    public Vector3 RightEyePosition {
        get { return (rightEyePosition); }
        private set { rightEyePosition = value; }
    }
    
    public Quaternion RightEyeRotation {
        get { return (rightEyeRotation); }
        private set { rightEyeRotation = value; }
    }

    public int LastUpdatedFrame { get; private set; }
    
    private Vector3 headPosition = Vector3.zero;
    private Quaternion headRotation = Quaternion.identity;
    
    private Vector3 centerEyePosition = Vector3.zero;
    private Quaternion centerEyeRotation = Quaternion.identity;
    
    private Vector3 leftEyePosition = Vector3.zero;
    private Quaternion leftEyeRotation = Quaternion.identity;
    
    private Vector3 rightEyePosition = Vector3.zero;
    private Quaternion rightEyeRotation = Quaternion.identity;


    /* ---------- Constructors ---------- */

    private Hmd(
            ulong uniqueIdHead, ulong uniqueIdCenterEye,
            ulong uniqueIdLeftEye, ulong uniqueIdRightEye,
            string model) {

        UniqueIdHead = uniqueIdHead;
        UniqueIdCenterEye = uniqueIdCenterEye;
        UniqueIdLeftEye = uniqueIdLeftEye;
        UniqueIdRightEye = uniqueIdRightEye;
        
        Model = model;
    }


    /* ---------- Methods ---------- */

    public static Hmd CreateFromNodeStates(IEnumerable<XRNodeState> nodeStates, string model) {

        if (nodeStates.Count() < RequiredNodeStatesForCreation) {
            
            throw new VrHardwareSystemException(
                "To create an HMD hardware device, " + RequiredNodeStatesForCreation + " XR node " +
                "states are necessary, but only " + nodeStates.Count() + " were given. The HMD " +
                "device could not be created."
            );
        }
        
        XRNodeState? headState = null;
        XRNodeState? centerEyeState = null;
        XRNodeState? leftEyeState = null;
        XRNodeState? rightEyeState = null;

        foreach (XRNodeState state in nodeStates) {

            if (state.nodeType == XRNode.Head) {
                
                if (headState.HasValue) {
                    throw new VrHardwareSystemException(
                        GenerateMultipleNodesExceptionString("Head")
                    );
                }
                
                headState = state;
            }
            else if (state.nodeType == XRNode.CenterEye) {
                
                if (centerEyeState.HasValue) {
                    throw new VrHardwareSystemException(
                        GenerateMultipleNodesExceptionString("Center Eye")
                    );
                }
                
                centerEyeState = state;
            }
            else if (state.nodeType == XRNode.LeftEye) {
                
                if (leftEyeState.HasValue) {
                    throw new VrHardwareSystemException(
                        GenerateMultipleNodesExceptionString("Left Eye")
                    );
                }
                
                leftEyeState = state;
            }
            else if (state.nodeType == XRNode.RightEye) {
                
                if (rightEyeState.HasValue) {
                    throw new VrHardwareSystemException(
                        GenerateMultipleNodesExceptionString("Right Eye")
                    );
                }
                
                rightEyeState = state;
            }
        }

        if ((headState.HasValue == false) || (centerEyeState.HasValue == false) ||
            (leftEyeState.HasValue == false) || (rightEyeState.HasValue == false)) {
            
            throw new VrHardwareSystemException(
                "A new HMD hardware device could not be created from the given XR node states, " +
                "because one or multiple of the nodes required for this are missing."
            );
        }

        return (
            new Hmd(
                headState.Value.uniqueID, centerEyeState.Value.uniqueID,
                leftEyeState.Value.uniqueID, rightEyeState.Value.uniqueID,
                model
            )
        );
    }

    /// <summary>
    /// Returns whether the given node state belongs to an HMD hardware device, or not.
    /// </summary>
    /// <param name="nodeState">The node state to be checked.</param>
    /// <returns>
    /// <c>true</c> if the given node state belongs to an HMD hardware device, and <c>false</c>
    /// otherwise.
    /// </returns>
    public static bool IsHmdState(XRNodeState nodeState) {

        return ((nodeState.nodeType == XRNode.Head) || (nodeState.nodeType == XRNode.CenterEye) ||
                (nodeState.nodeType == XRNode.LeftEye) || (nodeState.nodeType == XRNode.RightEye));
    }

    public bool BelongsToThisDevice(XRNodeState nodeState) {

        return ((nodeState.uniqueID == UniqueIdHead) ||
                (nodeState.uniqueID == UniqueIdCenterEye) ||
                (nodeState.uniqueID == UniqueIdLeftEye) ||
                (nodeState.uniqueID == UniqueIdRightEye));
    }

    public void UpdateTrackingState(IEnumerable<XRNodeState> nodeStates) {

        bool wasTracking = IsTracking;
        
        bool isHeadBeingTracked = false;
        bool isCenterEyeBeingTracked = false;
        bool isLeftEyeBeingTracked = false;
        bool isRightEyeBeingTracked = false;
        
        int skippedNodes = 0;

        foreach (XRNodeState state in nodeStates) {

            if (BelongsToThisDevice(state) == false) {
                skippedNodes++;
                continue;
            }

            if (state.nodeType == XRNode.Head) {
                
                isHeadBeingTracked = state.tracked;
            }
            else if (state.nodeType == XRNode.CenterEye) {

                isCenterEyeBeingTracked = state.tracked;
            }
            else if (state.nodeType == XRNode.LeftEye) {

                isLeftEyeBeingTracked = state.tracked;
            }
            else if (state.nodeType == XRNode.RightEye) {

                isRightEyeBeingTracked = state.tracked;
            }
        }
        
        if (isHeadBeingTracked != isCenterEyeBeingTracked != isLeftEyeBeingTracked !=
            isRightEyeBeingTracked) {
            
            throw new VrHardwareSystemException(
                "Not all the existing hardware nodes of the HMD hardware device have the same " +
                "tracking state. This should normally be impossible, and result in an illegal " + 
                "state, so no updates were performed at all."  + skippedNodes + " of the given " +
                "nodes were skipped because they do not belong to this device."
            );
        }
        
        CurrentTrackingState = isHeadBeingTracked ? TrackingState.Tracking : TrackingState.Untracked;

        if (IsTracking == false) {

            if (wasTracking) {

                CurrentUserPresenceState = UserPresenceState.Unknown;
                
                Debug.Log("HMD lost tracking. Setting user presence to \"unknown\".");
            }
        }
        else {

            if (wasTracking == false) {
                
                Debug.Log("HMD has regained tracking.");
            }
            
            CurrentUserPresenceState = XRDevice.userPresence;
        }
    }
    
    public void UpdateHardwareState(IEnumerable<XRNodeState> nodeStates) {
        
        bool isHeadUpdated = false;
        bool isCenterEyeUpdated = false;
        bool isLeftEyeUpdated = false;
        bool isRightEyeUpdated = false;

        int skippedNodes = 0;

        Vector3 tempHeadPosition = Vector3.zero;
        Quaternion tempHeadRotation = Quaternion.identity;
               
        Vector3 tempCenterEyePosition = Vector3.zero;
        Quaternion tempCenterEyeRotation = Quaternion.identity;
        
        Vector3 tempLeftEyePosition = Vector3.zero;
        Quaternion tempLeftEyeRotation = Quaternion.identity;
        
        Vector3 tempRightEyePosition = Vector3.zero;
        Quaternion tempRightEyeRotation = Quaternion.identity;
        
        foreach (XRNodeState state in nodeStates) {

            if (BelongsToThisDevice(state) == false) {
                skippedNodes++;
                continue;
            }
            
            if (state.nodeType == XRNode.Head) {
                
                if (state.TryGetPosition(out tempHeadPosition) == false) {
            
                    throw new VrHardwareSystemException(
                        GenerateUpdateExceptionString("Head", "position")
                    );
                }
                if (state.TryGetRotation(out tempHeadRotation) == false) {
            
                    throw new VrHardwareSystemException(
                        GenerateUpdateExceptionString("Head", "rotation")
                    );
                }

                isHeadUpdated = true;
            }
            else if (state.nodeType == XRNode.CenterEye) {
                        
                if (state.TryGetPosition(out tempCenterEyePosition) == false) {
            
                    throw new VrHardwareSystemException(
                        GenerateUpdateExceptionString("Center Eye", "position")
                    );
                }
                if (state.TryGetRotation(out tempCenterEyeRotation) == false) {
            
                    throw new VrHardwareSystemException(
                        GenerateUpdateExceptionString("Center Eye", "rotation")
                    );
                }

                isCenterEyeUpdated = true;
            }
            else if (state.nodeType == XRNode.LeftEye) {
                
                if (state.TryGetPosition(out tempLeftEyePosition) == false) {
            
                    throw new VrHardwareSystemException(
                        GenerateUpdateExceptionString("Left Eye", "position")
                    );
                }
                if (state.TryGetRotation(out tempLeftEyeRotation) == false) {
            
                    throw new VrHardwareSystemException(
                        GenerateUpdateExceptionString("Left Eye", "rotation")
                    );
                }

                isLeftEyeUpdated = true;
            }
            else if (state.nodeType == XRNode.RightEye) {
                
                if (state.TryGetPosition(out tempRightEyePosition) == false) {
            
                    throw new VrHardwareSystemException(
                        GenerateUpdateExceptionString("Right Eye", "position")
                    );
                }
                if (state.TryGetRotation(out tempRightEyeRotation) == false) {
            
                    throw new VrHardwareSystemException(
                        GenerateUpdateExceptionString("Right Eye", "rotation")
                    );
                }

                isRightEyeUpdated = true;
            }
        }
        
        if ((isHeadUpdated && isCenterEyeUpdated && isLeftEyeUpdated && isRightEyeUpdated)
            == false) {
            
            throw new VrHardwareSystemException(
                "Not all the existing hardware nodes of the HMD hardware device could be " + 
                "updated successfully. " + skippedNodes + " of the given nodes were skipped " +
                "because they do not belong to this device."
            );
        }
        
        headPosition = tempHeadPosition;
        headRotation = tempHeadRotation;
        
        centerEyePosition = tempCenterEyePosition;
        centerEyeRotation = tempCenterEyeRotation;
        
        leftEyePosition = tempLeftEyePosition;
        leftEyeRotation = tempLeftEyeRotation;
        
        rightEyePosition = tempRightEyePosition;
        rightEyeRotation = tempRightEyeRotation;
        
        LastUpdatedFrame = Time.frameCount;
    }

    private static string GenerateUpdateExceptionString(string nodeName, string updateValue) {
        
        return (
            "Attempting to update the " + updateValue + " of the \"" + nodeName + "\" hardware " + 
            "node of this HMD hardware device failed, because it was not possible to acquire " + 
            "that value from Unity's XR device."
        );
    }

    private static string GenerateMultipleNodesExceptionString(string nodeName) {

        return (
            "The XR node states list given to create an HMD hardware device from contains " +
            "multiple nodes of the \"" + nodeName + "\" node type. This is probably a mistake, " +
            "so the device creation has been cancelled."
        );
    }


    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}