using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionPostprocessing : MonoBehaviour {

	public Material material;
	public float waveSpeed = 0.01f;
	private float animationTimer = 0.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		animationTimer += waveSpeed*Time.deltaTime;
		animationTimer = animationTimer % 1.0f;
	}

	/// <summary>
	/// OnRenderImage is called after all rendering is complete to render image.
	/// </summary>
	/// <param name="src">The source RenderTexture.</param>
	/// <param name="dest">The destination RenderTexture.</param>
	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		material.SetFloat("animationTimer", animationTimer);
		Graphics.Blit(src, dest, material);
	}
}
