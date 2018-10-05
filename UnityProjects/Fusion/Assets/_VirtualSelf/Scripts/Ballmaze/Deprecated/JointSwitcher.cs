using UnityEngine;

namespace VirtualSelf.Ballmaze
{
    public class JointSwitcher : MonoBehaviour {

        private Rigidbody[] rbs = new Rigidbody[2];

        // Use this for initialization
        void Start () {
            FixedJoint[] j = GetComponents<FixedJoint>();
            rbs[0] = j[0].connectedBody;
            rbs[1] = j[1].connectedBody;
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        void OnJointBreak(float breakforce)
        {
            Debug.Log("joint broke");
            //j[0].

            //gameObject.GetComponent<HingeJoint>()
            if (GetComponents<FixedJoint>().Length == 1)
            {
                Debug.Log("adding joint");
                FixedJoint fj = gameObject.AddComponent<FixedJoint>();
                fj.connectedBody = rbs[0];
                fj.breakTorque = 1;
            }
            else Debug.Log("remaining joints: " + GetComponents<FixedJoint>().Length);
        }
    }
}