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
        new SerializedPropertyInfo(Room.fieldNameRoomName, "Room Name");

    private static readonly SerializedPropertyInfo PropDescriptionInfo = 
        new SerializedPropertyInfo(Room.fieldNameDescription, "Room Description");
    
    private static readonly SerializedPropertyInfo PropSceneInfo = 
        new SerializedPropertyInfo(Room.fieldNameScene, "Scene");   
    
    private static readonly SerializedPropertyInfo PropIsDiscoveredInfo =
        new SerializedPropertyInfo(nameof(Room.IsDiscovered), "Is discovered");

    private SerializedProperty propRoomName;
    private SerializedProperty propDescription;
    private SerializedProperty propScene;
    private SerializedProperty propIsDiscovered;

    private SceneReference refScene;
    
    private GUIStyle styleBox;


    /* ---------- Methods ---------- */

    private void OnEnable() {

        propRoomName = serializedObject.FindProperty(PropRoomNameInfo);
        propDescription = serializedObject.FindProperty(PropDescriptionInfo);
        propScene = serializedObject.FindProperty(PropSceneInfo);
        propIsDiscovered = serializedObject.FindProperty(PropIsDiscoveredInfo);

        refScene = PropertyUtils.GetActualObjectOfAs<SceneReference>(propScene);
    }


    /* ---------- Overrides ---------- */

    public override void OnInspectorGUI() {
        
        /* ---------- Section: Initialization ---------- */
        
        serializedObject.Update();
                
        /* Cannot be moved into "OnEnable", or anywhere else. That throws an exception. */
        styleBox = new GUIStyle("Box");
        
        refScene = PropertyUtils.GetActualObjectOfAs<SceneReference>(propScene);

        
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

        if (refScene == null) {
            
            EditorGUILayout.HelpBox(
                "No scene object has been added to this room asset yet. It cannot be used in the " +
                "keycode-scene mappings list until this has been done.",
                MessageType.Error
            );
        }
                
        
        /* ---------- Section: Finalization ---------- */
        
        serializedObject.ApplyModifiedProperties();
    }
}

}