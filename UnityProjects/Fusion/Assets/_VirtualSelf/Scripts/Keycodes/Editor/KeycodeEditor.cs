using UnityEditor;
using UnityEngine;
using VirtualSelf.Utility.Editor;


namespace VirtualSelf.GameSystems.Editor {

/// <summary>
/// TODO: Fill out this class description: KeycodeEditor
/// </summary>
[CustomEditor(typeof(Keycode))]
public sealed class KeycodeEditor : UnityEditor.Editor {

    /* ---------- Variables & Properties ---------- */
    
   private Keycode refObject;

    private static readonly SerializedPropertyInfo PropDigitOneInfo = 
        new SerializedPropertyInfo(Keycode.FieldNameDigitOne, "First Digit");
    
    private static readonly SerializedPropertyInfo PropDigitTwoInfo = 
        new SerializedPropertyInfo(Keycode.FieldNameDigitTwo, "Second Digit");
    
    private static readonly SerializedPropertyInfo PropDigitThreeInfo = 
        new SerializedPropertyInfo(Keycode.FieldNameDigitThree, "Third Digit");
    
    private static readonly SerializedPropertyInfo PropDigitFourInfo = 
        new SerializedPropertyInfo(Keycode.FieldNameDigitFour, "Fourth Digit");
    
    private static readonly SerializedPropertyInfo PropCodeStringInfo = 
        new SerializedPropertyInfo(Keycode.FieldNameCodeString, "");
    
    private static readonly SerializedPropertyInfo PropIsDiscoveredInfo =
        new SerializedPropertyInfo(Keycode.FieldNameIsDiscovered, "Is discovered");
    
    private static readonly SerializedPropertyInfo PropOnDiscoveredStateChangedInfo =
        new SerializedPropertyInfo(nameof(Keycode.OnDiscoveredStateChanged),
            "On \"Discovered\" State Changed");
        
    private SerializedProperty propDigitOne;
    private SerializedProperty propDigitTwo;
    private SerializedProperty propDigitThree;
    private SerializedProperty propDigitFour;
    private SerializedProperty propCodeString;
    private SerializedProperty propIsDiscovered;
    private SerializedProperty propOnDiscoveredStateChanged;

    private GUIStyle styleBox;
    

    /* ---------- Methods ---------- */

    private void OnEnable() {

        refObject = ((Keycode) target);

        propDigitOne = serializedObject.FindProperty(PropDigitOneInfo);
        propDigitTwo = serializedObject.FindProperty(PropDigitTwoInfo);
        propDigitThree = serializedObject.FindProperty(PropDigitThreeInfo);
        propDigitFour = serializedObject.FindProperty(PropDigitFourInfo);
        propCodeString = serializedObject.FindProperty(PropCodeStringInfo);
        propIsDiscovered = serializedObject.FindProperty(PropIsDiscoveredInfo);
        propOnDiscoveredStateChanged = serializedObject.FindProperty(PropOnDiscoveredStateChangedInfo);
    }
    
    
    /* ---------- Overrides ---------- */

    public override void OnInspectorGUI() {
        
        /* ---------- Section: Initialization ---------- */
        
        serializedObject.Update();
                
        /* Cannot be moved into "OnEnable", or anywhere else. That throws an exception. */
        styleBox = new GUIStyle("Box");

        
        /* ---------- Section: Code Customization ---------- */
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Code Customization", EditorStyles.boldLabel);
        
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
        
        
        /* ---------- Section: Resulting Code ---------- */
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Resulting Code:", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField(propCodeString.stringValue);
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
            
        EditorGUILayout.Space();
        
        
        /* ---------- Section: Ingame Settings ---------- */
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Ingame Settings", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();

        propIsDiscovered.boolValue = 
            EditorGUILayout.ToggleLeft(PropIsDiscoveredInfo.EditorText,
                propIsDiscovered.boolValue);

        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        
        /* ---------- Section: Editor Options ---------- */
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Editor Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("Rename asset file into keycode")) {
            
            refObject.RenameAssetToCode();
        }
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
         
        
        /* ---------- Section: Events ---------- */
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(propOnDiscoveredStateChanged);
        
        EditorGUILayout.EndVertical();
            
        EditorGUILayout.Space();       
        
        
        /* ---------- Section: Finalization ---------- */
        
        serializedObject.ApplyModifiedProperties();
    }
}

}