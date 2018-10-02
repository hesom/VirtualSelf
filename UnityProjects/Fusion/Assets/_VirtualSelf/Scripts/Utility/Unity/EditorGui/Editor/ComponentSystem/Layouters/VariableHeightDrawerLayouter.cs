using System;
using UnityEditor;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {


/// <summary>
/// An implementation of <see cref="EditorLayouter"/> specifically for Unity
/// <a href="https://docs.unity3d.com/Manual/editor-PropertyDrawers.html">custom Property
/// Drawers</a>.<br/>
/// This layouter is intended to be used for properties of variable height, meaning that each
/// instance of the property could have a different height, and the height might change at any time
/// during the lifetime of the instance.<br/>
/// <br/>
/// In addition to everything from <see cref="EditorLayouter"/>, this class takes and manages a
/// <see cref="SerializedProperty"/> for a height value of the property that is being drawn - this
/// way, no matter where the property is being drawn (like inside of a list), it will have the
/// correct height and the height can change dynamically.<br/>
/// Instances of this class are supposed to be constructed anew on every drawing call of the
/// property drawer, since they manage external state.
/// </summary>
public sealed class VariableHeightDrawerLayouter : EditorLayouter {

	/* ---------- Variables & Properties ---------- */
	
	/// <summary>
	/// The serialized property containing the height of the property that the property drawer
	/// belongs to.<br/>
	/// This is updated by this layouter whenever its height is updated through
	/// <see cref="AddHeightFromValue"/> (or any of the corresponding methods).
	/// </summary>
	private readonly SerializedProperty totalHeightProperty;

	
	/* ---------- Constructors ---------- */

	/// <summary>
	/// Constructs a new (variable-height) property drawer layouter, starting at
	/// <see cref="startingRect"/>, and managing the "total height" property
	/// <see cref="totalHeightProperty"/>.
	/// </summary>
	/// <param name="startingRect">
	/// The rectangle that determines where the property drawer starts (in both X- and Y-direction),
	/// and what its width is.
	/// </param>
	/// <param name="totalHeightProperty">
	/// The property referring to the "total height" value for the property that the property drawer
	/// belongs to, to be managed by this layouter. This must be of type
	/// <see cref="SerializedPropertyType"/>.<see cref="SerializedPropertyType.Float"/>.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If the width of <paramref name="startingRect"/> is not positive (>0) for some reason, or if
	/// <paramref name="totalHeightProperty"/> is not of type
	/// <see cref="SerializedPropertyType.Float"/>.
	/// </exception>
	public VariableHeightDrawerLayouter(Rect startingRect, SerializedProperty totalHeightProperty) {
		
		if (ComponentUtils.GreaterThanZero(startingRect.width) == false) {

			throw new ArgumentException(
				"The width of the starting rectangle (" + startingRect.width + ") inside of a " + 
				"property drawer must be positive (>0)."
			);
		}
		if (totalHeightProperty.propertyType != SerializedPropertyType.Float) {
			
			throw new ArgumentException(
				"The serialized property for tracking the total height of the property drawer " + 
				" must be of property type \"Float\" (but was of type \"" + 
				totalHeightProperty.propertyType + "\" instead)."
			);	
		}

		this.startingRect = startingRect;
		this.totalHeightProperty = totalHeightProperty;

		ResetValues();
	}

		
	/* ---------- Methods ---------- */
	
	/// <summary>
	/// Updates the value of <see cref="totalHeightProperty"/>, setting it to the total height of
	/// the property drawer.
	/// </summary>
	private void UpdateTotalHeightProperty() {

		totalHeightProperty.floatValue = GetTotalHeight();
	}
	

	/* ---------- Overrides ---------- */

	/// <inheritdoc/>
	public override void AddHeightFromValue(float heightValue, bool addPadding = true) {

		base.AddHeightFromValue(heightValue, addPadding);
		
		UpdateTotalHeightProperty();
	}
	
	/// <inheritdoc/>
	public override void ResetValues() {
		
		base.ResetValues();
		
		UpdateTotalHeightProperty();
	}
}

}