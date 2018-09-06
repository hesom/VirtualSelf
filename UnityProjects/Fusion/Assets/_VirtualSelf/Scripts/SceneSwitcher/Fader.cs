using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public Material fadeMaterial;
    public float fadeDuration;

    public void StartFade()
    {
        var frame = transform.Find("PortalMesh").Find("Frame");
        foreach (Transform child in frame)
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
            float blend = Mathf.Clamp01(t / fadeDuration);
            color.a = Mathf.Lerp(1.0f, 0.0f, blend);
            mat.color = color;

            yield return null;
        }

		Destroy(this.gameObject);
    }
}
