using UnityEditor;
using UnityEngine;


namespace VirtualSelf.GameSystems {

    
/// <summary>
/// This is the template class for creating keypad keycodes in the form of
/// <see cref="ScriptableObject"/> asset files. These keycodes are used in different parts of the
/// game, ultimately to connect all the rooms (scenes) we've made together.<br/>
/// TODO: More, link to other classes
/// </summary>
[CreateAssetMenu(
    fileName = "Keycode",
    menuName = "Keycodes/Keycode"
)]
public sealed class Keycode : ScriptableObject { 
    
    /* ---------- Variables & Properties ---------- */

    public const string FieldNameDigitOne = nameof(digitOne);
    public const string FieldNameDigitTwo = nameof(digitTwo);
    public const string FieldNameDigitThree = nameof(digitThree);
    public const string FieldNameDigitFour = nameof(digitFour);
    public const string FieldNameCodeString = nameof(codeString);
    public const string FieldNameRenameAutomatically = nameof(renameAutomatically);
    
    /// <summary>
    /// Contains all the possible values the digits of a keycode can have. Since we are using a
    /// classical keypad to type in keycodes, these are the numbers 0 to 9.
    /// </summary>
    public static readonly string[] PossibleDigits = 
            { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

    /// <summary>
    /// The first digit of the keycode. Together with the other three, this is stored in
    /// <see cref="codeString"/> as the final keycode.
    /// </summary>
    [SerializeField]
    private int digitOne;
    
    /// <summary>
    /// The second digit of the keycode. Together with the other three, this is stored in
    /// <see cref="codeString"/> as the final keycode.
    /// </summary>
    [SerializeField]
    private int digitTwo;
    
    /// <summary>
    /// The third digit of the keycode. Together with the other three, this is stored in
    /// <see cref="codeString"/> as the final keycode.
    /// </summary>
    [SerializeField]
    private int digitThree;
    
    /// <summary>
    /// The fourth digit of the keycode. Together with the other three, this is stored in
    /// <see cref="codeString"/> as the final keycode.
    /// </summary>
    [SerializeField]
    private int digitFour;

    /// <summary>
    /// Whether to rename this ScriptableObject asset automatically, whenever the code is changed,
    /// to exactly the code (e.g. for the code "1408", the asset file would be renamed into "1408"),
    /// or not. If this is <c>false</c>, the file will never be renamed by the code.
    /// </summary>
    [SerializeField]
    private bool renameAutomatically = true;

    /// <summary>
    /// The final resulting keycode, (necessarily) as a string. This is automatically created in
    /// <see cref="OnValidate"/>, from <see cref="digitOne"/> to <see cref="digitFour"/>, whenever a
    /// change to one of the digits is made.
    /// </summary>
    [SerializeField]
    private string codeString;

    /// <summary>
    /// Whether this keycode has been discovered by the player yet, or not. This means nothing to
    /// this class, but is important for other parts of the game.<br/>
    /// This can be set in the Inspector for testing purposes, but is ultimately supposed to be set
    /// by code.
    /// </summary>
    public bool IsDiscovered;


    /* ---------- Methods ---------- */

    private void OnValidate() {

        codeString = (
            digitOne.ToString() + digitTwo.ToString() +
            digitThree.ToString() + digitFour.ToString()
        );
        
        // Debug.Log("code is: " + codeString);

        if (renameAutomatically) {
            
            /* This code uses Unity's built-in asset database related utility methods to find the
             * ScriptableObject asset this particular instance belongs to, and to rename that
             * object accordingly. */
            
            string assetPath =  AssetDatabase.GetAssetPath(GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, codeString);
            AssetDatabase.SaveAssets();   
        }
    }
}

}