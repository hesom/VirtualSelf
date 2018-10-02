namespace VirtualSelf.Utility.Editor {


/// <summary>
/// An enumeration holding the possible different horizontal alignments of GUI
/// <see cref="Component"/>s for layouting them. The horizontal alignment describes how the
/// components should be placed horizontally in relation to the space available to them.<br/>
/// This enumeration is often used in tandem with <see cref="Position"/>, to specify where exactly
/// a component should be placed in relation to something else.
/// </summary>
public enum HorizontalAlignment {

	/// <summary>
	/// The components are aligned to the very left side of the available space.
	/// </summary>
	Left,

	/// <summary>
	/// The components are aligned to the center of the available space.
	/// </summary>
	Center,

	/// <summary>
	/// The components are aligned to the very right of the available space.
	/// </summary>
	Right
}

}
