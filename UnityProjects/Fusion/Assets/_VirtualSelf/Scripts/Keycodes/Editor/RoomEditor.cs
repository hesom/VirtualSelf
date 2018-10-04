using RoboRyanTron.SceneReference;
using UnityEditor;
using UnityEngine;
using VirtualSelf.Utility.Editor;


namespace VirtualSelf.GameSystems.Editor {

/// <summary>
/// TODO: Fill out this class description: RoomEditor
/// </summary>
[CustomEditor(typeof(Room))]
public sealed class RoomEditor : UnityEditor.Editor {

    /* ---------- Variables & Properties ---------- */
    
    private static readonly SerializedPropertyInfo PropRoomNameInfo = 
        new SerializedPropertyInfo(Room.FieldNameRoomName, "Room Name");

    private static readonly SerializedPropertyInfo PropDescriptionInfo = 
        new SerializedPropertyInfo(Room.FieldNameDescription, "Room Description");
    
    private static readonly SerializedPropertyInfo PropSceneInfo = 
        new SerializedPropertyInfo(Room.FieldNameScene, "Scene");   
    
    private static readonly SerializedPropertyInfo PropIsDiscoveredInfo =
        new SerializedPropertyInfo(Room.FieldNameIsDiscovered, "Is discovered");

    private Room refObject;

    private SerializedProperty propRoomName;
    private SerializedProperty propDescription;
    private SerializedProperty propScene;
    private SerializedProperty propIsDiscovered;
    
    private GUIStyle styleBox;


    /* ---------- Methods ---------- */

    private void OnEnable() {
        
        refObject = ((Room) target);

        propRoomName = serializedObject.FindProperty(PropRoomNameInfo);
        propDescription = serializedObject.FindProperty(PropDescriptionInfo);
        propScene = serializedObject.FindProperty(PropSceneInfo);
        propIsDiscovered = serializedObject.FindProperty(PropIsDiscoveredInfo);
    }


    /* ---------- Overrides ---------- */

    public override void OnInspectorGUI() {
        
        /* ---------- Section: Initialization ---------- */
        
        serializedObject.Update();
                
        /* Cannot be moved into "OnEnable", or anywhere else. That throws an exception. */
        styleBox = new GUIStyle("Box");
        
        
        /* ---------- Section: Room Attributes ---------- */
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Room Attributes", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        propRoomName.stringValue = EditorGUILayout.TextField(
                PropRoomNameInfo.EditorText, propRoomName.stringValue);
       
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField(PropDescriptionInfo.EditorText);
        
        propDescription.stringValue = EditorGUILayout.TextArea(
                propDescription.stringValue, GUILayout.ExpandHeight(true));
                
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();

        
        /* ---------- Section: Room Settings ---------- */
        
        EditorGUILayout.BeginVertical(styleBox);
        
        EditorGUILayout.LabelField("Ingame Settings/Data", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
                   
        EditorGUILayout.PropertyField(propScene, new GUIContent(PropSceneInfo.EditorText));
        
        EditorGUILayout.Space();

        propIsDiscovered.boolValue =
            EditorGUILayout.ToggleLeft(PropIsDiscoveredInfo.EditorText, propIsDiscovered.boolValue);
        
        EditorGUILayout.EndVertical();
            
        EditorGUILayout.Space();
        
        
        /* ---------- Section: Error Messages, etc. ---------- */

        if (refObject.HasSceneAttached() == false) {
            
            EditorGUILayout.HelpBox(
                "No scene object has been added to this room asset yet. It cannot be used in the " +
                "keycode-scene mappings list until this has been done.",
                MessageType.Error
            );
        }

        if (propRoomName.stringValue.Trim() == "") {
            
            EditorGUILayout.HelpBox(
                "This room asset has not been given a name yet. Please add a name to it, to be " +
                "displayed ingame. (Otherwise, the name of the scene file connected to this " +
                "asset will be used.)",
                MessageType.Warning
            );
        }
                
        
        /* ---------- Section: Finalization ---------- */
        
        serializedObject.ApplyModifiedProperties();
    }
}

}