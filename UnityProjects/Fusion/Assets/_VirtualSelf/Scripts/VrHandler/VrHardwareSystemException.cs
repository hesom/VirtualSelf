using System;


namespace VirtualSelf.GameSystems {


/// <summary>
// TODO: Fill out
/// </summary>
public class VrHardwareSystemException : Exception {

	public VrHardwareSystemException() { }

	public VrHardwareSystemException(string message) : base(message) {  }

	public VrHardwareSystemException(string message, Exception inner) : base(message, inner) { }
}

}
