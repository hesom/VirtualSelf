using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace VirtualSelf
{
	
public class IndicatorMaterialFail : IndicatorFail<Material>, IIndicator<Material> {
	
	public IndicatorMaterialFail() : base((renderer, color) => renderer.material = color)
	{
//		Indetermined = Color.white;
//		Valid = Color.green;
//		Invalid = Color.red;
	}
	
	Material IIndicator<Material>.GetVisualForState(States states)
	{
		switch (states)
		{
			case States.Indetermined: return Indetermined;
			case States.Valid: return Valid;
			case States.Invalid: return Invalid;
			default: throw new InvalidEnumArgumentException("not implemented: "+states);
		}
	}
	
//	public Material Indetermined;
//	public Material Valid;
//	public Material Invalid;
//	public States State
//	{
//		get { return _state; }
//		set
//		{
//			SetState(value);
//		}
//	}
//	
//	private States _state;
//
//	void Start ()
//	{
//		SetIndermined();
//	}
//
//	public Material GetVisualForState(VirtualSelf.States state)
//	{
//		Material col;
//		switch (state)
//		{
//			case States.Indetermined:
//				col = Indetermined;
//				break;
//			case States.Valid:
//				col = Valid;
//				break;
//			case States.Invalid:
//				col = Invalid;
//				break;
//			default:
//				throw new InvalidEnumArgumentException("not implemented: "+state);
//		}
//
//		return col;
//	}
//
//	public void SetState(VirtualSelf.States s)
//	{
//		_state = s;
//		GetComponent<Renderer>().sharedMaterial = GetVisualForState(s);
//	}
//
//	public void SetValidThenIndetermined()
//	{
//		State = States.Valid;
//		CancelInvoke("SetIndermined");
//		Invoke("SetIndermined", 1f);
//	}
//
//	private void SetIndermined()
//	{
//		State = States.Indetermined;
//	}
}

}