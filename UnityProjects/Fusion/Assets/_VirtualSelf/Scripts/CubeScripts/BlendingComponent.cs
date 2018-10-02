using System.Collections;
using UnityEngine;

namespace VirtualSelf.CubeScripts
{
    public class BlendingComponent : MonoBehaviour
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        [Range(0f, 0.9f)] public float MinAlpha = 0f;
        [Range(0.1f, 1f)] public float MaxAlpha = 0.4f;
        [Range(0.1f, 10f)] public float BlendSpeed = 0.75f;

        private enum State
        {
            Idle,
            BlendingIn,
            BlendingOut
        }

        private Material mat;
        private State currentState;

        private void Awake()
        {
            mat = GetComponent<MeshRenderer>().material;
            var color = mat.color;
            color.a = MinAlpha;
            mat.color = color;
            currentState = State.Idle;
        }

        public void StartBlendIn()
        {
            if (currentState == State.BlendingIn)
                return;

            StopAllCoroutines();

            if (Mathf.Abs(mat.color.a - MaxAlpha) < 0.000001f)
                return;

            currentState = State.BlendingIn;
            // gameObject.SetActive(true);
            StartCoroutine(BlendIn());
        }

        public void StartBlendOut()
        {
            if (currentState == State.BlendingOut)
                return;

            StopAllCoroutines();

            if (Mathf.Abs(mat.color.a - MinAlpha) < 0.000001f)
                return;

            currentState = State.BlendingOut;
            // gameObject.SetActive(true);
            StartCoroutine(BlendOut());
        }

        private IEnumerator BlendIn()
        {
            var color = mat.color;
            float diff = Mathf.Abs(color.a - MaxAlpha);

            while (diff > 0.001)
            {
                color.a += Mathf.Min(diff, BlendSpeed * Time.deltaTime);
                color.a = Mathf.Min(color.a, MaxAlpha);
                mat.color = color;
                diff = Mathf.Abs(color.a - MaxAlpha);
                yield return null;
            }

            currentState = State.Idle;
        }

        private IEnumerator BlendOut()
        {
            var color = mat.color;
            float diff = Mathf.Abs(color.a - MinAlpha);

            while (diff > 0.001)
            {
                color.a -= Mathf.Min(diff, BlendSpeed * Time.deltaTime);
                color.a = Mathf.Max(color.a, MinAlpha);
                mat.color = color;
                diff = Mathf.Abs(color.a - MinAlpha);
                yield return null;
            }

            currentState = State.Idle;
        }
    }
}
