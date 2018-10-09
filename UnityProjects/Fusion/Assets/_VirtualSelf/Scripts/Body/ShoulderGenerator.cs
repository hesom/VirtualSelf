using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
	
public class ShoulderGenerator : MonoBehaviour
{

	public Transform Neck;
	public Transform Left, Right;
	public Transform LeftUpperArm, RightUpperArm;

	private float _y;
	private float _width;
	
	void Start ()
	{
		// get y offset and shoulder point distance based on initial shoulder positions (averaged) relative to neck
		_y = Neck.position.y - (Left.position.y + Right.position.y) / 2;
		Vector3 neckPlane = Neck.position;
		Vector3 lP = Left.position;
		Vector3 rP = Right.position;
		neckPlane.y = 0;
		lP.y = 0;
		rP.y = 0;
		_width = (Vector3.Distance(neckPlane, lP) + Vector3.Distance(neckPlane, rP)) / 2;
	}
	
	void Update ()
	{
		Vector3 flat = Vector3.ProjectOnPlane(LeftUpperArm.position - RightUpperArm.position, Vector3.up).normalized;
		Vector3 neckdown = Neck.position;
		neckdown.y -= _y;
		Left.position = neckdown + flat * _width;
		Right.position = neckdown + flat * -_width;
	}
}

}