using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;
using UnityEngine;

namespace VirtualSelf
{
	
public class HeightDifferenceDetector : Detector {
	/**
 * The interval at which to check palm direction.
 * @since 4.1.2
 */
	[Units("seconds")]
	[Tooltip("The interval in seconds at which to check this detector's conditions.")]
	[MinValue(0)]
	public float Period = .1f; //seconds

	public HandModelBase HandModel1 = null;
	public HandModelBase HandModel2 = null;
	
	public float OnDifference = .03f;
	public float OffDifference = .06f;
	public Direction DifferenceAxis;
	
	
	private IEnumerator _watcherCoroutine;
	private float _h1TimeVisible;
	private float _h2TimeVisible;
	
	private void Awake()
	{
		_watcherCoroutine = HeightWatcher();
	}

	private IEnumerator HeightWatcher()
	{
		while (true)
		{
			Hand h1 = HandModel1.GetLeapHand();
			Hand h2 = HandModel2.GetLeapHand();
			
			if (h1 != null && h2 != null)
			{
				
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (h1.TimeVisible == _h1TimeVisible || h2.TimeVisible == _h2TimeVisible)
				{
					// some hand is no longer visible, consider this gesture as failure (rather than keeping it alive)
					Deactivate();
				}
				else
				{
					_h1TimeVisible = h1.TimeVisible;
					_h2TimeVisible = h2.TimeVisible;
					Vector3 p1 = h1.PalmPosition.ToVector3();
					Vector3 p2 = h2.PalmPosition.ToVector3();
					
					float diff;
					switch (DifferenceAxis)
					{
						case Direction.X: diff = Mathf.Abs(p1.x-p2.x);
							break;
						case Direction.Y: diff = Mathf.Abs(p1.y-p2.y);
							break;
						case Direction.Z: diff = Mathf.Abs(p1.z-p2.z);
							break;
						default: diff = float.MaxValue;
							break;
					}
	//				Debug.Log("diff "+diff);
					if (diff < OnDifference) Activate();
					else if (diff > OffDifference) Deactivate();
				}
			}
			
			yield return new WaitForSeconds(Period);
		}
	}
	
	private void OnEnable () {
		StartCoroutine(_watcherCoroutine);
	}

	private void OnDisable () {
		StopCoroutine(_watcherCoroutine);
		Deactivate();
	}

	private void OnValidate()
	{
		if (OffDifference < OnDifference) {
			OffDifference = OnDifference;
		}
	}
		
	
	public enum Direction {X,Y,Z}
}

}