using UnityEngine;

namespace VirtualSelf.Ballmaze
{
    public class Throw : MonoBehaviour
    {

        public float speed = 1;
        public float lifetime = 10;

        // Use this for initialization
        void Start ()
        {
            Transform cam = Camera.main.transform;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.position = cam.position;
            rb.velocity = cam.rotation * new Vector3(0, 0, 1) * speed;

            Destroy(gameObject, lifetime);
        }

        // Update is called once per frame
        void Update () {

        }

        void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
            //GetComponent<SphereCollider>().enabled = false;
        }
    }
}