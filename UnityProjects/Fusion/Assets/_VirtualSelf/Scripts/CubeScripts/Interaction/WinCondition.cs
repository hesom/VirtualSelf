using Leap.Unity;
using UnityEngine;

namespace VirtualSelf.CubeScripts.Interaction
{
    public sealed class WinCondition
    {
        private readonly Cube2X2 cube;
        private readonly GameObject[] whiteCubes;
        private bool alreadyWon;

        public WinCondition(Cube2X2 cube)
        {
            this.cube = cube;
            whiteCubes = cube.CubesOnSide(Cube2X2.Side.Top);
        }

        public void WhiteTop()
        {
            if (!WhiteFaceUp(Cube2X2.Position.FrontTopLeft)
                || !WhiteFaceUp(Cube2X2.Position.FrontTopRight)
                || !WhiteFaceUp(Cube2X2.Position.BackTopLeft)
                || !WhiteFaceUp(Cube2X2.Position.BackTopRight)
                || alreadyWon) 
                return;
            alreadyWon = true;
            Debug.Log("You just won by completing the top white side!");

        }

        public void CheckWhiteSideSolved()
        {
            if (alreadyWon) return;

            Vector3 firstNormal = whiteCubes[0].transform.up;
            float firstLength = firstNormal.magnitude;

            for (int i = 1; i < Cube2X2.CubesPerSide; i++)
            {
                Vector3 tmpNormal = whiteCubes[i].transform.up;
                float tmpLength = tmpNormal.magnitude;
                float angle = Mathf.Acos(Vector3.Dot(firstNormal, tmpNormal) / firstLength * tmpLength);
                if (angle > 0.001f) return;
            }

            Debug.Log("Hello there! \n General Kenobi!");
            alreadyWon = true;
        }

        private bool WhiteFaceUp(Cube2X2.Position position)
        {
            var upAxis = cube.Cube(position).transform.up;
            return upAxis.ApproxEquals(cube.ParentTransform.up);
        }
    }
}
