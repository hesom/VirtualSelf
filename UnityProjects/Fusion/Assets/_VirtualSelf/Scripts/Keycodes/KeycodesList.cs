using System;
using System.Collections.Generic;
using RoboRyanTron.SceneReference;
using UnityEngine;

namespace VirtualSelf.GameSystems {


/// <summary>
/// TODO: Fill out this class description: KeycodesList
/// </summary>
[CreateAssetMenu(
    fileName = "KeycodesList",
    menuName = "Keycodes/KeycodesList"
)]
public sealed class KeycodesList : ScriptableObject {

    /* ---------- Variables & Properties ---------- */

    public const string FieldNameKeycodeSceneMappings = nameof(keycodeSceneMappings);

    public IList<KeycodeSceneMapping> KeycodeSceneMappings => keycodeSceneMappings.AsReadOnly();

    [SerializeField]
    private List<KeycodeSceneMapping> keycodeSceneMappings;



    /* ---------- Constructors ---------- */






    /* ---------- Methods ---------- */






    /* ---------- Overrides ---------- */




}

}