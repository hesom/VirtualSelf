using RoboRyanTron.SceneReference;
using Rotorz.ReorderableList;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VirtualSelf.Utility.Editor;


namespace VirtualSelf.GameSystems.Editor {


/// <summary>
/// TODO: Fill out this class description: KeycodesListEditor
/// </summary>
[CustomEditor(typeof(KeycodesList))]
public sealed class KeycodesListEditor : UnityEditor.Editor {

    /* ---------- Variables & Properties ---------- */

    private static readonly SerializedPropertyInfo propKeycodeSceneMappingsInfo =
        new SerializedPropertyInfo(
            KeycodesList.FieldNameKeycodeSceneMappings, "Keycode - Scene List");
    
    private KeycodesList refObject;

    private SerializedProperty propKeycodeSceneMappings;


    /* ---------- Methods ---------- */

    private void OnEnable() {

        refObject = ((KeycodesList) target);

    }
    
    
    /* ---------- Overrides ---------- */
    
    public override void OnInspectorGUI() {
    
        /* ---------- Section: Initialization ---------- */
        
        serializedObject.Update();

        propKeycodeSceneMappings = serializedObject.FindProperty(propKeycodeSceneMappingsInfo);
        
        
        /* ---------- YOUR EDITOR CODE HERE ---------- */       
        
        ReorderableListGUI.Title(propKeycodeSceneMappingsInfo.EditorText);

        ReorderableListGUI.ListField(propKeycodeSceneMappings, ReorderableListFlags.ShowSizeField);
        
        
        
        /* ---------- Section: Finalization ---------- */
        
        serializedObject.ApplyModifiedProperties();
    }
}

}