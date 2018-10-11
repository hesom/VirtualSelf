using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace VirtualSelf 
{

public class MeltOnCollision : MonoBehaviour
{

    [Range(0,1)]
    public float scaleRemoveThreshold = 0.3f;
    public float scaleDecrement = 0.05f;
    public float updateSecondsMin = 0.1f;
    public float updateSecondsMax = 0.2f;
    public int iterationSteps = 5;
    public float iterationRotation = 0.3f;
    public UnityEvent meltedCallback;
    public GameObject drip; // currently unused
    public GameObject vapor;

    private float scale = 1;
    private Vector3[] childScales;
    private Quaternion[] childRotations;
    //private Vector3[] childLossyScales;
    private Vector3 baseScale;
    private float lastUpdate;
    //private float tmpDownScale = 0.02f;

    // Use this for initialization
    void Start()
    {
        childScales = new Vector3[transform.childCount];
        childRotations = new Quaternion[transform.childCount];
        //childLossyScales = new Vector3[transform.childCount];
        baseScale = transform.localScale;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform c = transform.GetChild(i);
            childScales[i] = c.localScale;
            childRotations[i] = c.rotation;

            // disable physics for frozen objects
            Rigidbody r = c.GetComponent<Rigidbody>();
            if (r != null) r.isKinematic = true;
            InteractionBehaviour ib = c.GetComponent<InteractionBehaviour>();
            if (ib != null) ib.ignoreGrasping = true;
        }
    }

    IEnumerator ScaleDownSequence()
    {
        //scale -= scaleDecrement;

        // iterationSteps of the FPS would correspond to continuous melting (under flames)
        for (int i = 0; i < iterationSteps; i++)
        {

            // scaleDecrement/iterationSteps would correspond to perfectly smooth melting between layers (no jump), so go a bit slower
            scale -= i * (scaleDecrement/(iterationSteps*2)); 
            //scale -= i * scaleDecrement * (updateSecondsMin);
            ScaleDown(i == 0, scale);
            CheckMelted();

            // updateSecondsMin/iterationSteps would correspond to changes every frame, so go a bit slower than this
            yield return new WaitForSeconds(updateSecondsMin/(iterationSteps*0.5f)); 
        }
    }

    void ScaleDown(bool rotate, float cycleScale)
    {
        
        // scale this object down, while scaling children up by same amount
        transform.localScale = baseScale * cycleScale;

        // on the first "tick" of the melt sequence, set a random (y axis, so the texture doesn't get flipped too much) rotation
        // this resembles a new "layer" of ice being uncovered
        // on the remaining ticks, just rotate ever so slightly, just enough to not make the texture look completely static, but still close to the "layer"
        if (rotate) transform.rotation = Quaternion.Euler(Rand()*2, Random.value*360, Rand()*2);
        else transform.rotation *= Quaternion.Euler(iterationRotation, iterationRotation, iterationRotation);
        //if (rotate) transform.rotation = Quaternion.Euler(Rand()*2, transform.rotation.eulerAngles.y + Rand()*80, Rand()*2);
        //else transform.rotation = Quaternion.Euler(transform.rotation.x+1, transform.rotation.y + 100, transform.rotation.z + 1);


        for (int i = 0; i < transform.childCount; i++)
        {
            Transform c = transform.GetChild(i);
            bool isParticle = c.GetComponent<ParticleSystem>() != null;
            Vector3 initial = isParticle ? Vector3.one : childScales[i];

            // particle system scales are unaffected by parent scales, just set the scale to the global value
            // non particle systems are just scaled by the inverse of the parent
            c.localScale = isParticle ? initial * (cycleScale + 0.1f) : initial / cycleScale;

            // for rotation we can simply set it globally, buf if needed it can also be a local inverse of the parent rotation
            //c.localRotation = Quaternion.Inverse(transform.rotation);
            c.rotation = isParticle ? Quaternion.identity : childRotations[i];
        }
    }

    void CheckMelted()
    {
        // melting complete
        if (scale <= scaleRemoveThreshold)
        {
            //Debug.Log("melted");
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform c = transform.GetChild(i);
                if (c.GetComponent<ParticleSystem>() != null) continue;
                c.parent = transform.parent;
                
                // enable physics again
                Rigidbody r = c.GetComponent<Rigidbody>();
                if (r != null) r.isKinematic = false;
                InteractionBehaviour ib = c.GetComponent<InteractionBehaviour>();
                if (ib != null) ib.ignoreGrasping = false;
            }

            transform.DetachChildren();
            Destroy(gameObject);
            meltedCallback.Invoke();
        }
    }

    // return value between -1 and 1
    static float Rand()
    {
        return 1 - Random.value * 2;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Flame"))
        {
            float t = Time.time;
            if (t - lastUpdate > Random.Range(updateSecondsMin, updateSecondsMax))
            {
                lastUpdate = t;

                // essentially, every now and then (updateSecondsMin/Max) we "peel" back a layer of ice
                // the min/max roll is repeated on every collision event, so the amount of flames hitting the ice matters, as well as the time passed
                // the melting size loss should neither be instantaneous in rough steps, nor be perfectly smooth with every frame
                // one flame hit (between acceptable delays) will cause a melting sequence that lasts a few ticks, which are not necessarily frames
                // the sequence of ticks should also finish faster than the next update comes in
                // (see the ScaleDown methods for detailed documentation)
//                StopCoroutine(ScaleDownSequence());
                StopAllCoroutines();
                StartCoroutine(ScaleDownSequence()); 

                // spawn melt effects
                // drip particles are near impossible to make work properly (e.g. actually colliding with the ground)
                //Destroy(Instantiate(drip, transform.position, transform.rotation), 2);
                GameObject v = Instantiate(vapor, transform);
                v.transform.parent = transform; // parent the vapor to the ice, so that it automatically gets scaled down
                Destroy(v, 2);





            }

        }
        
    }

}


}
