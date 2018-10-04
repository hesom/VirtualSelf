using System;
using Rotorz.ReorderableList;
using UnityEditor;


namespace VirtualSelf {

namespace Utility {

namespace Editor {
    
    
/// <summary>
/// A custom adaptor for <see cref="Rotorz.ReorderableList"/> that is specialized for lists where
/// each item has a dynamic height. A "dynamic height", in this context, means that each list item
/// will generally have a height different from each other one, and the height of each item might
/// change within every frame.<br/>
/// This adaptor works by expecting each list item to expose a serialized field variable of type
/// <c>float</c> containing the height of the item for drawing. This adaptor obtains that
/// serialized field (as a <see cref="SerializedProperty"/>) value and sets the height of each list
/// item to their respective height field value.<br/>
/// This means that the height of each item in lists using this adaptor can be totally dynamic, and
/// change at any time (and will be reflected by the list the next time Unity redraws it). However,
/// it also means that the list items have to have, and update, such a property for their height,
/// as described above.<br/>
/// This adaptor does not care about how items update their height values - it is generally
/// recommended, though, that a Unity custom property drawer is written for them, and the height
/// value for each of them is then updated within the drawing method
/// (<see cref="PropertyDrawer.OnGUI"/>) of that property drawer).
/// <remarks>
/// Sadly, there is currently no proper system for ensuring that the items within the list property
/// given to this adaptor actually have a "height" field (and that it is serialized), nor that it is
/// of type <c>float</c>, nor that its name is the right one. All this adaptor can do is to
/// try to retrieve the serialized field, and then its value, and throw an exception if anything
/// does not work.<br/>
/// It should also be noted that due to the nature of how this adaptor works, it will be a good deal
/// slower than one that simply has a fixed height value for every possible item in the list, which
/// is set only once and then just read. More specifically, the larger the amount of items in the
/// list, the slower the drawing of the entire list will become.
/// </remarks>
/// </summary>
public class DynamicHeightListAdaptor : SerializedPropertyAdaptor {
    
    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// The name of the serialized property within each item of the list that contains the height
    /// of the respective item. This is required by this adaptor to retrieve the height for each
    /// item.
    /// </summary>
    private readonly string heightPropertyName;
    

    /* ---------- Constructors ---------- */

    /// <summary>
    /// Constructs a new <see cref="DynamicHeightListAdaptor"/> for the list property given by
    /// <see cref="listProperty"/>, and using the height property name
    /// <see cref="heightPropertyName"/> for each item within the list.
    /// </summary>
    /// <param name="listProperty">
    /// The serialized property for the list that this adaptor should be used for.
    /// </param>
    /// <param name="heightPropertyName">
    /// The name of the serialized property of each list item that contains its height. This must
    /// be a property of type <see cref="SerializedPropertyType.Float"/>.
    /// </param>
    public DynamicHeightListAdaptor(
            SerializedProperty listProperty, string heightPropertyName) : base(listProperty) {

        this.heightPropertyName = heightPropertyName;
    }


    /* ---------- Overrides ---------- */

    /// <summary>
    /// See: <see cref="SerializedPropertyAdaptor.GetItemHeight"/>
    /// </summary>
    /// <exception cref="SystemException">
    /// If no serialized property with the name <see cref="heightPropertyName"/> can be found within
    /// the serialized items of <see cref="SerializedPropertyAdaptor.arrayProperty"/>, or if the
    /// type of that property is not <see cref="SerializedPropertyType.Float"/>.
    /// </exception>
    public override float GetItemHeight(int index) {

        SerializedProperty itemProperty = this[index];
        SerializedProperty itemHeightProperty = itemProperty.FindPropertyRelative(heightPropertyName);

        if (itemHeightProperty == null) {

            throw new SystemException(
                "A serialized property for retrieving the item hight with the name \"" +
                heightPropertyName + "\" could not be found. Item height cannot be determined.");
        }
        
        if (itemHeightProperty.propertyType != SerializedPropertyType.Float) {
			
            throw new SystemException(
                "The serialized property for retrieving the item height for each item must be of " + 
                "property type \"Float\" (but was of type \"" + itemHeightProperty.propertyType + 
                "\" instead).");	
        }
        
        return (itemHeightProperty.floatValue);
    }
}

}

}

}
