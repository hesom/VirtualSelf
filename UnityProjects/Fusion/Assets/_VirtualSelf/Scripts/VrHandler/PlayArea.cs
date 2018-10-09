using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using VirtualSelf.Utility;


namespace VirtualSelf.GameSystems {

    
/// <summary>
/// This class models a VR "Play Area". This is a term commonly used to describe the area/space that
/// is available to the user for VR interaction. Other common terms are "tracking area", "tracking
/// space", "play space", and more.<br/>
/// Most VR SDKs (like, for example, OpenVR) force this to be a rectangular area. The user defines
/// the area by walking along its edges, or by defining corners, and the SDK then creates a
/// rectangle fitting into this polygon. As this is the lowest common denominator for VR SDKs, this
/// class also models a rectangle.<br/>
/// How exactly the Play Area is defined also varies from one VR SDK to another. It can be assumed,
/// though, that the Play Area only contains "tracked space". This means that tracking for the VR
/// HMD (and any other VR equipment, like controllers) is available everywhere within that area.
/// If the user leaves the area, though, "all bets are off".<br/>
/// The Play Area is commonly assumed to have no height. In reality, there obviously is a height,
/// but if the tracking equipment is placed correctly and there is no weird setup (with e.g. a very
/// high ceiling and something to climb on available), this is never a problem in practice. If it
/// is, this can be assumed to be the user's fault, and is out of the scope of this game to handle.
/// <br/><br/>
/// This class is currently a simple data class, and features no logic. This might change in the
/// future, if required.
/// <remarks>
/// Instances of this class are supposed to be created by the <see cref="VrHandler"/> class. They
/// should not be created "by hand" (as it would make no sense to do so).
/// </remarks>
/// </summary>
public sealed class PlayArea {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// The amount of corners we expect the Play Area to have. Some VR SDKs might return a Play Area
    /// with a different amount, but this class cannot work with that.
    /// </summary>
    private const int CornersAmount = 4;

    /// <summary>
    /// The first corner of the Play Area. Which corner this exactly is, is dependent on the
    /// implementation of the VR SDK the data was queried from.
    /// </summary>
    public Vector3 Corner01 { get; }
    
    /// <summary>
    /// The second corner of the Play Area. Which corner this exactly is, is dependent on the
    /// implementation of the VR SDK the data was queried from.
    /// </summary>
    public Vector3 Corner02 { get; }
    
    /// <summary>
    /// The third corner of the Play Area. Which corner this exactly is, is dependent on the
    /// implementation of the VR SDK the data was queried from.
    /// </summary>
    public Vector3 Corner03 { get; }
    
    /// <summary>
    /// The fourth corner of the Play Area. Which corner this exactly is, is dependent on the
    /// implementation of the VR SDK the data was queried from.
    /// </summary>
    public Vector3 Corner04 { get; }

    /// <summary>
    /// The width of the Play Area; this is the distance between the corners on the X-axis.
    /// </summary>
    public float Width { get; }
    
    /// <summary>
    /// The length of the Play Area; this is the distance between the corners on the Z-axis.
    /// </summary>
    public float Length { get; }

    
    /* ---------- Constructors ---------- */

    /// <summary>
    /// Constructs a new <see cref="PlayArea"/> from the given corners and dimensions.
    /// </summary>
    /// <param name="corner01">The first corner of the Play Area.</param>
    /// <param name="corner02">The second corner of the Play Area.</param>
    /// <param name="corner03">The third corner of the Play Area.</param>
    /// <param name="corner04">The fourth corner of the Play Area.</param>
    /// <param name="width">The width of the Play Area.</param>
    /// <param name="length">The length of the Play Area.</param>
    private PlayArea(
        Vector3 corner01, Vector3 corner02, Vector3 corner03, Vector3 corner04,
        float width, float length) {

        Corner01 = corner01;
        Corner02 = corner02;
        Corner03 = corner03;
        Corner04 = corner04;

        Width = width;
        Length = length;
    }
    
    
    /* ---------- Methods ---------- */

    public static Optional<PlayArea> CreateFromVrData() {
        
        List<Vector3> corners = new List<Vector3>();

        if (Boundary.TryGetGeometry(corners, Boundary.Type.PlayArea) == false) {

            return (Optional<PlayArea>.Empty());
        }

        // TODO: Does this return the length of the sides??
        
        Vector3 dimensions;
        
        if (Boundary.TryGetDimensions(out dimensions, Boundary.Type.PlayArea) == false) {

            Debug.LogWarning("I guess \"TryGetDimensions()\" doesn't want to work...");
        }
        
        Debug.Log("Play Area \"TryGetDimensions()\" returns: " + dimensions);

        if (corners.Count != CornersAmount) {
            
            throw new VrHardwareSystemException(
                "The currently loaded VR SDK returned a Play Area with " + corners.Count +
                " corners, but this class can only represent a Play Area with " + CornersAmount +
                " corners. Cannot create a Play Area from the loaded data."
            );
        }
        
        /* Calculate the length of the sides of the Play Area. I can't remember where I found this
         * code, but it seems to work correctly, at least for the "OpenVR" SDK. */
        
        float width = Math.Abs(corners[0].x - corners[1].x);
        float length = Math.Abs(corners[2].z - corners[1].z);

        return (
            Optional<PlayArea>.Of(
                new PlayArea(
                    corners[0], corners[1], corners[2], corners[3],
                    width, length
                )
            )
        );
    }
}

}