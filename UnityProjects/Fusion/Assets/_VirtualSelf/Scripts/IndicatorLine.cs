using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

namespace VirtualSelf
{

public class IndicatorLine : MonoBehaviour
{
	public GameObject Segment;
	public Material[] Materials;
	[Range(0,5)]
	public int MatIndex;
	public float ChangeDelay = .1f;
	[Range(0,0.1f)]
	public float Spacing;
		
	private Transform[] _children;
	private List<GameObject> _segments;
	private bool _postStart;
	
	void Start ()
	{
		_segments = new List<GameObject>();

		Init();
		_postStart = true;
	}

	public void Init()
	{
		foreach (var o in _segments) Destroy(o);
		_segments.Clear();
		
		_children = new Transform[transform.childCount];
		for (int i = 0; i < _children.Length; i++) _children[i] = transform.GetChild(i);
		
		Bounds b = Segment.GetComponent<Renderer>().bounds;
		float size = b.size.CompMax() + Spacing;

		for (int j = 0; j < _children.Length-1; j++)
		{
			Vector3 s0 = _children[j].position;
			Vector3 s1 = _children[j+1].position;
			Vector3 line = s1 - s0;
			int segments = (int)(line.magnitude / size);
			Quaternion rot = Quaternion.LookRotation(line) * Quaternion.FromToRotation(Vector3.right, Vector3.forward);
			for (int i = 0; i < segments; i++)
			{
				GameObject s = Instantiate(Segment, Vector3.Lerp(s0, s1, (float)i/segments), rot);
				_segments.Add(s);
			}
		}
		
		Material m = Materials[0];
		foreach (GameObject o in _segments)
		{
			o.GetComponent<Renderer>().material = m;
		}
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		if (_postStart)
		{
			UnityEditor.EditorApplication.delayCall += () =>
			{
				Init();
				if (MatIndex < Materials.Length) ChangeMaterial(MatIndex);
			};
		}
	}
#endif
	
	public void ChangeMaterial(int i)
	{
		Material m = Materials[i];
		StopAllCoroutines();
		StartCoroutine(ChangeSegments(m));
	}

	IEnumerator ChangeSegments(Material m)
	{
		foreach (GameObject o in _segments)
		{
			o.GetComponent<Renderer>().material = m;
			yield return new WaitForSeconds(ChangeDelay);
		}
	}

	public void NextMaterial()
	{
		MatIndex = (MatIndex + 1) % _children.Length;
		ChangeMaterial(MatIndex);
	}
}

}