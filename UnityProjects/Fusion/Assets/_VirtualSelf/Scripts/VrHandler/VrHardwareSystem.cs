using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.XR;
using VirtualSelf.Utility;


namespace VirtualSelf.GameSystems {

/// <summary>
/// TODO: Fill out this class description: VrHardware
/// </summary>
public sealed class VrHardwareSystem {

    /* ---------- Variables & Properties ---------- */

    public Hmd Hmd { get; }
    
    public IList<TrackingCamera> TrackingCameras => (trackingCameras.AsReadOnly());

    public Optional<PlayArea> PlayArea { get; private set; } = Optional<PlayArea>.Empty();
    
    public bool IsPlayAreaAvailable => (PlayArea.IsPresent() == true);
    
    private List<TrackingCamera> trackingCameras = new List<TrackingCamera>();
    
    private bool haveCamerasBeenSearched = false;


    /* ---------- Constructors ---------- */

    private VrHardwareSystem(Hmd hmd) {

        Hmd = hmd;
    }


    /* ---------- Methods ---------- */

    public static VrHardwareSystem CreateFromNodeStates(
            IEnumerable<XRNodeState> nodeStates, string hmdModel) {
        
        Hmd hmd = Hmd.CreateFromNodeStates(nodeStates, hmdModel);
        
        return (new VrHardwareSystem(hmd));
    }

    public void UpdateHardwareStates(IEnumerable<XRNodeState> nodeStates) {
               
        /* First, we let the HMD update its tracking state. This will determine what else we want to
         * do. */
        
        Hmd.UpdateTrackingState(nodeStates);

        /* If the HMD is not tracking, we don't want to update the positions etc. of all the VR
         * hardware systems. This means that we will keep the last known state around, instead of
         * resetting values to e.g. the world origin (of course, Unity will still do this to the
         * cameras, etc., on its own). */
        
        if (Hmd.IsTracking == false) {

            return;
        }
        
        /* If the HMD is tracking, we can now update all the other hardware. */
        
        Hmd.UpdateHardwareState(nodeStates);

        if (haveCamerasBeenSearched == false) {
            
            Debug.Log("Searching for available tracking camera hardware devices...");
            
            List<TrackingCamera> cameras = new List<TrackingCamera>();

            foreach (XRNodeState state in nodeStates) {

                if (TrackingCamera.IsTrackingCameraNodeState(state)) {
                
                    cameras.Add(TrackingCamera.CreateFromNodeState(state));
                }
                else {
                    
                    Debug.Log("The node called \"" + InputTracking.GetNodeName(state.uniqueID) + "\" is not a camera...");
                }
            }

            if (cameras.Count == 0) {
                
                Debug.LogWarning(
                    "No tracking camera hardware devices have been found. Either there are none, " +
                    "or their data was not exposed by the currently selected VR SDK."
                );
            }
            else {
                
                Debug.Log(
                    cameras.Count + " tracking camera hardware devices have been found. Adding " + 
                    "them to the hardware system. Their data will be available from now on."
                );

                trackingCameras = cameras;
            }

            haveCamerasBeenSearched = true;
        }
        else {
            
            foreach (XRNodeState state in nodeStates) {

                bool found = false;

                foreach (TrackingCamera camera in trackingCameras) {

                    if (camera.BelongsToThisDevice(state) == false) { continue; }

                    if (found == true) {
                        
                        throw new VrHardwareSystemException(
                            "There seem to be more than one XR node states among the given ones " +
                            "which belong to the same tracking camera hardware device. This " +
                            "should not be possible..."
                        );
                    }

                    found = true;
                    camera.UpdateHardwareState(state);
                }
            }
        }
        
        /* Lastly, if we have not yet managed to acquire any "Play Area" data yet, we will try this
         * now - and then again every time this method runs, until we get the data.
         * This is required because many VR SDKs, like OpenVR too, can only acquire data about the
         * Play Area (or tracking space, or however it is called within that SDK) while VR is
         * enabled, an HMD is connected, and the HMD is currently being tracked and within the Play
         * Area.
         * Thus, it may take a bit of time, after when this class has been created, until all the
         * conditions are fulfilled (for example, the HMD could still be lying somewhere in the
         * room, outside of the Play Area, until someone picks it up and takes it into that area).
         * 
         * If we already have the data, we do not need to try to get it again, because we do not
         * support changing the Play Area while the game is running (I don't even know if OpenVR
         * supports this at all, anyway). */

        if (IsPlayAreaAvailable == false) {

            Optional<PlayArea> playArea = VirtualSelf.GameSystems.PlayArea.CreateFromVrData();

            if (playArea.IsPresent()) {
                
                PlayArea = playArea;
                
                Debug.Log(
                    "VrHandler: \"Play Area\" data queried successfully. The data is:\n" +
                    "Corner 01: " + PlayArea.Get().Corner01 + ", " +
                    "Corner 02: " + PlayArea.Get().Corner02 + ", " +
                    "Corner 03: " + PlayArea.Get().Corner03 + ", " +
                    "Corner 04: " + PlayArea.Get().Corner04 + ", " +
                    "Width: " + PlayArea.Get().Width + ", " +
                    "Length: " + PlayArea.Get().Length
                );
            }
        }
    }

    public string PrintHardwareData() {
        
        StringBuilder dataBuilder = new StringBuilder();
        dataBuilder.AppendLine("VR Hardware System data:");
        
        dataBuilder.Append("Contains: HMD (model: \"");
        dataBuilder.Append(Hmd.Model);
        dataBuilder.Append("\"), ");
        dataBuilder.Append(TrackingCameras.Count);
        dataBuilder.Append(" tracking cameras, and");
        if (IsPlayAreaAvailable == false) { dataBuilder.Append(" no"); }
        else { dataBuilder.Append(" available"); }
        dataBuilder.AppendLine(" Play Area data.");

        dataBuilder.AppendLine("HMD:");
        dataBuilder.Append("Last updated in frame ");
        dataBuilder.Append(Hmd.LastUpdatedFrame);
        dataBuilder.Append("\n");
        
        dataBuilder.Append("Head: ID ");
        dataBuilder.Append(Hmd.UniqueIdHead);
        dataBuilder.Append(" ; Pos ");
        dataBuilder.Append(Hmd.HeadPosition);
        dataBuilder.Append("; Rot ");
        dataBuilder.Append(Hmd.HeadRotation.eulerAngles);
        dataBuilder.Append("\n");
        
        dataBuilder.Append("Center Eye: ID ");
        dataBuilder.Append(Hmd.UniqueIdCenterEye);
        dataBuilder.Append(" ; Pos ");
        dataBuilder.Append(Hmd.CenterEyePosition);
        dataBuilder.Append("; Rot ");
        dataBuilder.Append(Hmd.CenterEyeRotation.eulerAngles);
        dataBuilder.Append("\n");
        
        dataBuilder.Append("Left Eye: ID ");
        dataBuilder.Append(Hmd.UniqueIdLeftEye);
        dataBuilder.Append(" ; Pos ");
        dataBuilder.Append(Hmd.LeftEyePosition);
        dataBuilder.Append("; Rot ");
        dataBuilder.Append(Hmd.LeftEyeRotation.eulerAngles);
        dataBuilder.Append("\n");
        
        dataBuilder.Append("Right Eye: ID ");
        dataBuilder.Append(Hmd.UniqueIdRightEye);
        dataBuilder.Append(" ; Pos ");
        dataBuilder.Append(Hmd.RightEyePosition);
        dataBuilder.Append("; Rot ");
        dataBuilder.Append(Hmd.RightEyeRotation.eulerAngles);
        dataBuilder.Append("\n");

        dataBuilder.AppendLine("Tracking Cameras: ");

        if (TrackingCameras.Count == 0) {

            dataBuilder.AppendLine("None");
        }
        else {

            int count = 0;
            foreach (TrackingCamera camera in TrackingCameras) {

                dataBuilder.Append("Camera ");
                dataBuilder.Append(count++);
                dataBuilder.Append(": ");
                dataBuilder.Append("Name: \"");
                dataBuilder.Append(camera.Name);
                dataBuilder.Append("\"; ");
                dataBuilder.Append("ID ");
                dataBuilder.Append(camera.UniqueId);
                dataBuilder.Append(" ; Pos ");
                dataBuilder.Append(camera.Position);
                dataBuilder.Append("; Rot ");
                dataBuilder.Append(camera.Rotation.eulerAngles);
                dataBuilder.Append("\n");
            }
        }
        
        dataBuilder.AppendLine("Play Area: ");

        if (IsPlayAreaAvailable == false) {

            dataBuilder.Append("Not yet queried; not available.");
        }
        else {

            dataBuilder.Append("Corner 01: ");
            dataBuilder.Append(PlayArea.Get().Corner01);
            dataBuilder.Append(", ");
            dataBuilder.Append("Corner 02: ");
            dataBuilder.Append(PlayArea.Get().Corner02);
            dataBuilder.Append(", ");
            dataBuilder.Append("Corner 03: ");
            dataBuilder.Append(PlayArea.Get().Corner03);
            dataBuilder.Append(", ");
            dataBuilder.Append("Corner 04: ");
            dataBuilder.Append(PlayArea.Get().Corner04);
            dataBuilder.Append(", Width: ");
            dataBuilder.Append(PlayArea.Get().Width);
            dataBuilder.Append(", ");
            dataBuilder.Append(", Length: ");
            dataBuilder.Append(PlayArea.Get().Length);
        }

        return (dataBuilder.ToString());
    }
    

    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}