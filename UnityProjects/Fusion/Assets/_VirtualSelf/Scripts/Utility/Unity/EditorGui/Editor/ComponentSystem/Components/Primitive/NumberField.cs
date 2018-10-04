using System;
using UnityEditor;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {


/// <summary>
/// A <see cref="Component"/> corresponding to
/// <see cref="EditorGUI"/>.<see cref="EditorGUI.FloatField(UnityEngine.Rect,float)"/>,
/// <see cref="EditorGUI"/>.<see cref="EditorGUI.IntField(UnityEngine.Rect,int)"/>, and the fields
/// for all other numeric data types (as well as all related overloads).<br/>
/// A number field has a numeric (number) type, and accepts numbers inputed as numbers of the given
/// type. It can also be "delayed", which means that changes to its input are only "set" once ENTER
/// is pressed on the keyboard or keyboard focus moves away from the field.
/// </summary>
public sealed class NumberField : Field {

	/* ---------- Enumerations ---------- */

	/// <summary>
	/// The different "number types" a number field component can have. A number type, in this
	/// context, means what type of numbers (e.g. floats or integers) the field expects, displays,
	/// and stores.
	/// </summary>
	public enum NumberType {

		/// <summary>
		/// The field will work with integer numbers, and correspond to
		/// <see cref="EditorGUI.IntField(UnityEngine.Rect,int)"/> (and related overloads).
		/// </summary>
		Integer,
		/// <summary>
		/// The field will work with long integer numbers, and correspond to
		/// <see cref="EditorGUI.LongField(UnityEngine.Rect,long)"/> (and related overloads).
		/// </summary>
		Long,
		/// <summary>
		/// The field will work with single-precision floating point numbers, and correspond to
		/// <see cref="EditorGUI.FloatField(UnityEngine.Rect,float)"/> (and related overloads).
		/// </summary>
		Float,
		/// <summary>
		/// The field will work with double-precision floating point numbers, and correspond to
		/// <see cref="EditorGUI.DoubleField(UnityEngine.Rect,double)"/> (and related overloads).
		/// </summary>
		Double
	}
	
	
	/* ---------- Variables & Properties ---------- */

	/// <summary>
	/// The minimum reasonable width for a number field component.
	/// </summary>
	private const float MinimumWidthNumberFields = 30.0f;

	/// <inheritdoc/>
	public override float MinimumWidth { get; protected set; } = MinimumWidthNumberFields;
	
	/// <summary>
	/// The "number type" of this number field component.
	/// </summary>
	public NumberType NumType { get; }
	
	/// <summary>
	/// Specifies whether this number field component is a "delayed" field, or not. A delayed field
	/// will not change the field's underlying value (the number) until the user has pressed the
	/// "Enter" key, or moved keyboard focus away from the field, in the actual GUI.<br/>
	/// The default value for this is <c>false</c>.
	/// </summary>
	/// <remarks>
	/// If this is set to <c>false</c>, the underlying Unity calls for this class will be
	/// <see cref="EditorGUI.IntField(UnityEngine.Rect,int)"/>, etc.; if this is set to <c>true</c>,
	/// they will be <see cref="EditorGUI.DelayedIntField(UnityEngine.Rect,int)"/>, etc.<br/>
	/// If this is set to <c>true</c>, and <see cref="NumType"/> is set to
	/// <see cref="NumberType.Long"/>, the field will nonetheless be <c>not</c> a delayed one. This
	/// is because Unity does not have a "delayed" version of a number field for <c>long</c>.
	/// </remarks>
	public bool IsDelayedField { get; }

	/// <summary>
	/// The current value of this number field component, if <see cref="NumType"/> is set to
	/// <see cref="NumberType.Integer"/>. If it isn't, this value is undefined, and will not be used.
	/// </summary>
	private int intValue;
	
	/// <summary>
	/// The current value of this number field component, if <see cref="NumType"/> is set to
	/// <see cref="NumberType.Long"/>. If it isn't, this value is undefined, and will not be used.
	/// </summary>
	private long longValue;
	
	/// <summary>
	/// The current value of this number field component, if <see cref="NumType"/> is set to
	/// <see cref="NumberType.Float"/>. If it isn't, this value is undefined, and will not be used.
	/// </summary>
	private float floatValue;
	
	/// <summary>
	/// The current value of this number field component, if <see cref="NumType"/> is set to
	/// <see cref="NumberType.Double"/>. If it isn't, this value is undefined, and will not be used.
	/// </summary>
	private double doubleValue;
	
	
	/* ---------- Constructors ---------- */

	/// <summary>
	/// Creates a <see cref="NumberField"/> of the type <see cref="NumberType"/> and with the given
	/// width and GUI style, holding a number value of the given number type.
	/// </summary>
	/// <param name="width">The width that the number field component will have.</param>
	/// <param name="guiStyle">
	/// The GUI style that the number field component will use to draw itself.
	/// </param>
	/// <param name="numberType">
	/// The type of numbers the number field component will hold.
	/// </param>
	/// <param name="isDelayedField">
	/// Whether this number field component features delayed inputs, or not.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive (>0).
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public NumberField(float width, GUIStyle guiStyle,
					   NumberType numberType, bool isDelayedField = false) {
		
		ComponentUtils.AssertGreaterThanZero(width, "width");
		ComponentUtils.AssertMinimumWidth(width, MinimumWidth);
		
		Width = width;
		GuiStyle = guiStyle;
		
		NumType = numberType;
		IsDelayedField = isDelayedField;
		
		CalculateHeight();
	}

	/// <summary>
	/// Creates a <see cref="NumberField"/> of the type <see cref="NumberType"/> and with the given
	/// width, holding a number value of the given number type, and using the default GUI style for
	/// number fields.
	/// </summary>
	/// <param name="width">The width that the number field component will have.</param>
	/// <param name="numberType">
	/// The type of numbers the number field component will hold.
	/// </param>
	/// <param name="isDelayedField">
	/// Whether this number field component features delayed inputs, or not.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive (>0).
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public NumberField(float width, NumberType numberType, bool isDelayedField = false) : 
					   this(width, EditorStyles.numberField, numberType, isDelayedField) { }
	
	
	/* ---------- Getters & Setters ---------- */

	/// <summary>
	/// Returns the int value that this number field component holds.
	/// </summary>
	/// <returns>
	/// The int value that this number field component holds.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this number field component does not hold int values (<see cref="NumType"/> does not have
	/// the value <see cref="NumberType.Integer"/>).
	/// </exception>
	public int GetIntValue() {

		if (NumType == NumberType.Integer) {

			return (intValue);
		}
		
		throw new InvalidOperationException(
			"This number field component does not hold integer numbers, so it does not contain " + 
			"such a value to return.");
	}
	
	/// <summary>
	/// Returns the long value that this number field component holds.
	/// </summary>
	/// <returns>
	/// The long value that this number field component holds.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this number field component does not hold long values (<see cref="NumType"/> does not
	/// have the value <see cref="NumberType.Long"/>).
	/// </exception>
	public long GetLongValue() {

		if (NumType == NumberType.Long) {

			return (longValue);
		}
		
		throw new InvalidOperationException(
			"This number field component does not hold long integer numbers, so it does not " + 
			"contain such a value to return.");
	}
	
	/// <summary>
	/// Returns the float value that this number field component holds.
	/// </summary>
	/// <returns>
	/// The float value that this number field component holds.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this number field component does not hold float values (<see cref="NumType"/> does not
	/// have the value <see cref="NumberType.Float"/>).
	/// </exception>
	public float GetFloatValue() {

		if (NumType == NumberType.Float) {

			return (floatValue);
		}
		
		throw new InvalidOperationException(
			"This number field component does not hold single-precision floating point numbers, " + 
			"so it does not contain such a value to return.");
	}
	
	/// <summary>
	/// Returns the double value that this number field component holds.
	/// </summary>
	/// <returns>
	/// The double value that this number field component holds.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this number field component does not hold double values (<see cref="NumType"/> does not
	/// have the value <see cref="NumberType.Double"/>).
	/// </exception>
	public double GetDoubleValue() {

		if (NumType == NumberType.Double) {

			return (doubleValue);
		}
		
		throw new InvalidOperationException(
			"This number field component does not hold double-precision floating point numbers, " + 
			"so it does not contain such a value to return.");
	}
	
	/// <summary>
	/// Sets the int value that this number field component holds to the given value.
	/// </summary>
	/// <param name="intValue">
	/// The int value that this number field component should hold.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this number field component does not hold int values (<see cref="NumType"/> does not have
	/// the value <see cref="NumberType.Integer"/>).
	/// </exception>
	public void SetIntValue(int intValue) {

		if (NumType != NumberType.Integer) {
			
			throw new InvalidOperationException(
				"This number field component does not hold integer numbers, so it does not " + 
				"contain such a value to be set.");
		}

		this.intValue = intValue;
	}
	
	/// <summary>
	/// Sets the int value that this number field component holds to the given value.
	/// </summary>
	/// <param name="longValue">
	/// The long value that this number field component should hold.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this number field component does not hold long values (<see cref="NumType"/> does not
	/// have the value <see cref="NumberType.Long"/>).
	/// </exception>
	public void SetLongValue(long longValue) {

		if (NumType != NumberType.Long) {
			
			throw new InvalidOperationException(
				"This number field component does not hold long integer numbers, so it does not " +
				"contain such a value to be set.");
		}

		this.longValue = longValue;
	}
	
	/// <summary>
	/// Sets the float value that this number field component holds to the given value.
	/// </summary>
	/// <param name="floatValue">
	/// The float value that this number field component should hold.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this number field component does not hold float values (<see cref="NumType"/> does not
	/// have the value <see cref="NumberType.Float"/>).
	/// </exception>
	public void SetFloatValue(float floatValue) {

		if (NumType != NumberType.Float) {
			
			throw new InvalidOperationException(
				"This number field component does not hold single-precision floating point " + 
				"numbers, so it does not contain such a value to be set.");
		}

		this.floatValue = floatValue;
	}
	
	/// <summary>
	/// Sets the double value that this number field component holds to the given value.
	/// </summary>
	/// <param name="doubleValue">
	/// The double value that this number field component should hold.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this number field component does not hold double values (<see cref="NumType"/> does not
	/// have the value <see cref="NumberType.Double"/>).
	/// </exception>
	public void SetDoubleValue(double doubleValue) {

		if (NumType != NumberType.Double) {
			
			throw new InvalidOperationException(
				"This number field component does not hold double-precision floating point " + 
				"numbers, so it does not contain such a value to be set.");
		}

		this.doubleValue = doubleValue;
	}
	
	
	/* ---------- Overrides ---------- */

	/// <inheritdoc/>
	public override void Draw(float positionX, float positionY) {

		if (NumType == NumberType.Integer) {

			if (IsDelayedField == false) {
				
				intValue = EditorGUI.IntField(GetRect(positionX, positionY), intValue, GuiStyle);
			}
			else {
				
				intValue = EditorGUI.DelayedIntField(
								GetRect(positionX, positionY), intValue, GuiStyle);				
			}
		}
		else if (NumType == NumberType.Long) {
				
			longValue = EditorGUI.LongField(GetRect(positionX, positionY), longValue, GuiStyle);
		}
		else if (NumType == NumberType.Float) {

			if (IsDelayedField == false) {
				
				floatValue = EditorGUI.FloatField(
									GetRect(positionX, positionY), floatValue, GuiStyle);
			}
			else {
				
				floatValue = 
					EditorGUI.DelayedFloatField(
									GetRect(positionX, positionY), floatValue, GuiStyle);				
			}
		}
		else if (NumType == NumberType.Double) {

			if (IsDelayedField == false) {
				
				doubleValue = EditorGUI.DoubleField(
									GetRect(positionX, positionY), doubleValue, GuiStyle);
			}
			else {
				
				doubleValue = EditorGUI.DelayedDoubleField(
									GetRect(positionX, positionY), doubleValue, GuiStyle);				
			}
		}
		else {
			
			throw new NotImplementedException();
		}
	}

	/// <inheritdoc/>
	public override GUIStyle GetDefaultGuiStyle() {

		return (EditorStyles.numberField);
	}
}

}
