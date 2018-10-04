using System;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {

    
/// <summary>
/// An implementation of <see cref="EditorLayouter"/> specifically for Unity
/// <a href="https://docs.unity3d.com/Manual/editor-PropertyDrawers.html">custom Property
/// Drawers</a>.<br/>
/// This layouter is intended to be used for properties of a fixed height, meaning that each
/// instance of the property has the exact same, given, height, and this height cannot change
/// throughout the (entire) lifetime of the instance.<br/>
/// <br/>
/// In addition to everything from <see cref="EditorLayouter"/>, this class takes a value for a
/// maximum height on construction, and will then continuously check its current height against that
/// value. If the current height would exceed it, the operation is refused and a
/// <see cref="LayoutException"/> is thrown instead.<br/>
/// Instances of this class do not manage any external state, and can thus be constructed once and
/// then reused, in theory (although reconstructing them is cheap).
/// </summary>
public sealed class FixedHeightDrawerLayouter : EditorLayouter {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// The maximum height that the property this property drawer belongs to may have.
    /// </summary>
    public float MaximumHeight { get; }


    /* ---------- Constructors ---------- */

    public FixedHeightDrawerLayouter(Rect startingRect, float maximumHeight) {
        
        if (ComponentUtils.GreaterThanZero(startingRect.width) == false) {

            throw new ArgumentException(
                "The width of the starting rectangle (" + startingRect.width + ") inside of a " + 
                "property drawer must be positive (>0)."
            );
        }
        if (ComponentUtils.GreaterThanZero(maximumHeight) == false) {

            throw new ArgumentException(
                "The maximum height (" + maximumHeight + ") for a fixed-width property drawer " + 
                "must be positive (>0)."
            );
        }

        this.startingRect = startingRect;
        MaximumHeight = maximumHeight;
        
        ResetValues();
    }


    /* ---------- Overrides ---------- */ 
    
    /// <summary>
    /// Adds the given height value to the height of the property drawer. By default,
    /// <see cref="EditorLayouter.PaddingRows"/> is also added to the height.
    /// </summary>
    /// <param name="heightValue">The height value to add.</param>
    /// <param name="addPadding">
    /// Whether to add the value of <see cref="EditorLayouter.PaddingRows"/> to the height as well.
    /// </param>
    /// <exception cref="LayoutException">
    /// If the new resulting <see cref="EditorLayouter.totalHeight"/> would be greater than
    /// <see cref="MaximumHeight"/>.
    /// </exception>
    public override void AddHeightFromValue(float heightValue, bool addPadding = true) {
        
        float tempNewHeight = (totalHeight + heightValue);

        if (addPadding == true) { tempNewHeight += PaddingRows; }

        if (ComponentUtils.GreaterThan(tempNewHeight, MaximumHeight) == true) {
            
            throw new LayoutException(
                "The new total height (" + tempNewHeight + ") of this fixed-height property " +
                "drawer would be greater than its defined maximum height (" + MaximumHeight + 
                "), which is not allowed."
            );
        }
        
        base.AddHeightFromValue(heightValue, addPadding);
    }
}

}