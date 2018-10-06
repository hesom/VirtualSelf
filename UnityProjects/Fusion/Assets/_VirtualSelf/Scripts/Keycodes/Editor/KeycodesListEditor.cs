using System.Collections.Generic;
using System.Text;
using Rotorz.ReorderableList;
using UnityEditor;
using UnityEngine;
using VirtualSelf.Utility;
using VirtualSelf.Utility.Editor;


namespace VirtualSelf.GameSystems.Editor {


/// <summary>
/// TODO: Fill out this class description: KeycodesListEditor
/// </summary>
[CustomEditor(typeof(KeycodesList))]
public sealed class KeycodesListEditor : UnityEditor.Editor {

    /* ---------- Variables & Properties ---------- */

    private const string MessageDuplicateMappings =
        "The list currently contains duplicate mappings for some keycodes and/or rooms. " +
        "This is not allowed. Please fix all the responsible mappings to eliminate the " +
        "duplicates.\n" +
        "Any leftover duplicate mappings will be ignored, and not be available in the " +
        "game.\n" +
        "The existing duplicates are:";

    private const string MessageNoDuplicatesInList =
        "There are currently no duplicate mappings in the list.";
    
    private static readonly SerializedPropertyInfo PropKeycodeRoomMappingsInfo =
        new SerializedPropertyInfo(
            KeycodesList.FieldNameKeycodeRoomMappings, "Keycode - Room Mappings List");
    
    private static readonly SerializedPropertyInfo PropOnAnyListElementStateChangedInfo =
        new SerializedPropertyInfo(nameof(KeycodesList.OnAnyListElementStateChanged),
            "On Any List Element State Changed");
    
    private static readonly SerializedPropertyInfo PropOnKeycodeStateChangedInfo =
        new SerializedPropertyInfo(nameof(KeycodesList.OnKeycodeStateChanged),
            "On Keycode State Changed");
    
    private static readonly SerializedPropertyInfo PropOnRoomStateChangedInfo =
        new SerializedPropertyInfo(nameof(KeycodesList.OnRoomStateChanged),
            "On Room State Changed");
    
    // private KeycodesList refObject;

    private SerializedProperty propKeycodeRoomMappings;
    private SerializedProperty propOnAnyListElementStateChanged;
    private SerializedProperty propOnKeycodeStateChanged;
    private SerializedProperty propOnRoomStateChanged;

    private List<KeycodeRoomMapping> refKeycodeRoomMappings;
    
    private ReorderableListControl mappingsListControl;
    private IReorderableListAdaptor mappingsListAdaptor;
    
    private GUIStyle styleBox;


    /* ---------- Methods ---------- */

    private void OnEnable() {

        // refObject = ((KeycodesList) target);
        
        propKeycodeRoomMappings = serializedObject.FindProperty(PropKeycodeRoomMappingsInfo);
        propOnAnyListElementStateChanged = serializedObject.FindProperty(
            PropOnAnyListElementStateChangedInfo);
        propOnKeycodeStateChanged = serializedObject.FindProperty(PropOnKeycodeStateChangedInfo);
        propOnRoomStateChanged = serializedObject.FindProperty(PropOnRoomStateChangedInfo);

        refKeycodeRoomMappings =
            PropertyUtils.GetActualObjectOfAs<List<KeycodeRoomMapping>>(propKeycodeRoomMappings);

        mappingsListControl = new ReorderableListControl(ReorderableListFlags.ShowSizeField);
        mappingsListAdaptor = new DynamicHeightListAdaptor(
            propKeycodeRoomMappings, KeycodeRoomMapping.FieldNamePropDrawingHeight);
    }
    
    
    /* ---------- Overrides ---------- */
    
    public override void OnInspectorGUI() {
    
        /* ---------- Section: Initialization ---------- */
        
        serializedObject.Update();
        
        /* Cannot be moved into "OnEnable", or anywhere else. That throws an exception. */
        styleBox = new GUIStyle("Box");
        
       
        /* ---------- Section: Message(s) ---------- */
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("List Status", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        SortedDictionary<int, IList<int>> duplicates = 
            CollectionsUtils.FindAllDuplicatesIn(refKeycodeRoomMappings);

        if (duplicates.Count != 0) {

            StringBuilder messageDuplicates = new StringBuilder();
            
            messageDuplicates.Append(MessageDuplicateMappings);

            foreach (var element in duplicates) {

                messageDuplicates.Append("\n- Mapping " + element.Key + " has duplicates: ");

                foreach (int foundElement in element.Value) {

                    messageDuplicates.Append(foundElement + ", ");
                }

                messageDuplicates.Remove(messageDuplicates.Length - 2, 2);
            }
            
            EditorGUILayout.HelpBox(messageDuplicates.ToString(), MessageType.Error);
        }
        else {
            
            EditorGUILayout.HelpBox(MessageNoDuplicatesInList, MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        
        /* ---------- Section: List ---------- */
        
        ReorderableListGUI.Title(PropKeycodeRoomMappingsInfo.EditorText);
        
        mappingsListControl.Draw(mappingsListAdaptor);

        // serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.Space();
        
        
        /* ---------- Section: Events ---------- */
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(propOnAnyListElementStateChanged);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(propOnKeycodeStateChanged);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(propOnRoomStateChanged);
        
        EditorGUILayout.EndVertical();

        
        /* ---------- Section: Finalization ---------- */
        
        serializedObject.ApplyModifiedProperties();
    }
}

}