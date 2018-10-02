namespace VirtualSelf.Utility.Editor {

    
/// <summary>
/// An enumeration holding the different possible "positions" that GUI <see cref="Component"/>s can
/// be placed at when they are layouted. A position, here, mostly refers to a position relative to
/// some other component, or boundary.<br/>
/// This enumeration is often used in tandem with <see cref="HorizontalAlignment"/> or
/// <see cref="VerticalAlignment"/>, to specify where exactly a component should be placed in
/// relation to something else.
/// </summary>
public enum Position {

    /// <summary>
    /// The components are placed to the left of something else, or on the left side. An instance of
    /// <see cref="VerticalAlignment"/> might specify where exactly they are placed vertically.
    /// </summary>
    Left,
    
    /// <summary>
    /// The components are placed to the right of something else, or on the right side. An instance
    /// of <see cref="VerticalAlignment"/> might specify where exactly they are placed vertically.
    /// </summary>
    Right,
    
    /// <summary>
    /// The components are placed above something else, or at the top. An instance of
    /// <see cref="HorizontalAlignment"/> might specify where exactly they are placed horizontally.
    /// </summary>
    Top,
    
    /// <summary>
    /// The components are placed below something else, or at the bottom. An instance of
    /// <see cref="HorizontalAlignment"/> might specify where exactly they are placed horizontally.
    /// </summary>
    Bottom
}

}
