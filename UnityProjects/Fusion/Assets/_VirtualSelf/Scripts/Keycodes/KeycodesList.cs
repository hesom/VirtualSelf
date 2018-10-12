using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using VirtualSelf.Utility;


namespace VirtualSelf.GameSystems {


/// <summary>
/// A "container" class, which is supposed to hold all mappings of <see cref="Keycode"/>s to
/// <see cref="Room"/>s that the game uses.<br/>
/// The class mostly features a list of all these mappings, and additional utility and validation
/// methods.<br/>
/// The mappings list is supposed to be created via the Unity Inspector. There is no code access
/// possible to it from the outside.
/// </summary>
[CreateAssetMenu(
    fileName = "KeycodesList",
    menuName = "Keycodes/KeycodesList"
)]
public sealed class KeycodesList : ScriptableObject {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameKeycodeRoomMappings = nameof(keycodeRoomMappings);

    /// <summary>
    /// The list of valid keycode-room-mappings that this asset instance holds.<br/>
    /// This list is created when the game starts up, specifically when <see cref="Initialize"/> is
    /// called on this instance.<br/>
    /// The list is created from the mappings list shown in the Inspector. All duplicate mappings
    /// and mappings that are invalid are thrown out, and not part of this list - it can be assumed
    /// that any mappings contained here are valid, and safe to access and use.<br/>
    /// <br/>
    /// This is a <b>read-only</b> list, and it <b>only</b> exists at runtime! During edit time,
    /// there is currently no access available to the (not-validated) list of mappings.
    /// </summary>
    /// <remarks>
    /// Attempting to access this list before <see cref="Initialize"/> has been called will throw
    /// an exception, as the list does not exist yet.<br/>
    /// This list is never serialized. It ceases to exist once the game exits.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// If this list is accessed before <see cref="Initialize"/> has been called.
    /// </exception>
    public IList<KeycodeRoomMapping> ValidMappings {

        get {
            if (isInitialized == false) {

                throw new InvalidOperationException(
                    "The list of valid mappings is only populated when the KeycodesList has been " +
                    "initialized. Before that, it does not exist. Call Initialize() first."
                );
            }

            return (validMappings.AsReadOnly());
        }
    }

    /// <summary>
    /// This list contains all the keycode-room-mappings that we have for our games. This list is
    /// created and modified in the Inspector.<br/>
    /// This list is <i>not</i> supposed to be accessed by any outside code, as validation of all
    /// the mappings only happens when the game starts up. This is necessary because other
    ///
    /// Code interested in this list can take an instance of this class
    /// (asset), and then retrieve a (cleaned-up) version of this list, to work with.
    /// </summary>
    // TODO: Remove this attribute later on.
    [FormerlySerializedAs("keycodeSceneMappings")]
    [SerializeField]
    private List<KeycodeRoomMapping> keycodeRoomMappings;

    /// <summary>
    /// The cleaned-up version of <see cref="KeycodeRoomMapping"/>. This is filled by
    /// <see cref="CreateValidMappingsList"/>. This list is not serialized here, as this class does
    /// not need to display it (and indeed does not even call this method on its own).
    /// </summary>
    [NonSerialized]
    private List<KeycodeRoomMapping> validMappings;

    /// <summary>
    /// Denotes whether this asset has already been initialized. Before this is true, the
    /// "validated" mappings list accessible to outside code will be empty, and no events will be
    /// fired, either.<br/>
    /// This is set to <c>true</c> by <see cref="Initialize"/>.
    /// </summary>
    [NonSerialized]
    private bool isInitialized;
    
    
    /* ---------- Events & Delegates ---------- */
    
    /// <summary>
    /// Invoked whenever the state of any element (any <see cref="Keycode"/> or <see cref="Room"/>)
    /// in the mappings list (<see cref="ValidMappings"/>) changes. This event is concerned about
    /// general changes to the list, not specific changes. Classes that just want to know when
    /// there is a change to the list in general, can subscribe to this.<br/>
    /// The index of the list element that changed (in <see cref="ValidMappings"/> is returned by
    /// the event.<br/>
    /// For events specific to the Keycode or Room that changed, there are
    /// <see cref="OnKeycodeStateChanged"/> and <see cref="OnRoomStateChanged"/>.
    /// </summary>
    public Utility.UnityEvents.IntUE OnAnyListElementStateChanged;

    /// <summary>
    /// Invoked whenever the state of any specific <see cref="Keycode"/> in the mappings list
    /// changes. The keycode in question is returned by the event.
    /// </summary>
    public Utility.UnityEvents.ObjectUE OnKeycodeStateChanged;

    /// <summary>
    /// Invoked whenever the state of any specific <see cref="Room"/> in the mappings list
    /// changes. The room in question is returned by the event.
    /// </summary>
    public Utility.UnityEvents.ObjectUE OnRoomStateChanged;
    

    /* ---------- Methods ---------- */

    /// <summary>
    /// Returns the mapping from the mappings list (<see cref="ValidMappings"/>) which has the
    /// <see cref="Keycode"/> corresponding to the given keycode code string, if it exists.
    /// </summary>
    /// <param name="codeString">
    /// The <see cref="Keycode"/> code string of which the corresponding keycode should be returned
    /// for.
    /// </param>
    /// <returns>
    /// A value containing the mapping with the <see cref="Keycode"/> corresponding to
    /// <paramref name="codeString"/>, or an empty value if such a mapping does not exist within
    /// <see cref="ValidMappings"/>.
    /// </returns>
    public Optional<KeycodeRoomMapping> GetKeycodeFromCodeString(string codeString) {

        return (
            Optional<KeycodeRoomMapping>.OfNullable(
                ValidMappings.FirstOrDefault(
                    mapping => mapping.KeycodeReference.CodeString == codeString
                )
            )
        );
    }

    /// <summary>
    /// Initializes this <see cref="KeycodesList"/>, and makes it ready to be used at runtime.<br/>
    /// Calling this method populates <see cref="ValidMappings"/>, and initializes all the events
    /// this class exposes.<br/>
    /// This method should be called (exactly) once, right when the game starts.
    /// </summary>
    public void Initialize() {

        if (isInitialized) {
            
            Debug.LogWarning(
                "KeycodesList: The list has already been initialized. Cancelling the second " + 
                "attempt."
            );
            return;
        }
        
        Debug.Log(
            "KeycodesList: Initializing. The list can now be accessed by other code, and events " + 
            "will be fired."
        );
        
        CreateValidMappingsList();

        foreach (KeycodeRoomMapping mapping in validMappings) {
            
            mapping.KeycodeReference.OnDiscoveredStateChanged.AddListener(
                OnKeycodeStateChangedInvocation);
            mapping.RoomReference.OnVisitedStateChanged.AddListener(OnRoomStateChangedInvocation);
        }
        
        isInitialized = true;
    }
    
    private void OnDestroy() {

        if (isInitialized) {
            
            foreach (KeycodeRoomMapping mapping in validMappings) {
    
                mapping.KeycodeReference.OnDiscoveredStateChanged.RemoveListener(
                    OnKeycodeStateChangedInvocation);
                mapping.RoomReference.OnVisitedStateChanged.RemoveListener(
                    OnRoomStateChangedInvocation);
            }           
        }
    }

    /// <summary>
    /// Creates a list of only valid, and non-duplicate mappings from
    /// <see cref="keycodeRoomMappings"/>, and populates <see cref="validMappings"/> with it. After
    /// this, the valid mappings can be used and accessed from the outside.<br/>
    /// This method needs to be called once, when the game starts, before
    /// <see cref="validMappings"/> can be accessed.
    /// </summary>
    private void CreateValidMappingsList() {
        
        /* First, we eliminate all the duplicates from the mappings list (we only copy over the
         * non-duplicates). */
        
        var duplicates = CollectionsUtils.FindAllDuplicatesIn(keycodeRoomMappings);
        
        IList<KeycodeRoomMapping> cleanedUpMappings = new List<KeycodeRoomMapping>();

        for (int i = 0; i < keycodeRoomMappings.Count; i++) {

            if (duplicates.ContainsKey(i) == false) {
                
                cleanedUpMappings.Add(keycodeRoomMappings[i]);
            }
        }
        
        /* Now, we eliminate all mappings that don't have all their references etc. set. */

        validMappings = new List<KeycodeRoomMapping>();

        foreach (KeycodeRoomMapping mapping in cleanedUpMappings) {

            if ((mapping.KeycodeReference == null) || (mapping.RoomReference == null)) { continue; }

            if (mapping.RoomReference.HasSceneAttached() == false) { continue; }
            
            validMappings.Add(mapping);
        }
    }
    
    
    /* ---------- Event Methods ---------- */

    private void OnKeycodeStateChangedInvocation(UnityEngine.Object keycode) {

        if (isInitialized) {

            Keycode eventCode = keycode as Keycode;
            bool hasBeenFound = false;
            
            for (int i = 0; i < ValidMappings.Count; i++) {
                
                if (ValidMappings[i].KeycodeReference.Equals(eventCode)) {
                    
                    OnAnyListElementStateChanged.Invoke(i);
                    
                    hasBeenFound = true;
                    break;
                }
            }

            if (hasBeenFound == false) {
                
                throw new SystemException(
                    "Could not find the keycode that has its event invoked within the keycodes " +
                    "list. This should not be possible to happen..."
                );
            }
        }
        
        OnKeycodeStateChanged.Invoke(keycode);
    }

    private void OnRoomStateChangedInvocation(UnityEngine.Object room) {

        if (isInitialized) {

            Room eventRoom = room as Room;
            bool hasBeenFound = false;
            
            for (int i = 0; i < ValidMappings.Count; i++) {
                
                if (ValidMappings[i].RoomReference.Equals(eventRoom)) {
                    
                    OnAnyListElementStateChanged.Invoke(i);
                    
                    hasBeenFound = true;
                    break;
                }
            }

            if (hasBeenFound == false) {
                
                throw new SystemException(
                    "Could not find the room that has its event invoked within the keycodes " +
                    "list. This should not be possible to happen..."
                );
            }
        }
        
        OnRoomStateChanged.Invoke(room);
    }
}

}