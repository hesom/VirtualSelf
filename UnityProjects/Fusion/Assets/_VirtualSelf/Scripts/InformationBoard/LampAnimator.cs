using UnityEngine;
using Oscillator = VirtualSelf.Utility.AnimationUtils.Oscillator;


namespace VirtualSelf.Utility {


/// <summary>
/// TODO: Fill out this class description: LightAnimator
/// </summary>
public sealed class LampAnimator : MonoBehaviour {
    
    /* ---------- Enumerations ---------- */

    public enum AnimationMode {

        Breathe,
        Blink
    }
    

    /* ---------- Variables & Properties ---------- */

    public Material LampMaterial;

    public Light LampLight;

    public Color ColorLightOff;

    public Color ColorLightOn;

    public AnimationMode AniMode = AnimationMode.Breathe;

    public float BreatheTimeLength = 2.5f;

    public Oscillator LightIntensityOscillator;
    
    private Oscillator emissionOscillator;

    private float currentEmission;


    /* ---------- Methods ---------- */

    private void Start() {

        emissionOscillator = new Oscillator {
            OscillType = LightIntensityOscillator.OscillType,
            MinValue = 0.0f,
            MaxValue = 1.0f,
            LengthMultiplier = BreatheTimeLength
        };
    }

    private void Update() {

        currentEmission = emissionOscillator.Oscillate();
        
        Color finalColor = ColorLightOn * Mathf.LinearToGammaSpace(currentEmission);
        
        LampMaterial.SetColor("_EmissionColor", finalColor);

        LampLight.intensity = LightIntensityOscillator.Oscillate();
    }


    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}