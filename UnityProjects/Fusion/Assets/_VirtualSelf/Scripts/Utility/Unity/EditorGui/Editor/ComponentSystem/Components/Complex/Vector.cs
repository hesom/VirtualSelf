using System;
using UnityEditor;
using UnityEngine;
using VirtualSelf.Utility.Editor.Internal;


namespace VirtualSelf.Utility.Editor {

	
/// <summary>
/// A (complex) component corresponding to (but not actually using, see below)
/// <see cref="EditorGUI"/>.<see cref="EditorGUI.Vector3Field(Rect,string,Vector3)"/>, and all
/// similar methods for other amounts of dimensions and other number types that Unity offers (as
/// well as all the corresponding overloads).<br/>
/// A vector component consists of 2, 3 or 4 <see cref="NumberField"/> field components, each
/// standing for one component (dimension) of the vector, and accompanying labels for each of them,
/// as well as an additional label that names the entire vector. All the fields together form a
/// single <see cref="UnityEngine.Vector3"/> (etc.) instance.<br/>
/// This class has two enumerations that, for each instance, specify the amount of dimensions it has
/// and what type of numerical data it holds.<br/>
/// This class does not actually use the aforementioned Unity methods for drawing, since they are
/// too limited. Instead, it is made up of multiple <see cref="LabelledField{T}"/> and label
/// components. Each instance of this class is created with an accompanying instance of
/// <see cref="VectorLayout"/>, which determines how exactly it is layouted (and thus, drawn).
/// 
/// </summary>
public sealed class Vector : DynamicComplexComponent {

	/* ---------- Enumerations ---------- */

	/// <summary>
	/// The different numeric data types a vector component can hold. This is limited by the data
	/// types that the underlying <see cref="Vector3"/>, etc., classes can hold.<br/>
	/// A vector component can only have one of these data types at the same time.
	/// </summary>
	public enum DataType {

		/// <summary>
		/// The vector component holds (single-precision) floating-point numbers.<br/>
		/// The corresponding Unity class is <see cref="Vector2"/>, <see cref="Vector3"/> or
		/// <see cref="Vector4"/>.
		/// </summary>
		Float,
		
		/// <summary>
		/// The vector component holds integer numbers.<br/>
		/// The corresponding Unity class is <see cref="Vector2Int"/> or <see cref="Vector3Int"/>.
		/// </summary>
		Integer
	}

	/// <summary>
	/// The different amounts of dimensions a vector component can have. This is limited by the
	/// existing <see cref="Vector3"/>, etc., classes that Unity has.<br/>
	/// The amount of dimensions corresponds to the amount of fields that will be drawn for the
	/// vector component.
	/// </summary>
	public enum DimensionsCount {

		/// <summary>
		/// The vector component has two dimensions.<br/>
		/// The corresponding Unity class is <see cref="Vector2"/> or <see cref="Vector2Int"/>.
		/// </summary>
		Two,
		
		/// <summary>
		/// The vector component has three dimensions.<br/>
		/// The corresponding Unity class is <see cref="Vector3"/> or <see cref="Vector3Int"/>.
		/// </summary>
		Three,
		
		/// <summary>
		/// The vector component has four dimensions.<br/>
		/// The corresponding Unity class is <see cref="Vector4"/>.
		/// </summary>
		Four
	}
	
	
	/* ---------- Variables & Properties ---------- */

	/// <summary>
	/// The default name for <see cref="FieldOne"/>, that it will be labelled with.
	/// </summary>
	public const string FieldOneNameDefault = "X";
	
	/// <summary>
	/// The default name for <see cref="FieldTwo"/>, that it will be labelled with.
	/// </summary>
	public const string FieldTwoNameDefault = "Y";
	
	/// <summary>
	/// The default name for <see cref="FieldThree"/>, that it will be labelled with.
	/// </summary>
	public const string FieldThreeNameDefault = "Z";
	
	/// <summary>
	/// The default name for <see cref="FieldFour"/>, that it will be labelled with.
	/// </summary>
	public const string FieldFourNameDefault = "W";
	
	/// <inheritdoc/>
	public override float MinimumWidth { get; protected set; }

	/// <summary>
	/// The label for this entire vector component - this corresponds to the name of the vector
	/// component. Its position and alignments are controlled by <see cref="Layout"/>.
	/// </summary>
	public Label Label { get; }

	/// <summary>
	/// The first field of the vector component, corresponding to its first (X) dimension.<br/>
	/// This field does exist for all possible amounts of dimension a vector component can have.
	/// </summary>
	public LabelledField<NumberField> FieldOne { 
			get { return (vectorComp.FieldOne); }
			private set { vectorComp.FieldOne = value; }
	}
	
	/// <summary>
	/// The second field of the vector component, corresponding to its second (Y) dimension.<br/>
	/// This field does exist for all possible amounts of dimension a vector component can have.
	/// </summary>
	public LabelledField<NumberField> FieldTwo { 
			get { return (vectorComp.FieldTwo); }
			private set { vectorComp.FieldTwo = value; }
	}
	
	/// <summary>
	/// The third field of the vector component, corresponding to its third (Z) dimension.<br/>
	/// This field does only exist if <see cref="DimsCount"/> is <see cref="DimensionsCount.Three"/>
	/// or <see cref="DimensionsCount.Four"/>; otherwise it is <c>null</c>.
	/// </summary>
	public LabelledField<NumberField> FieldThree { 
			get { return (vectorComp.FieldThree); }
			private set { vectorComp.FieldThree = value; }
	}
	
	/// <summary>
	/// The fourth field of the vector component, corresponding to its fourth (W) dimension.<br/>
	/// This field does only exist if <see cref="DimsCount"/> is <see cref="DimensionsCount.Four"/>.
	/// </summary>
	public LabelledField<NumberField> FieldFour { 
			get { return (vectorComp.FieldFour); }
			private set { vectorComp.FieldFour = value; }
	}
	
	/// <summary>
	/// The data type of the vector component. Depending on the value of this, it stores a different
	/// type of numeric data.
	/// </summary>
	public DataType DatType { 
			get { return (vectorComp.DatType); }
			private set { vectorComp.DatType = value; }
	}
	
	/// <summary>
	/// The amount of dimensions this vector component has. This determines how many fields are
	/// drawn for it.
	/// </summary>
	public DimensionsCount DimsCount { 			
			get { return (vectorComp.DimsCount); }
			private set { vectorComp.DimsCount = value; } 
	}

	/// <summary>
	/// The layout of the vector component, that determines how it is layouted drawn.
	/// <remarks>
	/// Changing any values of this after the vector component has already been constructed will
	/// only change the layout of it when <see cref="LayoutComponents"/> is called - outside of
	/// this, it will have no effects.
	/// </remarks>
	/// </summary>
	public VectorLayout Layout {
			get { return (vectorComp.Layout); }
			private set { vectorComp.Layout = value; } 
	}

	/// <summary>
	/// The internal vector component this component is made up of. This contains most part of the
	/// component, except for its label.
	/// </summary>
	private readonly VectorComponent vectorComp;
	
	/// <summary>
	/// The "local" position of the <see cref="Label"/> component. For more information, see the
	/// <see cref="LayoutComponents"/> documentation.
	/// </summary>
	private Vector2 localPosLabel;

	/// <summary>
	/// The "local" position of the <see cref="vectorComp"/> component. For more information, see
	/// the <see cref="LayoutComponents"/> documentation.
	/// </summary>
	private Vector2 localPosVectorComp;

	
	/* ---------- Static Factory Methods ---------- */

	/// <summary>
	/// Creates a <see cref="Vector"/> holding floating-point numbers and with two dimensions.<br/>
	/// The vector component has the given with and label, and will be layouted and drawn according
	/// to the <paramref name="layout"/>. Its two fields have the given names.
	/// </summary>
	/// <param name="width">
	/// The width that the vector component will have. Must be positive (>0).
	/// </param>
	/// <param name="label">
	/// The label that the vector component will have, this serves as a "name" for the component.
	/// </param>
	/// <param name="layout">
	/// The vector layout to layout the vector component after.<br/>
	/// Not that after the vector component has been constructed, changing any properties of this
	/// layout will not impact the vector component anymore.
	/// </param>
	/// <param name="fieldOneName">
	/// The name that the first field of the vector component (<see cref="FieldOne"/> should have.
	/// </param>
	/// <param name="fieldTwoName">
	/// The name that the second field of the vector component (<see cref="FieldTwo"/> should have.
	/// </param>
	/// <returns>
	/// The newly constructed vector component.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive (>0).
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public static Vector OfFloatIn2D(
			float width,
			Label label,
			VectorLayout layout,
			string fieldOneName = FieldOneNameDefault,
			string fieldTwoName = FieldTwoNameDefault) {

		return (new Vector(
					width, label,
					DataType.Float, DimensionsCount.Two,
					layout,
					fieldOneName, fieldTwoName, "", ""));
	}
	
	/// <summary>
	/// Creates a <see cref="Vector"/> holding integer numbers and with two dimensions.<br/>
	/// The vector component has the given with and label, and will be layouted and drawn according
	/// to the <paramref name="layout"/>. Its two fields have the given names.
	/// </summary>
	/// <param name="width">
	/// The width that the vector component will have. Must be positive (>0).
	/// </param>
	/// <param name="label">
	/// The label that the vector component will have, this serves as a "name" for the component.
	/// </param>
	/// <param name="layout">
	/// The vector layout to layout the vector component after.<br/>
	/// Not that after the vector component has been constructed, changing any properties of this
	/// layout will not impact the vector component anymore.
	/// </param>
	/// <param name="fieldOneName">
	/// The name that the first field of the vector component (<see cref="FieldOne"/> should have.
	/// </param>
	/// <param name="fieldTwoName">
	/// The name that the second field of the vector component (<see cref="FieldTwo"/> should have.
	/// </param>
	/// <returns>
	/// The newly constructed vector component.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive (>0).
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public static Vector OfIntIn2D(
			float width,
			Label label,
			VectorLayout layout,
			string fieldOneName = FieldOneNameDefault,
			string fieldTwoName = FieldTwoNameDefault) {

		return (new Vector(
					width, label,
					DataType.Integer, DimensionsCount.Two,
					layout,
					fieldOneName, fieldTwoName, "", ""));
	}
	
	/// <summary>
	/// Creates a <see cref="Vector"/> holding floating-point numbers and with three dimensions.
	/// <br/>
	/// The vector component has the given with and label, and will be layouted and drawn according
	/// to the <paramref name="layout"/>. Its two fields have the given names.
	/// </summary>
	/// <param name="width">
	/// The width that the vector component will have. Must be positive (>0).
	/// </param>
	/// <param name="label">
	/// The label that the vector component will have, this serves as a "name" for the component.
	/// </param>
	/// <param name="layout">
	/// The vector layout to layout the vector component after.<br/>
	/// Not that after the vector component has been constructed, changing any properties of this
	/// layout will not impact the vector component anymore.
	/// </param>
	/// <param name="fieldOneName">
	/// The name that the first field of the vector component (<see cref="FieldOne"/> should have.
	/// </param>
	/// <param name="fieldTwoName">
	/// The name that the second field of the vector component (<see cref="FieldTwo"/> should have.
	/// </param>
	/// <param name="fieldThreeName">
	/// The name that the third field of the vector component (<see cref="FieldThree"/> should have.
	/// </param>
	/// <returns>
	/// The newly constructed vector component.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive (>0).
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public static Vector OfFloatIn3D(
			float width,
			Label label,
			VectorLayout layout,
			string fieldOneName = FieldOneNameDefault,
			string fieldTwoName = FieldTwoNameDefault,
			string fieldThreeName = FieldThreeNameDefault) {

		return (new Vector(
					width, label,
					DataType.Float, DimensionsCount.Three,
					layout,
					fieldOneName, fieldTwoName, fieldThreeName, ""));
	}
	
	/// <summary>
	/// Creates a <see cref="Vector"/> holding integer numbers and with three dimensions.<br/>
	/// The vector component has the given with and label, and will be layouted and drawn according
	/// to the <paramref name="layout"/>. Its two fields have the given names.
	/// </summary>
	/// <param name="width">
	/// The width that the vector component will have. Must be positive (>0).
	/// </param>
	/// <param name="label">
	/// The label that the vector component will have, this serves as a "name" for the component.
	/// </param>
	/// <param name="layout">
	/// The vector layout to layout the vector component after.<br/>
	/// Not that after the vector component has been constructed, changing any properties of this
	/// layout will not impact the vector component anymore.
	/// </param>
	/// <param name="fieldOneName">
	/// The name that the first field of the vector component (<see cref="FieldOne"/> should have.
	/// </param>
	/// <param name="fieldTwoName">
	/// The name that the second field of the vector component (<see cref="FieldTwo"/> should have.
	/// </param>
	/// <param name="fieldThreeName">
	/// The name that the third field of the vector component (<see cref="FieldThree"/> should have.
	/// </param>
	/// <returns>
	/// The newly constructed vector component.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive (>0).
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public static Vector OfIntIn3D(
			float width,
			Label label,
			VectorLayout layout,
			string fieldOneName = FieldOneNameDefault,
			string fieldThreeName = FieldTwoNameDefault,
			string fieldTwoName = FieldTwoNameDefault) {

		return (new Vector(
					width, label,
					DataType.Integer, DimensionsCount.Three,
					layout,
					fieldOneName, fieldTwoName, fieldThreeName, ""));
	}
	
	/// <summary>
	/// Creates a <see cref="Vector"/> holding floating-point numbers and with four dimensions.
	/// <br/>
	/// The vector component has the given with and label, and will be layouted and drawn according
	/// to the <paramref name="layout"/>. Its two fields have the given names.
	/// </summary>
	/// <param name="width">
	/// The width that the vector component will have. Must be positive (>0).
	/// </param>
	/// <param name="label">
	/// The label that the vector component will have, this serves as a "name" for the component.
	/// </param>
	/// <param name="layout">
	/// The vector layout to layout the vector component after.<br/>
	/// Not that after the vector component has been constructed, changing any properties of this
	/// layout will not impact the vector component anymore.
	/// </param>
	/// <param name="fieldOneName">
	/// The name that the first field of the vector component (<see cref="FieldOne"/> should have.
	/// </param>
	/// <param name="fieldTwoName">
	/// The name that the second field of the vector component (<see cref="FieldTwo"/> should have.
	/// </param>
	/// <param name="fieldThreeName">
	/// The name that the third field of the vector component (<see cref="FieldThree"/> should have.
	/// </param>
	/// <param name="fieldFourName">
	/// The name that the fourth field of the vector component (<see cref="FieldFour"/> should have.
	/// </param>
	/// <returns>
	/// The newly constructed vector component.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive (>0).
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public static Vector OfFloatIn4D(
			float width,
			Label label,
			VectorLayout layout,
			string fieldOneName = FieldOneNameDefault,
			string fieldTwoName = FieldTwoNameDefault,
			string fieldThreeName = FieldThreeNameDefault,
			string fieldFourName = FieldFourNameDefault) {

		return (new Vector(
					width, label,
					DataType.Float, DimensionsCount.Four,
					layout,
					fieldOneName, fieldTwoName, fieldThreeName, fieldFourName));
	}
	
	
	/* ---------- Constructors ---------- */
	
	/// <summary>
	/// See the documentation of <see cref="OfFloatIn4D"/> and the other similar methods. All of
	/// these call this constructor internally.
	/// </summary>
	private Vector(
			float width, Label label,
			DataType dataType, DimensionsCount dimsCount,
			VectorLayout layout,
			string fieldOneName, string fieldTwoName,string fieldThreeName,string fieldFourName) {
		
		ComponentUtils.AssertGreaterThanZero(width, "width");
		
		if ((dataType == DataType.Integer) && (dimsCount == DimensionsCount.Four)) {

			throw new ArgumentException(
				"A vector with four dimensions and of data type \"Integer\" does not exist in " + 
				"Unity, and is therefore not supported here.");
		}
		
		Width = width;
		Label = label;

		vectorComp = new VectorComponent(
				width,
				dataType, dimsCount,
				layout,
				fieldOneName, fieldTwoName, fieldThreeName, fieldFourName);
		
		LayoutComponents();
		CalculateHeight();	
	}
	
	
	/* ---------- Getters & Setters ---------- */

	/// <summary>
	/// Returns the value held by this <see cref="Vector"/>, if <see cref="DatType"/> is
	/// <see cref="DataType.Float"/> and <see cref="DimsCount"/> is
	/// <see cref="DimensionsCount.Two"/>.
	/// </summary>
	/// <returns>
	/// The value held by this vector component.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this vector component does not hold floating-point values, or does not have two
	/// dimensions.
	/// </exception>
	public Vector2 GetValueFloat2D() {

		if ((DatType == DataType.Float) && (DimsCount == DimensionsCount.Two)) {

			return (vectorComp.GetValueFloatTwo());
		}

		throw new InvalidOperationException(
			"This vector component does not have the data type \"Float\", or has a different " + 
			"amount of dimensions than \"2\", so it does not contain such a value to return.");
	}
	
	/// <summary>
	/// Returns the value held by this <see cref="Vector"/>, if <see cref="DatType"/> is
	/// <see cref="DataType.Integer"/> and <see cref="DimsCount"/> is
	/// <see cref="DimensionsCount.Two"/>.
	/// </summary>
	/// <returns>
	/// The value held by this vector component.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this vector component does not hold integer values, or does not have two dimensions.
	/// </exception>
	public Vector2 GetValueInt2D() {

		if ((DatType == DataType.Integer) && (DimsCount == DimensionsCount.Two)) {

			return (vectorComp.GetValueIntTwo());
		}

		throw new InvalidOperationException(
			"This vector component does not have the data type \"Integer\", or has a different " + 
			"amount of dimensions than \"2\", so it does not contain such a value to return.");
	}
	
	/// <summary>
	/// Returns the value held by this <see cref="Vector"/>, if <see cref="DatType"/> is
	/// <see cref="DataType.Float"/> and <see cref="DimsCount"/> is
	/// <see cref="DimensionsCount.Three"/>.
	/// </summary>
	/// <returns>
	/// The value held by this vector component.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this vector component does not hold floating-point values, or does not have three
	/// dimensions.
	/// </exception>
	public Vector3 GetValueFloat3D() {

		if ((DatType == DataType.Float) && (DimsCount == DimensionsCount.Three)) {

			return (vectorComp.GetValueFloatThree());
		}

		throw new InvalidOperationException(
			"This vector component does not have the data type \"Float\", or has a different " + 
			"amount of dimensions than \"3\", so it does not contain such a value to return.");
	}
	
	/// <summary>
	/// Returns the value held by this <see cref="Vector"/>, if <see cref="DatType"/> is
	/// <see cref="DataType.Integer"/> and <see cref="DimsCount"/> is
	/// <see cref="DimensionsCount.Three"/>.
	/// </summary>
	/// <returns>
	/// The value held by this vector component.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this vector component does not hold integer values, or does not have three dimensions.
	/// </exception>
	public Vector3 GetValueInt3D() {

		if ((DatType == DataType.Integer) && (DimsCount == DimensionsCount.Three)) {

			return (vectorComp.GetValueIntThree());
		}

		throw new InvalidOperationException(
			"This vector component does not have the data type \"Integer\", or has a different " + 
			"amount of dimensions than \"3\", so it does not contain such a value to return.");
	}
	
	/// <summary>
	/// Returns the value held by this <see cref="Vector"/>, if <see cref="DatType"/> is
	/// <see cref="DataType.Float"/> and <see cref="DimsCount"/> is
	/// <see cref="DimensionsCount.Four"/>.
	/// </summary>
	/// <returns>
	/// The value held by this vector component.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this vector component does not hold floating-point values, or does not have four
	/// dimensions.
	/// </exception>
	public Vector2 GetValueFloat4D() {

		if ((DatType == DataType.Float) && (DimsCount == DimensionsCount.Four)) {

			return (vectorComp.GetValueFloatFour());
		}

		throw new InvalidOperationException(
			"This vector component does not have the data type \"Float\", or has a different " + 
			"amount of dimensions than \"4\", so it does not contain such a value to return.");
	}
	
	/// <summary>
	/// Sets the value held by this <see cref="Vector"/> to the given value,
	/// if <see cref="DatType"/> is <see cref="DataType.Float"/> and <see cref="DimsCount"/> is
	/// <see cref="DimensionsCount.Two"/>.
	/// </summary>
	/// <param name="valueFloat2D">
	/// The new value to set for the value of this vector component.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this vector component does not hold floating-point values, or does not have two
	/// dimensions.
	/// </exception>
	public void SetValueFloat2D(Vector2 valueFloat2D) {

		if ((DatType != DataType.Float) || (DimsCount != DimensionsCount.Two)) {
			
			throw new InvalidOperationException(
				"This vector component does not have the data type \"Float\", or has a " +
				"different amount of dimensions than \"2\", so it does not contain such a value " + 
				"to be set.");
		}

		vectorComp.SetValueFloatTwo(valueFloat2D);
	}
	
	/// <summary>
	/// Sets the value held by this <see cref="Vector"/> to the given value,
	/// if <see cref="DatType"/> is <see cref="DataType.Integer"/> and <see cref="DimsCount"/> is
	/// <see cref="DimensionsCount.Two"/>.
	/// </summary>
	/// <param name="valueInt2D">
	/// The new value to set for the value of this vector component.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this vector component does not hold integer values, or does not have two
	/// dimensions.
	/// </exception>
	public void SetValueInt2D(Vector2Int valueInt2D) {

		if ((DatType != DataType.Integer) || (DimsCount != DimensionsCount.Two)) {
			
			throw new InvalidOperationException(
				"This vector component does not have the data type \"Integer\", or has a " +
				"different amount of dimensions than \"2\", so it does not contain such a value " + 
				"to be set.");
		}

		vectorComp.SetValueIntTwo(valueInt2D);
	}
	
	/// <summary>
	/// Sets the value held by this <see cref="Vector"/> to the given value,
	/// if <see cref="DatType"/> is <see cref="DataType.Float"/> and <see cref="DimsCount"/> is
	/// <see cref="DimensionsCount.Three"/>.
	/// </summary>
	/// <param name="valueFloat3D">
	/// The new value to set for the value of this vector component.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this vector component does not hold floating-point values, or does not have three
	/// dimensions.
	/// </exception>
	public void SetValueFloat3D(Vector3 valueFloat3D) {

		if ((DatType != DataType.Float) || (DimsCount != DimensionsCount.Three)) {
			
			throw new InvalidOperationException(
				"This vector component does not have the data type \"Float\", or has a " +
				"different amount of dimensions than \"3\", so it does not contain such a value " + 
				"to be set.");
		}

		vectorComp.SetValueFloatThree(valueFloat3D);
	}
	
	/// <summary>
	/// Sets the value held by this <see cref="Vector"/> to the given value,
	/// if <see cref="DatType"/> is <see cref="DataType.Integer"/> and <see cref="DimsCount"/> is
	/// <see cref="DimensionsCount.Three"/>.
	/// </summary>
	/// <param name="valueInt3D">
	/// The new value to set for the value of this vector component.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this vector component does not hold floating-point values, or does not have three
	/// dimensions.
	/// </exception>
	public void SetValueInt3D(Vector3Int valueInt3D) {

		if ((DatType != DataType.Integer) || (DimsCount != DimensionsCount.Three)) {
			
			throw new InvalidOperationException(
				"This vector component does not have the data type \"Integer\", or has a " +
				"different amount of dimensions than \"3\", so it does not contain such a value " + 
				"to be set.");
		}

		vectorComp.SetValueIntThree(valueInt3D);
	}
	
	/// <summary>
	/// Sets the value held by this <see cref="Vector"/> to the given value,
	/// if <see cref="DatType"/> is <see cref="DataType.Float"/> and <see cref="DimsCount"/> is
	/// <see cref="DimensionsCount.Four"/>.
	/// </summary>
	/// <param name="valueFloat4D">
	/// The new value to set for the value of this vector component.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this vector component does not hold floating-point values, or does not have four
	/// dimensions.
	/// </exception>
	public void SetValueFloat4D(Vector4 valueFloat4D) {

		if ((DatType != DataType.Float) || (DimsCount != DimensionsCount.Four)) {
			
			throw new InvalidOperationException(
				"This vector component does not have the data type \"Float\", or has a " +
				"different amount of dimensions than \"4\", so it does not contain such a value " + 
				"to be set.");
		}

		vectorComp.SetValueFloatFour(valueFloat4D);
	}
	

	/* ---------- Overrides ---------- */

	/// <inheritdoc/>
	public override void Draw(float positionX, float positionY) {
		
		Label.Draw((positionX + localPosLabel.x), (positionY + localPosLabel.y));
		vectorComp.Draw((positionX + localPosVectorComp.x), (positionY + localPosVectorComp.y));
	}

	/// <inheritdoc/>
	protected override void LayoutComponents() {

		if ((Layout.LabelPos == Position.Left) || (Layout.LabelPos == Position.Right)) {

			float vecWidth = (Width - Label.Width - Layout.LabelPadding);
			vectorComp.SetNewWidth(vecWidth);

			MinimumWidth = (Label.Width + Layout.LabelPadding + vectorComp.MinimumWidth);
			ComponentUtils.AssertMinimumWidth(Width, MinimumWidth);
		}
		else if ((Layout.LabelPos == Position.Top) || (Layout.LabelPos == Position.Bottom)) {
			
			vectorComp.SetNewWidth(Width);

			MinimumWidth = Math.Max(vectorComp.MinimumWidth, Label.Width);
			ComponentUtils.AssertMinimumWidth(Width, MinimumWidth);
		}
		
		LayoutUtils.LayoutLabelAndComponent(
				Label, vectorComp,
				Layout.LabelPadding,
				Layout.LabelPos,
				Layout.LabelHorizAlign, Layout.LabelVertAlign,
				out localPosLabel, out localPosVectorComp);
	}

	/// <inheritdoc/>
	protected override void CalculateHeight() {
		
		if ((Layout.LabelPos == Position.Left) || (Layout.LabelPos == Position.Right)) {

			Height = Math.Max(Label.Height, vectorComp.Height);
		}
		else {

			Height = (Label.Height + Layout.LabelPadding + vectorComp.Height);
		}
	}
}

}