using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace VirtualSelf.Utility.Editor {

	
/// <summary>
/// A <see cref="Component"/> corresponding to
/// <see cref="EditorGUI.ObjectField(UnityEngine.Rect,UnityEditor.SerializedProperty)"/> (and all
/// related overloads).<br/>
/// An object field is a <see cref="Field"/> that can hold an (Unity) object or a
/// <see cref="SerializedObject"/> instance, which may then be referred to by code. The user can
/// drag an object from the Unity scene hierarchy into it, select it through a little GUI window,
/// or set one programmatically.
/// <remarks>
/// The type parameter <typeparamref name="T"/> sadly does not actually give this component
/// compile-time type safety, as the underlying Unity methods do not support this themselves. It is
/// only used for runtime checks.
/// </remarks>
/// </summary>
/// <typeparam name="T">
/// The type of the object that is being stored by this object field component.
/// </typeparam>
public sealed class ObjectField<T> : Field {
	
	/* ---------- Enumerations ---------- */

	/// <summary>
	/// The different types of objects that an object field component can (potentially) hold. Any
	/// given component will only ever hold exactly one of these types.
	/// </summary>
	public enum ObjectType {

		/// <summary>
		/// The object field component can hold any object of type, or inheriting from,
		/// <see cref="UnityEngine"/>.<see cref="Object"/>.
		/// </summary>
		UnityObject,
		/// <summary>
		/// The object field component can hold any object of type
		/// <see cref="UnityEditor"/>.<see cref="SerializedProperty"/>.
		/// </summary>
		SerializedProperty
	}
	
	
	/* ---------- Variables & Properties ---------- */

	/// <summary>
	/// The minimum reasonable width for an object field component.
	/// </summary>
	private const float MinimumWidthObjectFields = 50.0f;

	/// <inheritdoc/>
	public override float MinimumWidth { get; protected set; } = MinimumWidthObjectFields;

	/// <summary>
	/// The object type of this object field component. Only objects of this type can be stored and
	/// retrieved from it. The object type cannot be changed anymore after the creation of the
	/// component.
	/// </summary>
	public ObjectType ObjType { get; }
	
	/// <summary>
	/// If this object field component was created from a Unity Object, it is stored here (and
	/// <see cref="ObjType"/> is set to <see cref="ObjectType.UnityObject"/>.<br/>
	/// Otherwise, this is <c>null</c> and will not be used.
	/// </summary>
	private Object objectInstance;

	/// <summary>
	/// If this object field component was created from a Unity serialized property, it is stored
	/// here (and <see cref="ObjType"/> is set to <see cref="ObjectType.SerializedProperty"/>.<br/>
	/// Otherwise, this is <c>null</c> and will not be used.
	/// </summary>
	private SerializedProperty propertyInstance;
	
	
	/* ---------- Constructors ---------- */

	/// <summary>
	/// Creates an <see cref="ObjectField{T}"/> of type <typeparamref name="T"/> and with a width of
	/// <paramref name="width"/> and the GUI style <see cref="guiStyle"/>, holding the (Unity)
	/// object <paramref name="objectInstance"/>.
	/// </summary>
	/// <param name="width">
	/// The width that this object field component will have. Must be positive (>0).
	/// </param>
	/// <param name="guiStyle">
	/// The GUI style that this object field component will use to draw itself and calculate its
	/// height.
	/// </param>
	/// <param name="objectInstance">
	/// The object that this object field component holds. This can be <c>null</c>, meaning that
	/// no object is currently held.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive.
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public ObjectField(float width, GUIStyle guiStyle, Object objectInstance = null) {

		ComponentUtils.AssertGreaterThanZero(width, "width");
		ComponentUtils.AssertMinimumWidth(width, MinimumWidth);

		Width = width;
		this.objectInstance = objectInstance;
		GuiStyle = guiStyle;

		propertyInstance = null;
		ObjType = ObjectType.UnityObject;
		
		CalculateHeight();
	}

	/// <summary>
	/// Creates an <see cref="ObjectField{T}"/> of type <typeparamref name="T"/> and with a width of
	/// <paramref name="width"/>, holding the (Unity) object <paramref name="objectInstance"/>. The
	/// GUI style it uses is the default style for object fields.
	/// </summary>
	/// <param name="width">
	/// The width that this object field component will have. Must be positive (>0).
	/// </param>
	/// <param name="objectInstance">
	/// The object that this object field component holds. This can be <c>null</c>, meaning that
	/// no object is currently held.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive.
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public ObjectField(float width, Object objectInstance = null) : 
				this(width, EditorStyles.objectField, objectInstance) { }
	
	/// <summary>
	/// Creates an <see cref="ObjectField{T}"/> of type <typeparamref name="T"/> and with a width
	/// of <paramref name="width"/> and the GUI style <see cref="guiStyle"/>, holding the Unity
	/// serialized property <paramref name="propertyInstance"/>.
	/// </summary>
	/// <param name="width">
	/// The width that this object field component will have. Must be positive (>0).
	/// </param>
	/// <param name="guiStyle">
	/// The GUI style that this object field component will use to draw itself and calculate its
	/// height.
	/// </param>
	/// <param name="propertyInstance">
	/// The serialized property that this object field component holds. This can be <c>null</c>,
	/// meaning that no property is currently held.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive.
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public ObjectField(float width, GUIStyle guiStyle, SerializedProperty propertyInstance = null) {

		ComponentUtils.AssertGreaterThanZero(width, "width");
		ComponentUtils.AssertMinimumWidth(width, MinimumWidth);

		Width = width;
		this.propertyInstance = propertyInstance;
		GuiStyle = guiStyle;

		objectInstance = null;
		ObjType = ObjectType.SerializedProperty;
		
		CalculateHeight();
	}
	
	/// <summary>
	/// Creates an <see cref="ObjectField{T}"/> of type <typeparamref name="T"/> and with a width
	/// of <paramref name="width"/>, holding the Unity serialized property
	/// <paramref name="propertyInstance"/>. The GUI style it uses is the default style for object
	/// fields.
	/// </summary>
	/// <param name="width">
	/// The width that this object field component will have. Must be positive (>0).
	/// </param>
	/// <param name="propertyInstance">
	/// The serialized property that this object field component holds. This can be <c>null</c>,
	/// meaning that no property is currently held.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If <paramref name="width"/> is not positive.
	/// </exception>
	/// <exception cref="LayoutException">
	/// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
	/// </exception>
	public ObjectField(float width, SerializedProperty propertyInstance = null) : 
					this(width, EditorStyles.objectField, propertyInstance) { }
	
	
	/* ---------- Getters & Setters ---------- */

	/// <summary>
	/// Returns the Unity Object that this object field component holds.
	/// </summary>
	/// <remarks>
	/// If the component does not currently hold an object, this method will return <c>null</c>.
	/// </remarks>
	/// <returns>
	/// The Unity Object that this object field component holds.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this object field component is not of type "Unity Object" (<see cref="ObjType"/> does not
	/// have the value <see cref="ObjectType.UnityObject"/>).
	/// </exception>
	public Object GetObject() {

		if (ObjType == ObjectType.UnityObject) {

			return (objectInstance);
		}

		throw new InvalidOperationException(
			"This object field component does not have the type \"Unity Object\", so it does not " +
			"contain such a value to return.");
	}
	
	/// <summary>
	/// Returns the Unity serialized property that this object field component holds.
	/// </summary>
	/// <remarks>
	/// If the component does not currently hold a property, this method will return <c>null</c>.
	/// </remarks>
	/// <returns>
	/// The Unity serialized property that this object field component holds.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// If this object field component is not of type "Serialized Property" (<see cref="ObjType"/>
	/// does not have the value <see cref="ObjectType.SerializedProperty"/>).
	/// </exception>
	public SerializedProperty GetProperty() {

		if (ObjType == ObjectType.SerializedProperty) {

			return (propertyInstance);
		}

		throw new InvalidOperationException(
			"This object field component does not have the type \"Serialized Property\", so it " + 
			"does not contain such a value to return.");
	}

	/// <summary>
	/// Sets the Unity Object that this object field component holds to the given value.
	/// </summary>
	/// <remarks>
	/// A value of <c>null</c> is valid; this will set the component to "no object held".
	/// </remarks>
	/// <param name="objectInstance">
	/// The Unity object that this object field component should hold.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this object field component is not of type "Unity Object" (<see cref="ObjType"/> does not
	/// have the value <see cref="ObjectType.UnityObject"/>).
	/// </exception>
	public void SetObject(Object objectInstance) {

		if (ObjType == ObjectType.UnityObject) {

			this.objectInstance = objectInstance;
		}

		throw new InvalidOperationException(
			"This object field component does not have the type \"Unity Object\", so it does not " +
			"contain such a value to be set.");
	}
	
	/// <summary>
	/// Sets the Unity serialized property that this object field component holds to the given value.
	/// </summary>
	/// <remarks>
	/// A value of <c>null</c> is valid; this will set the component to "no property held".
	/// </remarks>
	/// <param name="propertyInstance">
	/// The Unity serialized property that this object field component should hold.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// If this object field component is not of type "Serialized Property" (<see cref="ObjType"/>
	/// does not have the value <see cref="ObjectType.SerializedProperty"/>).
	/// </exception>
	public void SetProperty(SerializedProperty propertyInstance) {

		if (ObjType == ObjectType.SerializedProperty) {

			this.propertyInstance = propertyInstance;
		}

		throw new InvalidOperationException(
			"This object field component does not have the type \"Serialized Property\", so it " + 
			"does not contain such a value to be set.");
	}
	
	
	/* ---------- Overrides ---------- */

	/// <inheritdoc/>
	public override void Draw(float positionX, float positionY) {

		if (ObjType == ObjectType.UnityObject) {
			
			objectInstance = 
				EditorGUI.ObjectField(
					GetRect(positionX, positionY), GUIContent.none,
					objectInstance, typeof(T), false);
		}
		else if (ObjType == ObjectType.SerializedProperty) {

			EditorGUI.ObjectField(
				GetRect(positionX, positionY),
				propertyInstance, typeof(T), GUIContent.none);
		}
		else {
			
			throw new NotImplementedException();
		}
	}
	
	/// <inheritdoc/>
	public override GUIStyle GetDefaultGuiStyle() {

		return (EditorStyles.objectField);
	}
}

}
