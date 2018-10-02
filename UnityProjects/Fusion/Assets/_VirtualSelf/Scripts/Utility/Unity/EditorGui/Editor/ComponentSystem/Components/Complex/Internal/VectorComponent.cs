using System;
using UnityEngine;


namespace VirtualSelf.Utility.Editor.Internal {
		

/// <summary>
/// A class made for internal usage by the <see cref="Vector"/> component. This class contains the
/// actual fields, their labels and their values of the vector component - the <see cref="Vector"/>
/// class only contains the label for the whole component, and provides outside access to all the
/// data.<br/>
/// This class is a full <see cref="ComplexComponent"/> in itself, but it is not intended to be used
/// on its own (that's why it is an internal class) - the <see cref="Vector"/> class encapsulates
/// it. The separation was mostly done to make the layouting code vastly simpler, and to follow the
/// pattern used elsewhere (e.g. for <see cref="Label"/> and <see cref="LabelledField{T}"/>).
/// </summary>
internal sealed class VectorComponent : DynamicComplexComponent {
	
	/* ---------- Variables & Properties ---------- */
	
	/// <inheritdoc/>
	public override float MinimumWidth { get; protected set; }

	/// <summary>
	/// See: <see cref="Vector.FieldOne"/>
	/// </summary>
	public LabelledField<NumberField> FieldOne { get; set; }
	
	/// <summary>
	/// See: <see cref="Vector.FieldTwo"/>
	/// </summary>
	public LabelledField<NumberField> FieldTwo { get; set; }
	
	/// <summary>
	/// See: <see cref="Vector.FieldThree"/>
	/// </summary>
	public LabelledField<NumberField> FieldThree { get; set; }
	
	/// <summary>
	/// See: <see cref="Vector.FieldFour"/>
	/// </summary>
	public LabelledField<NumberField> FieldFour { get; set; }
	
	/// <summary>
	/// See: <see cref="Vector.DatType"/>
	/// </summary>
	public Vector.DataType DatType { get; set; }
	
	/// <summary>
	/// See: <see cref="Vector.DimsCount"/>
	/// </summary>
	public Vector.DimensionsCount DimsCount { get; set; }
	
	/// <summary>
	/// See: <see cref="Vector.Layout"/>
	/// </summary>
	public VectorLayout Layout { get; set; }
	
	/// <summary>
	/// The value that this vector component holds, if <see cref="DatType"/> is
	/// <see cref="Vector.DataType.Float"/> and <see cref="DimsCount"/> is
	/// <see cref="Vector.DimensionsCount.Two"/>. Otherwise, this is <c>null</c>.
	/// </summary>
	private Vector2 valueFloatTwo;
	
	/// <summary>
	/// The value that this vector component holds, if <see cref="DatType"/> is
	/// <see cref="Vector.DataType.Integer"/> and <see cref="DimsCount"/> is
	/// <see cref="Vector.DimensionsCount.Two"/>. Otherwise, this is <c>null</c>.
	/// </summary>
	private Vector2Int valueIntTwo;
	
	/// <summary>
	/// The value that this vector component holds, if <see cref="DatType"/> is
	/// <see cref="Vector.DataType.Float"/> and <see cref="DimsCount"/> is
	/// <see cref="Vector.DimensionsCount.Three"/>. Otherwise, this is <c>null</c>.
	/// </summary>
	private Vector3 valueFloatThree;
	
	/// <summary>
	/// The value that this vector component holds, if <see cref="DatType"/> is
	/// <see cref="Vector.DataType.Integer"/> and <see cref="DimsCount"/> is
	/// <see cref="Vector.DimensionsCount.Three"/>. Otherwise, this is <c>null</c>.
	/// </summary>
	private Vector3Int valueIntThree;
	
	/// <summary>
	/// The value that this vector component holds, if <see cref="DatType"/> is
	/// <see cref="Vector.DataType.Float"/> and <see cref="DimsCount"/> is
	/// <see cref="Vector.DimensionsCount.Four"/>. Otherwise, this is <c>null</c>.
	/// </summary>
	private Vector4 valueFloatFour;
	
	/// <summary>
	/// The "local" position of the <see cref="FieldOne"/> component. For more information, see the
	/// <see cref="LayoutComponents"/> documentation.
	/// </summary>
	private Vector2 localPosFieldOne;
	
	/// <summary>
	/// The "local" position of the <see cref="FieldTwo"/> component. For more information, see the
	/// <see cref="LayoutComponents"/> documentation.
	/// </summary>
	private Vector2 localPosFieldTwo;
	
	/// <summary>
	/// The "local" position of the <see cref="FieldThree"/> component, if it exists. For more
	/// information, see the <see cref="LayoutComponents"/> documentation.
	/// </summary>
	private Vector2 localPosFieldThree;
	
	/// <summary>
	/// The "local" position of the <see cref="FieldFour"/> component, if it exists. For more
	/// information, see the <see cref="LayoutComponents"/> documentation.
	/// </summary>
	private Vector2 localPosFieldFour;
	
	
	/* ---------- Constructors ---------- */

	public VectorComponent(
			float width,
			Vector.DataType dataType,
			Vector.DimensionsCount dimensionsCount,
			VectorLayout layout,
			string fieldOneName, string fieldTwoName, string fieldThreeName, string fieldFourName) {
	
		ComponentUtils.AssertGreaterThanZero(width, "width");
		
		Width = width;

		DatType = dataType;
		DimsCount = dimensionsCount;

		Layout = layout;

		FieldOne = FieldTwo = FieldThree = FieldFour = null;
		
		NumberField.NumberType fieldsNumberType = 
				(DatType == Vector.DataType.Float)
				? NumberField.NumberType.Float
				: NumberField.NumberType.Integer;

		FieldOne = new LabelledField<NumberField>(
					width, new Label(fieldOneName),
					new NumberField(width, fieldsNumberType),
					Layout.FieldsLabelsPadding,
					Layout.FieldsLabelsPos,
					Layout.FieldsLabelsHorizAlign, Layout.FieldsLabelsVertAlign);
		
		FieldTwo = new LabelledField<NumberField>(
					width, new Label(fieldTwoName),
					new NumberField(width, fieldsNumberType),
					Layout.FieldsLabelsPadding,
					Layout.FieldsLabelsPos,
					Layout.FieldsLabelsHorizAlign, Layout.FieldsLabelsVertAlign);

		if ((DimsCount == Vector.DimensionsCount.Three) || 
		    (DimsCount == Vector.DimensionsCount.Four)) {
			
			FieldThree = new LabelledField<NumberField>(
						width, new Label(fieldThreeName),
						new NumberField(width, fieldsNumberType),
						Layout.FieldsLabelsPadding,
						Layout.FieldsLabelsPos,
						Layout.FieldsLabelsHorizAlign, Layout.FieldsLabelsVertAlign);
		}
		if (DimsCount == Vector.DimensionsCount.Four) {
			
			FieldFour = new LabelledField<NumberField>(
						width, new Label(fieldFourName),
						new NumberField(width, fieldsNumberType),
						Layout.FieldsLabelsPadding,
						Layout.FieldsLabelsPos,
						Layout.FieldsLabelsHorizAlign, Layout.FieldsLabelsVertAlign);
		}
		
		LayoutComponents();
		CalculateHeight();
	}
	
	/// <summary>
	/// Constructs a new <see cref="VectorComponent"/> instance. For more information, see
	/// <see cref="Vector.OfFloatIn3D"/>, and the other methods similar to that one.
	/// </summary>
	public VectorComponent(
			float width,
			Vector.DataType dataType,
			Vector.DimensionsCount dimensionsCount,
			string fieldOneName, string fieldTwoName, string fieldThreeName, string fieldFourName) : 
			this(width, dataType, dimensionsCount,
				 new VectorLayout(),
				 fieldOneName, fieldTwoName, fieldThreeName, fieldFourName) { }
	
	
	/* ---------- Getters & Setters ---------- */

	public Vector2 GetValueFloatTwo() { return (valueFloatTwo); }
	public Vector2Int GetValueIntTwo() { return (valueIntTwo); }
	public Vector3 GetValueFloatThree() { return (valueFloatThree); }
	public Vector3Int GetValueIntThree() { return (valueIntThree); }
	public Vector4 GetValueFloatFour() { return (valueFloatFour); }

	public void SetValueFloatTwo(Vector2 valueFloatTwo) {

		this.valueFloatTwo = valueFloatTwo;
		FieldOne.Field.SetFloatValue(valueFloatTwo.x);
		FieldTwo.Field.SetFloatValue(valueFloatTwo.y);
	}
	
	public void SetValueIntTwo(Vector2Int valueIntTwo) {

		this.valueIntTwo = valueIntTwo;
		FieldOne.Field.SetIntValue(valueIntTwo.x);
		FieldTwo.Field.SetIntValue(valueIntTwo.y);
	}
	
	public void SetValueFloatThree(Vector3 valueFloatThree) {

		this.valueFloatThree = valueFloatThree;
		FieldOne.Field.SetFloatValue(valueFloatThree.x);
		FieldTwo.Field.SetFloatValue(valueFloatThree.y);
		FieldThree.Field.SetFloatValue(valueFloatThree.z);
	}
	
	public void SetValueIntThree(Vector3Int valueIntThree) {

		this.valueIntThree = valueIntThree;
		FieldOne.Field.SetIntValue(valueIntThree.x);
		FieldTwo.Field.SetIntValue(valueIntThree.y);
		FieldThree.Field.SetIntValue(valueIntThree.z);
	}
	
	public void SetValueFloatFour(Vector4 valueFloatFour) {

		this.valueFloatFour = valueFloatFour;
		FieldOne.Field.SetFloatValue(valueFloatFour.x);
		FieldTwo.Field.SetFloatValue(valueFloatFour.y);
		FieldThree.Field.SetFloatValue(valueFloatFour.z);
		FieldFour.Field.SetFloatValue(valueFloatFour.w);
	}
	
	
	/* ---------- Overrides ---------- */

	/// <inheritdoc/>
	public override void Draw(float positionX, float positionY) {
		
		FieldOne.Draw((positionX + localPosFieldOne.x), (positionY + localPosFieldOne.y));
		FieldTwo.Draw((positionX + localPosFieldTwo.x), (positionY + localPosFieldTwo.y));

		if (FieldThree != null) {
			FieldThree.Draw((positionX + localPosFieldThree.x), (positionY + localPosFieldThree.y));
		}
		if (FieldFour != null) {
			FieldFour.Draw((positionX + localPosFieldFour.x), (positionY + localPosFieldFour.y));
		}
	}

	/// <inheritdoc/>
	protected override void LayoutComponents() {

		/* Calculate the widths of all fields. */
		
		if (Layout.FieldsPlace == VectorLayout.FieldsPlacement.Horizontal) {

			/* If the fields are all in one row, we first find out the required width we definitely
			 * need. This is the width of all the padding space between the fields, since that space
			 * cannot be "resized".
			 * Then the leftover width we have is divided by the amount of fields we have, and the
			 * fields are rescaled to that size. */
			
			float requiredWidth = Layout.FieldsPadding;
			if (FieldThree != null) { requiredWidth += Layout.FieldsPadding; }
			if (FieldFour != null) { requiredWidth += Layout.FieldsPadding; }

			float availableWidth = (Width - requiredWidth);

			float fieldsCount = 2.0f;
			if (FieldThree != null) { fieldsCount += 1.0f; }
			if (FieldFour != null) { fieldsCount += 1.0f; }

			float fieldWidth = (availableWidth / fieldsCount);
			
			FieldOne.SetNewWidth(fieldWidth);
			FieldTwo.SetNewWidth(fieldWidth);
			if (FieldThree != null) { FieldThree.SetNewWidth(fieldWidth); }
			if (FieldFour != null) { FieldFour.SetNewWidth(fieldWidth); }

			/* The minimum width is then the minimum width of all the fields combined, plus the
			 * required width stated before. */
			
			float minimumFieldsWidth = (FieldOne.MinimumWidth + FieldTwo.MinimumWidth);
			if (FieldThree != null) { minimumFieldsWidth += FieldThree.MinimumWidth; }
			if (FieldFour != null) { minimumFieldsWidth += FieldFour.MinimumWidth; }

			MinimumWidth = (requiredWidth + minimumFieldsWidth);
			ComponentUtils.AssertMinimumWidth(Width, MinimumWidth);
		}
		else if (Layout.FieldsPlace == VectorLayout.FieldsPlacement.Vertical) {
			
			/* If the fields are below each other, finding their width is easy: Each field occupies
			 * the full width given, as each is alone in a single row. */
			
			FieldOne.SetNewWidth(Width);
			FieldTwo.SetNewWidth(Width);
			if (FieldThree != null) { FieldThree.SetNewWidth(Width); }
			if (FieldFour != null) { FieldFour.SetNewWidth(Width); }

			float fieldOneMinimumWidth = FieldOne.MinimumWidth;
			float fieldTwoMinimumWidth = FieldTwo.MinimumWidth;
			float fieldThreeMinimumWidth = (FieldThree != null) ? FieldThree.MinimumWidth : 0.0f;
			float fieldFourMinimumWidth = (FieldFour != null) ? FieldFour.MinimumWidth : 0.0f;

			/* The minimum width for the entire component is just greatest the minimum width among
			 * all the fields. */
			
			MinimumWidth = (MathExtensions.Max(fieldOneMinimumWidth, fieldTwoMinimumWidth,
											   fieldThreeMinimumWidth, fieldFourMinimumWidth));
			ComponentUtils.AssertMinimumWidth(Width, MinimumWidth);
		}
		
		/* Place all the fields locally. */
		
		Vector2 fieldOnePos, fieldTwoPos, fieldThreePos, fieldFourPos;
		fieldOnePos = fieldTwoPos = fieldThreePos = fieldFourPos = Vector2.zero;

		if (Layout.FieldsPlace == VectorLayout.FieldsPlacement.Horizontal) {
		
			fieldOnePos.y = fieldTwoPos.y = fieldThreePos.y = fieldFourPos.y = 0.0f;

			fieldOnePos.x = 0.0f;
			fieldTwoPos.x = (fieldOnePos.x + FieldOne.Width + Layout.FieldsPadding);

			if (FieldThree != null) {
				fieldThreePos.x = (fieldTwoPos.x + FieldTwo.Width + Layout.FieldsPadding);
			}
			if (FieldFour != null) {
				fieldFourPos.x = (fieldThreePos.x + FieldThree.Width + Layout.FieldsPadding);
			}
		}
		else if (Layout.FieldsPlace == VectorLayout.FieldsPlacement.Vertical) {
			
			fieldOnePos.x = fieldTwoPos.x = fieldThreePos.x = fieldFourPos.x = 0.0f;

			fieldOnePos.y = 0.0f;
			fieldTwoPos.y = (fieldOnePos.y + FieldOne.Height + Layout.FieldsPadding);
			
			if (FieldThree != null) {
				fieldThreePos.y = (fieldTwoPos.y + FieldTwo.Height + Layout.FieldsPadding);
			}
			if (FieldFour != null) {
				fieldFourPos.y = (fieldThreePos.y + FieldThree.Height + Layout.FieldsPadding);
			}
		}

		localPosFieldOne = fieldOnePos;
		localPosFieldTwo = fieldTwoPos;
		localPosFieldThree = fieldThreePos;
		localPosFieldFour = fieldFourPos;
	}

	/// <inheritdoc/>
	protected override void CalculateHeight() {

		float fieldOneHeight = FieldOne.Height;
		float fieldTwoHeight = FieldTwo.Height;
		float fieldThreeHeight = (FieldThree != null) ? FieldThree.Height : 0.0f;
		float fieldFourHeight = (FieldFour != null) ? FieldFour.Height : 0.0f;
		
		if (Layout.FieldsPlace == VectorLayout.FieldsPlacement.Horizontal) {

			Height = MathExtensions.Max(fieldOneHeight, fieldTwoHeight,
										fieldThreeHeight, fieldFourHeight);
		}
		else if (Layout.FieldsPlace == VectorLayout.FieldsPlacement.Vertical) {

			float paddingHeight = Layout.FieldsPadding;
			if (FieldThree != null) { paddingHeight += Layout.FieldsPadding; }
			if (FieldFour != null) { paddingHeight += Layout.FieldsPadding; }

			Height = (paddingHeight + 
			          fieldOneHeight + fieldTwoHeight + 
			          fieldThreeHeight + fieldFourHeight);
		}
	}
}

}