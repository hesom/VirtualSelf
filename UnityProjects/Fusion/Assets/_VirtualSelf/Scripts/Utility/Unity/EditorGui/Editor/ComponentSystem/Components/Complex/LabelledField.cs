using System;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {


/// <summary>
/// A (complex) component consisting of a <see cref="VirtualSelf.Utility.Editor.Field"/> (of any
/// kind), and a corresponding label that is drawn next to it.<br/>
/// This component has some styling values for where to place the label in relation to the field
/// component.
/// <typeparam name="T">The type of the field component.</typeparam>
/// </summary>
public sealed class LabelledField<T> : DynamicComplexComponent where T : Field {
	
	/* ---------- Variables & Properties ---------- */
	
	/// <inheritdoc/>
	public override float MinimumWidth { get; protected set; }
	
	/// <summary>
	/// The label of this labelled field component. The label is shown next to the field component,
	/// depending on the values of <see cref="LabelPos"/>,
	/// <see cref="LabelHorizAlign"/>, and <see cref="LabelVertAlign"/>.
	/// </summary>
	public Label Label { get; }
	
	/// <summary>
	/// The actual field of this labelled field component. 
	/// </summary>
	public T Field { get; }
	
	/// <summary>
	/// The position of the label in relation to the field.
	/// </summary>
	public Position LabelPos { get; }
	
	/// <summary>
	/// The position of the label in relation to the field, if the label is placed at the top or
	/// bottom of it (the value of <see cref="LabelPos"/> is <see cref="Position.Top"/> or
	/// <see cref="Position.Bottom"/>). This value is ignored if it is not.
	/// </summary>
	public HorizontalAlignment LabelHorizAlign { get; }
	
	/// <summary>
	/// The position of the label in relation to the field, if the label is placed at the left or
	/// right of it (the value of <see cref="LabelPos"/> is <see cref="Position.Left"/> or
	/// <see cref="Position.Right"/>). This value is ignored if it is not.
	/// </summary>
	public VerticalAlignment LabelVertAlign { get; }
	
	/// <summary>
	/// The padding (empty space) between the label and the field component.<br/>
	/// The default value for this is <c>1.0</c> if the label is placed at the top or bottom of the
	/// field, and <c>3.0</c> if it is placed at the left or right side of it.
	/// </summary>
	public float Padding { get; }

	/// <summary>
	/// The "local" position of the <see cref="Label"/> component. For more information, see the
	/// <see cref="LayoutComponents"/> documentation.
	/// </summary>
	private Vector2 localPosLabel;
	
	/// <summary>
	/// The "local" position of the <see cref="Field"/> component. For more information, see the
	/// <see cref="LayoutComponents"/> documentation.
	/// </summary>
	private Vector2 localPosField;
	
	
	/* ---------- Constructors ---------- */

	/// <summary>
	/// Creates a new <see cref="LabelledField{T}"/> with the width <paramref name="width"/>, and
	/// consisting of the given label and field components, and the padding in between those set to
	/// <paramref name="padding"/>.
	/// </summary>
	/// <param name="width">
	/// The width that this labelled field component will have.
	/// </param>
	/// <param name="label">The label component of this labelled field component.</param>
	/// <param name="field">The field component of this labelled field component.</param>
	/// <param name="padding">The padding between the label and the field components.</param>
	/// <param name="labelPos">
	/// The position of the label component relative to the field component.
	/// </param>
	/// <param name="labelHorizAlign">
	/// The horizontal alignment of the label component relative to the field component.
	/// </param>
	/// <param name="labelVertAlign">
	/// The horizontal alignment of the label component relative to the field component.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive (>0).
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public LabelledField(
			float width,
			Label label, T field,
			float padding,
			Position labelPos = Position.Top,
			HorizontalAlignment labelHorizAlign = HorizontalAlignment.Left,
			VerticalAlignment labelVertAlign = VerticalAlignment.Center) {

		ComponentUtils.AssertGreaterThanZero(width, "width");
		
		Width = width;
		
		Label = label;
		Field = field;

		Padding = padding;
		LabelPos = labelPos;
		LabelHorizAlign = labelHorizAlign;
		LabelVertAlign = labelVertAlign;
	
		LayoutComponents();
		CalculateHeight();
	}
	
	/// <summary>
	/// Creates a new <see cref="LabelledField{T}"/> with the width <paramref name="width"/>, and
	/// consisting of the given label and field components, and the padding in between them set to
	/// its default value.
	/// </summary>
	/// <param name="width">
	/// The width that this labelled field component will have.
	/// </param>
	/// <param name="label">The label component of this labelled field component.</param>
	/// <param name="field">The field component of this labelled field component.</param>
	/// <param name="labelPos">
	/// The position of the label component relative to the field component.
	/// </param>
	/// <param name="labelHorizAlign">
	/// The horizontal alignment of the label component relative to the field component.
	/// </param>
	/// <param name="labelVertAlign">
	/// The horizontal alignment of the label component relative to the field component.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive (>0).
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public LabelledField(
			float width,
			Label label, T field,
			Position labelPos = Position.Top,
			HorizontalAlignment labelHorizAlign = HorizontalAlignment.Left,
			VerticalAlignment labelVertAlign = VerticalAlignment.Center) {

		if (width.CompareTo(0.0f) <= 0) {

			throw new ArgumentException(
				"Invalid component width (" + width + "). The width of a component must " + 
				"be positive.");
		}
		
		Width = width;
		
		Label = label;
		Field = field;

		if (labelPos == Position.Left || labelPos == Position.Right) { Padding = 3.0f; }
		else { Padding = 1.0f; }
		
		LabelPos = labelPos;
		LabelHorizAlign = labelHorizAlign;
		LabelVertAlign = labelVertAlign;
		
		LayoutComponents();
		CalculateHeight();
	}
	
	
	/* ---------- Overrides ---------- */
	
	/// <inheritdoc/>
	public override void Draw(float positionX, float positionY) {
		
		Label.Draw((positionX + localPosLabel.x), (positionY + localPosLabel.y));
		Field.Draw((positionX + localPosField.x), (positionY + localPosField.y));
	}

	/// <inheritdoc/>
	protected override void LayoutComponents() {
		
		if ((LabelPos == Position.Left) || (LabelPos == Position.Right)) {
			
			float fieldWidth = (Width - Label.Width - Padding);
			Field.SetNewWidth(fieldWidth);

			MinimumWidth = (Label.Width + Padding + Field.MinimumWidth);
			ComponentUtils.AssertMinimumWidth(Width, MinimumWidth);
		}
		else if (LabelPos == Position.Top || LabelPos == Position.Bottom) {
			
			Field.SetNewWidth(Width);

			MinimumWidth = Math.Max(Field.MinimumWidth, Label.Width);
			ComponentUtils.AssertMinimumWidth(Width, MinimumWidth);
		}

		LayoutUtils.LayoutLabelAndComponent(
				Label, Field,
				Padding,
				LabelPos,
				LabelHorizAlign, LabelVertAlign,
				out localPosLabel, out localPosField);
	}

	/// <inheritdoc/>
	protected override void CalculateHeight() {
		
		if ((LabelPos == Position.Left) || (LabelPos == Position.Right)) {

			Height = Math.Max(Label.Height, Field.Height);
		}
		else {

			Height = (Label.Height + Padding + Field.Height);
		}
	}
}

}
