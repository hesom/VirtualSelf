using RoboRyanTron.SceneReference;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VirtualSelf.Utility.Editor;


namespace VirtualSelf.GameSystems.Editor {


/// <summary>
/// TODO: Fill out this class description: KeycodeSceneMappingEditor
/// </summary>
[CustomPropertyDrawer(typeof(KeycodeSceneMapping))]
public sealed class KeycodeSceneMappingDrawer : PropertyDrawer {

    /* ---------- Variables & Properties ---------- */

    public const float DrawerHeight = 70.0f;
    
    private static readonly SerializedPropertyInfo PropKeycodeInfo =
        new SerializedPropertyInfo(nameof(KeycodeSceneMapping.keycode), "Keycode");
    
    private static readonly SerializedPropertyInfo PropSceneInfo =
        new SerializedPropertyInfo(nameof(KeycodeSceneMapping.scene), "Scene");
    
    private static readonly RowLayouter LayouterDrawer = new RowLayouter {
        HorizontalAlignment = HorizontalAlignment.Left
    };
    
    private FixedHeightDrawerLayouter layouterProp;

    private LabelledField<ObjectField<Keycode>> compKeycodeField;
    private ObjectField<Keycode> compKeycode;

    private SerializedProperty PropKeycode;
    private SerializedProperty PropScene;



    /* ---------- Methods ---------- */



    
    
    /* ---------- Overrides ---------- */
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

        return (DrawerHeight);
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    
        if (position.width <= 0.0f) { return; }
        
    
        /* ---------- Section: Initialization ---------- */
        
        PropKeycode = property.FindPropertyRelative(PropKeycodeInfo);
        PropScene = property.FindPropertyRelative(PropSceneInfo);
        
        layouterProp = new FixedHeightDrawerLayouter(position, DrawerHeight);
        int compFieldsWidth = 160;
        
        compKeycode = new ObjectField<Keycode>(compFieldsWidth, PropKeycode);
        compKeycodeField = new LabelledField<ObjectField<Keycode>>(
            compFieldsWidth, new Label(PropKeycodeInfo.EditorText), compKeycode);
               
        
        /* ---------- YOUR EDITOR CODE HERE ---------- */       
        
        compKeycodeField.Draw(
            layouterProp.GetXPosition(), layouterProp.GetAbsoluteCurrentYPosition());
              
        // TODO: Replace with "LabelledComponent"...
        
        EditorGUI.PropertyField(
            new Rect(layouterProp.GetXPosition() + compFieldsWidth + 30,
                     layouterProp.GetCurrentYPosition() + 12,
                      compFieldsWidth, EditorGUIUtility.singleLineHeight),
            PropScene, new GUIContent(""));
        
        layouterProp.AddHeightFromComponent(compKeycodeField);
        
        
        /* ---------- Section: Finalization ---------- */
        

    }
}

}