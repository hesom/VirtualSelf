using System;
using UnityEditor;
using UnityEngine;
using VirtualSelf.Utility.Editor;


namespace VirtualSelf.GameSystems.Editor {


/// <summary>
/// TODO: Fill out this class description: KeycodeSceneMappingEditor
/// </summary>
[CustomPropertyDrawer(typeof(KeycodeRoomMapping))]
public sealed class KeycodeRoomMappingDrawer : PropertyDrawer {

    /* ---------- Variables & Properties ---------- */

    private const string MessageFillFields =
        "Please fill both the \"Keycode\" and \"Room\" fields. Otherwise, this mapping cannot be " +
        "used in the game.";

    private const string MessageRoomHasNoScene =
        "This room does not yet have a scene file associated with it. Add a scene file to the " +
        "room asset, or this mapping cannot be used in the game.";
    
    private static readonly SerializedPropertyInfo PropKeycodeReferenceInfo =
        new SerializedPropertyInfo(KeycodeRoomMapping.FieldNameKeycodeReference, "Keycode");
    
    private static readonly SerializedPropertyInfo PropRoomReferenceInfo =
        new SerializedPropertyInfo(KeycodeRoomMapping.FieldNameRoomReference, "Room");
    
    private static readonly SerializedPropertyInfo PropDrawingHeightInfo =
        new SerializedPropertyInfo(KeycodeRoomMapping.FieldNamePropDrawingHeight, "");
    
    private SerializedProperty propKeycodeReference;
    private SerializedProperty propRoomReference;
    private SerializedProperty propDrawingHeight;

    private KeycodeRoomMapping refObject;
    private Keycode refKeycodeReference;
    private Room refRoomReference;
    
    private VariableHeightDrawerLayouter layouterProp;

    private LabelledField<ObjectField<Keycode>> compKeycodeField;
    private ObjectField<Keycode> compKeycode;
    
    private LabelledField<ObjectField<Room>> compRoomField;
    private ObjectField<Room> compRoom;

    private HelpBox compMessageFillFields;
    private HelpBox compMessageRoomHasNoScene;
  
    
    /* ---------- Overrides ---------- */
       
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    
        if (position.width <= 0.0f) { return; }
        
    
        /* ---------- Section: Initialization ---------- */
        
        propKeycodeReference = property.FindPropertyRelative(PropKeycodeReferenceInfo);
        propRoomReference = property.FindPropertyRelative(PropRoomReferenceInfo);
        propDrawingHeight = property.FindPropertyRelative(PropDrawingHeightInfo);

        refObject = PropertyUtils.GetActualObjectOfAs<KeycodeRoomMapping>(property);
        refKeycodeReference = refObject.KeycodeReference;
        refRoomReference = refObject.RoomReference;
        
        layouterProp = new VariableHeightDrawerLayouter(position, propDrawingHeight) {

            Margins = new Margins(0.0f, 8.0f, 3.0f, 3.0f),
            PaddingRows = 10.0f
        };
        
        int compFieldsPadding = 30;
        int compFieldsWidth = 
                ((int) (Math.Round((layouterProp.GetWidth() - compFieldsPadding) / 2.0f)));
        
        compKeycode = new ObjectField<Keycode>(compFieldsWidth, propKeycodeReference);
        compKeycodeField = new LabelledField<ObjectField<Keycode>>(
            compFieldsWidth, new Label(PropKeycodeReferenceInfo.EditorText), compKeycode);
        
        compRoom = new ObjectField<Room>(compFieldsWidth, propRoomReference);
        compRoomField = new LabelledField<ObjectField<Room>>(
            compFieldsWidth, new Label(PropRoomReferenceInfo.EditorText), compRoom);
               
        
        /* ---------- Section: Fields ---------- */

        compKeycodeField.Draw(layouterProp.GetXPosition(), layouterProp.GetCurrentYPosition());
        compRoomField.Draw((layouterProp.GetXPosition() + compFieldsWidth + compFieldsPadding),
            layouterProp.GetCurrentYPosition());
        
        layouterProp.AddHeightFromComponent(compKeycodeField);

        
        /* ---------- Section: Messages ---------- */
        
        if ((refKeycodeReference == null) || (refRoomReference == null)) {
            
            compMessageFillFields = new HelpBox(
                layouterProp.GetWidth(), MessageFillFields, MessageType.Error);
            
            compMessageFillFields.Draw(
                layouterProp.GetXPosition(), layouterProp.GetCurrentYPosition());
            
            layouterProp.AddHeightFromComponent(compMessageFillFields);
        }

        if (refRoomReference != null && refRoomReference.HasSceneAttached() == false) {
            
            compMessageRoomHasNoScene = new HelpBox(
                layouterProp.GetWidth(), MessageRoomHasNoScene, MessageType.Warning);
            
            compMessageRoomHasNoScene.Draw(
                layouterProp.GetXPosition(), layouterProp.GetCurrentYPosition());
            
            layouterProp.AddHeightFromComponent(compMessageRoomHasNoScene);          
        }
        
        
        /* ---------- Section: Finalization ---------- */
        

    }
}

}