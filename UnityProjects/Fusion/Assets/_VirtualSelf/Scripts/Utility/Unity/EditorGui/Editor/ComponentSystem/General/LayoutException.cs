using System;


namespace VirtualSelf.Utility.Editor {

	
/// <summary>
/// A custom exception with normal exception behavior (similar to
/// <see cref="System"/>.<see cref="Exception"/>).<br/>
/// This exception is intended to be thrown in a Unity Editor drawing and/or layouting context, e.g.
/// for custom Inspectors or Property Drawers.<br/>
/// This exception is intended to be thrown for failed layouting operations, or invalid values
/// during layouting. An example could be trying to layout a <see cref="Component"/> inside of a
/// space that is too small for it, or providing negative <c>width</c> or <c>height</c> values to
/// layouting methods, etc.
/// </summary>
public class LayoutException : Exception {

	/* ---------- Constructors ---------- */

	public LayoutException() { }
	
	public LayoutException(string message) : base(message) {  }
	
	public LayoutException(string message, Exception inner) : base(message, inner) { }
}

}
