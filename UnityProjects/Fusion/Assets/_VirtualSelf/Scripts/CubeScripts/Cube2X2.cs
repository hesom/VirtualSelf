using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace VirtualSelf.CubeScripts
{
    /// <summary>
    /// Manages positions and gameobjects related to the 2X2 rubiks cube.
    /// Allows access to cube positions and adjusting them with <see cref="RotateCubes"/>.
    /// </summary>
    /// <remarks>
    /// Initial Positions defined through prefab:
    /// <para>Front = Red</para>
    /// <para>Back = Orange</para>
    /// <para>Left = Green</para>
    /// <para>Right = Blue</para>
    /// <para>Top = White</para>
    /// <para>Bottom = Yellow</para>
    /// </remarks>
    public sealed class Cube2X2
    {
        /* ---------- Variables & Properties ---------- */
        
        public const int SideCount = 6;
        public const int CubeCount = 8;
        public const int CubesPerSide = 4;
        public const float HalfCubeWidth = 0.5f;

        /// <summary>
        /// Specifies the available sides of the cube. The rubiks cube itself is stationary,
        /// therefore each side stays on the same local axis, even after rotating the single cubes.
        /// </summary>
        public enum Side
        {
            Front = 0,
            Back,
            Top,
            Bottom,
            Left,
            Right
        }

        /// <summary>
        /// Specifies all gridlike positions of the 2x2 cube. The rubiks cube itself is stationary,
        /// therefore each position stay on the same local axis, even if the sides rotate.
        /// </summary>
        public enum Position
        {
            FrontTopLeft = 0,
            FrontTopRight,
            FrontBottomLeft,
            FrontBottomRight,
            BackTopLeft,
            BackTopRight,
            BackBottomLeft,
            BackBottomRight
        }

        /// <summary>
        /// Array of cube objects contained in the rubiks cube.
        /// Number of cubes is defined through <see cref="CubeCount"/>
        /// </summary>
        /// <remarks>
        /// Indexing can also be done via <see cref="Position"/> enum.
        /// </remarks>
        private readonly GameObject[] cubes = new GameObject[CubeCount];

        /// <summary>
        /// Contains the indices for each side to go through the cubes clockwise.
        /// </summary>
        /// <remarks>
        /// <para>First index is specified by the wanted <see cref="Side"/></para>
        /// <para>Second index has the clockwise ordered <see cref="Position"/>
        /// of the cubes belonging to the <see cref="Side"/></para>
        /// <para>Total array size is [<see cref="SideCount"/>][<see cref="CubesPerSide"/>]</para>
        /// </remarks>
        [SuppressMessage("ReSharper", "RedundantExplicitArraySize")]
        private static readonly int[][] SideIndices = new int[SideCount][]
        {
            new int[CubesPerSide] {0, 1, 3, 2},
            new int[CubesPerSide] {5, 4, 6, 7},
            new int[CubesPerSide] {0, 4, 5, 1},
            new int[CubesPerSide] {2, 3, 7, 6},
            new int[CubesPerSide] {0, 2, 6, 4},
            new int[CubesPerSide] {1, 5, 7, 3}
        };


        /* ---------- Constructors ---------- */

        /// <summary>
        /// Finds the related cube gameobjects via name lookup defined in the cube prefab.
        /// </summary>
        public Cube2X2(GameObject parent)
        {
            for (int i = 0; i < CubeCount; i++)
                cubes[i] = parent.transform.Find("rubiksCube0" + i).gameObject;
        }


        /* ---------- Methods ---------- */

        /// <summary>
        /// Gets the transform of the parent gameobject, under which all sub cubes are grouped.
        /// </summary>
        public Transform ParentTransform
        {
            get { return cubes[0].transform.parent; }
        }

        public GameObject Cube(Position pos)
        {
            return cubes[(int) pos];
        }

        public GameObject Cube(int pos)
        {
            return cubes[pos];
        }

        public static int[] CubeSideIndices(Side side)
        {
            return SideIndices[(int) side];
        }

        public static int[] CubeSideIndices(int side)
        {
            return SideIndices[side];
        }

        public GameObject[] CubesOnSide(Side side)
        {
            var sideCubes = new GameObject[CubesPerSide];
            for (int i = 0; i < CubesPerSide; i++)
            {
                sideCubes[i] = cubes[SideIndices[(int)side][i]];
            }
            return sideCubes;
        }

        /// <summary>
        /// Normal of the given side in world coordinates.
        /// </summary>
        public Vector3 SideNormal(Side side)
        {
            var transform = cubes[0].transform.parent;
            switch (side)
            {
                case Side.Left:
                    return -transform.right;
                case Side.Right:
                    return transform.right;
                case Side.Top:
                    return transform.up;
                case Side.Bottom:
                    return -transform.up;
                case Side.Front:
                    return -transform.forward;
                case Side.Back:
                    return transform.forward;
                default:
                    return new Vector3(1, 1, 1);
            }
        }

        /// <summary>
        /// Normal at the local scale of the parent object.
        /// </summary>
        public static Vector3 LocalSideNormal(Side side)
        {
            switch (side)
            {
                case Side.Front:
                    return new Vector3(0, 0, -1f);
                case Side.Back:
                    return new Vector3(0, 0, 1f);
                case Side.Top:
                    return new Vector3(0, 1f, 0);
                case Side.Bottom:
                    return new Vector3(0, -1f, 0);
                case Side.Left:
                    return new Vector3(-1f, 0, 0);
                case Side.Right:
                    return new Vector3(1f, 0, 0);
                default:
                    return new Vector3(1, 1, 1);
            }
        }

        /// <summary>
        /// Adjusts the cube positions for the given number of turns.
        /// </summary>
        /// <param name="side">specifies the cubes to turn by specifying a side</param>
        /// <param name="clockwiseTurns">number of clockwise 90 degree turns</param>
        public void RotateCubes(Side side, int clockwiseTurns)
        {
            var indexList = SideIndices[(int) side];
            var tmpCubes = new GameObject[CubesPerSide];
            for (int i = 0; i < CubesPerSide; i++)
            {
                tmpCubes[i] = cubes[indexList[i]];
            }
            
            const int turnsForFullRotation = 4;
            // Rotations add up to 360 degree after 4 turns
            // 3 turns clockwise or 1 turn in the other direciton cancel each other out.
            clockwiseTurns = clockwiseTurns % turnsForFullRotation;
            // This ensures a positive modulo outcome for easier array access.
            clockwiseTurns = (clockwiseTurns + turnsForFullRotation) % turnsForFullRotation;
            
            for (int i = 0; i < turnsForFullRotation; i++)
            {
                int updatedIndex = (i + clockwiseTurns) % turnsForFullRotation;
                cubes[indexList[updatedIndex]] = tmpCubes[i];
            }
        }
    }
}