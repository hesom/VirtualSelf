using UnityEngine;

namespace VirtualSelf.CubeScripts
{
    public sealed class IndicatorRing : MonoBehaviour
    {
        public Color DefaultColor = Color.gray;
        public Color RotateColor = Color.green;
        public Color CorrectColor = Color.red;
        private Renderer meshRenderer;
        private BlendingComponent blendClass;

        public Cube2X2.Side CurrentSide { get; private set; }

        public bool Active
        {
            set
            {
                if (value)
                    blendClass.StartBlendIn();
                else
                    blendClass.StartBlendOut();
            }
        }

        private void Awake()
        {
            meshRenderer = GetComponent<Renderer>();
            blendClass = GetComponent<BlendingComponent>();
            CurrentSide = Cube2X2.Side.Top;
        }

        public void TransformToSide(Cube2X2.Side side)
        {
            if (side == CurrentSide) return;
            var axis = Cube2X2.LocalSideNormal(side);
            transform.localPosition = axis * Cube2X2.HalfCubeWidth;
            transform.localRotation = Quaternion.identity * Quaternion.LookRotation(axis);
            CurrentSide = side;
        }

        public void SetColorToDefault() {
            SetColorWithoutAlpha(DefaultColor);
        }

        public void SetColorToRotate()
        {
            SetColorWithoutAlpha(RotateColor);
        }

        public void SetColorToCorrect()
        {
            SetColorWithoutAlpha(CorrectColor);
        }


        private void SetColorWithoutAlpha(Color color)
        {
            color.a = meshRenderer.material.color.a;
            meshRenderer.material.color = color;
        }
    }
}
