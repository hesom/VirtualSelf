using System;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;


namespace VirtualSelf.GameSystems {

    
/// <summary>
/// This is the template class for creating keypad keycodes in the form of
/// <see cref="ScriptableObject"/> asset files. These keycodes are used in different parts of the
/// game, ultimately to connect all the rooms (scenes) we've made together.<br/>
/// For each <see cref="Room"/> we want to have, a corresponding keycode asset file should be
/// created, and its values populated.<br/>
/// <br/>
/// This class is used within <see cref="KeycodesList"/>, together with the <see cref="Room"/>s,
/// but any code can obtain a reference to it (preferably by using the Unity Inspector) and then
/// read or modify its values at runtime. There is also an event than can be subscribed to, to
/// listen to relevant state changes.
/// </summary>
[CreateAssetMenu(
    fileName = "Keycode",
    menuName = "Keycodes/Keycode"
)]
public sealed class Keycode : ScriptableObject, ISerializationCallbackReceiver {
    
    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameDigitOne = nameof(digitOne);
    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameDigitTwo = nameof(digitTwo);
    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameDigitThree = nameof(digitThree);
    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameDigitFour = nameof(digitFour);
    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameCodeString = nameof(codeString);
    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameIsDiscovered = nameof(isDiscovered);
    
    /// <summary>
    /// Contains all the possible values the digits of a keycode can have. Since we are using a
    /// classical keypad to type in keycodes, these are the numbers 0 to 9.
    /// </summary>
    public static readonly string[] PossibleDigits = 
            { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    
    /// <summary>
    /// The actual keycode code of this asset instance. This code is created via the Unity Inspector
    /// of this class.
    /// </summary>
    /// <seealso cref="codeString"/>
    public string CodeString => codeString;
    
    /// <summary>
    /// Denotes whether this keycode has already been "discovered" by the player, or not.
    /// Discovering, in this case, means that the player has found the keycode in the game, and can
    /// now use it on the keypad to reach the <see cref="Room"/> associated with that code.
    /// </summary>
    public bool IsDiscovered {

        get {
            if (Application.isPlaying) { return (isDiscoveredRuntimeValue); }
            else { return (isDiscovered); }
        }
        set {
            if (Application.isPlaying) {
                if (value != isDiscoveredRuntimeValue) {
                    isDiscoveredRuntimeValue = value;
                    OnDiscoveredStateChanged.Invoke(this);
                }
            }
            else {
                if (value != isDiscovered) {
                    isDiscovered = value;
                    OnDiscoveredStateChanged.Invoke(this);
                }               
            }
        }
    }
    
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
    /// See <see cref="IsDiscovered"/> for details.
    /// </summary>
    [SerializeField]
    private bool isDiscovered;

    /// <summary>
    /// The final resulting keycode, (necessarily) as a string. This is automatically created in
    /// <see cref="OnValidate"/>, from <see cref="digitOne"/> to <see cref="digitFour"/>, whenever a
    /// change to one of the digits is made.
    /// </summary>
    /// <seealso cref="CodeString"/>
    [SerializeField]
    private string codeString;
    
    /// <summary>
    /// This is used (only) in <see cref="OnValidate"/>, to make it possible to raise
    /// <see cref="OnDiscoveredStateChanged"/> via changing the value of
    /// <see cref="IsDiscovered"/> within the Inspector of this class, as well.
    /// </summary>
    private bool isDiscoveredOldValue;
    
    /// <summary>
    /// This value is initialized from <see cref="isDiscovered"/> during runtime, and is not
    /// serialized. All runtime exposure to the outside is actually this value, and not the "real"
    /// <see cref="isDiscovered"/>.<br/>
    /// This is because otherwise, changes made to <see cref="isDiscovered"/> would persist even
    /// after Playmode is closed, which is (usually) not what we want.
    /// </summary>
    [NonSerialized]
    private bool isDiscoveredRuntimeValue;
    
    
    /* ---------- Events & Delegates ---------- */

    /// <summary>
    /// Invoked whenever the value of <see cref="IsDiscovered"/> is changed.<br/>
    /// This is intended for classes which are interested in when a keycode has been "discovered"
    /// by the player.<br/>
    /// The object returned by this event is the keycode instance that invoked it.
    /// </summary>
    public Utility.UnityEvents.ObjectUE OnDiscoveredStateChanged;


    /* ---------- Methods ---------- */

    private void OnValidate() {
        
        codeString = (
            digitOne.ToString() + digitTwo.ToString() +
            digitThree.ToString() + digitFour.ToString()
        );

        if (isDiscovered != isDiscoveredOldValue) {
            
            OnDiscoveredStateChanged.Invoke(this);
            isDiscoveredOldValue = isDiscovered;
        }
    }

    /// <summary>
    /// Renames the asset file this keycode instance is associated with, into
    /// <see cref="CodeString"/>.<br/>
    /// This method can only be called in the Unity Editor, during edit time. If it is called
    /// during runtime, nothing will happen.
    /// </summary>
    public void RenameAssetToCode() {

        /* This is critical. If we are in Playmode and try to run this, Unity apparently will be
         * stuck in an infinite loop forever. */
        
        if (Application.isPlaying == true) { return; }
        
        /* This code uses Unity's built-in asset database related utility methods to find the
        * ScriptableObject asset this particular instance belongs to, and to rename that
        * object accordingly. */
    
        string assetPath =  AssetDatabase.GetAssetPath(GetInstanceID());
        AssetDatabase.RenameAsset(assetPath, codeString);
        AssetDatabase.SaveAssets();          
    }
        
    
    /* ---------- Overrides ---------- */

    public void OnAfterDeserialize() {
        
        isDiscoveredRuntimeValue = isDiscovered;
    }

    public void OnBeforeSerialize() {  }
}

}