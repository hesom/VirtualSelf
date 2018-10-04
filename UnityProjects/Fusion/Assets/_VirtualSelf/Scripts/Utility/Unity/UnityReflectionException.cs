using System;


namespace VirtualSelf {

namespace Utility {
	

/// <summary>
/// A custom exception with normal exception behavior (similar to
/// <see cref="System"/>.<see cref="Exception"/>).<br/>
/// This exception is intended to be thrown by methods using reflection to access data (like fields
/// and methods) that is not part of Unity's public API (and thus is not intended by Unity to be
/// accessed from the outside).<br/>
/// If this exception is thrown, the reflection has failed to work correctly. This most likely means
/// that something changed internally within Unity, and what the method tries to do does not work
/// anymore now within a new Unity version.<br/>
/// Methods throwing this exception should include a message, if possible, that gives at least some
/// pointers to what exactly went wrong.
/// </summary>
public class UnityReflectionException : Exception {

	public UnityReflectionException() { }
	
	public UnityReflectionException(string message) : base(message) {  }
	
	public UnityReflectionException(string message, Exception inner) : base(message, inner) { }
}

}

}
