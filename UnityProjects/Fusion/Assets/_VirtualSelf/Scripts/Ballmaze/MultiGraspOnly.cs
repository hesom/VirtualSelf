using System;
using Leap.Unity.Infix;
using Leap.Unity.Interaction;
using UnityEngine;

namespace VirtualSelf.Ballmaze
{
    [RequireComponent(typeof(InteractionBehaviour))]
    public class MultiGraspOnly : MonoBehaviour
    {
        public enum UpdateMethod {Update, LateUpdate, FixedUpdate, PostPhysics, PrePhysics}

        public UpdateMethod UpdateIn;
        public InteractionHand Left;
        public InteractionHand Right;
        public InteractionBehaviour Swap;

        private InteractionBehaviour _ib;
        private bool _leftGrasp;
        private bool _rightGrasp;
        private bool _isMoving;
        private bool _triggeringRelease;
        private bool _newGraspIsLeft;
        private Vector3 _leftGraspOffset;
        private Vector3 _rightGraspOffset;
        private Rigidbody _rb;
        private Quaternion _startRot;

        private Vector3 offset;
        private Quaternion leftOffsetRot;
        private Quaternion rightOffsetRot;


        private Vector3 freezePosition;
        private Quaternion freezeRotation;
	
        // Use this for initialization
        void Start ()
        {
            _ib = GetComponent<InteractionBehaviour>();
//		_ib.moveObjectWhenGrasped = false;
            _rb = GetComponent<Rigidbody>();
		
            Left.OnGraspBegin += LeftGrasp;
            Left.OnGraspEnd += LeftGraspEnd;
            Right.OnGraspBegin += RightGrasp;
            Right.OnGraspEnd += RightGraspEnd;

            PhysicsCallbacks.OnPostPhysics += PostPhysics;
            PhysicsCallbacks.OnPrePhysics += PrePhysics;
        }

        void Update()
        {
            if (UpdateIn == UpdateMethod.Update) Move();
        }

        void FixedUpdate()
        {
            if (UpdateIn == UpdateMethod.FixedUpdate) Move();
        }

        void LateUpdate()
        {
            if (UpdateIn == UpdateMethod.LateUpdate) Move();
        }

        void PostPhysics()
        {
            if (UpdateIn == UpdateMethod.PostPhysics) Move();
        }

        void PrePhysics()
        {
            if (UpdateIn == UpdateMethod.PrePhysics) Move();
        }

        // called by a unityevent on the hand models
        public void LeftFinish()
        {
            _leftGrasp = false;
        }

        public void RightFinish()
        {
            _rightGrasp = false;
        }
	
        /// <summary> isolate the y-Component of a rotation </summary>
        private Quaternion Yrotation(Quaternion q)
        {
            float theta = Mathf.Atan2(q.y, q.w);

            // quaternion representing rotation about the y axis
            return new Quaternion(0, Mathf.Sin(theta), 0, Mathf.Cos(theta));
        }

        private bool _somethingwasmoving;


        public enum State
        {
            NothingGrasp,
            SingleGrasp,
            MultiGrasp
        }

        public State currentState = State.NothingGrasp;

        void SetFreezeState()
        {
            freezePosition = lastPosition;
            freezeRotation = lastRotation;
		
            _rb.position = freezePosition;
            _rb.rotation = freezeRotation;
        }


        private Vector3 lastPosition;
        private Quaternion lastRotation;

        private void Transition(State from, State to)
        {
            if (from != to)
            {
                currentState = to;
                Debug.Log("Transition "+from+" -> "+to);
            }
        }
	
        void Move()
        {
            bool multiGrasped = _leftGrasp && _rightGrasp;
            bool somethingGrasped = _leftGrasp || _rightGrasp;
		
            var nextState = UpdateState(multiGrasped, somethingGrasped);
		
            switch (currentState)
            {
                case State.NothingGrasp:
                    if (nextState == State.SingleGrasp)
                        SetFreezeState();
                    break;
                case State.SingleGrasp:
                    _rb.position = freezePosition;
                    _rb.rotation = freezeRotation;
                    if (nextState == State.NothingGrasp)
                    {
                        Left.ReleaseGrasp();
                        Right.ReleaseGrasp();
                    }
                    break;
                case State.MultiGrasp:
                    if (nextState == State.SingleGrasp)
                    {
                        SetFreezeState();
                    }
                    else if (nextState == State.NothingGrasp)
                    {
                        Left.ReleaseGrasp();
                        Right.ReleaseGrasp();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            lastPosition = _rb.position;
            lastRotation = _rb.rotation;

            Transition(currentState, nextState);
//		currentState = nextState;

            return;
		
		
		
		
		
		
            if (multiGrasped)
            {
                if (_isMoving)
                {
                    freezePosition = _rb.position;
                    freezeRotation = _rb.rotation;
                }
            }
            else
            {
                if (somethingGrasped)
                {
                    if (!_somethingwasmoving)
                    {
                        freezePosition = _rb.position;
                        freezeRotation = _rb.rotation;
                    }

                    _rb.position = freezePosition;
                    _rb.rotation = freezeRotation;
                }
            }

            _somethingwasmoving = somethingGrasped;
		

            _isMoving = multiGrasped;
		
            return;
            if (multiGrasped)
            {
                Vector3 pos = (Left.GetGraspPoint() + Right.GetGraspPoint()) * 0.5f;
                if (_isMoving == false)
                {
                    offset = _rb.position - pos;
                    leftOffsetRot = Quaternion.Inverse(Left.rotation);
                    rightOffsetRot = Quaternion.Inverse(Right.rotation);
                }
			
                _rb.position = pos + offset;

                Quaternion rot = Left.rotation * leftOffsetRot;
			
//			Quaternion rot = Quaternion.LookRotation(Left.position - Right.position, Quaternion.Lerp(Left.rotation, Right.rotation, 0.5f).GetUp());
			
//			Quaternion rot = Quaternion.LookRotation(Left.position - Right.position, (Left.rotation.GetUp()+Right.rotation.GetUp())/2);
//			Quaternion rot = Quaternion.LookRotation(Left.GetGraspPoint() - Right.GetGraspPoint(), (Left.rotation.GetUp()+Right.rotation.GetUp())/2);
//			Quaternion rot = Quaternion.LookRotation(Left.GetGraspPoint() - Right.GetGraspPoint(), (Left.rotation.GetForward()));
//			Quaternion rot = Quaternion.LookRotation(offset, (Left.rotation.GetUp()+Right.rotation.GetUp())/2);
//			_rb.MoveRotation(rot);
                _rb.rotation =  rot;
            }
		
            Debug.Log(Right.rotation.GetRight()+" "+Right.rotation.GetUp()+" "+Right.rotation.GetForward()); 
		
            _isMoving = multiGrasped;
            return;

            if (_isMoving != multiGrasped)
            {
                if (multiGrasped)
                {
                    // just started double grasping
                    _leftGraspOffset = Left.GetGraspPoint() - _rb.position;
//				Debug.Log(Left.GetGraspPoint()+" "+_rb.position);
//				_leftGraspOffset.y = 0;
                    _rightGraspOffset = Right.GetGraspPoint() - _rb.position;
//				_rightGraspOffset.y = 0;
                    _startRot = Quaternion.AngleAxis(_rb.rotation.eulerAngles.y, Vector3.up);
//				_startRot = Yrotation(_rb.rotation);
                }
            }
		
            if (multiGrasped)
            {
                Vector3 left = Left.position;
                Vector3 right = Right.position;			
//			Vector3 left = Left.GetGraspPoint() - _leftGraspOffset;
//			Vector3 right = Right.GetGraspPoint() - _rightGraspOffset;
                Vector3 pos = (left + right) / 2;
//			Vector3 pos = (Left.GetGraspPoint() + Right.GetGraspPoint()) / 2;
//			Quaternion rot = Quaternion.LookRotation(Left.position - Right.position, (Left.rotation.GetUp()+Right.rotation.GetUp())/2);
//			Quaternion rot = Quaternion.LookRotation((Left.position - Right.position).normalized, Quaternion.Lerp(Left.rotation, Right.rotation, 0.5f).GetUp());
                Quaternion rot = Quaternion.LookRotation(Left.position - Right.position, Quaternion.Lerp(Left.rotation, Right.rotation, 0.5f).GetUp());
//			Quaternion rot = Quaternion.LookRotation(left - right, Quaternion.Lerp(Left.rotation, Right.rotation, 0.5f).GetUp());
//			_rb.MovePosition(pos - (_leftGraspOffset+_rightGraspOffset)/2);
                _rb.MovePosition(pos);
                _rb.MoveRotation(rot * _startRot);
//			_rb.MoveRotation(rot);
            }

            _isMoving = multiGrasped;
        }

        private State UpdateState(bool multiGrasped, bool somethingGrasped)
        {
            State nextState = currentState;
            switch (currentState)
            {

                case State.NothingGrasp:

                    if (multiGrasped)
                        return State.MultiGrasp;
                    else if (somethingGrasped)
                        nextState = State.SingleGrasp;
                    break;
                case State.SingleGrasp:

                    if (multiGrasped)
                        nextState = State.MultiGrasp;
                    else
                    {
                        if (!somethingGrasped)
                            nextState = State.NothingGrasp;
                    }

                    break;
                case State.MultiGrasp:

                    if (!multiGrasped)
                    {
                        if (somethingGrasped)
                            nextState = State.SingleGrasp;
                        else
                            nextState = State.NothingGrasp;
                    }

                    break;
                default:
                    throw new NotImplementedException(nextState.ToString());
            }
            return nextState;
        }


        private void CheckMulti()
        {	
            if (_triggeringRelease) return;
		
            bool movingNow = _leftGrasp && _rightGrasp;
            Debug.Log("both hands on: "+movingNow);

            if (movingNow != _isMoving)
            {
//			_triggeringRelease = true;
//			Left.ReleaseGrasp();
//			Right.ReleaseGrasp();
//			
//			
//			_ib.moveObjectWhenGrasped = true;
//			Left.TryGrasp(GetComponent<InteractionBehaviour>());
//			Right.TryGrasp(GetComponent<InteractionBehaviour>());
//			
//			_triggeringRelease = false;
//			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                if (movingNow)
                {
                    if (_newGraspIsLeft)
                    {
                        // swap object is in right hand
//					Right.SwapGrasp(_ib);
                    }
                    else
                    {
                        // swap object is in left hand
//					Left.SwapGrasp(_ib);
                    }
//				GetComponent<SwapGraspExample>().scheduleSwap();
				
                }
            }
            else
            {
//			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
//			_ib.moveObjectWhenGrasped = false;
            }
		

//		if (!movingNow && movingNow != _isMoving)
//		{
////			Debug.Log("releasing grasp now");
//			_triggeringRelease = true;
//			Left.ReleaseGrasp();
//			Right.ReleaseGrasp();
//			_triggeringRelease = false;
//		}
		
            _isMoving = movingNow;
        }

        private void LeftGrasp()
        {
            _leftGrasp = true;
            _newGraspIsLeft = true;
//		GetComponent<SwapGraspExample>().scheduleSwap();
//		CheckMulti();
        }
	
        private void LeftGraspEnd()
        {
            _leftGrasp = false;
//		CheckMulti();
        }	
	
        private void RightGrasp()
        {
            _rightGrasp = true;
//		Right.SwapGrasp(Swap);
            _newGraspIsLeft = false;
//		CheckMulti();
        }
	
        private void RightGraspEnd()
        {
            _rightGrasp = false;
//		CheckMulti();
        }
    }
}