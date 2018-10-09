using System.Collections;
using UnityEngine;


namespace VirtualSelf.Utility {

    
/// <summary>
/// A collection of different static utility methods and inner classes that provide functionality
/// for dealing with time within Unity.
/// </summary>
public static class TimeUtils {
    
/* ---------- Inner Classes ---------- */
    
    /// <summary>
    /// This class contains methods for making methods in Unity wait for different amounts of time,
    /// in different time units.<br/>
    /// Each of the methods uses a different time unit (e.g. seconds, frames, etc.), as denoted by
    /// its name.
    /// <remarks>
    /// This is taken from here: https://forum.unity.com/threads/how-to-wait-for-a-frame-in-c.24616/
    /// </remarks>
    /// </summary>
    public static class WaitFor {
        
        /// <summary>
        /// Wait for the given amount of frames.
        /// </summary>
        /// <remarks>
        /// This method yields <c>null</c>. As per the rules within Unity, yielding for anything but
        /// a <see cref="YieldInstruction"/>, or an <see cref="IEnumerator"/> or a
        /// <see cref="MonoBehaviour.StartCoroutine(string)"/>, the method will wait for exactly a
        /// single frame.
        /// </remarks>
        /// <param name="frameCount">
        /// The amount of frames to wait. Must be positive (>0), or the method will not wait at all.
        /// </param>
        public static IEnumerator Frames(int frameCount) {
            
            while (frameCount > 0) {
                
                frameCount--;
                yield return (null);
            }
        }
    }
}

}