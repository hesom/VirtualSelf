using UnityEngine;


namespace VirtualSelf.Utility.Editor {

	
/// <summary>
/// A component corresponding to the general idea of an abstract "field". A field is a GUI component
/// that generally has the height of roughly a single line of text (and a variable width), and that
/// the user can interact with in some way, usually by inputting something or clicking on it.<br/>
/// Common examples for fields are text fields (which include number fields), but there can also be
/// components like color fields that can be clicked, open a color selector, and then display the
/// selected color.<br/>
/// Note that this class only models a single field. GUI components like a field for a vector, which
/// actually consists of 2 or more fields, are subclasses of <see cref="ComplexComponent"/>.<br/>
/// Also note that this class does not feature a label - it is just a single field, and nothing
/// else. For versions that include a label, see the complex component
/// <see cref="LabelledField{T}"/>.
/// <br/>
/// As fields do not share that much with each other, despite their dimensions, this abstract class
/// does not contain much.
/// </summary>
public abstract class Field : DynamicPrimitiveComponent {

	/* ---------- Overrides ---------- */
	
	/// <summary>
	/// Calculating the height of a field is trivial. It is also not really dependent on the width
	/// of the field (but the calculation is so simple that it does not hurt when the height is
	/// re-calculated on each width change).<br/>
	/// Since a field holding a single line of text of any kind, or something similar, always has
	/// the same height, we only need to get this "base height".
	/// </summary>
	protected override void CalculateHeight() {
		
		Height = GuiStyle.CalcHeight(GUIContent.none, Width);
	}
}

}