using UnityEngine;

public class StealPerspectiveFromParent : MonoBehaviour
{
	// taken from SteamVR.instance.eye[0].pos
	static readonly Vector3 left = new Vector3(-0.03455f, 0.00000f, -0.01500f);
	static readonly Vector3 right = new Vector3(-left.x, left.y, left.z);

	// Use this for initialization
	void Start () {
		// note: do not use this!
		//GetComponent<Camera>().nonJitteredProjectionMatrix = transform.parent.GetComponent<Camera>().nonJitteredProjectionMatrix;
		//GetComponent<Camera>().projectionMatrix = transform.parent.GetComponent<Camera>().projectionMatrix;
		// also note: UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftEye); gives weird absolute values
		
		bool isLeft = transform.parent.GetComponent<Camera>().stereoTargetEye == StereoTargetEyeMask.Left;
		
		GetComponent<Camera>().projectionMatrix = transform.parent.GetComponent<Camera>().GetStereoProjectionMatrix(
			isLeft ? Camera.StereoscopicEye.Left : Camera.StereoscopicEye.Right);

		transform.localPosition = isLeft ? left : right;
	}
}
