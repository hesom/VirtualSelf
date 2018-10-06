using System.Reflection;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

namespace VirtualSelf.Ballmaze
{
    
[RequireComponent(typeof(CapsuleHand))]
public class CapsuleHandSpawnPhaser : HandTransitionBehavior
{
    public Material transparentMaterial;
    public InteractionHand interactionHand;
    public float phaseDuration = 1;

    private CapsuleHand capsuleHand;
    private Material defaultMaterial;
    private FieldInfo _material;

    protected override void Awake()
    {
        base.Awake();

        capsuleHand = GetComponent<CapsuleHand>();
        _material = capsuleHand.GetType().GetField("_material", BindingFlags.NonPublic | BindingFlags.Instance);
        defaultMaterial = (Material)_material.GetValue(capsuleHand);
    }

    protected override void HandReset() {
        CancelInvoke("RestoreHand");
//        Debug.Log("Hand reset "+capsuleHand.Handedness);
        _material.SetValue(capsuleHand, transparentMaterial);
        interactionHand.contactEnabled = false;
        Invoke("RestoreHand", phaseDuration);
    }

    private void RestoreHand() {
//        Debug.Log("Hand restored "+capsuleHand.Handedness);
        _material.SetValue(capsuleHand, defaultMaterial);
        interactionHand.contactEnabled = true;
    }

    protected override void HandFinish() {
        
    }
}

}