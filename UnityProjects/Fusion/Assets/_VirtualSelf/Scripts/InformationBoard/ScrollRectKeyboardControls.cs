using System;
using RoboRyanTron.SearchableEnum;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualSelf.Utility {

    
/// <summary>
/// TODO: Fill out this class description: ScrollRectKeyboardControls
/// </summary>
public sealed class ScrollRectKeyboardControls : MonoBehaviour {

    /* ---------- Variables & Properties ---------- */

    public float HorizontalSpeed = 0.003f;
    public float VerticalSpeed = 0.003f;

    [SearchableEnum]
    public KeyCode HorizontalLeftKey = KeyCode.H;
    [SearchableEnum]
    public KeyCode HorizontalRightKey = KeyCode.K;
    [SearchableEnum]
    public KeyCode VerticalUpKey = KeyCode.U;
    [SearchableEnum]
    public KeyCode VerticalDownKey = KeyCode.J;
    
    private ScrollRect scrollRect;

    


    /* ---------- Constructors ---------- */






    /* ---------- Methods ---------- */

    private void Start() {

        scrollRect = GetComponent<ScrollRect>();
    }

    private void Update() {

        if (Input.GetKey(HorizontalLeftKey)) {

            scrollRect.horizontalNormalizedPosition =
                Math.Max(scrollRect.horizontalNormalizedPosition - HorizontalSpeed, 0.0f);
        }
        if (Input.GetKey(HorizontalRightKey)) {

            scrollRect.horizontalNormalizedPosition =
                Math.Min(scrollRect.horizontalNormalizedPosition + HorizontalSpeed, 1.0f);
        }
        if (Input.GetKey(VerticalUpKey)) {

            scrollRect.verticalNormalizedPosition =
                Math.Min(scrollRect.verticalNormalizedPosition + VerticalSpeed, 1.0f);
        }
        if (Input.GetKey(VerticalDownKey)) {

            scrollRect.verticalNormalizedPosition =
                Math.Max(scrollRect.verticalNormalizedPosition - VerticalSpeed, 0.0f);
        }
    }


    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}