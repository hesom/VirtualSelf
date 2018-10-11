using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
	
public class MaterialSwapper : MonoBehaviour
{
	public Material Material;
	public Material MatAlt;
	public CollisionConnector[] CollisionConnectors;
	public Renderer[] OtherMaterials;

	private Material _copy;

	void Awake()
	{
		_copy = MatAlt;
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		UnityEditor.EditorApplication.delayCall += Apply;
	}
#endif

	public void Apply()
	{
		Apply(Material);
	}
	
	public void Apply(Material m)
	{
		Material = m;
		foreach (CollisionConnector c in CollisionConnectors)
		{
			c.Advanced.Material = Material;
			c.ValidateFields();
		}

		foreach (Renderer ren in OtherMaterials)
		{
			ren.material = Material;
			ren.enabled = Material != null;
		}
	}

	public void SwapAlt()
	{
		if (_copy == MatAlt)
		{
			_copy = Material;
			Apply(MatAlt);
		}
		else
		{
			// Material is MatAlt
			// MatAlt is MatAlt
			// _copy is Material
			Material = _copy;
			_copy = MatAlt;
			Apply();
		}
	}
}

}