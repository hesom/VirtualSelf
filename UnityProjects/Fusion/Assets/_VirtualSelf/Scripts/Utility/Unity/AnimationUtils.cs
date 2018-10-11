using System;
using System.Collections;
using UnityEngine;


namespace VirtualSelf.Utility {


/// <summary>
/// A collection of static utility methods, and classes, for doing simple things related to
/// animation (e.g. animating <see cref="GameObject"/>s or values, in code, in ways that do not have
/// a simple, built-in solution.
/// </summary>
public static class AnimationUtils {

    /* ---------- Public Methods ---------- */

    /// <summary>
    /// Moves the given object from the point (in 3D space) <paramref name="source"/> to the point
    /// <paramref name="destination"/>, smoothly within the given time.
    /// </summary>
    /// <remarks>
    /// This method must be run as a Unity coroutine, e.g. via
    /// <see cref="MonoBehaviour.StartCoroutine(string)"/>.<br/>
    /// This method runs <see cref="MoveObjectFromToAtSpeed"/> to perform the actual moving.
    /// </remarks>
    /// <param name="objectToMove">
    /// The object to move. This method is intended for objects living in 3D space. The rotation of
    /// the object is not touched by this method.
    /// </param>
    /// <param name="source">The point in 3D space the object starts off at.</param>
    /// <param name="destination">
    /// The point in 3D space the object is supposed to be at after this method has finished
    /// running.
    /// </param>
    /// <param name="time">
    /// The amount of time the object should be moving before arriving at
    /// <paramref name="destination"/>. This is in seconds.
    /// </param>
    /// <returns>Nothing. (This method is run as a Unity coroutine.</returns>
    public static IEnumerator MoveObjectFromToInTime(
        Transform objectToMove,
        Vector3 source, Vector3 destination,
        float time) {

        float distance = (source - destination).magnitude;
        float targetSpeed = (distance / (time));

        return (MoveObjectFromToAtSpeed(objectToMove, source, destination, targetSpeed));
    }
    
    /// <summary>
    /// This method is the same as <see cref="MoveObjectFromToInTime"/>, but is intended to be
    /// used for Unity UI elements living in 2D space, within e.g. a <see cref="Canvas"/>.<br/>
    /// For 3D elements living in 3D space, and for more details, refer to the other method.
    /// </summary>
    /// <remarks>
    /// This method moves the element via <see cref="RectTransform.anchoredPosition"/>.
    /// </remarks>
    /// <seealso cref="MoveObjectFromToInTime"/>
    public static IEnumerator MoveUiElementFromToInTime(
        RectTransform elementToMove,
        Vector2 source, Vector2 destination,
        float time) {

        float distance = (source - destination).magnitude;
        float targetSpeed = (distance / (time));

        return (MoveUiElementFromToAtSpeed(elementToMove, source, destination, targetSpeed));
    }
    
    /// <summary>
    /// Moves the given object from the point (in 3D space) <paramref name="source"/> to the point
    /// <paramref name="destination"/>, smoothly at the given (constant) speed.
    /// </summary>
    /// <remarks>
    /// This method must be run as a Unity coroutine, e.g. via
    /// <see cref="MonoBehaviour.StartCoroutine(string)"/>.<br/>
    /// The code for this method is taken from here:<br/>
    /// https://gamedev.stackexchange.com/a/100549/111766
    /// </remarks>
    /// <param name="objectToMove">
    /// The object to move. This method is intended for objects living in 3D space. The rotation of
    /// the object is not touched by this method.
    /// </param>
    /// <param name="source">The point in 3D space the object starts off at.</param>
    /// <param name="destination">
    /// The point in 3D space the object is supposed to be at after this method has finished
    /// running.
    /// </param>
    /// <param name="speed">
    /// The (constant) speed to move the object at. This is in (Unity) units per second.
    /// </param>
    /// <returns>Nothing. (This method is run as a Unity coroutine.</returns>
    public static IEnumerator MoveObjectFromToAtSpeed(
        Transform objectToMove,
        Vector3 source, Vector3 destination,
        float speed) {
        
        /* We use "fixedDeltaTime" and "WaitForFixedUpdate" here, to make sure that the object moves
         * at a constant speed. "Update()" and "deltaTime" run at a variable speed, and also Unity's
         * "time scale" settings could be changed in the meantime, to produce e.g. slow motion
         * effects, which would break this movement. */
        
        float distance = (source - destination).magnitude;
        float step = (speed / distance) * Time.fixedDeltaTime;
        
        float t = 0;
        while (t <= 1.0f) {
            
            t += step;
            objectToMove.position = Vector3.Lerp(source, destination, t);
            
            yield return (new WaitForFixedUpdate());
        }
        
        /* Makes sure that the object arrives at the exact destination location, since adding "step"
         * to "t" might jump higher than 1.0 at the end, so the last step would be left out. */
        
        objectToMove.position = destination;
    }

    /// <summary>
    /// This method is the same as <see cref="MoveObjectFromToAtSpeed"/>, but is intended to be
    /// used for Unity UI elements living in 2D space, within e.g. a <see cref="Canvas"/>.<br/>
    /// For 3D elements living in 3D space, and for more details, refer to the other method.
    /// </summary>
    /// <remarks>
    /// This method moves the element via <see cref="RectTransform.anchoredPosition"/>.
    /// </remarks>
    /// <seealso cref="MoveObjectFromToAtSpeed"/>
    public static IEnumerator MoveUiElementFromToAtSpeed(
        RectTransform elementToMove,
        Vector2 source, Vector2 destination,
        float speed) {
               
        float step = (speed / (source - destination).magnitude) * Time.fixedDeltaTime;
        
        float t = 0;
        while (t <= 1.0f) {
            
            t += step;
            elementToMove.anchoredPosition = Vector2.Lerp(source, destination, t);
            
            yield return (new WaitForFixedUpdate());
        }
        
        /* Makes sure that the object arrives at the exact destination location, since adding "step"
         * to "t" might jump higher than 1.0 at the end, so the last step would be left out. */
        
        elementToMove.anchoredPosition = destination;       
    }


    /* ---------- Inner Classes ---------- */

    [Serializable]
    public class Oscillator {
        
        /* ---------- Enumerations ---------- */

        public enum OscillationType {

            PingPong,
            Sine
        }
        

        /* ---------- Variables & Properties ---------- */

        public OscillationType OscillType;
        public float MinValue;
        public float MaxValue;
        public float LengthMultiplier;
        
        
        /* ---------- Methods ---------- */

        public float Oscillate() {

            return (Oscillate(Time.time));
        }
        
        public float Oscillate(float customTime) {

            float finalValue;
            float timeValue = (customTime * (1.0f / LengthMultiplier));
            
            if (OscillType == OscillationType.PingPong) {

                float ppValue = Mathf.PingPong(timeValue, (MaxValue - MinValue));
                
                finalValue = MinValue + ppValue;
            }
            else if (OscillType == OscillationType.Sine) {

                float sinValue = Mathf.Sin(timeValue);
                sinValue = ((sinValue + 1.0f) / 2.0f);
                
                finalValue = MinValue + (sinValue * (MaxValue - MinValue));
            }
            else {
                
                throw new NotImplementedException();
            }

            return (finalValue);
        }
    }
}

}