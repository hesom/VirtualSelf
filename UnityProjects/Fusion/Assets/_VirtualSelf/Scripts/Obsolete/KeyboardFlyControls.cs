using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Controls {

    public KeyCode lockMouseLook = KeyCode.Escape;
    public KeyCode ascend = KeyCode.Q;
    public KeyCode descend = KeyCode.E;
    public KeyCode moveFaster = KeyCode.LeftShift;
    public KeyCode moveSlower = KeyCode.Space;
}

[Serializable]
public class Modifiers {

    [Range(0.01f, 1000.0f)]
    public float cameraSensitivity = 90;

    [Range(0.01f, 100.0f)]
    public float normalClimbSpeed = 1.5f;
    [Range(0.01f, 100.0f)]
    public float normalMoveSpeed = 5;

    [Range(0.01f, 1.0f)]
    public float slowMoveFactor = 0.20f;
    [Range(1.0f, 100.0f)]
    public float fastMoveFactor = 5;

    /* Denotes whether to transform the camera with the local or the global up vector during
     * climbing (both ascending and descending). If the former, the camera will climb "up" and "down"
     * relative to where the camera is currently looking, instead of using the world axes. */
    public bool useLocalClimbTransform = false;
}

/*
 * This class models a set of configurable controls for a classical "WASD-style", first-person,
 * flying (not affected by gravity or physics) controller (where a "controller" can be in theory any
 * object), combined with a free "mouse look".
 * In more technical terms, the keyboard keys this control set uses transform the position of the
 * controller, and the input axes transform its rotation. The position transformation is not
 * restricted in any way, the rotation is clamped to [-90, 90] on the Y-axis, so that no rolling on
 * that axis is possible.
 *
 * The script is supposed to be attached to something containing a camera, so that the user can see
 * and move it through the scene.
 *
 * Almost all of the inputs for this script can be changed in the Unity inspector.
 *
 * This is a modified version of another script. The original version of it was taken from here:
 * https://wiki.unity3d.com/index.php/FlyCam_Extended
 *
 * Compared to the original script, a lot of modifications have been made. These include:
 * - Changes to the code style and layout.
 * - Additional functionality.
 * - Making most of the variables configurable.
 * - Making most of the variables public to be configurable in the Unity Inspector.
 */
public class KeyboardFlyControls : MonoBehaviour {

    public Modifiers modifiers;

    public Controls controls;

    private float rotationX;
    private float rotationY;

    private Vector3 globalUp = new Vector3(0.0f, 1.0f, 0.0f);

    /* Only relevant for Play Mode inside Unity.
     * Denotes whether the mouse look of the camera is currently locked or not. While it is locked,
     * the camera cannot look around and the cursor will be visible and be able to leave the Play
     * Window. */
    private bool isMouseLocked = false;

    void Start() {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {

        /* When using the Play Mode in unity, make the camera stop following the mouse when this key
         * is pressed. This is so that you can click on things in the editor while testing. */
        bool isPlaying = false;
#if UNITY_EDITOR
        isPlaying = EditorApplication.isPlaying;
#endif

        if (isPlaying && (Input.GetKeyDown(controls.lockMouseLook))) {

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isMouseLocked = true;
        }
        /* Clicking into the window with the left mouse button will lock the mouse cursor again. */
        if (isPlaying && (Input.GetMouseButtonUp(0))) {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isMouseLocked = false;
        }

        /* ----- Mouse Look ------ */

        if (isMouseLocked == false) {

            rotationX += Input.GetAxis("Mouse X") * modifiers.cameraSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * modifiers.cameraSensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
        }

        /* ----- Movement ------ */

        float movementSpeed = modifiers.normalMoveSpeed;

        if (Input.GetKey(controls.moveFaster)) { movementSpeed *= modifiers.fastMoveFactor; }
        if (Input.GetKey(controls.moveSlower)) { movementSpeed *= modifiers.slowMoveFactor; }

        transform.position += (transform.forward * movementSpeed *
                              Input.GetAxis("Vertical") * Time.deltaTime);

        transform.position += (transform.right * movementSpeed *
                              Input.GetAxis("Horizontal") * Time.deltaTime);


        float climbingSpeed = modifiers.normalClimbSpeed;
        Vector3 upVector = modifiers.useLocalClimbTransform ? transform.up : globalUp;

        if (Input.GetKey(controls.moveFaster)) { climbingSpeed *= modifiers.fastMoveFactor; }
        if (Input.GetKey(controls.moveSlower)) { climbingSpeed *= modifiers.slowMoveFactor; }

        if (Input.GetKey(controls.descend)) {

            transform.position -= (upVector * climbingSpeed * Time.deltaTime);
        }
        if (Input.GetKey(controls.ascend)) {

            transform.position += (upVector * climbingSpeed * Time.deltaTime);
        }
    }
}

