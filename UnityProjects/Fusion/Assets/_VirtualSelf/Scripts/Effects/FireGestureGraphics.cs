using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using Leap.Unity.Attachments;
using UnityEngine;

namespace VirtualSelf
{

public class FireGestureGraphics : MonoBehaviour
{

    public GameObject flameEffectPrefab;
    public float flameDuration = 3;
    public AttachmentPointBehaviour attachPoint;
    public Vector3 flameRotate;
    public GameObject preFlamePrefab;

    private GameObject flameEffectInstance;
    private GameObject preFlameEffectInstance;

    public void StartPreFlame()
    {
        if (preFlameEffectInstance != null) Destroy(preFlameEffectInstance);
        preFlameEffectInstance = Instantiate(preFlamePrefab);
    }

    public void StartFlames()
    {
        // remove any previous flame immediately
        CancelInvoke("RemoveFlamesFade");
        if (flameEffectInstance != null) Destroy(flameEffectInstance);
        RemovePreFlamesFade();

        flameEffectInstance = Instantiate(flameEffectPrefab);
        Invoke("RemoveFlamesFade", flameDuration);
    }

    // Update is called once per frame
    void Update()
    {

        if (flameEffectInstance != null /*&& c.Frame().Hands.Count > 0*/)
        {
            Quaternion old = attachPoint.transform.localRotation;
            attachPoint.transform.localRotation =
                attachPoint.transform.localRotation * Quaternion.Euler(flameRotate);
            flameEffectInstance.transform.position = attachPoint.transform.position;
            flameEffectInstance.transform.rotation = attachPoint.transform.rotation;
            attachPoint.transform.localRotation = old;
            
        }

        if (preFlameEffectInstance != null)
        {
            Quaternion old = attachPoint.transform.localRotation;
            attachPoint.transform.localRotation =
                attachPoint.transform.localRotation * Quaternion.Euler(flameRotate);
            preFlameEffectInstance.transform.position = attachPoint.transform.position;
            preFlameEffectInstance.transform.rotation = attachPoint.transform.rotation;
            attachPoint.transform.localRotation = old;
        }
    }

//    static Vector3 Vec(Vector v)
//    {
//        return new Vector3(v.x, v.y, v.z);
//    }

    public void RemoveFlamesFade()
    {
        if (flameEffectInstance != null)
        {
            foreach (Transform c in flameEffectInstance.transform.GetChildren())
            {
                if (c.tag.Equals("Flame"))
                {
                    c.GetComponent<ParticleSystem>().Stop();
                }
                else
                {
                    // this is the light
                    StartCoroutine(FadeLight(c.GetComponent<ParticleSystem>()));
                }
            }

            Destroy(flameEffectInstance, 2.5f);
            flameEffectInstance = null;
        }

        RemovePreFlamesFade();
    }

    public void RemoveFlamesFadeAndDeactivate()
    {
        RemoveFlamesFade();
        Invoke("Deactivate", 2.51f);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    void RemovePreFlamesFade()
    {
        if (preFlameEffectInstance != null)
        {
            preFlameEffectInstance.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
//            preFlameEffectInstance.GetComponent<ParticleSystem>().Stop();
            Invoke("DestroyPreFlame", 1);
        }
    }

    void DestroyPreFlame()
    {
        if (preFlameEffectInstance != null) Destroy(preFlameEffectInstance);
    }

    IEnumerator FadeLight(ParticleSystem p)
    {
        ParticleSystem.LightsModule lm = p.lights;

        for (float f = lm.intensityMultiplier; f > 0; f -= 0.01f)
        {
            lm.intensityMultiplier = f;
            yield return null;
        }

        //yield return new WaitForSeconds(2);
    }
}

}
