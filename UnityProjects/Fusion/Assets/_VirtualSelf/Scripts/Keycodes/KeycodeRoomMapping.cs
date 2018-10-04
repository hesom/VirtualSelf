using System;
using UnityEngine;


namespace VirtualSelf.GameSystems {


/// <summary>
/// This class models a mapping of a single <see cref="Keycode"/> (asset file) to a single
/// <see cref="Room"/> (asset file).<br/>
/// It is intended for use within the <see cref="KeycodesList"/> of the game.<br/>
/// This is mostly a "data container" class, and does not contain much logic (except for the
/// equality comparison in <see cref="Equals(KeycodeRoomMapping)"/>, which is somewhat complex).
/// </summary>
[Serializable]
public sealed class KeycodeRoomMapping {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameKeycodeReference = nameof(keycodeReference);
    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNameRoomReference = nameof(roomReference);
    /// <summary>
    /// This is just used for interfacing with Unity Editor code, while keeping the assorted
    /// variable private.
    /// </summary>
    public const string FieldNamePropDrawingHeight = nameof(propDrawingHeight);

    /// <summary>
    /// The reference to the <see cref="Keycode"/> asset associated with this particular mapping.
    /// </summary>
    public Keycode KeycodeReference => keycodeReference;

    /// <summary>
    /// The reference to the <see cref="Room"/> asset associated with this particular mapping.
    /// </summary>
    public Room RoomReference => roomReference;
    
    /// <summary>
    /// See <see cref="KeycodeReference"/> for details.
    /// </summary>
    [SerializeField]
    private Keycode keycodeReference;

    /// <summary>
    /// See <see cref="RoomReference"/> for details.
    /// </summary>
    [SerializeField]
    private Room roomReference;

    /// <summary>
    /// Stores the height that this mapping instance will have in a custom Property Drawer. This is
    /// to enable Property Drawers of variable height.
    /// </summary>
    [SerializeField]
    private float propDrawingHeight;


    /* ---------- Methods ---------- */

    /// <summary>
    /// Returns whether this <see cref="KeycodeRoomMapping"/> instance is equal to the instance
    /// <paramref name="other"/>, or not.<br/>
    /// <br/>
    /// The rules for this comparison are somewhat different than one would probably expect. We care
    /// about whether there are any mappings that try to map the same keycode to rooms (no matter if
    /// those rooms are different or not), or the same room being mapped to multiple different
    /// keycodes. Both of this are cases we do <b>not</b> want.
    /// <br/>
    /// Thus, two <see cref="KeycodeRoomMapping"/> instances are equal if either their
    /// <see cref="keycodeReference"/>s are equal, and/or their <see cref="roomReference"/>s are
    /// equal. For these comparisons, <c>null</c> is considered <b>not</b> equal to itself, since
    /// <c>null</c> just means that we haven't filled in the references yet.
    /// </summary>
    /// <param name="other">
    /// The <see cref="KeycodeRoomMapping"/> instance to compare this one (for equality) to.
    /// </param>
    /// <returns>
    /// <c>true</c> if this <see cref="KeycodeRoomMapping"/> is equal to <see cref="other"/>, and
    /// <c>false</c> otherwise.
    /// </returns>
    private bool Equals(KeycodeRoomMapping other) {

        if ((keycodeReference == null) && (other.keycodeReference == null)) {

            if ((roomReference == null) && (other.roomReference == null)) {

                return (false);
            }
            else {
                return (Equals(roomReference, other.roomReference));
            }
        }
        else if ((roomReference == null) && (other.roomReference == null)) {
            
            if ((keycodeReference == null) && (other.keycodeReference == null)) {

                return (false);
            }
            else {
                return (Equals(keycodeReference, other.keycodeReference));
            }           
        }
        else {
            return (Equals(roomReference, other.roomReference) ||
                    Equals(keycodeReference, other.keycodeReference));
        }
    }
    

    /* ---------- Overrides ---------- */

    public override bool Equals(object obj) {
        
        if (ReferenceEquals(null, obj)) { return (false); }
        if (ReferenceEquals(this, obj)) { return (true); }

        return (obj is KeycodeRoomMapping && Equals((KeycodeRoomMapping) obj));
    }

    public override int GetHashCode() {
        
        /* I'm not sure if that really always returns the same result as Equals() does. */
        
        if (keycodeReference != null) { return (keycodeReference.GetHashCode()); }
        else if (roomReference != null) { return (roomReference.GetHashCode()); }

        return (0);
    }

    
    /* ---------- Operator Overloads ---------- */
    
    public static bool operator ==(KeycodeRoomMapping left, KeycodeRoomMapping right) {
        
        return (Equals(left, right));
    }

    public static bool operator !=(KeycodeRoomMapping left, KeycodeRoomMapping right) {
        
        return (Equals(left, right) == false);
    }
}

}