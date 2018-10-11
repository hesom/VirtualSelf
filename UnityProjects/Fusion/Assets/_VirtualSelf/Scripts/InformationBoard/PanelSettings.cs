using UnityEngine;
using UnityEngine.UI;


namespace VirtualSelf.GameSystems {


/// <summary>
/// This is a tiny class only used by the "MessagePanel" Prefab.<br/>
/// This class marks the prefab as a "Message Panel", to be used within the Information Board, and
/// specifically the <see cref="InformationBoardController"/> class.<br/>
/// <br/>
/// The class pretty much only contains some public references, to be set within the Unity
/// Inspector. The references are to scripts somewhere within the hierarchy of the Prefab (as I do
/// not want to use methods like <see cref="GameObject.GetComponent{T}"/> for this).
/// </summary>
public sealed class PanelSettings : MonoBehaviour {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// A reference to the GameObject within the prefab that has the "Scroll Rect" component. This
    /// should normally be the "Scroll View" GameObject, which holds the rest of the prefab's
    /// GameObjects.
    /// </summary>
    public ScrollRect TextScrollRect;

    /// <summary>
    /// A reference to the "Scrollbar" of the "Scroll View" component/GameObject. This is normally
    /// on a separate GameObject, within the hierarchy of the "Scroll View" GameObject.
    /// </summary>
    public Scrollbar ScrollRectScrollbar;
    
    /// <summary>
    /// A reference to the "Rect Transform" component of the "Content" GameObject, which should
    /// normally be holding the GameObject which has the actual text for the panel. 
    /// </summary>
    public RectTransform TextContentTransform;
    
    /// <summary>
    /// A reference to the "Text" GameObject, which holds the actual text for the panel. This should
    /// normally be the last (deepest) element in the hierarchy of the prefab.
    /// </summary>
    public Text TextObject;
}

}