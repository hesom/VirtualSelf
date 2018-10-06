using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

namespace VirtualSelf.Ballmaze
{
	
[RequireComponent(typeof(RiggedHand))]
public class RiggedHandSpawnPhaser : HandTransitionBehavior
{
	public Material PhaseMaterial;
	public float PhaseDuration = 1;
	public InteractionHand InteractionHand;

	private Material _baseMaterial;
	private SkinnedMeshRenderer _renderer;

	void Start()
	{
		_renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		_baseMaterial = _renderer.material;
	}

	protected override void HandReset() {
		CancelInvoke("RestoreHand");
		Invoke("RestoreHand", PhaseDuration);
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
