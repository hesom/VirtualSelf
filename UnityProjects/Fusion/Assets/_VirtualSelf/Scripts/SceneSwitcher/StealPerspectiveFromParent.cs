using UnityEngine;
using UnityEngine.XR;

public class StealPerspectiveFromParent : MonoBehaviour
{

	private bool isLeft;
	
	// taken from SteamVR.instance.eye[0].pos
	//static Vector3 left = new Vector3(-0.03435f, 0.00000f, -0.01500f);
	//static Vector3 right = new Vector3(-left.x, left.y, left.z);

	// Use this for initialization
	void Start () {
		// note: do not use this!
		//GetComponent<Camera>().nonJitteredProjectionMatrix = transform.parent.GetComponent<Camera>().nonJitteredProjectionMatrix;
		//GetComponent<Camera>().projectionMatrix = transform.parent.GetComponent<Camera>().projectionMatrix;
		// also note: UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftEye); gives weird absolute values
		
		isLeft = transform.parent.GetComponent<Camera>().stereoTargetEye == StereoTargetEyeMask.Left;
		
		GetComponent<Camera>().projectionMatrix = transform.parent.GetComponent<Camera>().GetStereoProjectionMatrix(
			isLeft ? Camera.StereoscopicEye.Left : Camera.StereoscopicEye.Right);
	}


	private void Update()
	{
		// Eye distance is only different in x coordinate, y and z are constant.
		// Because the camera transform component has no parent
		// the world to local matrix of the camera has no scaling (=1), the distance between the eyes As there is no scaling applied,
		// the distance between the xrnodes do not differ from the unity positions.
		// This is the proper replacement for steamvr eye position. 
		// additionally, unity xr provides live updates to eye distance changes, so we change in update here
		
		// TODO profile if doing all this is expensive
		
		// note that there is still a visible skip between portal and original scene
		// but this is not and probably never was caused by eye position inaccuracies?
		
		Vector3 worldHead = Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.Head)) *
		                 InputTracking.GetLocalPosition(XRNode.Head);

		Vector3 worldEye = isLeft
			? Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.LeftEye)) *
			  InputTracking.GetLocalPosition(XRNode.LeftEye)
			: Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.RightEye)) *
			  InputTracking.GetLocalPosition(XRNode.RightEye);

		Vector3 localPos = worldEye - worldHead;
//		Debug.Log(isLeft+" "+localPos.ToString("F5"));

		if (localPos.sqrMagnitude > 1e-6f) transform.localPosition = localPos; // skip changing if tracking is lost
	}

}
