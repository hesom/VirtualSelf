using Rotorz.ReorderableList;


namespace VirtualSelf.Utility.Editor {

/// <summary>
/// A utility class holding different pre-configured configurations for the style of a
/// <see cref="Rotorz.ReorderableList.ReorderableListControl"/>. These can be used as a drop-in
/// when creating new reorderable list fields in the Unity Editor.<br/>
/// This saves the work of having to check (and potentially test out) all the different existing
/// combinations of <see cref="ReorderableListFlags"/>, until the desired configuration is reached.
/// </summary>
public static class ReorderableListConfigurations {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// A configuration for an immutable reorderable list. This list cannot be modified by the user
    /// in the Unity Inspector at all.<br/>
    /// There is no context menu for it, no buttons for adding or removing items, no reordering of
    /// existing items, no way to duplicate items, and so on.<br/>
    /// This configuration is intended to be used if the list is just for viewing, not for doing
    /// anything with it.
    /// </summary>
    public const ReorderableListFlags ImmutableList = (
        
            ReorderableListFlags.DisableContextMenu | 
            ReorderableListFlags.DisableDuplicateCommand |
            ReorderableListFlags.DisableReordering |
            ReorderableListFlags.HideAddButton |
            ReorderableListFlags.HideRemoveButtons
    );
    
    /// <summary>
    /// A configuration for a read-only reorderable list. This is similar to the
    /// <see cref="ImmutableList"/>, with one difference: While the elements can't be modified
    /// (including removing or adding new ones), the list itself can still be reordered.<br/>
    /// This configuration is intended to be used if the list is mostly just for viewing, with the
    /// exception that reordering should be possible.
    /// </summary>
    public const ReorderableListFlags ReadOnlyList = (
        
            ReorderableListFlags.DisableContextMenu | 
            ReorderableListFlags.DisableDuplicateCommand |
            ReorderableListFlags.HideAddButton |
            ReorderableListFlags.HideRemoveButtons
    );
}

}