namespace VirtualSelf.Utility.Editor {

    
/// <summary>
/// An enumeration holding the possible different vertical alignments of GUI
/// <see cref="Component"/>s for layouting them. The vertical alignment describes how the components
/// should be placed vertically in relation to the space available to them.<br/>
/// This enumeration is often used in tandem with <see cref="Position"/>, to specify where exactly
/// a component should be placed in relation to something else.
/// </summary>
public enum VerticalAlignment {

    /// <summary>
    /// The components are aligned to the top of the available space.
    /// </summary>
    Top,
    
    /// <summary>
    /// The components are aligned to the center of the available space.
    /// </summary>
    Center,
    
    /// <summary>
    /// The components are aligned to the bottom of the available space.
    /// </summary>
    Bottom
}

}