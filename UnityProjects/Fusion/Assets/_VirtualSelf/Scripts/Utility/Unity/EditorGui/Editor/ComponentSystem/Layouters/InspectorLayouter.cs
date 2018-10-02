using System;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {
	

/// <summary>
/// TODO: Unfinished, do not use.
/// </summary>
public sealed class InspectorLayouter : EditorLayouter {
	
	/* ---------- Constructors ---------- */
	
	/// <summary>
	/// Constructs a new inspector layouter, starting at <see cref="startingRect"/>.
	/// </summary>
	/// <param name="startingRect">
	/// The rectangle that determines where the property drawer starts (in both X- and Y-direction),
	/// and what its width is.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If the width of <paramref name="startingRect"/> is not positive (>0) for some reason.
	/// </exception>
	public InspectorLayouter(Rect startingRect) {
		
		if (ComponentUtils.GreaterThanZero(startingRect.width) == false) {

			throw new ArgumentException(
				"The width of the starting rectangle (" + startingRect.width + ") inside of an " + 
				"inspector must be positive (>0).");
		}

		this.startingRect = startingRect;

		ResetValues();
	}
	
	
	/* ---------- Getters & Setters ---------- */

	/// <summary>
	/// Sets a new starting rectangle for this layouter.<br/>
	/// This results in resetting the current total height of the layouter, meaning that it
	/// basically goes back to how it was during its construction.<br/>
	/// After this method has been called, the layouter can be used anew, instead of having to
	/// create an entirely new one for one full "drawing process" of an inspector.
	/// </summary>
	/// <param name="startingRect">The new rectangle to set as the starting rectangle.</param>
	/// <exception cref="ArgumentException">
	/// If the width of <paramref name="startingRect"/> is not positive (>0) for some reason.
	/// </exception>
	public void SetNewStartingRect(Rect startingRect) {
		
		if (ComponentUtils.GreaterThanZero(startingRect.width) == false) {

			throw new ArgumentException(
				"The width of the starting rectangle (" + startingRect.width + ") inside of an " + 
				"inspector must be positive (>0).");
		}

		this.startingRect = startingRect;
		
		ResetValues();
	}
}

}
