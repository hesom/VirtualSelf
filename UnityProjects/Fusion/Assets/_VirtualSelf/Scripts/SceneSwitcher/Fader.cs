using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public Material fadeMaterial;
    public float fadeDuration;
    public bool moveDown = true;

    private Vector3 initialPosition;
    private Transform portalFrame;
    public void StartFade()
    {
        portalFrame = transform.Find("PortalMesh").Find("Frame");
        initialPosition = portalFrame.position;
        foreach (Transform child in portalFrame)
        {
            var renderer = child.gameObject.GetComponent<MeshRenderer>();
            renderer.material = fadeMaterial;
            StartCoroutine(fadeOut(fadeMaterial));
        }
    }

    IEnumerator fadeOut(Material mat)
    {
        float t = 0;
        Color color = mat.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (!moveDown)
            {
                float blend = Mathf.Clamp01(t / fadeDuration);
                color.a = Mathf.Lerp(1.0f, 0.0f, blend);
                mat.color = color;
            }
            else
            {
                t += Time.deltaTime;
                float lerp = Mathf.Lerp(0.0f, 2.0f, t);
                portalFrame.position = initialPosition + lerp * Vector3.down;
            }

            yield return null;
        }

		Destroy(this.gameObject);
    }
}
