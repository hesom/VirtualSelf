using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace VirtualSelf
{

public class ReferenceFinder : MonoBehaviour {

	[Serializable]
	public class Reference
	{
		public MonoBehaviour Script; // PinchDetector
		public string FieldName; // public HandModelBase HandModel;
		public string SourceObject; // CapsuleHand_L
	}

	public List<Reference> References;

//	private GameObject[] _allGameObjects;

	void Awake()
	{
		GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase;
		
		foreach (Reference r in References)
		{
			r.FieldName = r.FieldName.Replace(" ", "");
			Type setFieldType = r.Script.GetType(); // PinchDetector
			MemberInfo srcField = GetFieldOrProperty(setFieldType, r.FieldName);
			Type getComponentType = srcField is FieldInfo
				? (srcField as FieldInfo).FieldType
				: (srcField as PropertyInfo).PropertyType;

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
						if (getComponentType.IsInstanceOfType(m))
						{
							c = m;
							break;
						} 
					}
				}
			}
			if (c == null)
				throw new EntryPointNotFoundException("did not find any gameobject named " + r.SourceObject +
				                                      " with component " + getComponentType + " (which is what field " +
				                                      r.FieldName + " requires)");

			// set the field or property on the found script
			FieldInfo f = setFieldType.GetField(r.FieldName, flags);
			if (f != null)
			{
				f.SetValue(r.Script, c);
			}
			else
			{
				PropertyInfo p = setFieldType.GetProperty(r.FieldName, flags);
				if (p != null)
				{
					p.SetValue(r.Script, c);
				}
				else
				{
					Debug.LogError("field or property "+r.FieldName+" was not found");
				}
			}
		}
	}

	public static MemberInfo GetFieldOrProperty(Type t, string name)
	{
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase;
		FieldInfo f = t.GetField(name, flags);
		if (f != null) return f;
		PropertyInfo p = t.GetProperty(name, flags);
		if (p != null) return p;

		throw new EntryPointNotFoundException("no field or property "+name+" in "+t);
	}
}
	
}
