using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VirtualSelf.Utility {


/// <summary>
/// TODO: Fill out this class description: AnimationUtils
/// </summary>
public sealed class AnimationUtils {

    /* ---------- Public Methods ---------- */





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