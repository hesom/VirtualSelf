using System;
using UnityEngine;


namespace VirtualSelf.Utility {


/// <summary>
/// This class models a set of configurable controls for a classical "WASD-style", first-person,
/// flying (not affected by gravity or physics) controller (where a "controller" can be in theory
/// any object), combined with a free "mouse look".<br/>
/// In more technical terms, the keyboard keys this control set uses transform the position of the
/// controller, and the input axes transform its rotation. The position transformation is not
/// restricted in any way, the rotation is clamped to [-90, 90] on the Y-axis, so that no rolling on
/// that axis is possible.<br/>
/// <br/>
/// The script is supposed to be attached to something containing a <see cref="Camera"/>, so that
/// the user can see and move it through the scene.<br/>
/// Almost all of the inputs for this script can be changed in the Unity inspector.<br/>
/// <br/>
/// This is a modified version of another script. The original version of it was taken from here:
/// https://wiki.unity3d.com/index.php/FlyCam_Extended <br/>
/// Compared to the original script, a lot of modifications have been made. These include (in no
/// particular order):
/// <list type="bullet">
/// <item><description>
/// Changes to the code style and layout.
/// </description></item>
/// <item><description>
/// Additional functionality.
/// </description></item>
/// <item><description>
/// Making most of the variables configurable.
/// </description></item>
/// <item><description>
/// Making most of the variables public to be configurable in the Unity Inspector.
/// </description></item>
/// <item><description>
/// Splitting up the control code into <see cref="Update"/> and <see cref="FixedUpdate"/>.
/// </description></item>
/// <item><description>
/// Adding more options, e.g. choosing whether to ascent/descend respective to the global world
/// axis or the local axis.
/// </description></item>
/// </list>
/// </summary>
public sealed class KeyboardFlyControls : MonoBehaviour {
    
    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// The possible modifier values for the behavior of this class.
    /// </summary>
    [SerializeField]
    private Modifiers modifiers;

    /// <summary>
    /// The possible modifier values for the behavior of this class.
    /// </summary>
    [SerializeField]
    private Controls controls;

    /// <summary>
    /// The current (local) horizontal rotation of the controller.
    /// </summary>
    private float rotationX;
    
    /// <summary>
    /// The current (local) vertical rotation of the controller.
    /// </summary>
    private float rotationY;

    /// <summary>
    /// Only relevant for "Play Mode", inside of Unity.<br/>
    /// Denotes whether the mouse look of the camera is currently locked or not. While it is locked,
    /// the camera cannot look around and the cursor will be visible and be able to leave the "Play
    /// Window".
    /// </summary>
    private bool isMouseLocked;

    
    /* ---------- Methods ---------- */

    void Start() {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update() {

        /* It seems that things related to detecting keypresses (GetKeyDown() and similar things,
         * continuous pressing seems to work fine) have to be done in Update() to work correctly.
         * If they are done in FixedUpdate(), they are often not registered and one has to "mash" a
         * key multiple times until it "gets through". */

        /* When using the Play Mode in unity, make the camera stop following the mouse when this key
         * is pressed. This is so that you can click on things in the editor while testing. */
        if ((Application.isPlaying == true) && (Input.GetKeyDown(controls.LockMouseLook))) {

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isMouseLocked = true;
        }
        /* Clicking into the window with the left mouse button will lock the mouse cursor again. */
        if ((Application.isPlaying == true) && (Input.GetMouseButtonUp(0))) {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isMouseLocked = false;
        }
    }

    void FixedUpdate() {

        /* The actual movement of the controller is done here in FixedUpdate(), because physics and
         * generally rigid-body related calculations are done here. If it were done in Update(),
         * this could lead to problems with anything related to those. */

        /* ----- Mouse Look ------ */

        if (isMouseLocked == false) {

            rotationX += Input.GetAxis("Mouse X") * modifiers.CameraSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * modifiers.CameraSensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
        }

        /* ----- Movement ------ */

        float movementSpeed = modifiers.NormalMoveSpeed;

        if (Input.GetKey(controls.MoveFaster)) { movementSpeed *= modifiers.FastMoveFactor; }
        if (Input.GetKey(controls.MoveSlower)) { movementSpeed *= modifiers.SlowMoveFactor; }

        transform.position += (transform.forward * movementSpeed *
                              Input.GetAxis("Vertical") * Time.deltaTime);

        transform.position += (transform.right * movementSpeed *
                              Input.GetAxis("Horizontal") * Time.deltaTime);


        float climbingSpeed = modifiers.NormalClimbSpeed;
        /* If this is false, make the global world axis (0, 1, 0) the vector to climb on. */
        Vector3 upVector = modifiers.UseLocalClimbTransform ? transform.up : Vector3.up;

        if (Input.GetKey(controls.MoveFaster)) { climbingSpeed *= modifiers.FastMoveFactor; }
        if (Input.GetKey(controls.MoveSlower)) { climbingSpeed *= modifiers.SlowMoveFactor; }

        if (Input.GetKey(controls.Descend)) {

            transform.position -= (upVector * climbingSpeed * Time.deltaTime);
        }
        if (Input.GetKey(controls.Ascend)) {

            transform.position += (upVector * climbingSpeed * Time.deltaTime);
        }
    }
    
    
    /* ---------- Inner Classes ---------- */
    
    /// <summary>
    /// A small, data-only class that contains modifier values which change the behavior of this
    /// class.<br/>
    /// They are mostly related to transformation (movement, rotation) speeds, and can be changed in
    /// the Unity Inspector.
    /// </summary>
    [Serializable]
    private sealed class Modifiers {

        /// <summary>
        /// Denotes how "sensitive" the camera reacts to mouse movements. The higher the value, the
        /// more the camera will move from a given mouse movement.
        /// </summary>
        [Range(0.01f, 1000.0f)]
        public float CameraSensitivity = 90;

        /// <summary>
        /// The "normal" speed of climbing (ascending/descending), normal meaning without any
        /// modifying factor coming into play.
        /// </summary>
        [Range(0.01f, 100.0f)]
        public float NormalClimbSpeed = 1.5f;
        
        /// <summary>
        /// The "normal" speed of moving around, normal meaning without any modifying factor coming
        /// into play.
        /// </summary>
        [Range(0.01f, 100.0f)]
        public float NormalMoveSpeed = 5;

        /// <summary>
        /// The factor for how much (any) movement shall be slowed down. This is applied when
        /// <see cref="Controls.MoveSlower"/> is pressed.
        /// </summary>
        [Range(0.01f, 1.0f)]
        public float SlowMoveFactor = 0.20f;
        
        /// <summary>
        /// The factor for how much (any) movement shall be slowed down. This is applied when
        /// <see cref="Controls.MoveFaster"/> is pressed.
        /// </summary>
        [Range(1.0f, 100.0f)]
        public float FastMoveFactor = 5;

        /// <summary>
        /// Denotes whether to transform the controller with the local or the global up vector
        /// during "climbing" (both ascending and descending).<br/>
        /// If the former, the controller will climb "up" and "down" relative to where its attached
        /// camera is currently looking - its own, local (rotation) axes. If the latter (which is
        /// the default), it will instead climb relative to the world axes, independently of the
        /// camera or of anything else.
        /// </summary>
        public bool UseLocalClimbTransform = false;
    }
    
    /// <summary>
    /// A small, data-only class that contains the controls for this class. Currently, these are
    /// just keyboard key codes that can be changed in the Unity Inspector. It is not possible to
    /// control the controller with anything else.
    /// </summary>
    [Serializable]
    private sealed class Controls {

        public KeyCode LockMouseLook = KeyCode.Escape;
        public KeyCode Ascend = KeyCode.Q;
        public KeyCode Descend = KeyCode.E;
        public KeyCode MoveFaster = KeyCode.LeftShift;
        public KeyCode MoveSlower = KeyCode.Space;
    }
}

}