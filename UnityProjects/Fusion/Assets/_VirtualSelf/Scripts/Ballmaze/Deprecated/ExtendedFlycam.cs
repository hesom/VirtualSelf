using UnityEngine;

namespace VirtualSelf.Ballmaze
{
    /*
     * This script is taken from:
     * https://wiki.unity3d.com/index.php/FlyCam_Extended
     *
     * Some modifications were made by me:
     * - Changed code style somewhat, for example split too long lines.
     * - Changed "Screen.lockCursor" to "Cursor.lockState" as per Unity recommendation.
     *   - Also added Cursor.visible to make the cursor visible or invisible with respect to this.
     * - Switched "Q" and "E" key functionality.
     * - Switched "End" key functionality to "L" key.
     * - Increased default climbing speed.
     * - Changed the way the "L" key functionality works. Previously, it just toggled the cursor being
     *   locked at the screen, but the camera would still move around when the cursor moves. I found
     *   that functionality to be pretty pointless like this.
     *   Now, the "L" key will toggle a complete lock of the camera, meaning that it won't move at all
     *   until the key is pressed again. This makes it very useful for "getting into position" and then
     *   locking the camera to make and observe changes in Unity while the game is running.
     */
    public class ExtendedFlycam : MonoBehaviour {

        /*
	EXTENDED FLYCAM
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.

	LICENSE
		Free as in speech, and free as in beer.


	*/

        public float cameraSensitivity = 90;
        public float climbSpeed = 1.5f;
        public float normalMoveSpeed = 5;
        public float slowMoveFactor = 0.20f;
        public float fastMoveFactor = 5;

        public KeyCode lock_ = KeyCode.L;
        public KeyCode up = KeyCode.Q;
        public KeyCode down = KeyCode.E;
        public KeyCode fast = KeyCode.LeftShift;
        public KeyCode slow = KeyCode.Space;

        private float rotationX;
        private float rotationY;

        /* Denotes whether the camera is currently locked or not. While it is locked, it will not move
     * at all, until the locking is released again. */
        private bool isLocked = false;
//        private bool isMouseLocked;

        void Start() {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update() {

            if (Input.GetKeyDown(lock_)) {

                if (Cursor.lockState == CursorLockMode.Locked) {

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    isLocked = true;
                }
                else if (Cursor.lockState == CursorLockMode.None) {

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    isLocked = false;
                }
            }

            // when using the play mode in unity, make the camera stop following the mouse when escape is pressed (but keep key inputs)
            // this is so that you can click on things in the editor while testing
            /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (EditorApplication.isPlaying)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isMouseLocked = true;
                //Debug.Log(Cursor.lockState + " " + Cursor.visible);
                
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (EditorApplication.isPlaying)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isMouseLocked = false;
                //Debug.Log(Cursor.lockState + " " + Cursor.visible);

            }
        }*/

            if (isLocked == true) { return; }

            /* All the actual update code for the camera is only being run if the camera is not
         * currently locked. */

            rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

//            if (!isMouseLocked)
//            {
//                transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
//                transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
//            }

            if (Input.GetKey(fast)) {

                transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) *
                                      Input.GetAxis("Vertical") * Time.deltaTime;

                transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) *
                                      Input.GetAxis("Horizontal") * Time.deltaTime;
            }
            else if (Input.GetKey(slow)) {

                transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) *
                                      Input.GetAxis("Vertical") * Time.deltaTime;

                transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) *
                                      Input.GetAxis("Horizontal") * Time.deltaTime;
            }
            else {

                transform.position += transform.forward * normalMoveSpeed *
                                      Input.GetAxis("Vertical") * Time.deltaTime;

                transform.position += transform.right * normalMoveSpeed *
                                      Input.GetAxis("Horizontal") * Time.deltaTime;
            }

            if (Input.GetKey(down)) {

                transform.position -= transform.up * climbSpeed * Time.deltaTime;
            }
        
            if (Input.GetKey(up)) {

                transform.position += transform.up * climbSpeed * Time.deltaTime;
            }
        }
    }
}