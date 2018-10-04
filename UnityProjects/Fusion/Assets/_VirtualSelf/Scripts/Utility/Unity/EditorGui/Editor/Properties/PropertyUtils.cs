using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {

    
/// <summary>
/// A collection of static utility methods for working with the Unity serialization in the context
/// of serialized properties (see: <see cref="SerializedProperty"/>) and objects (see:
/// <see cref="SerializedObject"/>), which are part of Unity's "IMGUI" system for working with the
/// Unity Inspector, etc. <br/>
/// (Also see: https://docs.unity3d.com/Manual/GUIScriptingGuide.html)<br/> 
/// </summary>
/// <remarks>
/// Many of the methods inside of this class use reflection to achieve their goals. This means that
/// they are generally <b>not</b> safe against changes to the Unity API, and might potentially break
/// with any new Unity version.<br/>
/// For this reason, all of these methods contain safety and null checks where appropriate, and
/// generally return optional values.<br/>
/// <br/>
/// As many of the methods inside of this class use Unity Editor code (from the
/// <see cref="UnityEditor"/> namespace), they can only be used anywhere where Unity Editor code is
/// allowed. This specifically means that they cannot be used in code that is compiled into the game.
/// </remarks>
public static class PropertyUtils {
    
    /* ---------- Public Methods ---------- */
    
    /// <summary>
    /// Parameterized version of <see cref="GetActualObjectOf"/>.<br/>
    /// <seealso cref="GetActualObjectOf"/>
    /// </summary>
    /// <exception cref="UnityReflectionException">
    /// If <see cref="GetActualObjectOf"/> throws this exception.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// If the object was retrieved, but could not be cast to type <typeparamref name="T"/>.
    /// </exception>
    public static T GetActualObjectOfAs<T>(SerializedProperty property) {

        object actualObject = GetActualObjectOf(property);

        return (((T) actualObject));
    }

    /// <summary>
    /// Returns the "actual" object for the serialized property <paramref name="property"/>. This
    /// refers to the underlying object that the property represents - the field the property was
    /// serialized from.
    /// </summary>
    /// <remarks>
    /// This method generally supports nested and complex properties, and also properties that
    /// reside within collections (like arrays and lists). It does not, however, currently support
    /// properties that reside within of nested collections - the maximum nesting level of
    /// collections it supports is 1. This is due to difficulties outlined in the next paragraph.
    /// For collections, the method also expects them to be implementing <see cref="IEnumerable"/>
    /// - if they do not, the method will not be able to obtain the object.<br/> 
    /// This method works by parsing the <see cref="SerializedProperty.propertyPath"/> path member
    /// of the serialized property, and by using reflection in various places. Thus, the
    /// implementation is somewhat brittle, and the method might in theory break for any new Unity
    /// release.
    /// </remarks>
    /// <param name="property">The serialized property to get the underlying object of.</param>
    /// <returns>
    /// The underlying object of the serialized property.
    /// </returns>
    /// <exception cref="UnityReflectionException">
    /// If the underlying object could not be retrieved. Most likely, this is because Unity changed
    /// something internally in a new version and this method stopped working (also see above).
    /// </exception>
    public static object GetActualObjectOf(SerializedProperty property) {
        
        /* To get the underlying object of a serialized property, we need to use reflection to
         * access the actual field that holds this object in the class the property was originally
         * serialized from.
         * Each serialized property holds a reference to the serialized object it came from, which
         * in turn holds a reference to its "target object" - the underlying object of that
         * serialized object (exactly what we want to get for the serialized property, too).
         * Each property also has a "property path", which contains the entire "path" from the
         * serialized object up to the property.
         * By parsing this property path, it can be found out where the field that the property
         * was serialized from resides. The field can then be accessed by using reflection, at which
         * point the task is finished.
         * Additional complications arise if the property is more complex, for example, is an element
         * of a collection (which is, in turn, of course a serialized property itself, too).
         * As outlined above, nested collections are currently not supported - this would require
         * more complex parsing of the property path - it could be done, it would just be more work
         * to implement and be potentially even more brittle than the method already is. However,
         * this should rarely be relevant in practice, since nested collections serialized for
         * Unity Editor code should only occur very rarely. */
        
        SerializedObject rootObject = property.serializedObject;
        object actualObject;

        try {

            actualObject =
                    GetNestedObjectFromPropertyPath(rootObject, property.propertyPath, false);
        }
        catch (UnityReflectionException) {
            
            /* If the first attempt to obtain the actual object of the property didn't work, we can
             * try another attempt where we also take into account all the parent types of the root
             * object.
             * If that one also fails, we throw the exception that it delivers. */

            actualObject = GetNestedObjectFromPropertyPath(rootObject, property.propertyPath, true);
        }
        
        return (actualObject);
    }
    
    /// <summary>
    /// Parameterized version of <see cref="GetValueOf"/>.<br/>
    /// <seealso cref="GetValueOf"/>
    /// </summary>
    /// <remarks>
    /// Returns an empty value if either <see cref="GetValueOf"/> would also have returned an
    /// empty value, or if the value could not be cast to the type <typeparamref name="T"/>.
    /// </remarks>
    public static Optional<T> GetValueOfAs<T>(SerializedProperty property) {

        Optional<object> valueObject = GetValueOf(property);
        
        if (valueObject.IsPresent() == false) { return (Optional<T>.Empty()); }

        T castValueObject;
        try { castValueObject = ((T) valueObject.Get()); }
        catch (InvalidCastException) { return (Optional<T>.Empty()); }

        return (Optional<T>.Of(castValueObject));
    }

    /// <summary>
    /// Returns the value of the serialized property <paramref name="property"/>. The value will be
    /// of a different type depending on the type of the property - or more specifically, depending
    /// on the type of the underlying object that the property represents.
    /// </summary>
    /// <remarks>
    /// This method attempts to obtain the value type of the property dynamically, over its
    /// <see cref="SerializedProperty.propertyType"/> member. While for most possible property
    /// types, this does not require reflection, it is still somewhat brittle, and might fail in the
    /// future depending on the changes Unity makes in new versions.<br/>
    /// The caller also has to assume the right return type at runtime, since the returned type is
    /// dynamic - this might also fail, and this method cannot make any guarantees about the
    /// returned type.<br/>
    /// <br/>
    /// The idea for this method is taken from here: https://gist.github.com/capnslipp/8516384
    /// </remarks>
    /// <param name="property">
    /// The <see cref="SerializedProperty"/> instance to obtain the value of.
    /// </param>
    /// <returns>
    /// The value of <paramref name="property"/>, depending on its property type, or an empty object
    /// if the property type could not be determined successfully, the value could not be obtained
    /// successfully, or the property type is unknown to this method.<br/>
    /// Some specific types returned are:
    /// <list type="bullet">
    /// <item><description>
    /// LayerMask Property: An <c>int</c> value.
    /// </description></item>
    /// <item><description>
    /// Enum Property: A <see cref="KeyValuePair{TKey,TValue}"/> where <c>TKey</c> is an <c>int</c>
    /// holding the index of the enum, and <c>TValue</c> is a <see cref="string"/> holding its name.
    /// </description></item>
    /// <item><description>
    /// Gradient Property: A <see cref="Gradient"/> instance created from the hidden (non-public)
    /// <c>gradientValue</c> field of the property, obtained via reflection.
    /// </description></item>
    /// </list>
    /// </returns>
    public static Optional<object> GetValueOf(SerializedProperty property) {

        switch (property.propertyType) {

            case SerializedPropertyType.Integer:
                return (Optional<object>.OfNullable(property.intValue));
            case SerializedPropertyType.Boolean:
                return (Optional<object>.OfNullable(property.boolValue));
            case SerializedPropertyType.Float:
                return (Optional<object>.OfNullable(property.floatValue));
            case SerializedPropertyType.String:
                return Optional<object>.OfNullable((property.stringValue));
            case SerializedPropertyType.Color:
                return Optional<object>.OfNullable((property.colorValue));
            case SerializedPropertyType.ObjectReference:
                return Optional<object>.OfNullable((property.objectReferenceValue));
            case SerializedPropertyType.LayerMask:
                return Optional<object>.OfNullable((property.intValue));
            case SerializedPropertyType.Enum:
                int enumI = property.enumValueIndex;
                var result = new KeyValuePair<int, string>(enumI, property.enumNames[enumI]);
                return (Optional<object>.Of(result));
            case SerializedPropertyType.Vector2:
                return (Optional<object>.OfNullable(property.vector2Value));
            case SerializedPropertyType.Vector3:
                return (Optional<object>.OfNullable(property.vector3Value));
            case SerializedPropertyType.Vector4:
                return (Optional<object>.OfNullable(property.vector4Value));
            case SerializedPropertyType.Rect:
                return (Optional<object>.OfNullable(property.rectValue));
            case SerializedPropertyType.ArraySize:
                return (Optional<object>.OfNullable(property.intValue));
            case SerializedPropertyType.Character:
                return (Optional<object>.OfNullable((char) property.intValue));
            case SerializedPropertyType.AnimationCurve:
                return (Optional<object>.OfNullable(property.animationCurveValue));
            case SerializedPropertyType.Bounds:
                return (Optional<object>.OfNullable(property.boundsValue));
            case SerializedPropertyType.Gradient:
                /* The "gradientValue" field of the "SerializedProperty" class does in fact exist,
                 * but it is not public. To return it, we need to use reflection. */
                Gradient gradientValue;
                try { gradientValue = GetGradientValue(property); }
                catch (UnityReflectionException) { return (Optional<object>.Empty()); }
                return Optional<object>.OfNullable(gradientValue);
            case SerializedPropertyType.Quaternion:
                return (Optional<object>.OfNullable(property.quaternionValue));
            case SerializedPropertyType.ExposedReference:
                return (Optional<object>.OfNullable(property.exposedReferenceValue));
            case SerializedPropertyType.FixedBufferSize:
                return (Optional<object>.OfNullable(property.fixedBufferSize));
            case SerializedPropertyType.Vector2Int:
                return (Optional<object>.OfNullable(property.vector2IntValue));
            case SerializedPropertyType.Vector3Int:
                return (Optional<object>.OfNullable(property.vector3IntValue));
            case SerializedPropertyType.RectInt:
                return (Optional<object>.OfNullable(property.rectIntValue));
            case SerializedPropertyType.BoundsInt:
                return (Optional<object>.OfNullable(property.boundsIntValue));

            default:
                return (Optional<object>.Empty());
        }
    }
    
    
    /* ---------- Private Helper Methods ---------- */
    
    /// <summary>
    /// Helper method for retrieving an object (most likely in the form of a
    /// <see cref="SerializedProperty"/> instance) nested inside of the serialized object
    /// <paramref name="rootObject"/>, in the form of nested fields, by the property path
    /// <paramref name="propertyPath"/> pointing at it.
    /// </summary>
    /// <remarks>
    /// Also see the "remarks" section and method comments of <see cref="GetActualObjectOf"/> for
    /// more implementation details, limitations and potential dangers of this method.<br/>
    /// This method, and in turn all methods within this class using it or the ones being used by
    /// it, are partially inspired by:<br/>
    /// https://answers.unity.com/questions/627090/convert-serializedproperty-to-custom-class.html
    /// </remarks>
    /// <seealso cref="GetActualObjectOf"/>
    /// <param name="rootObject">
    /// The serialized root object of the nested object to retrieve.
    /// </param>
    /// <param name="propertyPath">
    /// The property path pointing at the nested object within the root object.
    /// </param>
    /// <param name="includeParentTypes">
    /// Whether to include all parent types of the root object (meaning base types it inherits from,
    /// and interfaces it implements) while searching for the nested object via reflection.<br/>
    /// This increases the runtime of the method, but can find fields that are not directly part of
    /// the root object.
    /// </param>
    /// <returns>
    /// The object nested inside of the root object.
    /// </returns>
    /// <exception cref="UnityReflectionException">
    /// If the object could not be found or retrieved - in almost all cases, this will be a problem
    /// with the reflection used to get the object.
    /// </exception>
    private static object GetNestedObjectFromPropertyPath(
            SerializedObject rootObject,
            string propertyPath,
            bool includeParentTypes = false) {
        
        /* First, parse the property path. If it's invalid, something is wrong - we can't recover
         * from that. */

        PropertyPath resultPath;

        try { resultPath = ParsePropertyPath(propertyPath); }
        catch (ArgumentException e) {
            
            throw new UnityReflectionException(
                    "The property path of the serialized property could not be parsed: ", e);
        }
        
        /* Walk through the property path. Since each "element" of the path is a field, we can get
         * them via reflection. If at any point we fail to retrieve a field, we'll have to give up
         * the search. */
        
        UnityEngine.Object targetObject = rootObject.targetObject;
        Optional<object> partObject = Optional<object>.Of(targetObject);
        bool interrupted = false;
        
        foreach (string part in resultPath.PathObjects) {

            if (interrupted == true) { break; }

            partObject = ReflectionUtils.GetFieldOrPropertyValueOf(
                    partObject.Get(), part, includeParentTypes);
            
            interrupted = !partObject.IsPresent();
        }

        if (interrupted == true) {
            
            throw new UnityReflectionException(
                    "Walking through the object hierarchy making up the property's property path " + 
                    "failed because one of the objects could not be retrieved (via reflection).");
        }

        object resultObject;

        if (resultPath.IsCollectionElement == true) {

            /* If the nested property object is inside of a collection, we have to try to get it out
             * of it. Since we have no idea about the nature of the collection, we can only make the
             * most basic assumption: It implements "IEnumerable". (Since we don't have access to
             * C#4, we don't have covariance for collections, and cannot even use type parameters
             * here.) If this should not be true, and we cannot cast to this, we have to give up. */
            
            IEnumerable collection;

            try {
                collection = ((IEnumerable) partObject.Get());
            }
            catch (InvalidCastException e) {
                
                throw new UnityReflectionException(
                        "The property object is inside of a collection, and could not be retrieved " 
                      + "from it successfully because the collection does not implement a common " + 
                        "interface: ", e);
            }

            int index = 0;
            object element = null;
            foreach (object elem in collection) {

                if (index == resultPath.ElementCollectionIndex) {
                    
                    element = elem;
                    break;
                }
                index++;
            }

            if (element == null) {
                
                throw new UnityReflectionException(
                        "The property object is inside of a collection, and could not be retrieved " 
                      + "from it successfully because of some unknown problem.");               
            }

            resultObject = element;
        }
        else {

            /* If the nested property object is just a normal field, we should be done at this
             * point - it was the last field we retrieved via the property path. */
            
            resultObject = partObject.Get();
        }

        return (resultObject);
    }

    /// <summary>
    /// Helper method to parse and interpret a "property path".<br/>
    /// A property path belongs to a <see cref="SerializedProperty"/> object, and describes the
    /// entire "path" from the serialized root object (<see cref="SerializedObject"/>) the property
    /// came from, up to the property itself.<br/>
    /// Depending on the nature of the property, this requires different amounts of parsing work.
    /// </summary>
    /// <remarks>
    /// Also see the "remarks" section and method comments of <see cref="GetActualObjectOf"/> for
    /// more implementation details, limitations and potential dangers of this method. 
    /// </remarks>
    /// <seealso cref="GetActualObjectOf"/>
    /// <param name="path">The property path to be parsed.</param>
    /// <returns>
    /// The parsed property path and its attributes.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the parsing failed for any reason (e.g. because <paramref name="path"/> was not a valid
    /// property path).
    /// </exception>
    private static PropertyPath ParsePropertyPath(string path) {
        
        /* Property paths use the dot ('.') as their separator, and otherwise look somewhat similar
         * to file paths. Each "folder" is a field of the previous "folder", starting with the
         * (serialized) root object, and up to the serialized property itself. */
        
        List<string> splitPath = path.Split('.').ToList();

        List<string> pathObjects;
        bool isCollection = false;
        int collectionIndex = -1;

        if (splitPath.Contains("Array") == true) {
            
            /* The occurence of "Array" somewhere in the path means that the property resides inside
             * of a collection like "List", or an actual array. We'll check this for validity
             * first. The property itself should have the name "data". */

            if (splitPath.Count < 3) {
                
                throw new ArgumentException(
                        "The property path looks like it contains a collection, but its format " + 
                        "doesn't seem to match this.");
            }

            string arrayPart = splitPath[splitPath.Count - 2];

            if (arrayPart != "Array") {
                
                throw new ArgumentException(
                        "The property path looks like it contains a collection, but it has an " + 
                        "invalid form.");
            }
            
            string elementPart = splitPath[splitPath.Count - 1];

            if (elementPart.StartsWith("data") == false) {
                
                throw new ArgumentException(
                        "The property path looks like it contains a collection, but it has an " + 
                        "invalid form.");
            }

            /* Now we parse this last "data" element, and retrieve its index within the
             * collection. */
            
            int firstIndex = elementPart.IndexOf('[');
            int secondIndex = elementPart.IndexOf(']');

            if (firstIndex == -1 || secondIndex == -1) {
                
                throw new ArgumentException(
                        "The property path looks like it contains a collection, but it has an " + 
                        "invalid form.");
            }
            
            string elementIndex = 
                    elementPart.Substring((firstIndex + 1), (secondIndex - firstIndex - 1));
            
            int index;

            try { index = Int32.Parse(elementIndex); }
            catch (Exception) {
                                
                throw new ArgumentException(
                        "The property path looks like it contains a collection, but it has an " + 
                        "invalid form.");
            }

            /* We don't want to return the "Array" part of the path, because that is irrelevant for
             * retrieving the property - we already return the fact that it is inside of a
             * collection, as well as its index. */
            
            pathObjects = splitPath.GetRange(0, splitPath.Count - 2);
            isCollection = true;
            collectionIndex = index;
        }
        else {
            
            /* If the property is just a field inside of some class, there isn't anything else we
             * need to do here. It can be easily reached. */

            pathObjects = splitPath;
        }
        
        return (new PropertyPath(pathObjects, isCollection, collectionIndex));
    }

    /// <summary>
    /// Helper method to get the "gradient value" of a <see cref="SerializedProperty"/> instance of
    /// the (enum value) type <see cref="SerializedPropertyType.Gradient"/>.<br/>
    /// This field is (for some reason) not public, so its value needs to be obtained via reflection.
    /// </summary>
    /// <remarks>
    /// This method might break for any new Unity release, since it relies on the name of the field
    /// corresponding to the gradient value staying the same - since the field is not part of the
    /// public API of the <see cref="SerializedProperty"/> class, its name could change at any time.
    /// In case the field could not be found, the method throws an exception.
    /// </remarks>
    /// <param name="property">
    /// The <see cref="SerializedProperty"/> instance to obtain the gradient value from. Must be of
    /// (neum value) type
    /// <see cref="SerializedPropertyType"/>.<see cref="SerializedPropertyType.Gradient"/>.
    /// </param>
    /// <returns>
    /// The value of the "gradient value" field of <paramref name="property"/> (which is of type
    /// <see cref="Gradient"/>).
    /// </returns>
    /// <exception cref="UnityReflectionException">
    /// 
    /// </exception>
    private static Gradient GetGradientValue(SerializedProperty property) {
        
        BindingFlags anyAccessFlags = 
                (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        PropertyInfo propertyInfo = typeof(SerializedProperty).GetProperty(
            "gradientValue",
            anyAccessFlags,
            null,
            typeof(Gradient),
            new Type[0],
            null
        );

        if (propertyInfo == null) {
            
            throw new UnityReflectionException(
                    "The gradient value could not be found within the property - Unity might " + 
                    "have changed something internally.");
        }
		
        Gradient gradientValue = (propertyInfo.GetValue(property, null) as Gradient);
        
        return (gradientValue);
    }
    
    
    /* ---------- Inner Classes ---------- */

    /// <summary>
    /// A (read-only and data-only) helper class containing a (valid) parsed
    /// <see cref="SerializedProperty"/> property path, and its attributes. It can be used to work
    /// with the property path, e.g. for retrieving the serialized property it points at from its
    /// (serialized) root object.
    /// </summary>
    private class PropertyPath {

        /// <summary>
        /// A collection containing the "split" property path, in its original order. If the
        /// property is inside of a collection (like a list), the path will stop at the list and
        /// not contain the "Array" part anymore.
        /// </summary>
        public readonly IList<string> PathObjects;

        /// <summary>
        /// Whether the property pointed at by this property path is inside of a collection (like a
        /// list).
        /// </summary>
        public readonly bool IsCollectionElement;

        /// <summary>
        /// If <see cref="IsCollectionElement"/> is <c>true</c>, this is the index of the property
        /// inside of the collection. If it is <c>false</c>, the value of this field is not valid.
        /// </summary>
        public readonly int ElementCollectionIndex;

        public PropertyPath(
                IList<string> pathObjects,
                bool isCollectionElement = false,
                int elementCollectionIndex = -1) {

            PathObjects = pathObjects;
            IsCollectionElement = isCollectionElement;
            ElementCollectionIndex = elementCollectionIndex;
        }
    }
}

}
