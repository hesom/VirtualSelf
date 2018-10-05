using UnityEngine;

namespace VirtualSelf.Ballmaze
{
    public class ResetButton : MonoBehaviour {

        public Rigidbody[] bodies;

        private Vector3[] position;
        private Quaternion[] rotation;

        // Use this for initialization
        void Start () {
            position = new Vector3[bodies.Length];
            rotation = new Quaternion[bodies.Length];
            for (int i=0;i<position.Length;i++)
            {
                position[i] = bodies[i].position;
                rotation[i] = bodies[i].rotation;
            }
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        public void ResetAll()
        {
            for (int i = 0; i < position.Length; i++)
            {
                //bodies[i].velocity = Vector3.zero;
                bodies[i].position = position[i];
                bodies[i].rotation = rotation[i];
                //bodies[i].rotation = Quaternion.identity;
                bodies[i].Sleep();
                bodies[i].velocity = bodies[i].angularVelocity = Vector3.zero;
//		    bodies[i].maxAngularVelocity = 5;
//		    bodies[i].maxDepenetrationVeloCcity = 0.1f;
            }
        }
    }
}