using System;
using System.Collections;
using UnityEngine;

namespace VirtualSelf.Utility
{

public static class TransformUtil {
	
	/// <summary>
	/// Returns a rotation where the y axis points exactly upwards.
	/// </summary>
	/// <param name="start"></param>
	/// <returns></returns>
	public static Quaternion UprightRotation(this Quaternion start)
	{
		Vector3 tforward = start * Vector3.forward;
		Vector3 tright = start * Vector3.right;
		Vector3 tup = start * Vector3.up;
        
		Vector3 forward = Vector3.ProjectOnPlane(tforward, Vector3.up);
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (forward.sqrMagnitude == 0) return Quaternion.identity; //TODO properly fix this case

		Vector3 right = Vector3.Cross(forward, tforward);
		float ra = Vector3.Angle(right, tright);
		if (ra > 90) right *= -1;

		Vector3 up = Vector3.Cross(forward, right);
		float ua = Vector3.Angle(up, tup);
		if (ua > 90) up *= -1;
		if (up.y < 0) up *= -1;
        
		return Quaternion.LookRotation(forward, up);
	}
}

}
