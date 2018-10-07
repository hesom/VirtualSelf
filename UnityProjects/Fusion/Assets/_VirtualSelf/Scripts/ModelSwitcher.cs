using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Leap.Unity;
using UnityEngine;
using VirtualSelf;

namespace VirtualSelf
{

[RequireComponent(typeof(HandModelManager))]
public class ModelSwitcher : MonoBehaviour
{
	public const string RIGGED = "Rigged Hands";
	public const string CAPSULE = "Capsule Hands";

	public CapsuleHand CapsuleLeft;
	public CapsuleHand CapsuleRight;
	public RiggedHand RiggedLeft;
	public RiggedHand RiggedRight;
	[Range(0,1)]
	public int ActiveGroup;
	
	private HandModelManager _man;
	private HashSet<ReferenceFinder.Reference> _refFinders = new HashSet<ReferenceFinder.Reference>();
	private bool _postStart;

	void OnValidate()
	{
		if (_man == null) _man = GetComponent<HandModelManager>();
		
		ActivateGroup(ActiveGroup);
	}

	void Start()
	{
		_postStart = true;
	}

	public void ActivateGroup(int index)
	{
		if (!_postStart) return;
		
		Debug.Log("Activating hand group "+index);
		HandModelBase newLeft;
		HandModelBase newRight;
		
		// easy: switch the currently active hand pair
		if (index == 0)
		{
			_man.DisableGroup(RIGGED);
			_man.EnableGroup(CAPSULE);
			newLeft = CapsuleLeft;
			newRight = CapsuleRight;
		}
		else
		{
			_man.DisableGroup(CAPSULE);
			_man.EnableGroup(RIGGED);
			newLeft = RiggedLeft;
			newRight = RiggedRight;
		}
		
		// hard: for all scripts in all scenes, change the old left/right references to the new ones (e.g. in detectors)
		// convention: every ReferenceFinder in the scene registers itself with this class
		// now we can loop through all finders, which all have their script and field specified, and we replace those with the currently active hand
		foreach (ReferenceFinder.Reference reff in _refFinders)
		{
			reff.SetValue(IsRight(reff.SourceObject) ? newRight : newLeft);
		}
		
		// we could also loop through every field and property of every script in all scenes, but this sounds like a bad idea 
//		Resources.FindObjectsOfTypeAll<MonoBehaviour>();
	}

	public void CycleGroup()
	{
		ActiveGroup = (ActiveGroup + 1) % 2;
		ActivateGroup(ActiveGroup);
	}

	private bool IsRight(string name)
	{
		string[] split = Regex.Split(name.ToLower(), "[ _]");
		if (split.Contains("left")) return false;
		if (split.Contains("right")) return true;
		if (split.Contains("r")) return true;
		if (split.Contains("l")) return false;
		return true;
	}

	public void RegisterHandReference(ReferenceFinder.Reference reff)
	{
		_refFinders.Add(reff);
	}
}

}