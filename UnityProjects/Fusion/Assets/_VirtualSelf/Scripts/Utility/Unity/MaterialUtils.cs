using System;
using UnityEngine;

namespace VirtualSelf.Utility {


/// <summary>
/// TODO: Fill out this class description: MaterialUtils
/// </summary>
public static class MaterialUtils {

    /* ---------- Variables & Properties ---------- */

    public const string MatPropEmissionKeywordName = "_EMISSION";
    public const string MatPropEmissionColorName = "_EmissionColor";


    /* ---------- Public Methods ---------- */

    public static void SetEmissionState(Material material, bool isActivated) {

        if (isActivated) { material.EnableKeyword(MatPropEmissionKeywordName); }
        else { material.DisableKeyword(MatPropEmissionKeywordName); }
    }
    
    public static void SetEmissionValue(Material material, float emissionValue) {

        material.SetColor(
            MatPropEmissionColorName,
            (material.GetColor(MatPropEmissionColorName) * Mathf.LinearToGammaSpace(emissionValue))
        );
    }

    public static void SetEmissionColor(Material material, Color color) {
        
        material.SetColor(MatPropEmissionColorName, color);
    }

    public static void SetEmissionColorToRegularColor(Material material) {
        
        SetEmissionColor(material, material.color);
    }
    

    /* ---------- Inner Classes ---------- */






}

}