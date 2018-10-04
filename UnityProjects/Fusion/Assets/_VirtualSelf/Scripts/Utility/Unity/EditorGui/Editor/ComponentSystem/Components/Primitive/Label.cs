using UnityEngine;
using UnityEditor;


namespace VirtualSelf.Utility.Editor {


/// <summary>
/// A <see cref="Component"/> corresponding to
/// <see cref="EditorGUI"/>.<see cref="EditorGUI.LabelField(UnityEngine.Rect,string)"/> (and all
/// related overloads).<br/>
/// This is practically the most simple of all components - it is just a bit of text. It does not
/// manage any value, and does not have a state.
/// </summary>
public sealed class Label : AutomaticPrimitiveComponent {
	
	/* ---------- Enumerations ---------- */

	/// <summary>
	/// The different "base" styles a label component can have, as supported by Unity (note that
	/// additional styling is available through specifying different <see cref="GUIStyle"/>s when
	/// creating a label component).
	/// </summary>
	public enum LabelStyle {

		/// <summary>
		/// A normal-looking label with no specific (base) styling.
		/// </summary>
		Normal,
		/// <summary>
		/// A label that, in addition to all its other possible styling, also has a "drop shadow",
		/// giving it a more three-dimensional look. This effect is not really configurable, as
		/// Unity does not give possibilities for this.
		/// </summary>
		DropShadow
	}
	

	/* ---------- Variables & Properties ---------- */

	/// <summary>
	/// The (base) style that this label component will have when drawn.
	/// </summary>
	public LabelStyle LabelStyling { get; }
	
	/// <summary>
	/// The text of this label component. This is what is displayed when the component is drawn.
	/// </summary>
	public string Text { get; }
	

	/* ---------- Constructors ---------- */

	/// <summary>
	/// Creates a <see cref="Label"/> from a text and a GUI style, and with the specified (base)
	/// style.
	/// </summary>
	/// <param name="text">The text that this label component should have.</param>
	/// <param name="guiStyle">
	/// The GUI style that this label component will use to calculate its own dimensions.
	/// </param>
	/// <param name="labelStyle">
	/// The (base) style that this label component will have when drawn.
	/// </param>
	public Label(string text, GUIStyle guiStyle, LabelStyle labelStyle = LabelStyle.Normal) {

		Text = text;
		GuiStyle = guiStyle;
		LabelStyling = labelStyle;
        
		CalculateDimensions();
	}

	/// <summary>
	/// Creates a <see cref="Label"/> from a text, and with the specified (base) style. It will use
	/// the default GUI style of Unity for <c>LabelField</c> components.
	/// </summary>
	/// <param name="text">The text that this label component should have.</param>
	/// <param name="labelStyle">
	/// The (base) style that this label component will have when drawn.
	/// </param>
	public Label(string text, LabelStyle labelStyle = LabelStyle.Normal) : 
				 this(text, new GUIStyle(EditorStyles.label), labelStyle) { }
	

	/* ---------- Overrides ---------- */
	
	/// <inheritdoc/>
	public override void Draw(float positionX, float positionY) {
		
		EditorGUI.LabelField(GetRect(positionX, positionY), Text, GuiStyle);
	}

	/// <inheritdoc/>
	public override GUIStyle GetDefaultGuiStyle() {

		return (EditorStyles.label);
	}

	/// <summary>
	/// Since a label is literally just a bit of text that is drawn, its dimensions are as easy to
	/// calculate as it gets. The label will always just have the dimensions of its own text.
	/// </summary>
	protected override void CalculateDimensions() {
        
		float widthMin, widthMax;
		GUIContent guiContent = new GUIContent(Text);
		GuiStyle.CalcMinMaxWidth(guiContent, out widthMin, out widthMax);

		float height = GuiStyle.CalcHeight(guiContent, widthMax);

		Width = widthMax;
		Height = height;
	}
}

}