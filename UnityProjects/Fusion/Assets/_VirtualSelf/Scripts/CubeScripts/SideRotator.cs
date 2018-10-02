using System.Collections;
using UnityEngine;

namespace VirtualSelf.CubeScripts
{
	/// <summary>
	/// Statemachine to rotate the cubes
	/// </summary>
	public sealed class SideRotator
	{
		public enum State
		{
			Idle,
			Rotating,
			Correcting
		}

	    private readonly Cube2X2 rubiksCube;
        private readonly Cube2X2 ghostCube;
	    private GameObject[] rotationCubes;
        private GameObject[] ghostRotationCubes;

	    public State CurrentState { get; private set; }
	    public float CurrentAngle { get; private set; }
	    public Cube2X2.Side RotatingSide { get; private set; }
	    public readonly IndicatorRing Indicator;

		public SideRotator(Cube2X2 cube, Cube2X2 ghostRubiksCube, IndicatorRing indicatorRing)
		{
			rubiksCube = cube;
            ghostCube = ghostRubiksCube;
		    Indicator = indicatorRing;
		    CurrentState = State.Idle;
			CurrentAngle = 0f;
			RotatingSide = Cube2X2.Side.Left;
			rotationCubes = rubiksCube.CubesOnSide(RotatingSide);
            ghostRotationCubes = ghostCube.CubesOnSide(RotatingSide);
        }

        /// <summary>
        /// Setup function to define an initial rotation for the cube.
        /// Use preferably only in Awake() or Start() of some Monobehaviour.
        /// </summary>
	    public void RotateFullTurns(Cube2X2.Side side, int clockWiseTurns)
        {
            if (CurrentState != State.Idle) return;
	        RotatingSide = side;
	        rotationCubes = rubiksCube.CubesOnSide(RotatingSide);
            ghostRotationCubes = ghostCube.CubesOnSide(RotatingSide);
            RotateCubes(90f * clockWiseTurns);
            rubiksCube.RotateCubes(RotatingSide, clockWiseTurns);
            ghostCube.RotateCubes(RotatingSide, clockWiseTurns);
	        CurrentAngle = 0f;
	    }

		/// <summary>
		/// Starts the rotating state and specifies the rotating cubes.
		/// </summary>
		public void StartRotation(Cube2X2.Side side)
		{
			if (CurrentState == State.Idle)
			{
				RotatingSide = side;
				CurrentAngle = 0f;
				rotationCubes = rubiksCube.CubesOnSide(RotatingSide);
                ghostRotationCubes = ghostCube.CubesOnSide(RotatingSide);
                CurrentState = State.Rotating;
                Indicator.SetColorToRotate();
			}
			else
			{
				Debug.LogWarning("StartRotation() called before State was Idle again.");
			}
		}

		/// <summary>
		/// Stops the rotating state and starts the correction coroutine.
		/// </summary>
		public void StopRotation()
		{
			if (CurrentState == State.Rotating)
			{
				CurrentState = State.Correcting;
			    Indicator.SetColorToCorrect();
				Indicator.StartCoroutine(CorrectSideRotation());
			}
			Indicator.SetColorToDefault();
		}

		/// <summary>
		/// Rotate cubes of current <see cref="RotatingSide"/> around the parent center
		/// by given <paramref name="angle"/>. Call this function only between
		/// <see cref="StartRotation"/> and <see cref="StopRotation"/>. 
		/// </summary>
		/// <param name="angle">in degrees</param>
		public void RotateCubes(float angle)
		{
			var point = rubiksCube.ParentTransform.position;
			var axis = rubiksCube.SideNormal(RotatingSide);
			foreach (var cube in rotationCubes)
			{
			    cube.transform.RotateAround(point, axis, angle);
			}

            point = ghostCube.ParentTransform.position;
            axis = ghostCube.SideNormal(RotatingSide);
            foreach (var cube in ghostRotationCubes)
            {
                cube.transform.RotateAround(point, axis, angle);
            }

            CurrentAngle += angle;
		}

		/// <summary>
		/// Starts correcting the rotation to a full 90degree turn on the cube.
		/// Has to be called from a Monobehaviour with StartCoroutine()
		/// </summary>
		private IEnumerator CorrectSideRotation()
		{
			const float correctionSpeed = 180f;
			int turnsToCorrect = (int)Mathf.Round(CurrentAngle / 90f);
			float finalAngle = turnsToCorrect * 90f;

			float diff = Mathf.Abs(finalAngle - CurrentAngle);
			float sign = Mathf.Sign(finalAngle - CurrentAngle);

			while (diff > 0.001f)
			{
				float angle = sign * Mathf.Min(diff, correctionSpeed * Time.deltaTime);
				RotateCubes(angle);

				diff = Mathf.Abs(finalAngle - CurrentAngle);
				yield return null;
			}

			rubiksCube.RotateCubes(RotatingSide, turnsToCorrect);
            ghostCube.RotateCubes(RotatingSide, turnsToCorrect);
			CurrentState = State.Idle;
            Indicator.SetColorToDefault();
		}
	}
}
