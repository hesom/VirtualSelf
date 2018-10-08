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
    public KeyCode DebugStart = KeyCode.K;

    private GameObject flameEffectInstance;
    private GameObject ignitionFlameEffectInstance;

    public void StartIgnitionFlame()
    {
        RemoveFlamesFade();
        if (ignitionFlameEffectInstance != null) Destroy(ignitionFlameEffectInstance);
        ignitionFlameEffectInstance = Instantiate(preFlamePrefab);
    }

    public void StartFlames()
    {
        // remove any previous flame immediately
        CancelInvoke(nameof(RemoveFlamesFade));
        if (flameEffectInstance != null) Destroy(flameEffectInstance);
        RemoveIgnitionFlamesFade();

        flameEffectInstance = Instantiate(flameEffectPrefab);
        Invoke(nameof(RemoveFlamesFade), flameDuration);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(DebugStart))
        {
            Debug.Log("Debug flame trigger");
            flameRotate = new Vector3(-50, 0, 0);
            StartFlames();
        }
        
        if (flameEffectInstance != null /*&& c.Frame().Hands.Count > 0*/)
        {
            Quaternion old = attachPoint.transform.localRotation;
            
            attachPoint.transform.localRotation = attachPoint.transform.localRotation * Quaternion.Euler(flameRotate);
            flameEffectInstance.transform.position = attachPoint.transform.position;
            flameEffectInstance.transform.rotation = attachPoint.transform.rotation;
            
            attachPoint.transform.localRotation = old;
        }

        if (ignitionFlameEffectInstance != null)
        {
            Quaternion old = attachPoint.transform.localRotation;
            attachPoint.transform.localRotation =
                attachPoint.transform.localRotation * Quaternion.Euler(flameRotate);
            ignitionFlameEffectInstance.transform.position = attachPoint.transform.position;
            ignitionFlameEffectInstance.transform.rotation = attachPoint.transform.rotation;
            attachPoint.transform.localRotation = old;
        }
    }

    public void RemoveFlamesFade()
    {
        if (flameEffectInstance != null)
        {
            foreach (Transform c in flameEffectInstance.transform.GetChildren())
            {
                if (c.CompareTag("Flame"))
                {
                    c.GetComponent<ParticleSystem>().Stop();
                }
                else
                {
                    // this is the light
                    StartCoroutine(FadeLight(c.GetComponent<ParticleSystem>()));
                }
            }

            Invoke(nameof(RemoveFlamesImmediate), 2.5f);
//            Destroy(flameEffectInstance, 2.5f);
//            flameEffectInstance = null;
        }

        RemoveIgnitionFlamesFade();
    }

    public void RemoveFlamesFadeAndDeactivate()
    {
        RemoveFlamesFade();
        Deactivate();
//        Invoke(nameof(Deactivate), 2.51f);
    }

    private void Deactivate()
    {
        GetComponent<FireGesture>().enabled = false;
    }

    void RemoveIgnitionFlamesFade()
    {
        if (ignitionFlameEffectInstance != null)
        {
            ignitionFlameEffectInstance.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
//            preFlameEffectInstance.GetComponent<ParticleSystem>().Stop();
            Invoke(nameof(DestroyIgnitionFlame), 1);
        }
    }

    void DestroyIgnitionFlame()
    {
        if (ignitionFlameEffectInstance != null) Destroy(ignitionFlameEffectInstance);
    }

    private void RemoveFlamesImmediate() {
        if (flameEffectInstance != null) {
            StopAllCoroutines();
            Destroy(flameEffectInstance);
            flameEffectInstance = null;
        }
    }
    
    IEnumerator FadeLight(ParticleSystem p)
    {
        ParticleSystem.LightsModule lm = p.lights;

        for (float f = lm.intensityMultiplier; f > 0; f -= 0.01f)
        {
            if (!p.IsAlive()) yield break;
            
            lm.intensityMultiplier = f;
            yield return null;
        }

        //yield return new WaitForSeconds(2);
    }
}

}
