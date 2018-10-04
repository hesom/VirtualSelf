using UnityEngine;
using Oscillator = VirtualSelf.Utility.AnimationUtils.Oscillator;


namespace VirtualSelf.Utility {


/// <summary>
/// TODO: Fill out this class description: LightAnimator
/// </summary>
[RequireComponent(typeof(Light))]
public sealed class LampAnimator : MonoBehaviour {
    
    /* ---------- Enumerations ---------- */

    public enum AnimationMode {

        Static,
        Blink,
        Breathe
    }
    

    /* ---------- Variables & Properties ---------- */

    public Material LampMaterial;

    public Color ColorLightOff;

    public Color ColorLightOn;

    public AnimationMode AniMode = AnimationMode.Breathe;

    public float BreatheTimeLength = 2.5f;

    public Oscillator LightIntensityOscillator;
    
    private Light lampLight;
    
    private Oscillator emissionOscillator;

    private float currentEmission;


    /* ---------- Methods ---------- */

    private void Start() {

        lampLight = GetComponent<Light>();

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

        lampLight.intensity = LightIntensityOscillator.Oscillate();
    }


    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}