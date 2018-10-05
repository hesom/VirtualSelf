using UnityEngine;

namespace VirtualSelf.Ballmaze
{
    public class CopyTransformTo : MonoBehaviour {

        public GameObject[] target;

        private Vector3[] diff;

        // Use this for initialization
        void Start () {
            diff = new Vector3[target.Length];
            for (int i=0;i<target.Length;i++)
            {
                diff[i] = transform.position - target[i].transform.position;
            }
        }
	
        // Update is called once per frame
        void FixedUpdate () {
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i].transform.hasChanged)
                {
                    target[i].transform.hasChanged = false;
                    Rigidbody rb = target[i].GetComponent<Rigidbody>();
                    Vector3 newpos = transform.position - diff[i];
                    if (rb == null) target[i].transform.position = newpos;
                    else rb.position = newpos;
                }
            }
        }
    }
}