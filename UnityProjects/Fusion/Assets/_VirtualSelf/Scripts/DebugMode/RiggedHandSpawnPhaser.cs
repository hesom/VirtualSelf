using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

namespace VirtualSelf
{
	
[RequireComponent(typeof(RiggedHand))]
public class RiggedHandSpawnPhaser : HandTransitionBehavior
{
	public Material PhaseMaterial;
	public InteractionHand InteractionHand;
	public float PhaseDuration = 1;

	private Material _baseMaterial;
	private SkinnedMeshRenderer _renderer;

	void Start()
	{
		_renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		_baseMaterial = _renderer.material;
	}

	protected override void HandReset() {
		CancelInvoke(nameof(RestoreHand));
		Invoke(nameof(RestoreHand), PhaseDuration);
	}

	private void RestoreHand() {
		InteractionHand.contactEnabled = true;
		_renderer.material = _baseMaterial;
	}

	protected override void HandFinish()
	{
		InteractionHand.contactEnabled = false;
		_renderer.material = PhaseMaterial;
	}
	
}
	
}
