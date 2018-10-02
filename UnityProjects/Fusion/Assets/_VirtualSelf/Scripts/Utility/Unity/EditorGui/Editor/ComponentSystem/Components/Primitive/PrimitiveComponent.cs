using UnityEditor;
using UnityEngine;


namespace VirtualSelf {

namespace Utility {

namespace Editor {
			

/// <summary>
/// This class models a "primitive" component. This is a component which, basically, models a single
/// GUI element, and usually directly corresponds to one of the GUI elements that Unity's
/// <see cref="EditorGUI"/> has to offer via its immediate mode drawing calls.<br/>
/// Every primitive component has an instance of <see cref="UnityEngine.GUIStyle"/>, which it can
/// either receive through its constructor, or create itself (by using the default GUI style that
/// Unity offers for that particular GUI element), and which can be used for styling it, as far as
/// the capabilities of Unity allow it.<br/>
/// Some primitive components also manage a value (like a boolean, or an entire class) - in this
/// case, the value will automatically be updated by Unity and the component's class, is exposed to
/// the outside, and can also manpulated from the outside directly (via code).
/// <remarks>
/// Some concrete primitive components will correspond to more than one of the Unity
/// <see cref="EditorGUI"/> draw calls. They will usually have some constructor argument that
/// decides which of the draw calls is used under the hood - all of this is to make sure that the
/// user can get what they want, and that they also get the right thing.
/// </remarks>
/// </summary>
public abstract class PrimitiveComponent : Component {
	
	/* ---------- Variables & Properties ---------- */
	
	/// <summary>
	/// The GUI style of this component. The GUI style specifies specifics about how the component
	/// is to be drawn, and, depending on the component, is also involved in correctly calculating
	/// one or multiple dimensions (width and/or height) of the component. 
	/// </summary>
	public GUIStyle GuiStyle { get; protected set; }
	
	
	/* ---------- Methods ---------- */
	
	/// <summary>
	/// Returns the default GUI style that this component will use. This style is used if no style
	/// was specified during the construction of this component.
	/// <remarks>
	/// The "default" style in this case, as also stated in the class description, refers to the
	/// default GUI style that Unity has pre-created and available for this type of concrete GUI
	/// component.
	/// </remarks>
	/// </summary>
	/// <returns>
	/// The default GUI style that this component will use.
	/// </returns>
	public abstract GUIStyle GetDefaultGuiStyle();
}

}

}

}
