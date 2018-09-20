using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class HandSpawnPhaser : HandTransitionBehavior
{
    public Material transparentMaterial;
    public InteractionHand interactionHand;
    public float phaseDuration = 1;
    public bool debugPrint = false;
    public UnityEvent OnFinish;
    public UnityEvent OnReset;

    private CapsuleHand capsuleHand;
    private Material defaultMaterial;
    private FieldInfo _material;

    //// Use this for initialization
    //protected override void Start() {
    //    Debug.Log("HELLOOOOOO");
    //}

    protected override void Awake()
    {
        base.Awake();

        capsuleHand = GetComponent<CapsuleHand>();
        _material = capsuleHand.GetType().GetField("_material", BindingFlags.NonPublic | BindingFlags.Instance);
        defaultMaterial = (Material)_material.GetValue(capsuleHand);
    }


    protected override void HandReset() {
        OnReset.Invoke();
        if (debugPrint) Debug.Log("Hand reset "+capsuleHand.Handedness);
        _material.SetValue(capsuleHand, transparentMaterial);
        interactionHand.contactEnabled = false;
        Invoke("RestoreHand", phaseDuration);
    }

    private void RestoreHand() {
        if (debugPrint) Debug.Log("Hand restored "+capsuleHand.Handedness);
        _material.SetValue(capsuleHand, defaultMaterial);
        interactionHand.contactEnabled = true;
    }

    protected override void HandFinish() {
        OnFinish.Invoke();
    }
}
