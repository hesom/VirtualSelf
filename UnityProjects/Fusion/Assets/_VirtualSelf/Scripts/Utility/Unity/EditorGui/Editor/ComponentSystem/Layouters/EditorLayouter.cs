using UnityEditor;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {

	
/// <summary>
/// This class models an (abstract) layouter specifically for Unity "Editors". Editor, in this
/// context, means anything using the Unity Editor IMGUI. Most prominently, this includes
/// <a href="https://docs.unity3d.com/Manual/editor-CustomEditors.html">custom Inspectors</a> and
/// <a href="https://docs.unity3d.com/Manual/editor-PropertyDrawers.html">custom Property
/// Drawers</a>. It is supposed to simplify placing (and thus, drawing) <see cref="Component"/>s
/// within the specific editor, managing margins and paddings, "updating" the current Y-position
/// within the editor, and whatever else is necessary for the specific kind of editor the layouter
/// is used for.<br/>
/// <br/>
/// It should be noted that this layouter itself is just a simple helper class - it does not store
/// any components or manage them directly. Neither does it to any complex layouting calculations.
/// <br/><br/>
/// The layouter is supposed to be used in something like the following way:
/// <list type="bullet">
/// <item><description>
/// Start at <see cref="GetXPosition"/> and <see cref="GetStartingYPosition"/>
/// (or <see cref="GetCurrentYPosition"/>).
/// </description></item>
/// <item><description>
/// Use <see cref="GetWidth"/> to get the width, keep using <see cref="GetXPosition"/>, as that
/// doesn't change.
/// </description></item>
/// <item><description>
/// After each component has been drawn, call <see cref="AddHeightFromValue"/> (or any of the
/// corresponding methods) to add to the height of the editor.
/// </description></item>
/// <item><description>
/// Use <see cref="GetCurrentYPosition"/> to always get the updated, current Y-position within the
/// editor, to draw/layout components. Padding values are already applied to that.
/// </description></item>
/// <item><description>
/// Always use this layouter - if it ever goes out of sync with the editor, it becomes useless and
/// there will be drawing errors.
/// </description></item>
/// <item><description>
/// The "normal" methods all give values and positions with all set margins, paddings, etc. already
/// applied (so the values can be used as they are given), and should normally be used. The
/// corresponding methods to each of them "<c>Absolute</c>" give the value/position "as it is"
/// instead, meaning without any margins, paddings, etc. applied.
/// </description></item>
/// </list>
/// Concrete subclasses of this class might add different things that are necessary for this
/// layouter to work correctly within the respective kind of editor they are dealing with. In any
/// case, they need to add a constructor, which most likely needs to initialize at least
/// <see cref="startingRect"/>.
/// </summary>
public abstract class EditorLayouter {

	/* ---------- Variables & Properties ---------- */
	
	/// <summary>
	/// The (inner) margins around the editor. All other values (like positions, width and height
	/// values) will take these into account.
	/// </summary>
	public Margins Margins { get; set; } = new Margins(5.0f);

	/// <summary>
	/// The (vertical) padding between each "row" of <see cref="Component"/>s within the editor.
	/// This is added to <see cref="totalHeight"/> automatically by <see cref="AddHeightFromValue"/>
	/// (and the methods related to it).
	/// </summary>
	public float PaddingRows { get; set; } = 5.0f;
	
	/// <summary>
	/// The "starting" rectangle of the editor. This contains the position (<c>X</c> and <c>Y</c>)
	/// where the "public" drawing area of the editor starts, as provided by Unity, as well as its
	/// width.<br/>
	/// This is used by this layouter to determine the coordinates and width for each "row" of
	/// <see cref="Component"/>s.<br/>
	/// The rectangle can be determined in different ways depending on what kind of editor a
	/// subclass of this class manages. For custom property drawers (<see cref="PropertyDrawer"/>),
	/// the rectangle is given by <see cref="PropertyDrawer.OnGUI"/>. For custom inspectors
	/// (<see cref="UnityEditor.Editor"/>), a rectangle is not directly given, but can be retrieved
	/// by calling certain layout methods, like
	/// <see cref="GUILayoutUtility.GetRect(UnityEngine.GUIContent,UnityEngine.GUIStyle)"/>.
	/// </summary>
	protected Rect startingRect;

	/// <summary>
	/// The (current) total height of the editor (as tracked by this layouter). This starts out at
	/// "zero" (not counting margin values), and gets increased by calls to
	/// <see cref="AddHeightFromValue"/> (and the methods related to it).<br/>
	/// This value can not be set directly from the outside. To reset it, <see cref="ResetValues"/>
	/// can be called.
	/// </summary>
	protected float totalHeight;
	
	
	/* ---------- Methods ---------- */

	/// <summary>
	/// Adds the height of the given component to the height of the editor. By default,
	/// <see cref="PaddingRows"/> is also added to the height.
	/// </summary>
	/// <param name="component">The component to add the height of.</param>
	/// <param name="addPadding">
	/// Whether to add the value of <see cref="PaddingRows"/> to the height as well.
	/// </param>
	public void AddHeightFromComponent(Component component, bool addPadding = true) {
		
		AddHeightFromValue(component.Height, addPadding);
	}

	/// <summary>
	/// Adds the height of the given rectangle to the height of the editor. By default,
	/// <see cref="PaddingRows"/> is also added to the height.
	/// </summary>
	/// <param name="rect">The rectangle to add the height of.</param>
	/// <param name="addPadding">
	/// Whether to add the value of <see cref="PaddingRows"/> to the height as well.
	/// </param>
	public void AddHeightFromRect(Rect rect, bool addPadding = true) {
		
		AddHeightFromValue(rect.height, addPadding);
	}
	
	/// <summary>
	/// Adds the given height value to the height of the editor. By default,
	/// <see cref="PaddingRows"/> is also added to the height.
	/// </summary>
	/// <param name="heightValue">The height value to add.</param>
	/// <param name="addPadding">
	/// Whether to add the value of <see cref="PaddingRows"/> to the height as well.
	/// </param>
	public virtual void AddHeightFromValue(float heightValue, bool addPadding = true) {

		totalHeight += heightValue;

		if (addPadding == true) { totalHeight += PaddingRows; }
	}

	/// <summary>
	/// Resets all of the "progress" values of this layouter back to what they were when the
	/// layouter was constructed.<br/>
	/// "Progress" values here means the values that were updated during the drawing and layouting
	/// etc. process of the editor, by calling e.g. <see cref="AddHeightFromValue"/> (and the
	/// methods corresponding to it), like <see cref="totalHeight"/>.<br/>
	/// This method can be called whenever the drawing process for the editor starts from the
	/// beginning again, instead of having to create an entirely new instance of it. This means that
	/// this method is supposed to reset everything back properly - if state is retained, there
	/// will be errors during subsequent drawing processes.
	/// </summary>
	public virtual void ResetValues() {

		totalHeight = 0.0f;
	}
	
	
	/* ---------- Getters & Setters ---------- */
	
	/// <summary>
	/// Returns the "absolute" width of the editor, meaning the width with no margin values etc.
	/// applied.
	/// </summary>
	/// <returns>The absolute width of the editor.</returns>
	public float GetAbsoluteWidth() {

		return (startingRect.width);
	}

	/// <summary>Returns the width of the editor.</summary>
	/// <returns>The width of the editor.</returns>
	public float GetWidth() {

		return (GetAbsoluteWidth() - Margins.Left - Margins.Right);
	}
	
	/// <summary>
	/// Returns the "absolute" X-position of the editor, meaning the X-position with no margin
	/// values etc. applied.<br/>
	/// The "X-position" is the X-value of the very left side of the drawing area of the editor.
	/// </summary>
	/// <returns>The absolute X-position of the editor.</returns>
	public float GetAbsoluteXPosition() {

		return (startingRect.x);
	}
	
	/// <summary>
	/// Returns the X-position of the editor.<br/>
	///	The "X-position" is the (X-axis) value of the very left side of the drawing area of the
	/// editor, with <see cref="Margins"/> added on top of it.
	/// </summary>
	/// <returns>The X-position of the property drawer.</returns>
	public float GetXPosition() {

		return (GetAbsoluteXPosition() + Margins.Left);
	}
	
	/// <summary>
	/// Returns the "absolute" starting Y-position of the editor, meaning the starting Y-position
	/// with no margin values etc. applied.<br/>
	/// The starting "Y-position" is the (Y-axis) value at the very top of the drawing area of the
	/// editor.
	/// </summary>
	/// <returns>The absolute starting Y-position of the editor.</returns>
	public float GetAbsoluteStartingYPosition() {

		return (startingRect.y);
	}
	
	/// <summary>
	/// Returns the starting Y-position of the editor.<br/>
	/// The starting "Y-position" is the (Y-axis) value at the very top of the drawing area of the
	/// editor, with <see cref="Margins"/> added on top of it.
	/// </summary>
	/// <returns>The starting Y-position of the editor.</returns>
	public float GetStartingYPosition() {

		return (GetAbsoluteStartingYPosition() + Margins.Top);
	}

	/// <summary>
	/// Returns the "absolute" current Y-position of the editor, meaning the current Y-position with
	/// no padding values etc. applied.<br/>
	/// The current "Y-position" is the (Y-axis) value right below the last "row" of components
	/// drawn in the editor so far.
	/// </summary>
	/// <returns>The current Y-position of the editor.</returns>
	public float GetAbsoluteCurrentYPosition() {

		return (GetStartingYPosition() + GetAbsoluteTotalHeight());
	}
	
	/// <summary>
	/// Returns the current Y-position of the editor.<br/>
	/// The current "Y-position" is the (Y-axis) value right below the last "row" of components
	/// drawn in the editor so far, with <see cref="PaddingRows"/> added in between.
	/// </summary>
	/// <returns>The current Y-position of the editor.</returns>
	public float GetCurrentYPosition() {

		return (GetStartingYPosition() + GetAbsoluteTotalHeight() + PaddingRows);
	}

	/// <summary>
	/// Returns the "absolute" (current) total height of the editor, meaning the total height with
	/// no margin values etc. applied.<br/>
	/// The total height is the sum of all height values that were added through
	/// <see cref="AddHeightFromValue"/> (and the other methods related to it) so far.
	/// </summary>
	/// <returns>The absolute (current) total height of the editor.</returns>
	public float GetAbsoluteTotalHeight() {

		return (totalHeight);
	}
	
	/// <summary>
	/// Returns the (current) total height of the editor.<br/>
	/// The total height is the sum of all height values that were added through
	/// <see cref="AddHeightFromValue"/> (and the other methods related to it) so far.
	/// </summary>
	/// <returns>The (current) total height of the editor.</returns>
	public float GetTotalHeight() {

		return (GetAbsoluteTotalHeight() + (Margins.Top + Margins.Bottom));
	}
	
	/// <summary>
	/// Returns a rectangle encompassing the entire (drawing area of the) editor.
	/// <br/>
	/// "Entire" here means the full width and the height up to <see cref="GetTotalHeight"/>.
	/// </summary>
	/// <returns>A rectangle encompassing the entire editor.</returns>
	public Rect GetTotalContainingRect() {

		return (new Rect(GetXPosition(), GetStartingYPosition(), GetWidth(), GetTotalHeight()));
	}

	/// <summary>
	/// Returns a rectangle absolutely encompassing the entire (drawing area of the) editor, meaning
	/// the editor with no margin values etc. applied.
	/// <br/>
	/// "Entire" here means the full width and the height up to <see cref="GetAbsoluteTotalHeight"/>.
	/// </summary>
	/// <returns>A rectangle absolutely encompassing the entire editor.</returns>
	public Rect GetAbsoluteTotalContainingRect() {

		return (new Rect(GetAbsoluteXPosition(), GetAbsoluteStartingYPosition(),
						 GetAbsoluteWidth(), GetAbsoluteTotalHeight()));
	}
}

}

