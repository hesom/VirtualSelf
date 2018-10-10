using System;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.UI;


namespace VirtualSelf.Utility {


/// <summary>
/// TODO: Fill out this class description: ScrollRectTouchscreenControls
/// </summary>
[RequireComponent(typeof(BoxCollider), typeof(InteractionBehaviour))]
public sealed class ScrollRectTouchscreenControls : MonoBehaviour {

    /* ---------- Variables & Properties ---------- */
        
    public ScrollRect ScrollRect;
    
    public InteractionHand LeftHand;
    public InteractionHand RightHand;

    private InteractionBehaviour interBehavior;

    private bool isCurrentlyContact;

    private Vector3 screenCenter;
    private float screenWidth;
    private float screenHeight;

    private float lastFrameYPos;
    private float currentFrameYPos;

    // Only one hand can interact with the screen at the same time.
    private InteractionHand contactingHand;
    
    

    /* ---------- Methods ---------- */

    private void Start() {

        interBehavior = GetComponent<InteractionBehaviour>();

        screenCenter = gameObject.transform.position;
        screenWidth = Math.Max(gameObject.transform.lossyScale.x, gameObject.transform.lossyScale.z);
        screenHeight = gameObject.transform.lossyScale.y;
        
        
        
        Debug.Log("screenCenter: " + screenCenter);
        Debug.Log("screenWidth: " + screenWidth);
        Debug.Log("screenHeight: " + screenHeight);
    }

    private void OnCollisionEnter(Collision other) {
        
        // other.
    }

    private void Update() {

        if (contactingHand == null) {
            
            bool isLeftHand = false;
            bool isRightHand = false;

            if (LeftHand.contactingObjects.Contains(interBehavior)) {

                isLeftHand = true;
            }
            if (RightHand.contactingObjects.Contains(interBehavior)) {

                isRightHand = true;
            }

            if (isLeftHand && isRightHand) {
                
                throw new SystemException("Both hands are contacting at the exact same time??");
            }
            else if (isLeftHand) { contactingHand = LeftHand; }
            else if (isRightHand) { contactingHand = RightHand; }
            else {

                return;
            }
        }

        if (isCurrentlyContact == false) {

            OnContactBegin();
        }
        else {

            if (contactingHand.isTracked == false) {
                
                OnContactEnd();
                return;
            }

            if (contactingHand.contactingObjects.Contains(interBehavior) == false) {
                
                OnContactEnd();
                return;
            }
            else {
                
                OnContactStay();
            }
        }
    }

    private void OnContactBegin() {
        
        Debug.Log("Contact begins.");
        Debug.Log("Contacting hand is: " + (contactingHand.isLeft ? "left" : "right"));

        isCurrentlyContact = true;

        lastFrameYPos = contactingHand.position.y;
        currentFrameYPos = 0.0f;
    }

    private void OnContactStay() {

        currentFrameYPos = contactingHand.position.y;

        float absoluteDiff = Math.Abs(currentFrameYPos - lastFrameYPos);

        float normalizedDiff = (absoluteDiff / screenHeight);
        
        // Debug.Log("The normalized difference in this frame is: " + normalizedDiff);

        if (currentFrameYPos > lastFrameYPos) {

            normalizedDiff = (normalizedDiff * (-1));
        }

        if (normalizedDiff > 1.0f) {
            
            throw new SystemException("Normalized diff is greater than 1.0...");
        }
        
        ScrollRect.verticalNormalizedPosition += normalizedDiff;

        lastFrameYPos = currentFrameYPos;
    }

    private void OnContactEnd() {
        
        Debug.Log("Contact ends.");

        isCurrentlyContact = false;
        contactingHand = null;
        
        lastFrameYPos = 0.0f;
        currentFrameYPos = 0.0f;
    }
    

    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}