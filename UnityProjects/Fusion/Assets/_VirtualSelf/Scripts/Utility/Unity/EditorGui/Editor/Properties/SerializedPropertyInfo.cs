using UnityEditor;

namespace VirtualSelf.Utility.Editor {

/// <summary>
/// An immutable helper class for, mainly, Unity custom Property Drawers (and to an extend also
/// custom Inspectors).<br/>
/// This class models the information about a field of the class the Property Drawer or Inspector is
/// drawing, which can then be used in tandem with a <see cref="UnityEditor.SerializedProperty"/>.
/// The class does not do much at all, and is just meant for convenience.
/// </summary>
public sealed class SerializedPropertyInfo {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// The name of the field within the class to be drawn. This is needed for calls like
    /// <see cref="SerializedProperty.FindPropertyRelative"/> and
    /// <see cref="SerializedObject.FindProperty"/>.<br/>
    /// It is best to set this field by using C#'s <c>nameof()</c> function, to make sure it
    /// survives renaming of the field (or at least fails at compile time, and not at runtime).
    /// </summary>
    public string FieldName { get; }
    
    /// <summary>
    /// The text to be displayed for the field within the class to be drawn in the respective editor
    /// (be it a Property Drawer, an Inspector, etc.). This has no bearing on the functionality.
    /// </summary>
    public string EditorText { get; set; }


    /* ---------- Constructors ---------- */

    /// <summary>
    /// Creates a <see cref="SerializedPropertyInfo"/> with the given field name and editor text.
    /// </summary>
    public SerializedPropertyInfo(string fieldName, string editorText) {
        
        FieldName = fieldName;
        EditorText = editorText;
    }

    
    /* ---------- Overrides ---------- */

    /// <summary>
    /// Two <see cref="SerializedPropertyInfo"/> are equal if and only if their
    /// <see cref="FieldName"/>s are requal, <see cref="EditorText"/> is not considered.
    /// </summary>
    public override bool Equals(object obj) {
        
        if (ReferenceEquals(null, obj)) { return (false); }
        
        if (ReferenceEquals(this, obj)) { return (true); }

        var info = obj as SerializedPropertyInfo;
        return ((info != null) && Equals(info));
    }

    public override int GetHashCode() {
        
        return (FieldName.GetHashCode());
    }

    
    /* ---------- Operator Overloads ---------- */

    /// <summary>
    /// This class can be used in place of <see cref="System.String"/>, for example in containers
    /// of strings. Its string representation is equal to <see cref="FieldName"/>,
    /// <see cref="EditorText"/> is not considered. This makes it more convenient to work with
    /// instances of this class.
    /// </summary>
    public static implicit operator string(SerializedPropertyInfo input) {

        return (input.FieldName);
    }
    
    public static bool operator ==(SerializedPropertyInfo left, SerializedPropertyInfo right) {
    
        return (Equals(left, right));
    }

    public static bool operator !=(SerializedPropertyInfo left, SerializedPropertyInfo right) {
        
        return (Equals(left, right) == false);
    }
    
    private bool Equals(SerializedPropertyInfo other) {
        
        return string.Equals(FieldName, other.FieldName);
    }
}

}