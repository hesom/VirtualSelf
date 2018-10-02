using System;
using System.Reflection;


namespace VirtualSelf.Utility {

    
/// <summary>
/// A collection of static utility methods concerning the reflection capabilities of C#, and related
/// things.
/// </summary>
public static class ReflectionUtils {   
    
    /* ---------- Public Methods ---------- */
    
    /// <summary>
    /// Attempts to set the value of the field or property with the name <paramref name="name"/>
    /// from the object <paramref name="obj"/>, to the value <paramref name="value"/>, using
    /// reflection and the binding flags <paramref name="bindings"/>.
    /// </summary>
    /// <remarks>
    /// This method basically combines the <see cref="SetFieldValueOf"/> and
    /// <see cref="SetPropertyValueOf"/> methods into one.
    /// </remarks>
    /// <param name="obj">The object to set the field or property value of.</param>
    /// <param name="name">The name of the field or property to set the value of.</param>
    /// <param name="value">The value to set the field or property to.</param>
    /// <param name="includeParentTypes">
    /// Whether to also consider all parent types of <paramref name="obj"/>, in addition to its own
    /// type, when searching for the field/property. Default is <c>false</c>.
    /// </param>
    /// <param name="bindings">
    /// The binding flags to use while trying to access the field/property. By default, basically
    /// all kinds of fields/properties on all access levels are checked.
    /// </param>
    /// <returns>
    /// <c>true</c> if the field/property value was set successfully, and <c>false</c> if the
    /// field/property was not found, could not be accessed, or the type of <paramref name="value"/>
    /// could not be converted to the type of the field/property.
    /// </returns>
    public static bool SetFieldOrPropertyValueOf(
            object obj,
            string name,
            object value,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {

        if (SetFieldValueOf(obj, name, value, false, bindings) == true) { return (true); }

        if (SetPropertyValueOf(obj, name, value, false, bindings) == true) { return (true); }

        if (includeParentTypes == true) {

            if (SetFieldValueOf(obj, name, value, true, bindings) == true) { return (true); }

            if (SetPropertyValueOf(obj, name, value, true, bindings) == true) { return (true); }
        }
 
        return (false);
    }

    /// <summary>
    /// Attempts to set the value of the field with the name <paramref name="fieldName"/> from the
    /// object <paramref name="obj"/>, to the value <paramref name="value"/>, using reflection and
    /// the binding flags <paramref name="bindings"/>.
    /// </summary>
    /// <param name="obj">The object to set the field value of.</param>
    /// <param name="fieldName">The name of the field to set the value of.</param>
    /// <param name="value">The value to set the field to.</param>
    /// <param name="includeParentTypes">
    /// Whether to also consider all parent types of <paramref name="obj"/>, in addition to its own
    /// type, when searching for the field. Default is <c>false</c>.
    /// </param>
    /// <param name="bindings">
    /// The binding flags to use while trying to access the field. By default, basically all kinds
    /// of fields on all access levels are checked.
    /// </param>
    /// <returns>
    /// <c>true</c> if the field value was set successfully, and <c>false</c> if the field was not
    /// found, could not be accessed, or the type of <paramref name="value"/> could not be converted
    /// to the type of the field.
    /// </returns>    
    public static bool SetFieldValueOf(
            object obj, string fieldName, object value,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {
        
        FieldInfo fieldInfo = obj.GetType().GetField(fieldName, bindings);

        if (fieldInfo != null) {

            try { fieldInfo.SetValue(obj, value); }
            catch (ArgumentException) { return (false); }

            return (true);
        }

        if (includeParentTypes == true) {
            
            foreach (Type parentType in TypeSystemUtils.GetAllParentTypesOf(obj.GetType())) {
            
                fieldInfo = parentType.GetField(fieldName, bindings);

                if (fieldInfo != null) {
                    
                    try { fieldInfo.SetValue(obj, value); }
                    catch (ArgumentException) { return (false); }

                    return (true);
                }
            }
        }
        
        return (false);
    }
    
    /// <summary>
    /// Attempts to set the value of the property with the name <paramref name="propertyName"/> from
    /// the object <paramref name="obj"/>, to the value <paramref name="value"/>, using reflection
    /// and the binding flags <paramref name="bindings"/>.
    /// </summary>
    /// <param name="obj">The object to set the property value of.</param>
    /// <param name="propertyName">The name of the property to set the value of.</param>
    /// <param name="value">The value to set the property to.</param>
    /// <param name="includeParentTypes">
    /// Whether to also consider all parent types of <paramref name="obj"/>, in addition to its own
    /// type, when searching for the property. Default is <c>false</c>.
    /// </param>
    /// <param name="bindings">
    /// The binding flags to use while trying to access the property. By default, basically all
    /// kinds of properties on all access levels are checked.
    /// </param>
    /// <returns>
    /// <c>true</c> if the property value was set successfully, and <c>false</c> if the property was
    /// not found, could not be accessed, or the type of <paramref name="value"/> could not be
    /// converted to the type of the property.
    /// </returns>    
    public static bool SetPropertyValueOf(
            object obj, string propertyName, object value,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {
        
        PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, bindings);

        if (propertyInfo != null) {
    
            try { propertyInfo.SetValue(obj, value, null); }
            catch (ArgumentException) { return (false); }

            return (true);
        }

        if (includeParentTypes == true) {
            
            foreach (Type parentType in TypeSystemUtils.GetAllParentTypesOf(obj.GetType())) {
            
                propertyInfo = parentType.GetProperty(propertyName, bindings);

                if (propertyInfo != null) {
                    
                    try { propertyInfo.SetValue(obj, value, null); }
                    catch (ArgumentException) { return (false); }

                    return (true);
                }
            }
        }
        
        return (false);
    }
    
    /// <summary>
    /// Parameterized version of <see cref="GetFieldOrPropertyValueOf"/>.<br/>
    /// <seealso cref="GetFieldOrPropertyValueOf"/>
    /// </summary>
    /// <remarks>
    /// Returns an empty value if either <see cref="GetFieldOrPropertyValueOf"/> would also have
    /// returned an empty value, or if the value could not be cast to the type
    /// <typeparamref name="T"/>.
    /// </remarks>
    public static Optional<T> GetFieldOrPropertyValueOfAs<T>(
            object obj, string name,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {
        
        Optional<object> value = GetFieldOrPropertyValueOf(obj, name, includeParentTypes, bindings);

        if (value.IsPresent() == false) { return (Optional<T>.Empty()); }

        T castValue;
        try {
            castValue = ((T) value.Get());
        }
        catch (InvalidCastException) { return (Optional<T>.Empty()); }

        return (Optional<T>.Of(castValue)); 
    }
    
    /// <summary>
    /// Attempts to get the value of the field or property with the name <paramref name="name"/>
    /// from the object <paramref name="obj"/>, using reflection and the binding flags
    /// <paramref name="bindings"/>.
    /// </summary>
    /// <remarks>
    /// This method basically combines the <see cref="GetFieldValueOf"/> and
    /// <see cref="GetPropertyValueOf"/> methods into one.
    /// </remarks>
    /// <param name="obj">The object to get the field or property value from.</param>
    /// <param name="name">The name of the field or property to get the value from.</param>
    /// <param name="includeParentTypes">
    /// Whether to also consider all parent types of <paramref name="obj"/>, in addition to its own
    /// type, when searching for the field/property. Default is <c>false</c>.
    /// </param>
    /// <param name="bindings">
    /// The binding flags to use while trying to access the field/property. By default, basically
    /// all kinds of fields/properties on all access levels are checked.
    /// </param>
    /// <returns>
    /// The value of the field or property if it was successfully acquired, or an empty value if it
    /// wasn't.
    /// </returns>
    public static Optional<object> GetFieldOrPropertyValueOf(
            object obj,
            string name,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {

        Optional<object> fieldValue = GetFieldValueOf(obj, name, false, bindings);

        if (fieldValue.IsPresent() == true) { return (fieldValue); }
 
        Optional<object> propertyValue = GetPropertyValueOf(obj, name, false, bindings);

        if (propertyValue.IsPresent() == true) { return (propertyValue); }

        if (includeParentTypes == true) {
 
            fieldValue = GetFieldValueOf(obj, name, true, bindings);

            if (fieldValue.IsPresent() == true) { return (fieldValue); }
 
            propertyValue = GetPropertyValueOf(obj, name, true, bindings);

            if (propertyValue.IsPresent() == true) { return (propertyValue); }
        }
 
        return (Optional<object>.Empty());
    }

    /// <summary>
    /// Parameterized version of <see cref="GetFieldValueOf"/>.<br/>
    /// <seealso cref="GetFieldValueOf"/>
    /// </summary>
    /// <remarks>
    /// Returns an empty value if either <see cref="GetFieldValueOf"/> would also have returned an
    /// empty value, or if the value could not be cast to the type <typeparamref name="T"/>.
    /// </remarks>
    public static Optional<T> GetFieldValueOfAs<T>(
            object obj,
            string fieldName,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {

        Optional<object> fieldValue = GetFieldValueOf(obj, fieldName, includeParentTypes, bindings);

        if (fieldValue.IsPresent() == false) { return (Optional<T>.Empty()); }

        T castFieldValue;
        try {
            castFieldValue = ((T) fieldValue.Get());
        }
        catch (InvalidCastException) { return (Optional<T>.Empty()); }

        return (Optional<T>.Of(castFieldValue));
    }
    
    /// <summary>
    /// Parameterized version of <see cref="GetPropertyValueOf"/>.<br/>
    /// <seealso cref="GetPropertyValueOf"/>
    /// </summary>
    /// <remarks>
    /// Returns an empty value if either <see cref="GetPropertyValueOf"/> would also have returned
    /// an empty value, or if the value could not be cast to the type <typeparamref name="T"/>.
    /// </remarks>
    public static Optional<T> GetPropertyValueOfAs<T>(
            object obj,
            string propertyName,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {

        Optional<object> propertyValue = 
                GetPropertyValueOf(obj, propertyName, includeParentTypes, bindings);

        if (propertyValue.IsPresent() == false) { return (Optional<T>.Empty()); }

        T castPropertyValue;
        try {
            castPropertyValue = ((T) propertyValue.Get());
        }
        catch (InvalidCastException) { return (Optional<T>.Empty()); }

        return (Optional<T>.Of(castPropertyValue));
    }

    /// <summary>
    /// Attempts to get the value of the field with the name <paramref name="fieldName"/> from the
    /// object <paramref name="obj"/>, using reflection and the binding flags
    /// <paramref name="bindings"/>.
    /// </summary>
    /// <param name="obj">The object to get the field value from.</param>
    /// <param name="fieldName">The name of the field to get the value from.</param>
    /// <param name="includeParentTypes">
    /// Whether to also consider all parent types of <paramref name="obj"/>, in addition to its own
    /// type, when searching for the field. Default is <c>false</c>.
    /// </param>
    /// <param name="bindings">
    /// The binding flags to use while trying to access the field. By default, basically all kinds
    /// of fields on all access levels are checked.
    /// </param>
    /// <returns>
    /// The value of the field if it was successfully acquired, or an empty value if it wasn't.
    /// </returns>
    public static Optional<object> GetFieldValueOf(
            object obj,
            string fieldName,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {
 
        FieldInfo fieldInfo = obj.GetType().GetField(fieldName, bindings);

        if (fieldInfo != null) { return (Optional<object>.Of(fieldInfo.GetValue(obj))); }

        if (includeParentTypes == true) {
            
            foreach (Type parentType in TypeSystemUtils.GetAllParentTypesOf(obj.GetType())) {
            
                fieldInfo = parentType.GetField(fieldName, bindings);
                if (fieldInfo != null) { return (Optional<object>.Of(fieldInfo.GetValue(obj))); }
            }
        }
        
        return (Optional<object>.Empty());
    }
    
    /// <summary>
    /// Attempts to get the value of the property with the name <paramref name="propertyName"/> from
    /// the object <paramref name="obj"/>, using reflection and the binding flags
    /// <paramref name="bindings"/>.
    /// </summary>
    /// <remarks>
    /// Indexed properties are not supported by this method.
    /// </remarks>
    /// <param name="obj">The object to get the property value from.</param>
    /// <param name="propertyName">The name of the property to get the value from.</param>
    /// <param name="includeParentTypes">
    /// Whether to also consider all parent types of <paramref name="obj"/>, in addition to its own
    /// type, when searching for the property. Default is <c>false</c>.
    /// </param>
    /// <param name="bindings">
    /// The binding flags to use while trying to access the property. By default, basically all
    /// kinds of properties on all access levels are checked.
    /// </param>
    /// <returns>
    /// The value of the property if it was successfully acquired, or an empty value if it wasn't.
    /// </returns>
    public static Optional<object> GetPropertyValueOf(
            object obj,
            string propertyName,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {
        
        PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, bindings);

        if (propertyInfo != null) { return (Optional<object>.Of(propertyInfo.GetValue(obj, null))); }
        
        if (includeParentTypes == true) {
            
            foreach (Type parentType in TypeSystemUtils.GetAllParentTypesOf(obj.GetType())) {
            
                propertyInfo = parentType.GetProperty(propertyName, bindings);
                if (propertyInfo != null) {
                    return (Optional<object>.Of(propertyInfo.GetValue(obj, null)));
                }
            }
        }
        
        return (Optional<object>.Empty());
    }
    
    /// <summary>
    /// Attempts to get the type of the field or property with the name <paramref name="name"/>
    /// from the object <paramref name="obj"/>, using reflection and the binding flags
    /// <paramref name="bindings"/>.
    /// </summary>
    /// <remarks>
    /// This method basically combines the <see cref="GetFieldTypeOf"/> and
    /// <see cref="GetPropertyTypeOf"/> methods into one.
    /// </remarks>
    /// <param name="obj">The object to get the field or property type from.</param>
    /// <param name="name">The name of the field or property to get the type from.</param>
    /// <param name="includeParentTypes">
    /// Whether to also consider all parent types of <paramref name="obj"/>, in addition to its own
    /// type, when searching for the field/property. Default is <c>false</c>.
    /// </param>
    /// <param name="bindings">
    /// The binding flags to use while trying to access the field/property. By default, basically
    /// all kinds of fields/properties on all access levels are checked.
    /// </param>
    /// <returns>
    /// The type of the field or property if it was successfully acquired, or an empty value if it
    /// wasn't.
    /// </returns>
    public static Optional<Type> GetFieldOrPropertyTypeOf(
            object obj,
            string name,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {

        Optional<Type> fieldType = GetFieldTypeOf(obj, name, false, bindings);

        if (fieldType.IsPresent() == true) { return (fieldType); }
 
        Optional<Type> propertyType = GetPropertyTypeOf(obj, name, false, bindings);

        if (propertyType.IsPresent() == true) { return (propertyType); }

        if (includeParentTypes == true) {
 
            fieldType = GetFieldTypeOf(obj, name, true, bindings);

            if (fieldType.IsPresent() == true) { return (fieldType); }
 
            propertyType = GetPropertyTypeOf(obj, name, true, bindings);

            if (propertyType.IsPresent() == true) { return (propertyType); }
        }
 
        return (Optional<Type>.Empty());
    }
    
    /// <summary>
    /// Attempts to get the type of the field with the name <paramref name="fieldName"/> from the
    /// object <paramref name="obj"/>, using reflection and the binding flags
    /// <paramref name="bindings"/>.
    /// </summary>
    /// <remarks>
    /// Indexed fields are not supported by this method.
    /// </remarks>
    /// <param name="obj">The object to get the field type from.</param>
    /// <param name="fieldName">The name of the field to get the type from.</param>
    /// <param name="includeParentTypes">
    /// Whether to also consider all parent types of <paramref name="obj"/>, in addition to its own
    /// type, when searching for the field. Default is <c>false</c>.
    /// </param>
    /// <param name="bindings">
    /// The binding flags to use while trying to access the field. By default, basically all kinds
    /// of fields on all access levels are checked.
    /// </param>
    /// <returns>
    /// The type of the field if it was successfully acquired, or an empty type if it wasn't.
    /// </returns>
    public static Optional<Type> GetFieldTypeOf(
            object obj,
            string fieldName,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {

        FieldInfo fieldInfo = obj.GetType().GetField(fieldName, bindings);

        if (fieldInfo != null) { Optional<Type>.Of(fieldInfo.FieldType); }

        if (includeParentTypes == true) {
            
            foreach (Type parentType in TypeSystemUtils.GetAllParentTypesOf(obj.GetType())) {
            
                fieldInfo = parentType.GetField(fieldName, bindings);
                if (fieldInfo != null) {
                    return (Optional<Type>.Of(fieldInfo.FieldType));
                }
            }            
        }

        return (Optional<Type>.Empty());
    }

    /// <summary>
    /// Attempts to get the type of the property with the name <paramref name="propertyName"/> from
    /// the object <paramref name="obj"/>, using reflection and the binding flags
    /// <paramref name="bindings"/>.
    /// </summary>
    /// <remarks>
    /// Indexed properties are not supported by this method.
    /// </remarks>
    /// <param name="obj">The object to get the property type from.</param>
    /// <param name="propertyName">The name of the property to get the type from.</param>
    /// <param name="includeParentTypes">
    /// Whether to also consider all parent types of <paramref name="obj"/>, in addition to its own
    /// type, when searching for the property. Default is <c>false</c>.
    /// </param>
    /// <param name="bindings">
    /// The binding flags to use while trying to access the property. By default, basically all
    /// kinds of properties on all access levels are checked.
    /// </param>
    /// <returns>
    /// The type of the property if it was successfully acquired, or an empty type if it wasn't.
    /// </returns>
    public static Optional<Type> GetPropertyTypeOf(
            object obj,
            string propertyName,
            bool includeParentTypes = false,
            BindingFlags bindings = (
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic)) {

        PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, bindings);

        if (propertyInfo != null) { Optional<Type>.Of(propertyInfo.PropertyType); }

        if (includeParentTypes == true) {
            
            foreach (Type parentType in TypeSystemUtils.GetAllParentTypesOf(obj.GetType())) {
            
                propertyInfo = parentType.GetProperty(propertyName, bindings);
                if (propertyInfo != null) {
                    return (Optional<Type>.Of(propertyInfo.PropertyType));
                }
            }            
        }

        return (Optional<Type>.Empty());
    }
    
    // TODO: @Manuel: Comment this?
    public static MemberInfo GetFieldOrProperty(Type t, string name)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase;
        FieldInfo f = t.GetField(name, flags);
        if (f != null) return f;
        PropertyInfo p = t.GetProperty(name, flags);
        if (p != null) return p;

        throw new EntryPointNotFoundException("no field or property "+name+" in "+t);
    }
}

}
