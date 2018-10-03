using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Leap.Unity.Interaction;

namespace VirtualSelf
{
	public class AnchorAnimationCallback : MonoBehaviour
	{
		public Anchor anchor;
		public float maxSnapDistance = 0.01f;
		public UnityEvent onAnchorAnimationFinished;
		private bool animationStarted = false;
		
		public void StartAnchorAnimation()
		{
			animationStarted = true;
		}

		void Update()
		{
			if (animationStarted)
			{
				Vector3 dist = anchor.transform.position - transform.position;

				if (dist.magnitude < maxSnapDistance)
				{
					onAnchorAnimationFinished.Invoke();
					animationStarted = false;
				}
			}
		}

	}
}
