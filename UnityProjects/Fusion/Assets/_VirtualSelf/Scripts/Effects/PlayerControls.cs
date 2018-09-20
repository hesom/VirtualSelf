using System;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Attachments;
using UnityEngine;
using VirtualSelf;

public class PlayerControls : MonoBehaviour
{

    public GameObject spawn;
//    public RelativeReset tmp;// only for debugging without LM
    public GameObject flameEffectPrefab;
    public float flameDuration = 3;
    public bool attachToCamera = true;
    public AttachmentPointBehaviour attachPoint;
    public Vector3 flameRotate;
    public GameObject preFlamePrefab;
    public ChangeCamFov2 changeFov;

//    public HandPool pool;
//    public GameObject interactionManager;
//    public Vector3 rot;

    private GameObject flameEffectInstance;
    private GameObject preFlameEffectInstance;

    //public InteractionHand hand;
//    private Controller c;

    // Use this for initialization
    void Start () {
//        c = new Controller();
//        Controller c = new Controller();
//        c.Frame().ge
    }

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
	void Update () {
	    if (Input.GetKeyDown(KeyCode.F))
	    {
	        Instantiate(spawn);
	    }
        if (Input.GetKeyDown(KeyCode.G))
        {
            //tmp.resetAll();
        }
	    if (Input.GetKeyDown(KeyCode.R))
	    {
            StartFlames();
	    }

	    if (Input.GetKeyDown(KeyCode.Y))
	    {
	        changeFov.CycleFov();
	    }

	    if (flameEffectInstance != null /*&& c.Frame().Hands.Count > 0*/)
	    {
	        if (attachToCamera)
	        {
                Transform cam = Camera.main.transform;
    
                flameEffectInstance.transform.position = cam.position;
                flameEffectInstance.transform.rotation = cam.rotation;
	        }
	        else
	        {
	            Quaternion old = attachPoint.transform.localRotation;
	            attachPoint.transform.localRotation = attachPoint.transform.localRotation * Quaternion.Euler(flameRotate);
	            flameEffectInstance.transform.position = attachPoint.transform.position;
	            flameEffectInstance.transform.rotation = attachPoint.transform.rotation;
	            attachPoint.transform.localRotation = old;
	        }

            

            //Hand hand = c.Frame().Hands[0];
            //flameEffectInstance.transform.position = (UnityVectorExtension.ToVector3(hand.StabilizedPalmPosition) * 0.01f) + Camera.main.transform.position; //Vec(hand.StabilizedPalmPosition);
            //flameEffectInstance.transform.rotation = Quaternion.LookRotation(Vec(hand.PalmNormal));


            //HandModel hand = pool.GetHandModel<HandModel>(c.Frame().Hands[0].Id);
            //if (hand == null) throw new UnityException("HandModel not found");

            //flameEffectInstance.transform.position = hand.GetWristPosition();
            //flameEffectInstance.transform.rotation = hand.GetPalmRotation();


            //Transform t = interactionManager.transform.Find("Right Interaction Hand Contact Bones").Find("Contact Palm Bone");
            //flameEffectInstance.transform.position = t.position;
            ////flameEffectInstance.transform.rotation = t.rotation * Quaternion.AngleAxis(90, rot);
            //flameEffectInstance.transform.rotation = t.rotation * Quaternion.Euler(rot);
        }

	    if (preFlameEffectInstance != null)
	    {
	        Quaternion old = attachPoint.transform.localRotation;
	        attachPoint.transform.localRotation = attachPoint.transform.localRotation * Quaternion.Euler(flameRotate);
	        preFlameEffectInstance.transform.position = attachPoint.transform.position;
	        preFlameEffectInstance.transform.rotation = attachPoint.transform.rotation;
	        attachPoint.transform.localRotation = old;
	    }
	}

    static Vector3 Vec(Vector v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

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
