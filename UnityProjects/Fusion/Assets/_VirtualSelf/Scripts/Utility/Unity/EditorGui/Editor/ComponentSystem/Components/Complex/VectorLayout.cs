namespace VirtualSelf.Utility.Editor {

   
/// <summary>
/// This class contains layouting information for the <see cref="Vector"/> component class. A
/// <see cref="Vector"/> can be layouted (and thus drawn) in a number of different ways, and an
/// instance of this class determines how exactly it will be.<br/>
/// This is a POD-type of class, which can be reused for creating multiple <see cref="Vector"/>
/// instances. It should be noted that after a vector component has been created with a specific
/// instance of this class, changing any members of this class will only change the vector
/// component's layout again when <see cref="Vector.LayoutComponents"/> is called.
/// </summary>
public sealed class VectorLayout {
	
	/* ---------- Enumerations ---------- */

	/// <summary>
	/// The different ways the (two, three or four) fields of the given vector component can be
	/// placed (arranged) when drawing it.
	/// </summary>
	public enum FieldsPlacement {

		/// <summary>
		/// The fields will all be arranged in a single row, next to one another.
		/// </summary>
		Horizontal,
		
		/// <summary>
		/// The fields will all be arranged in a single column, above/below one another.
		/// </summary>
		Vertical
	}
	
	
	/* ---------- Variables & Properties ---------- */

	/// <summary>
	/// The placement of the fields of the given vector component.
	/// </summary>
	public FieldsPlacement FieldsPlace { get; set; } = FieldsPlacement.Horizontal;

	/// <summary>
	/// The position of the label of the given vector component, relative to the fields.
	/// </summary>
	public Position LabelPos { get; set; } = Position.Left;

	/// <summary>
	/// The horizontal alignment of the label of the given vector component. This is ignored if
	/// <see cref="LabelPos"/> is <see cref="Position.Left"/> or <see cref="Position.Right"/>.
	/// </summary>
	public HorizontalAlignment LabelHorizAlign { get; set; } = HorizontalAlignment.Left;

	/// <summary>
	/// The vertical alignment of the label of the given vector component. This is ignored if
	/// <see cref="LabelPos"/> is <see cref="Position.Top"/> or <see cref="Position.Bottom"/>.
	/// </summary>
	public VerticalAlignment LabelVertAlign { get; set; } = VerticalAlignment.Center;

	/// <summary>
	/// The positions of the labels that the fields of the given vector component have, relative to
	/// each of them. All labels will have the same position.
	/// </summary>
	public Position FieldsLabelsPos { get; set; } = Position.Left;

	/// <summary>
	/// The horizontal alignments of the labels that the fields of the given vector component have.
	/// This is ignored if <see cref="FieldsLabelsPos"/> is <see cref="Position.Left"/> or
	/// <see cref="Position.Right"/>.
	/// </summary>
	public HorizontalAlignment FieldsLabelsHorizAlign { get; set; } = HorizontalAlignment.Left;

	/// <summary>
	/// The vertical alignments of the labels that the fields of the given vector component have.
	/// This is ignored if <see cref="FieldsLabelsPos"/> is <see cref="Position.Top"/> or
	/// <see cref="Position.Bottom"/>.
	/// </summary>
	public VerticalAlignment FieldsLabelsVertAlign { get; set; } = VerticalAlignment.Center;

	/// <summary>
	/// The padding (empty space) between the label of the given vector component, and its fields.
	/// </summary>
	public float LabelPadding { get; set; } = 5.0f;
	
	/// <summary>
	/// The padding (empty space) between the labels of the given vector component, and their
	/// corresponding fields.
	/// </summary>
	public float FieldsLabelsPadding { get; set; } = 3.0f;

	/// <summary>
	/// The padding (empty space) between the each of the fields of the given vector component
	/// (including their corresponding labels).
	/// </summary>
	public float FieldsPadding { get; set; } = 5.0f;
}

}