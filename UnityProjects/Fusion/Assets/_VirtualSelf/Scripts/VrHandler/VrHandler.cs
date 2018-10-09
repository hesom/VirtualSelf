using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;
using UnityEngine.XR.WSA;
using VirtualSelf.Utility;

using InputTracking = UnityEngine.XR.InputTracking;
using UserPresenceState = UnityEngine.XR.UserPresenceState;


namespace VirtualSelf.GameSystems {

    
/// <summary>
/// This class is a handler for the VR integration of the game.<br/>
/// It abstracts away the specific VR SDK(s) the game is using, and provides control over them and
/// the functionality they offer.<br/>
/// The class only uses Unity's own VR (XR, actually) API, so there are no additional dependencies -
/// however, this also means that only a (potentially pretty small) subset of the full possible
/// functionality of each of the possible VR SDKs is being available.<br/>
/// As we do not need a lot of VR functionality, this should be sufficient.<br/>
/// It should also be noted that Unity's VR/XR API is still very new, and not a lot of documentation
/// is available for it. In the future, this class might be able to expose additional functionality,
/// when Unity's API is expanded.<br/>
/// <br/>
/// Currently, the only VR Head-Mounted Display (HMD) available to us is the "HTC Vive". As such, we
/// are only interfacing with the "OpenVR" SDK. It is still abstracted away, but as long as not more
/// is needed, there will be no specific support for anything else, nor a possibility to potentially
/// detect, switch, etc. VR SDKs.<br/>
/// TODO: Send events <br/>
/// <remarks>
/// Following is a list of my findings when experimenting with Unity's <see cref="UnityEngine.XR"/>
/// API. Since most of the documentation is very poor or straight up nonexistent, this is mostly
/// based on trial and error.<br/>
/// Since, as stated above, we are currently only able to work with OpenVR, all my experiments are
/// also based on OpenVR, and will specifically mention it, as well.
/// <list type="bullet">
/// <item><description>
/// <see cref="InputTracking.trackingAcquired"/> and <see cref="InputTracking.trackingLost"/> do not
/// seem to work with OpenVR. They both fire every frame, and also give the wrong tracking state
/// (always <c>true</c>).
/// </description></item>
/// <item><description>
/// <see cref="XRSettings.isDeviceActive"/> and <see cref="XRDevice.isPresent"/> do not seem to
/// ever change with OpenVR, so they are useless for us.
/// </description></item>
/// <item><description>
/// <see cref="WorldManager.state"/> is never updated with OpenVR, and neither does
/// <see cref="WorldManager.OnPositionalLocatorStateChanged"/> ever fire. Thus, they are both
/// useless for us.
/// </description></item>
/// </list>
/// </remarks>
/// </summary>
public sealed class VrHandler : MonoBehaviour { 
    
    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// The name of the VR SDK corresponding to OpenVR. This name is hardcoded into Unity. It is
    /// used to tell Unity, at runtime, to enable/disable this specific VR SDK.
    /// </summary>
    private const string VrSdkNameOpenVr = "OpenVR";
          
    /* Further VR SDKs, like "Oculus", could be specified here, by using a List instead of a single
     * string. They could then be iterated over and all checked, etc. As mentioned, we have no need
     * for this as of currently, and likely never will. */

    /// <summary>
    /// Denotes whether "VR Mode" is currently active, or not. If this is <c>true</c>, Unity will be
    /// interfacing with the currently selected VR SDK, and using it for e.g. controlling the
    /// camera, and so on. If this is <c>false</c>, VR will be disabled for Unity.<br/>
    /// This cannot be set directly - it is instead set by <see cref="EnableVrMode"/> and
    /// <see cref="DisableVrMode"/>. See those methods for details on the preconditions required
    /// for this class to be able to enable VR Mode.
    /// </summary>
    public bool IsVrModeActive { get; private set; }
    
    /// <summary>
    /// TODO
    /// </summary>
    public Optional<VrHardwareSystem> HardwareSystem { get; private set; } = 
           Optional<VrHardwareSystem>.Empty();
    
    public bool IsVrHardwareSystemAvailable => (HardwareSystem.IsPresent() == true);
    
    /// <summary>
    /// The key to toggle "VR Mode" (see <see cref="IsVrModeActive"/> for details) on and off. This
    /// calls <see cref="EnableVrMode"/> and <see cref="DisableVrMode"/>, check those for details
    /// and preconditions on what is required for this class to be able to enable VR Mode.
    /// </summary>
    public KeyCode ToggleVrModeKey = KeyCode.V;
    
    /// <summary>
    /// Whether to automatically enable "VR Mode" (see <see cref="IsVrModeActive"/> for details)
    /// after this class has started up, and successfully connected to the chosen VR SDK. If this
    /// is <c>false</c>, even after this happened VR will not be available, until
    /// <see cref="EnableVrMode"/> is called manually.
    /// </summary>
    [SerializeField]
    private bool automaticallyEnableVrMode = true;
    
    /* As stated further above, the following field could become a List instead if we cared about
     * multiple VR SDKs. */
    
    /// <summary>
    /// Denotes whether the currently selected VR SDK has been found on the system by Unity, was
    /// running, and has been successfully loaded by Unity, or not.<br/>
    /// This is set to <c>true</c> by <see cref="TryToLoadVrSdk"/>, if it runs successfully. As long
    /// as it is <c>false</c>, VR Mode will not be available.
    /// </summary>
    private bool isVrSdkLoaded;

    // TODO
    private bool isVrHardwareSystemSetUp;

    private bool isFirstVrModeActivation = true;
    
    private readonly List<XRNodeState> vrNodeStates = new List<XRNodeState>();


    /* ---------- Methods ---------- */
    
    private void Start() {

        /* At the beginning, we disable VR in Unity, just to make sure (it should already be
         * disabled because of the project settings, though. It will be re-enabled by us if we can
         * successfully start up and connect to OpenVR later on. */

        XRSettings.enabled = false;
              
        /* Currently, we just try to connect to OpenVR automatically here. I don't really see a
         * reason why the user should be able to do this manually. If they don't want VR to be
         * enabled, they can use the flag to disable the automatic "VR Mode" activation, and if the
         * first attempt of connecting to OpenVR fails, any subsequent one probably will as well.
         * However, if this is really needed, we can just build it in later easily most likely. */
        
        Debug.Log("VrHandler: Starting up; attempting to connect to the currently chosen VR SD...");

        StartCoroutine(TryToLoadVrSdk());
    }
       
    private void Update() {

        if (Input.GetKeyDown(ToggleVrModeKey)) {

            if (IsVrModeActive == false) { EnableVrMode(); }
            else { DisableVrMode(); }
        }
        else if (Input.GetKeyDown(KeyCode.H)) {
            
            if (IsVrHardwareSystemAvailable) {
                
                Debug.Log(HardwareSystem.Get().PrintHardwareData());
            }
        }
        
        /* If no VR is available, we don't even need to do anything here. */

        if (isVrSdkLoaded == false) { return; }

        if (IsVrModeActive == true) {

            Hmd.TrackingState oldHmdTrackingState =
                HardwareSystem.Get().Hmd.CurrentTrackingState;
            UserPresenceState oldHmdUserPresenceState =
                HardwareSystem.Get().Hmd.CurrentUserPresenceState;
            
            vrNodeStates.Clear();
            InputTracking.GetNodeStates(vrNodeStates);
            
            HardwareSystem.Get().UpdateHardwareStates(vrNodeStates);
            
            Hmd.TrackingState newHmdTrackingState = HardwareSystem.Get().Hmd.CurrentTrackingState;
            UserPresenceState newHmdUserPresenceState = 
                HardwareSystem.Get().Hmd.CurrentUserPresenceState;

            if (oldHmdTrackingState != newHmdTrackingState) {
                
                Debug.Log(
                    "HMD tracking state has changed, from \"" + oldHmdTrackingState + "\" to \"" +
                    newHmdTrackingState + "\"."
                );
            }
            
            if (oldHmdUserPresenceState != newHmdUserPresenceState) {

                Debug.Log(
                    "HMD user presence state has changed, from \"" + oldHmdUserPresenceState +
                    "\" to \"" + newHmdUserPresenceState + "\"."
                );
            }
            
//            Debug.Log(
//                "HMD Head: Position: " + HardwareSystem.Get().Hmd.HeadPosition.ToString("F4") +
//                ", Rotation: " + HardwareSystem.Get().Hmd.HeadRotation.eulerAngles.ToString("F4")
//            );
        }
    }


    /// <summary>
    /// Enables "VR Mode" (see <see cref="IsVrModeActive"/> for details) for the game.<br/>
    /// This only works if the currently chosen VR SDK has been found to be available, and
    /// successfully connected to. This is currently attempted automatically when this class starts
    /// up.
    /// <remarks>
    /// If VR Mode is already enabled, this method will do nothing. It will not attempt to enable it
    /// again.
    /// </remarks>
    /// </summary>
    /// <returns>
    /// <c>true</c> if VR Mode was successfully enabled, and <c>false</c> in any other
    /// case.
    /// </returns>
    public bool EnableVrMode() {

        if (IsVrModeActive == true) {
            
            Debug.Log("VR Mode is already active! Cancelling attempt to enable it again.");
            return (false);
        }

        if (isVrSdkLoaded == false) {
            
            Debug.LogError("No VR SDK is available, so VR Mode could not be enabled.");
            return (false);
        }

        if (isFirstVrModeActivation) {
            
            Debug.Log(
                "VrHandler: VR Mode is supposed to be enabled for the first time. The VR " +
                "hardware system is being set up..."
            );
            
            SetupVrHardwareSystem();

            isFirstVrModeActivation = true;
        }

        if (isVrHardwareSystemSetUp == false) {
            
            Debug.LogError(
                "VrHandler: Cannot enable VR Mode, because the VR hardware system has not been " +
                "set up correctly yet."
            );
            return (false);
        }
        
        Debug.Log("VrHandler: Enabling VR Mode.");
        
        XRSettings.enabled = true;
        IsVrModeActive = true;
        
        // TODO: Send an event here.

        return (true);
    }

    /// <summary>
    /// Disables "VR Mode" (see <see cref="IsVrModeActive"/> for details) for the game.
    /// <remarks>
    /// If VR Mode is already disabled, this method will do nothing. It will not attempt to disable
    /// it again.
    /// </remarks>
    /// </summary>
    /// <returns>
    /// <c>true</c> if VR Mode was successfully disabled, and <c>false</c> in any other
    /// case.
    /// </returns>
    public bool DisableVrMode() {

        if (IsVrModeActive == false) {
            
            Debug.Log("VR Mode was not enabled! Cancelling attempt to disable it.");
            return (false);
        }
        
        Debug.Log("VrHandler: Disabling VR Mode.");
       
        XRSettings.enabled = false;
        IsVrModeActive = false;
        
        // TODO: Send an event here

        return (true);
    }

    /// <summary>
    /// Attempts to load the currently selected VR SDK (which is always OpenVR, as of now).<br/>
    /// This can only succeed if the respective VR SDK is installed on the system, and running, so
    /// that Unity can establish a connection to it.<br/>
    /// If a VR SDK is already running, this will return and do nothing (having multiple VR SDKs
    /// loaded at the same time is currently not supported).<br/>
    /// Notice that this method runs over (exactly) <b>two frames</b>, as it takes one frame for the
    /// VR SDK to actually be loaded by Unity. The method will wait in between these, and the result
    /// of the loading operation will be available (and checked) on the next frame.<br/>
    /// TODO
    /// </summary>
    private IEnumerator TryToLoadVrSdk() {

        if (isVrSdkLoaded == true) {
            
            Debug.Log(
                "A VR SDK is already loaded, and running. Multiple VR SDKs are currently not " +
                "supported, so nothing will be done."
            );
            yield break;
        }
        
        Debug.Log(
            "VrHandler: Attempting to load the currently selected VR SDK. (This is \"" + 
            VrSdkNameOpenVr + "\"). The results of this will only be available during the next " +
            "frame."
        );
        
        XRSettings.LoadDeviceByName(VrSdkNameOpenVr);
        
        /* We now have to wait for at least one frame before we can see if this has worked out.
         * Thus, the rest of the loading process is done after that. */

        yield return (TimeUtils.WaitFor.Frames(1));
        
        if (IsVrSdkLoaded(VrSdkNameOpenVr) == true) {
            
            Debug.Log(
                "VrHandler: The VR SDK, \"" + VrSdkNameOpenVr + "\", has been found and enabled " +
                "successfully. VR Mode will now be available."
            );

            isVrSdkLoaded = true;

            if (automaticallyEnableVrMode) {
                
                Debug.Log("VrHandler: Automatically enabling VR Mode...");
                EnableVrMode();
            }
        }
        else {
            
            Debug.LogWarning(
                "VrHandler: The VR SDK could not be found, or not be loaded. No VR support is " + 
                "possible, and VR Mode will not be available.");
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    private void SetupVrHardwareSystem() {

        if (isVrSdkLoaded == false) {
            
            Debug.LogError(
                "There is currently no VR SDK loaded, so the VR Hardware System cannot be set " +
                "up."    
            );
            return;
        }

        if (isVrHardwareSystemSetUp == true) {
            
            Debug.LogWarning(
                "The VR hardware system has already been set up. Currently, setting it up again " +
                "or setting up multiple systems is not supported. Nothing has been done."
            );
        }
        
        Debug.Log("VrHandler: Attempting to set up the VR hardware system.");

        string hmdModelName = XRDevice.model;
        
        vrNodeStates.Clear();
        InputTracking.GetNodeStates(vrNodeStates);

        VrHardwareSystem hardwareSystem;
        
        try {

            hardwareSystem = VrHardwareSystem.CreateFromNodeStates(vrNodeStates, hmdModelName);
        }
        catch (VrHardwareSystemException) {
            
            Debug.LogError(
                "VrHandler: An exception occurred while trying to set up the VR hardware system. " +
                "The system setup failed. This means that no data about any VR hardware will be " + 
                "available.\n" +
                "The exception will be re-thrown now."
            );
            throw;
        }
        
        HardwareSystem = Optional<VrHardwareSystem>.Of(hardwareSystem);
        isVrHardwareSystemSetUp = true;
        
        Debug.Log(
            "VrHandler: The VR hardware system has been set up successfully. Data about the " +
            "connected and running VR hardware is now available.\n" +
            "However, notice that there will only be updated data for the hardware system once " +
            "VR Mode has been enabled, had been enabled for at least one frame, and the HMD " +
            "device is currently tracking."
        );
    }
        
    /// <summary>
    /// Returns whether the VR SDK with the given name is currently loaded within Unity, or not.
    /// </summary>
    /// <remarks>
    /// It does not matter whether VR Mode is currently enabled or not - this method is only
    /// concerned with whether the VR SDK is loaded at all.
    /// </remarks>
    /// <param name="vrSdkName">
    /// The name of the VR SDK to check.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="vrSdkName"/> is currently loaded within Unity, and
    /// <c>false</c> otherwise.
    /// </returns>
    private bool IsVrSdkLoaded(string vrSdkName) {

        return (XRSettings.loadedDeviceName == vrSdkName);
    }
}

}