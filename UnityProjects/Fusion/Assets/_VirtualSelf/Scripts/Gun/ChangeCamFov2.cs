using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;

namespace VirtualSelf
{

public class ChangeCamFov2 : MonoBehaviour {

	public Camera Camera;
//	[Tooltip("E.g. 8 means the scope surface takes 1/8th of the area of the main camera")]
	public float ScopeScreenFraction = 8;
	public Renderer Lens;

	private Dictionary<int, float> _zoomToFov;
	private int _selectedZoom = 1;
	
	// Use this for initialization
	void Start ()
	{
		CalculateLevels();
	}

	public void CalculateLevels()
	{
//		float scopeScreenFraction = 8;
//		float lensWidth = Lens.bounds.size.CompMax();
		
		_zoomToFov = new Dictionary<int, float>();
		float mainFov = Camera.main.fieldOfView;
		int zoom = 1;
		for (float fov = mainFov / ScopeScreenFraction; fov >= 1; fov /= 2)
		{
			_zoomToFov.Add(zoom, fov);
			Debug.Log(zoom+" = "+fov);
			zoom *= 2;
		}
	}

	public void CycleFov()
	{
//		Vector3 posStart = Camera.main.WorldToScreenPoint(Lens.bounds.min);
//		Vector3 posEnd = Camera.main.WorldToScreenPoint(Lens.bounds.max);
// 
//		int widthX = (int)(posEnd.x - posStart.x);
//		int widthY = (int)(posEnd.y - posStart.y);
//		Debug.Log(widthX+" "+widthY);
//		Debug.Log(GetScreenRect(Lens));
		
		
		_selectedZoom *= 2;
		if (_selectedZoom > Mathf.Pow(2, _zoomToFov.Count-1)) _selectedZoom = 1;
		Debug.Log("Set zoom to "+_selectedZoom+"x");
		Camera.fieldOfView = _zoomToFov[_selectedZoom];
	}
	
}
	
}
