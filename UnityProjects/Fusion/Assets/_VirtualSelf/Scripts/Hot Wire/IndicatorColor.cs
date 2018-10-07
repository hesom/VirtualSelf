using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace VirtualSelf
{
	
public class IndicatorColor : MonoBehaviour {

	public enum States {
		Indetermined, Valid, Invalid
	}

	private States _state;

	public Color Indetermined = Color.white, Valid = Color.green, Invalid = Color.red;
	public States State
	{
		get { return _state; }
		set
		{
			_state = value;
			Color col = ColorFromState(value);
			Material m = GetComponent<Renderer>().material;
			m.color = col;
			if (Emission > 0) m.SetColor("_EmissionColor", col * Mathf.LinearToGammaSpace(Emission));
		}
	}
	[Range(0,10)]
	public float Emission;

	void Start ()
	{
		State = States.Indetermined;
	}

	public void SetValidThenIndetermined()
	{
		State = States.Valid;
		CancelInvoke(nameof(SetIndermined));
		Invoke(nameof(SetIndermined), 1f);
	}

	public Color CurrentColor()
	{
		return ColorFromState(_state);
	}

	private void SetIndermined()
	{
		State = States.Indetermined;
	}

	private Color ColorFromState(States state)
	{
		Color col;
		switch (state)
		{
			case States.Indetermined:
				col = Indetermined;
				break;
			case States.Valid:
				col = Valid;
				break;
			case States.Invalid:
				col = Invalid;
				break;
			default:
				throw new InvalidEnumArgumentException("not implemented: "+state);
		}

		return col;
	}
}

}