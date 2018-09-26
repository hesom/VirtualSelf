using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace VirtualSelf
{
	
public class ReferenceFinder : MonoBehaviour {

	[Serializable]
	public class Reference
	{
		public MonoBehaviour Script; // PinchDetector
		public string FieldName; // public HandModelBase HandModel;
		public string SourceObject; // CapsuleHand_L
		[NonSerialized] public MemberInfo _setter;

		public void SetValue(object o)
		{
			if (_setter is FieldInfo) ((FieldInfo)_setter).SetValue(Script, o);
			else if (_setter is PropertyInfo) ((PropertyInfo)_setter).SetValue(Script, o);
			else Debug.LogError("field or property "+FieldName+" was not found");
		}
	}

	public List<Reference> References;
	
	void Awake()
	{
		GameObject[] allGameObjects = VirtualSelfUtil.FindObjectsOfTypeButNoPrefabs<GameObject>();
		
		foreach (Reference r in References)
		{
			r.FieldName = r.FieldName.Replace(" ", "");
			Type scriptType = r.Script.GetType(); // PinchDetector
			
			MemberInfo scriptSetter = VirtualSelfUtil.GetFieldOrProperty(scriptType, r.FieldName);
			r._setter = scriptSetter;
			Type componentType = scriptSetter is FieldInfo
				? (scriptSetter as FieldInfo).FieldType
				: (scriptSetter as PropertyInfo).PropertyType;

			Component c = null;
			foreach (GameObject o in allGameObjects)
			{
				if (o.name == r.SourceObject) // among ALL gameobjects, find the one with given name
				{
					// we actually don't even know which script to pick from the gameobject
					// search all scripts until one is found that is the type of the field we want to set
					foreach (MonoBehaviour m in o.GetComponents<MonoBehaviour>())
					{
//						if (m.GetType().IsAssignableFrom(getComponentType))
						if (componentType.IsInstanceOfType(m))
						{
							c = m;
							RegisterWithModelSwitcher(m.gameObject);
							goto FoundRef;
						} 
					}
				}
			}
			FoundRef:
			
			if (c == null) throw new EntryPointNotFoundException("did not find any GameObject named " + r.SourceObject +
				                                      " with component " + componentType + " (which is what field " +
				                                      r.FieldName + " requires)");

			// set the field or property on the found script
			r.SetValue(c);
		}
	}

	private void RegisterWithModelSwitcher(GameObject o)
	{
		Transform parent = o.transform.parent;
		if (parent != null) parent.GetComponent<ModelSwitcher>()?.RegisterHandReference(this);
	}
}
	
}
