using UnityEditor;
using UnityEngine;


namespace VirtualSelf.GameSystems.Editor {

/// <summary>
/// TODO: Fill out this class description: KeycodeEditor
/// </summary>
[CustomEditor(typeof(Keycode))]
public sealed class KeycodeEditor : UnityEditor.Editor {

    /* ---------- Variables & Properties ---------- */
    
    private Keycode refObject;

    private SerializedProperty propDigitOne;
    private SerializedProperty propDigitTwo;
    private SerializedProperty propDigitThree;
    private SerializedProperty propDigitFour;
    private SerializedProperty propCodeString;
    private SerializedProperty propRenameAutomatically;

    private GUIStyle styleBox;
    

    /* ---------- Methods ---------- */

    private void OnEnable() {

        refObject = ((Keycode) target);

        propDigitOne = serializedObject.FindProperty(Keycode.FieldNameDigitOne);
        propDigitTwo = serializedObject.FindProperty(Keycode.FieldNameDigitTwo);
        propDigitThree = serializedObject.FindProperty(Keycode.FieldNameDigitThree);
        propDigitFour = serializedObject.FindProperty(Keycode.FieldNameDigitFour);
        propCodeString = serializedObject.FindProperty(Keycode.FieldNameCodeString);
        propRenameAutomatically = serializedObject.FindProperty(Keycode.FieldNameRenameAutomatically);
    }

    public override void OnInspectorGUI() {
        
        serializedObject.Update();
                
        /* Cannot be moved into "OnEnable", or anywhere else. That throws an exception. */
        styleBox = new GUIStyle("Box");

        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Code Customization");
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();

        propDigitOne.intValue = EditorGUILayout.Popup(
                propDigitOne.intValue, Keycode.PossibleDigits);
        propDigitTwo.intValue = EditorGUILayout.Popup(
                propDigitTwo.intValue, Keycode.PossibleDigits);
        propDigitThree.intValue = EditorGUILayout.Popup(
                propDigitThree.intValue, Keycode.PossibleDigits);
        propDigitFour.intValue = EditorGUILayout.Popup(
                propDigitFour.intValue, Keycode.PossibleDigits);

        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Resulting Code:");
        
        EditorGUILayout.Space();
            
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField(propCodeString.stringValue);
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
            
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Options");

        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();

        propRenameAutomatically.boolValue = 
            EditorGUILayout.ToggleLeft("Rename asset file automatically",
                propRenameAutomatically.boolValue);

        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }


    /* ---------- Overrides ---------- */

    




    /* ---------- Inner Classes ---------- */






}

}