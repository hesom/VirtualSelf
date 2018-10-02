using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace VirtualSelf
{
	
public class IndicatorColorFail : IndicatorFail<Color>, IIndicator<Color> {

//	public new Color Indetermined = Color.white;
//	public new Color Valid = Color.green;
//	public new Color Invalid = Color.red;

	public IndicatorColorFail() : base((renderer, color) => renderer.material.color = color)
	{
		Indetermined = Color.white;
		Valid = Color.green;
		Invalid = Color.red;
	}
	
	Color IIndicator<Color>.GetVisualForState(States states)
	{
		switch (states)
		{
			case States.Indetermined: return Indetermined;
			case States.Valid: return Valid;
			case States.Invalid: return Invalid;
			default: throw new InvalidEnumArgumentException("not implemented: "+states);
		}
	}
	
//	private States _state;
}


}