using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

[RequireComponent(typeof(InteractionBehaviour))]
public class HandlesMover : MonoBehaviour {

    public Rigidbody ball;
    public BodyMover bodyMover;

    private InteractionBehaviour intObj;

    private Rigidbody boardGroupBody;
    private Vector3 offsetPos;
    private Quaternion offsetRot;

    void Start() {

        intObj = GetComponent<InteractionBehaviour>();

        boardGroupBody = bodyMover.gameObject.GetComponent<Rigidbody>();
        offsetPos = transform.localPosition;
        offsetRot = Quaternion.Inverse(transform.localRotation);

        intObj.OnGraspBegin += GraspBegins;
        intObj.OnGraspEnd += GraspEnds;
        intObj.OnGraspStay += GraspStays;
        intObj.OnGraspedMovement += GraspMovement;
        //intObj.manager.OnPostPhysicalUpdate += applyXAxisWallConstraint;
    }

    private void GraspBegins() {

        //Debug.Log(this.gameObject.name + ": Grasp begins.");
        bodyMover.Disabled = true;
        boardGroupBody.isKinematic = true;
        boardGroupBody.useGravity = false;
    }

    private void GraspEnds() {

        //Debug.Log(this.gameObject.name + ": Grasp ends.");
        bodyMover.Disabled = false;
        //boardGroupBody.isKinematic = false;
        //boardGroupBody.useGravity = true;
    }

    private void GraspStays() {

        // Debug.Log(this.gameObject.name + ": Is grasping...");
    }

    private void GraspMovement(Vector3 presolvedPos, Quaternion presolvedRot,
                               Vector3 solvedPos, Quaternion solvedRot,
                               List<InteractionController> graspingController) {

        // Project the vector of the motion of the object due to grasping along the world X axis.
        //Vector3 movementDueToGrasp = solvedPos - presolvePos;
        //float xAxisMovement = movementDueToGrasp.x;

        // Move the object back to its position before the grasp solve this frame,
        // then add just its movement along the world X axis.
        // _intObj.rigidbody.position = presolvePos;
        //_intObj.rigidbody.position += Vector3.right * xAxisMovement;

        Matrix4x4 m = Matrix4x4.Rotate(solvedRot);
        Vector3 offset = m * Matrix4x4.Rotate(offsetRot) * offsetPos;

        boardGroupBody.position = (gameObject.transform.position - offset);
        //boardGroupBody.MoveRotation(solvedRot * offsetRot);
        boardGroupBody.rotation = solvedRot * offsetRot; // MoveRotation is a gradual change

        // not sure if this makes sense 
        Vector3 velocity = solvedPos - presolvedPos;
        ball.velocity += velocity;

        //Debug.Log(this.gameObject.name + ": Is grasping and moving...");
    }

    private void applyXAxisWallConstraint() {
        /*
        // This constraint forces the interaction object to have a positive X coordinate.
        Vector3 objPos = _intObj.rigidbody.position;
        if (objPos.x < 0F)
        {
            objPos.x = 0F;
            _intObj.rigidbody.position = objPos;

            // Zero out any negative-X velocity when the constraint is applied.
            Vector3 objVel = _intObj.rigidbody.velocity;
            if (objVel.x < 0F)
            {
                objVel = 0F;
                _intObj.rigidbody.velocity = objVel;
            }
        }*/

        //Debug.Log("2222");
    }

}