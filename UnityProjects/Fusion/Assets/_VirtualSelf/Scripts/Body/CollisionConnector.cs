using System;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{

/// <summary>
/// This class takes a list of gameobject points and creates connection links between them.
/// These connectors are just primitive gameobjects, typically capsules, so each script instance generates basically a snake.
/// The primary purpose of this class is to create a crude collider body out of body tracking points.
/// </summary>
public class CollisionConnector : MonoBehaviour
{
	public enum Rotation {X,Y,Z}
	public enum Endpoint {None, AsSibling, AsChild}

	public string Name;
	public GameObject[] Objects;
	
	public AdvancedSettingsContainer Advanced = new AdvancedSettingsContainer();
	[Serializable] public class AdvancedSettingsContainer
	{
		public Rotation Orientation = Rotation.Y;
		public bool LoadEarly;
//		public bool CreateEndpoints;
		public Endpoint CreateEndpoints = Endpoint.None;
		public bool Small;
		public float LerpFraction = .5f;
		public PrimitiveType Primitive = PrimitiveType.Capsule;
		[NonSerialized] public float WidthFrac = 3; 
		// TODO the default capsule radius needs to be figured out, best on an actual human
		// TODO probably want to have a parameter for maximum capsule radius, maybe even minimum
		[NonSerialized] public float LengthFrac = 1.5f;
		public float Flatness = 1;
		public bool LookUp;
		// debug
//		public Rotation v1;
//		public Rotation v2;
//		public Rotation v3;
		public Material Material;
		public bool RenderAlways = true; // ideally the default material value would just be default diffuse, but access to that during or before serialization is not possible
	}
	
	[NonSerialized] public List<GameObject> Connectors;
	[NonSerialized] public List<GameObject> Endpoints;

	private bool _postStart;
	private bool _invalidated;
	private PrimitiveType _builtPrimitve;
	private Material _builtMaterial;
	private bool _builtBBPref;
	private float _scaleBias = 1;
	private bool _builtRenderAlways;

	public float ScaleBias
	{
		get { return _scaleBias; }
		set
		{
			_scaleBias = value;
			_invalidated = true;
		}
	}

	void Awake()
	{
		if (Advanced.LoadEarly && isActiveAndEnabled) Init();
	}

	void Start()
	{
		if (Connectors == null) Init(); // cannot use !LoadEarly here, for combinatin: loadearly + not active from start 
		_postStart = true;
	}

	void Update () {
		for (int i = 0; i < Objects.Length - 1; i++)
		{
			GameObject o1 = Objects[i];
			GameObject o2 = Objects[i+1];
			Quaternion q;
			bool skip = ShapeConnector( // debug: nullreference somewhere here
				o1, 
				o2, 
				Connectors[i], 
				out q);
			
			if (Advanced.CreateEndpoints != Endpoint.None && !skip)
			{
				MoveEndpoint(o1, o2, i, q);
			}
		}
	}

	void OnValidate()
	{
		_invalidated = true;
		
		if (_builtPrimitve != Advanced.Primitive && _postStart) // re-initialize connectors as a different primitve
		{
			foreach (GameObject o in Connectors) Destroy(o);
			if (Advanced.CreateEndpoints != Endpoint.None) foreach (GameObject o in Endpoints) Destroy(o);
			Init();
		}

		if (_builtMaterial != Advanced.Material && _postStart) // update materials
		{
			foreach (GameObject o in Connectors)
			{
				Renderer ren = o.GetComponent<Renderer>();
				if (Advanced.Material != null)
				{
					ren.material = Advanced.Material;
					ren.enabled = true;
				}
				else if (!Advanced.RenderAlways) ren.enabled = false;
			}

			_builtMaterial = Advanced.Material;
		}
		
		if (_builtRenderAlways != Advanced.RenderAlways && _postStart) // companion to update materials
		{
			foreach (GameObject o in Connectors)
			{
				Renderer ren = o.GetComponent<Renderer>();
				ren.enabled = Advanced.Material != null || Advanced.RenderAlways;
			}

			_builtRenderAlways = Advanced.RenderAlways;
		}

		if (_builtBBPref != PreferBoundingBox() && _postStart) // update smart colliders
		{
			foreach (GameObject o in Connectors)
			{
				if (PreferBoundingBox()) // change from not pref to pref, add box colliders
				{
					if (o.GetComponent<BoxCollider>() != null) Destroy(o.GetComponent<BoxCollider>());
					o.GetComponent<Collider>().enabled = false; // this should be capsule and sphere colliders
					o.AddComponent<BoxCollider>();
				}
				else // change from pref to not pref, disable box colliders and enable capsule colliders
				{
					Destroy(o.GetComponent<BoxCollider>());
					o.GetComponent<Collider>().enabled = true;
				}
			}

			_builtBBPref = PreferBoundingBox();
		}
		
		if (Advanced.CreateEndpoints != Endpoint.None) // for edit mode
		{
			Transform t = Advanced.CreateEndpoints == Endpoint.AsSibling ? transform.parent : transform;
			if (t == null)
			{
				Debug.LogWarning("Could not get transform");
				return;
			}
			bool foundLeft = false, foundRight = false;
			for (int j = 0; j < Objects.Length - 1; j++)
			{
				for (int i = 0; i < t.childCount; i++)
				{
					string n = t.GetChild(i).name;
					if (n.Equals(NameEndpoint(true, j))) foundLeft = true;
					else if (n.Equals(NameEndpoint(false, j))) foundRight = true;
					if (foundLeft && foundRight) break;
				}
	
				if (!foundLeft) NewEndpoint(true, j);
				if (!foundRight) NewEndpoint(false, j);
			}
		}
	}

	void OnDisable()
	{
		if (gameObject.activeInHierarchy && _postStart) SetActiveAll(false); // do not call when playmode is exiting
	}

	void OnEnable()
	{
		if (_postStart)
		{
			if (Connectors == null) Init(); // script was not enabled at start
			
			SetActiveAll(true);
		}
	}
	
	// create all the primitve gameobjects and endpoints, between each pair of objects
	private void Init()
	{
		if (Objects.Length < 2) throw new ArgumentException("at least two objects are required");

		Connectors = new List<GameObject>();
		if (Advanced.CreateEndpoints != Endpoint.None) Endpoints = new List<GameObject>();
		
		for (int i = 0; i < Objects.Length - 1; i++)
		{
			GameObject o1 = CheckPlaceholder(i);
			GameObject o2 = CheckPlaceholder(i+1);
			
			GameObject connector = GameObject.CreatePrimitive(Advanced.Primitive);
			_builtPrimitve = Advanced.Primitive;
			string num = i == 0 ? "" : " " + i;
			connector.name = string.Concat(Name, " ", _builtPrimitve, num);
			
			Renderer ren = connector.GetComponent<Renderer>();
			if (Advanced.Material != null) ren.material = Advanced.Material;
			else if (!Advanced.RenderAlways) ren.enabled = false;
			_builtMaterial = Advanced.Material;
			_builtRenderAlways = Advanced.RenderAlways;
			
			if (PreferBoundingBox())
			{
				_builtBBPref = true;
				connector.GetComponent<Collider>().enabled = false;
				connector.AddComponent<BoxCollider>();
			}
			else
			{
				_builtBBPref = false;
			}
			Quaternion q;
			ShapeConnector(o1, o2, connector, out q);
			Connectors.Add(connector);

			// endpoints are simply empty gameobjects that are placed at the two ends of the connector
			// they have no use on their own, but may be used for other instances of this script (via placeholders)
			if (Advanced.CreateEndpoints != Endpoint.None)
			{
				GameObject l = new GameObject();
				l.name = NameEndpoint(true, i);
				Endpoints.Add(l);
				GameObject r = new GameObject();
				r.name = NameEndpoint(false, i);
				Endpoints.Add(r);
				
				MoveEndpoint(o1, o2, i, q);
			}
		}
	}

	/// <summary>
	/// Updates the connector position, scale and rotation, unless the position is already up to date, in which case
	/// true is returned, so that further updates may be canceled.
	/// </summary>
	/// <param name="o1"></param>
	/// <param name="o2"></param>
	/// <param name="connector"></param>
	/// <param name="outrot"></param>
	/// <returns>true if this update was probably unnecessary</returns>
	private bool ShapeConnector(GameObject o1, GameObject o2, GameObject connector, out Quaternion outrot)
	{
		Vector3 p1 = o1.transform.position;
		Vector3 p2 = o2.transform.position;
		Vector3 lerped = Vector3.LerpUnclamped(p1, p2, Advanced.LerpFraction);
		
		if (_invalidated) _invalidated = false;
		else if (connector.transform.position == lerped)
		{
			outrot = Quaternion.identity;
			return true; // assume no updates necessary
		}
		
		connector.transform.position = lerped;
		float length = (p1 - p2).magnitude;
		connector.transform.localScale = new Vector3(
			_scaleBias * length*Advanced.Flatness/Advanced.WidthFrac, 
			_scaleBias * length/(Advanced.Small ? Advanced.WidthFrac : Advanced.LengthFrac), 
			_scaleBias * length/Advanced.WidthFrac/Advanced.Flatness);
		Vector3 objectVector = (p1 - p2).normalized;

		if (!Advanced.LookUp)
		{
			outrot = connector.transform.rotation = Quaternion.FromToRotation(Axis(), objectVector);
		}
		else
		{
			if (Advanced.Orientation == Rotation.Z)
			{
				outrot = connector.transform.rotation = Quaternion.LookRotation(objectVector) * Quaternion.FromToRotation(Vector3.forward, Vector3.right);
			}
			else
			{
				outrot = connector.transform.rotation =
					Quaternion.LookRotation(objectVector) * Quaternion.FromToRotation(Vector3.right, Axis())
					                                      * Quaternion.FromToRotation(Vector3.forward, Vector3.up);
////			outrot = connector.transform.rotation = Quaternion.LookRotation(objectVector) * Quaternion.FromToRotation(Axis(Advanced.v1), Axis()) 
////			                                                                            * Quaternion.FromToRotation(Axis(Advanced.v2), Axis(Advanced.v3));
			}
		}

		return false;
	}

	// mostly a mirror of ShapeCapsule(), but for endpoints
	private void MoveEndpoint(GameObject o1, GameObject o2, int index, Quaternion rot)
	{
		Vector3 origin = Connectors[index].transform.position;
		float length = (o1.transform.position - o2.transform.position).magnitude;

		float frac = Advanced.Small ? Advanced.WidthFrac : Advanced.LengthFrac;
		Vector3 axis = Axis();
		Vector3 offset = axis * (_scaleBias * length / frac);
		// undo the base rotation that is always there, so that only the rotation due to world positions remains
		offset = Quaternion.FromToRotation(axis, Vector3.up) * rot * offset; 
//		offset = Quaternion.FromToRotation(axis, Vector3.up) * Quaternion.FromToRotation(axis, (o1.transform.position - o2.transform.position).normalized) * offset; 
		Vector3 left = origin + offset;
		Vector3 right = origin - offset;
				
		Endpoints[index*2].transform.position = left;
		Endpoints[index*2+1].transform.position = right;
	}
	
	/// <summary>
	/// How the placeholder system works:
	/// This script takes a number of gameobjects and links them together via colliders.
	/// This script can generate endpoints, which are empty gameobjects on both ends of the capsule.
	/// Now you can have one instance of this script generate these endpoints, and use the them as starting points
	/// for a different instance of this script. But since they do not exist at edit time, you use placeholder objects.
	/// Make sure that the endpoints are created before they are expected in another script, by setting LoadEarly in the
	/// first one. A placeholder object needs the Placeholder tag, and will then not be used by this script;
	/// instead it will be deleted immediately and the first gameobject with the same name will be used.
	/// The name of the endpoints are always <code>Name + " Endpoint left"</code> (or right).
	/// </summary>
	/// <param name="i"></param>
	/// <returns></returns>
	private GameObject CheckPlaceholder(int i)
	{
		GameObject o = Objects[i];
		if (o.CompareTag("Placeholder"))
		{
			string n = o.name;
			DestroyImmediate(o);
			return Objects[i] = GameObject.Find(n);
			
			/*
			foreach (GameObject oo in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
			{
				if (oo.name.Equals(n))
				{
					Objects[i] = oo;
					return oo;
				}
			}
			throw new ArgumentException("placeholder object was given but no other object with matching type was found");
			*/
		}
		return o;
	}

	#region Small utility functions
	
	private void NewEndpoint(bool left, int i)
	{
		Debug.Log("New endpoint for "+Name);
		GameObject ep = new GameObject();
		ep.name = NameEndpoint(left, i);
		ep.tag = "Placeholder";
		ep.transform.parent = Advanced.CreateEndpoints == Endpoint.AsSibling ? transform.parent : transform;
		ep.transform.localScale = Vector3.one;
		ep.transform.localRotation = Quaternion.identity;
		ep.transform.localPosition = Vector3.zero;
	}
	
	private void SetActiveAll(bool a)
	{
		foreach (GameObject o in Connectors) o.SetActive(a);
		if (Advanced.CreateEndpoints != Endpoint.None) foreach (GameObject o in Endpoints) o.SetActive(a);
	}
	
	private string NameEndpoint(bool left, int i)
	{
		string num = i == 0 ? "" : " " + i;
		return Name + (left ? " Endpoint left" : " Endpoint right")+num;
	}
	
	private Vector3 Axis()
	{
		return Axis(Advanced.Orientation);
	}

	private Vector3 Axis(Rotation r)
	{
		Vector3 axis;
		switch (r)
		{
			case Rotation.X: axis = Vector3.right;
				break;
			case Rotation.Y: axis = Vector3.up;
				break;
			case Rotation.Z: axis = Vector3.forward;
				break;
			default: throw new Exception("impossible");
		}

		return axis;
	}
	
	private bool PreferBoundingBox()
	{
		if (!(Advanced.Primitive == PrimitiveType.Cylinder || 
		      Advanced.Primitive == PrimitiveType.Capsule || 
		      Advanced.Primitive == PrimitiveType.Sphere)) return false;
		float f = 1.5f;
		return Advanced.Flatness > f || Advanced.Flatness < 1 / f;
	}
	
	#endregion // small utility functions
}

}
