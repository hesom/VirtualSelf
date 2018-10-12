using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace VirtualSelf.GameSystems {


/// <summary>
/// TODO: Fill out this class description: CodesRoomsUi
/// </summary>
public sealed class CodesRoomsUi : MonoBehaviour {

    /* ---------- Variables & Properties ---------- */

    public GameObject UiScreen;
    
    public Canvas UiCanvas;

    public VerticalLayoutGroup CanvasLayoutGroup;

    public GridLayoutGroup PanelsGroup;

    public KeycodesList KeycodesRoomsList;

    public CodeRoomPanel PanelPrefab;

    public int PaddingUiSides = 30;

    public float PaddingPanelsVertical = 15.0f;

    public float PaddingPanelsHorizontal = 15.0f;

    public string UnknownRoomName = "????";

    private readonly List<CodeRoomPanel> panels = new List<CodeRoomPanel>();
    
    private float uiWidth;
    private float uiHeight;

    private float singlePanelWidth;
    private float singlePanelHeight;



    /* ---------- Methods ---------- */

    private void Start() {
       
        uiWidth = UiCanvas.gameObject.GetComponent<RectTransform>().rect.width;
        uiHeight = UiCanvas.gameObject.GetComponent<RectTransform>().rect.height;

        singlePanelWidth = PanelPrefab.ParentTransform.rect.width;
        singlePanelHeight = PanelPrefab.ParentTransform.rect.height;
        
        CanvasLayoutGroup.padding.top = PaddingUiSides;
        CanvasLayoutGroup.padding.left = PaddingUiSides;
        CanvasLayoutGroup.padding.right = PaddingUiSides;
        CanvasLayoutGroup.padding.bottom = PaddingUiSides;
        
        PanelsGroup.cellSize = new Vector2(singlePanelWidth, singlePanelHeight);
        PanelsGroup.spacing = new Vector2(PaddingPanelsHorizontal, PaddingPanelsVertical);
        
        CreateCodeRoomPanels();
        
        KeycodesRoomsList.OnAnyListElementStateChanged.AddListener(OnKeycodesListElementChanged);
    }

    private void OnDestroy() {
        
        KeycodesRoomsList.OnAnyListElementStateChanged.RemoveListener(OnKeycodesListElementChanged);
    }

    private void CreateCodeRoomPanels() {
        
        IList<KeycodeRoomMapping> codeRooms = KeycodesRoomsList.ValidMappings;

        int panelsCount = codeRooms.Count;

        int verticalFitCount = 
            ((int) Math.Floor(uiHeight / (singlePanelHeight + PaddingPanelsVertical)));
        
        int horizontalFitCount =
            ((int) Math.Floor(uiWidth / (singlePanelWidth + PaddingPanelsHorizontal)));

        int columns = ((int) Math.Ceiling(((float)panelsCount / (float)verticalFitCount)));

        if (horizontalFitCount < columns) {
            
            throw new SystemException(
            
                "Cannot create the User UI, because the amount of code room panels that would be " +
                "required exceeds the available UI (canvas) space, with the current layouting " +
                "settings."
            );
        }

        Transform panelsGroup = PanelsGroup.transform;
        
        foreach (KeycodeRoomMapping mapping in codeRooms) {

            Keycode keycodeRef = mapping.KeycodeReference;
            Room roomRef = mapping.RoomReference;
            
            CodeRoomPanel panel = Instantiate(PanelPrefab, panelsGroup);

            if (keycodeRef.IsDiscovered) {

                panel.CodeText.text = keycodeRef.CodeString;
                
                if (roomRef.HasBeenVisited) {

                    panel.RoomText.text = roomRef.RoomName;
                }
                else {

                    panel.RoomText.text = UnknownRoomName;
                }
            }
            else {

                panel.CodeText.text = "    ";
                panel.RoomText.text = "";
            }
            
            panels.Add(panel);
        }
    }


    /* ---------- Event Methods ---------- */

    public void OnKeycodesListElementChanged(int elementIndex) {
        
        
    }
    


    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}