using System;
using RoboRyanTron.SceneReference;
using UnityEngine;


namespace VirtualSelf.GameSystems {


/// <summary>
/// This class models the metadata for a room within the game, which are then created in the form of
/// <see cref="ScriptableObject"/> asset files.<br/>
/// An (asset) instance of this class basically represents a room within the game code (the actual
/// room is, of course, the scene file - but this class associates the scenes with the concept of a
/// "room", and the rest of the game).<br/>
/// For each room we want to have, an asset should be created, and its values populated.<br/>
/// <br/>
/// This class is used within <see cref="KeycodesList"/>, together with the <see cref="Keycode"/>s,
/// but any code can obtain a reference to it (preferably by using the Unity Inspector) and then
/// read or modify its values at runtime. There is also an event than can be subscribed to, to
/// listen to relevant state changes.
/// </summary>
[CreateAssetMenu(
    fileName = "Room",
    menuName = "Keycodes/Room"
)]
public sealed class Room : ScriptableObject, ISerializationCallbackReceiver {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameRoomName = nameof(roomName);
    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameDescription = nameof(description);
    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameScene = nameof(scene);
    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameIsDiscovered = nameof(hasBeenVisited);
    
    /// <summary>
    /// The name of this room asset. This will be used as the "display name" for the room ingame,
    /// but otherwise should <i>not</i> be used in game logic (as the name can change anytime).
    /// </summary>
    public string RoomName => roomName;

    /// <summary>
    /// The description of this room asset. This can be used to add e.g. help text, hints,
    /// instructions, and other information (or even lore!) about a room. This will be displayed
    /// ingame (so it should not contain "development information").
    /// </summary>
    public string Description => description;

    /// <summary>
    /// The reference to the scene asset file associated with this room asset. This can then be
    /// loaded/unloaded directly, and in a safe way.
    /// </summary>
    public SceneReference Scene => scene;

    /// <summary>
    /// Denotes whether this room has already been visited by the player (at least once), or not. In
    /// code terms, this means that the player has entered the scene (through the portal) associated
    /// with this room asset for the first time.
    /// </summary>
    public bool HasBeenVisited {

        get {
            if (Application.isPlaying) { return (hasBeenVisitedRuntimeValue); }
            else { return (hasBeenVisited); }
        }
        set {
            if (Application.isPlaying) {
                if (value != hasBeenVisitedRuntimeValue) {
                    hasBeenVisitedRuntimeValue = value;
                    OnVisitedStateChanged.Invoke(hasBeenVisitedRuntimeValue);
                }
            }
            else {
                if (value != hasBeenVisited) {
                    hasBeenVisited = value;
                    OnVisitedStateChanged.Invoke(hasBeenVisited);
                }               
            }
        }
    }

    /// <summary>
    /// See <see cref="RoomName"/> for details.
    /// </summary>
    [SerializeField]
    private string roomName;
    
    /// <summary>
    /// See <see cref="Description"/> for details.
    /// </summary>
    [SerializeField]
    private string description;
    
    /// <summary>
    /// See <see cref="Scene"/> for details.
    /// </summary>
    [SerializeField]
    private SceneReference scene;
    
    /// <summary>
    /// See <see cref="HasBeenVisited"/> for details.
    /// </summary>
    [SerializeField]
    private bool hasBeenVisited;
    
    /// <summary>
    /// This is used (only) in <see cref="OnValidate"/>, to make it possible to invoke
    /// <see cref="OnVisitedStateChanged"/> via changing the value of <see cref="HasBeenVisited"/>
    /// within the Inspector of this class, as well.
    /// </summary>
    private bool hasBeenVisitedOldValue;

    /// <summary>
    /// This value is initialized from <see cref="hasBeenVisited"/> during runtime, and is not
    /// serialized. All runtime exposure to the outside is actually this value, and not the "real"
    /// <see cref="hasBeenVisited"/>.<br/>
    /// This is because otherwise, changes made to <see cref="hasBeenVisited"/> would persist even
    /// after Playmode is closed, which is (usually) not what we want.
    /// </summary>
    [NonSerialized]
    private bool hasBeenVisitedRuntimeValue;


    /* ---------- Events & Delegates ---------- */

    /// <summary>
    /// Invoked whenever the value of <see cref="HasBeenVisited"/> is changed.<br/>
    /// This is intended for classes which are interested in when a room has been visited by the
    /// player.
    /// </summary>
    public Utility.UnityEvents.BoolUE OnVisitedStateChanged;
    
    
    /* ---------- Methods ---------- */
   
    private void OnValidate() {

        if (hasBeenVisited != hasBeenVisitedOldValue) {
            
            OnVisitedStateChanged.Invoke(hasBeenVisited);
            hasBeenVisitedOldValue = hasBeenVisited;
        }

        /* If the name for this Room is still empty, we will set it to the name of the scene - this
         * is better than just leaving it empty. (In any case, a warning will be shown for this,
         * as well.) */
        
        if (roomName.Trim() == "") {
            
            if (scene.Scene != null) {
                
                roomName = scene.Scene.name;
            }
        }
    }
    
    /// <summary>
    /// Returns whether this room asset currently has a scene file attached (within
    /// <see cref="Scene"/>), or not.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the room asset has a scene file attached, and <c>false</c> otherwise.
    /// </returns>
    public bool HasSceneAttached() {

        return ((scene != null) && (scene.Scene != null));
    }
    
    
    /* ---------- Overrides ---------- */

    public void OnAfterDeserialize() {
        
        hasBeenVisitedRuntimeValue = hasBeenVisited;
    }

    public void OnBeforeSerialize() {  }
}

}