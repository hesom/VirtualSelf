using UnityEngine;
using UnityEngine.Events;

namespace VirtualSelf.Ballmaze
{
    public class EventOnCollision : MonoBehaviour
    {
        public UnityEvent _OnTriggerEnter;
        public string RequiredTag;

        void OnTriggerEnter(Collider other)
        {
            if (RequiredTag != "") {
                if (other.CompareTag(RequiredTag)) _OnTriggerEnter.Invoke();
            }
            else {
                _OnTriggerEnter.Invoke();
            }
        }
    }
}