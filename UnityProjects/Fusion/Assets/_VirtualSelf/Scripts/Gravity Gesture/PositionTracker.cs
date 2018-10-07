using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine.Events;
using VirtualSelf;

namespace VirtualSelf
{

    public class PositionTracker : MonoBehaviour
    {
        public UnityEvent OnDownwardsComplete;
        public UnityEvent OnUpwardsComplete;
        public DetectorLogicGate DetectorLogicGate;
        public HandModelBase HandModel1;
        public HandModelBase HandModel2;
        public float DistanceRequired = .2f;
        public Direction DistanceAxis;
        public GameObject DebugDot;
        public bool debug;

        private bool _tracking;
        private Vector3 _startPos;
        private Vector3 _lastPos;

        // Update is called once per frame
        void Update()
        {
            Hand h1 = HandModel1.GetLeapHand();
            Hand h2 = HandModel2.GetLeapHand();

//            if (HandModel1.IsTracked && HandModel2.IsTracked)
            if (h1 != null && h2 != null)
            {
                _lastPos = PalmPos(h1, h2);
                //							
                if (_tracking)
                {
                    CheckEndPos();

                }
                else if (DetectorLogicGate.IsActive)
                {
                    if (debug) Debug.Log("tracking started");
                    _tracking = true;
                    _startPos = PalmPos(h1, h2);
                    if (debug) Instantiate(DebugDot, _startPos, Quaternion.identity);
                }
            }
            else if (_tracking)
            {
                if (debug) Debug.Log("leap hand not found, tracking canceled");
                CheckEndPos();
            }
        }

        //	private Vector3 BasePos(Hand h1, Hand h2)
        //	{
        //		return (h1.Basis.translation.ToVector3()+h2.Basis.translation.ToVector3())/2;
        //	}

        private Vector3 PalmPos(Hand h1, Hand h2)
        {
            return (HandModel1.transform.position
                    + HandModel2.transform.position
                    + h1.PalmPosition.ToVector3()
                    + h2.PalmPosition.ToVector3()
                    ) / 2;
        }

        private void CheckEndPos()
        {
            Vector3 endPos = _lastPos;

            //		Instantiate(DebugDot, endPos, Quaternion.identity);

            float diff = VectorComponent(_startPos, DistanceAxis) - VectorComponent(endPos, DistanceAxis);

            if (Mathf.Abs(diff) > DistanceRequired)
            {
                if (debug) Debug.Log("tracking completed with enough distance " + diff);
                (diff > 0 ? OnDownwardsComplete : OnUpwardsComplete).Invoke();
                _tracking = false;
                if (debug) Instantiate(DebugDot, endPos, Quaternion.identity);
            }
            else if (!DetectorLogicGate.IsActive)
            {
                if (debug) Debug.Log("tracking completed without enough distance " + diff);
                _tracking = false;
                if (debug) Instantiate(DebugDot, endPos, Quaternion.identity);
            }
        }

        private static float VectorComponent(Vector3 vec, Direction dir)
        {
            return dir == Direction.X ? vec.x : dir == Direction.Y ? vec.y : vec.z;
        }

        public enum Direction { X, Y, Z }
    }

}
