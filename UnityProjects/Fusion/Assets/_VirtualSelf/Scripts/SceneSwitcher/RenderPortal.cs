using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderPortal : MonoBehaviour
{

	public Camera portalCamera;

	private void OnPreRender()
	{
		if (portalCamera != null)
		{
			portalCamera.Render();
		}
	}

}
