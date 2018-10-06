using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf.Ballmaze
{
    public class EventOnCollision : MonoBehaviour
    {
        public UnityEvent _OnTriggerEnter;
	
        // Use this for initialization
        void Start () {
		
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        void OnTriggerEnter(Collider other)
        {
            _OnTriggerEnter.Invoke();
        }
    }
}