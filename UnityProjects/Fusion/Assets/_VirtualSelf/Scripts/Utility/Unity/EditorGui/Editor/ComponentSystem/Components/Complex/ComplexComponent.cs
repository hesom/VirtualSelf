

namespace VirtualSelf {

namespace Utility {

namespace Editor {
	

/// <summary>
/// A "complex" component is a component that behaves just like a normal <see cref="Component"/> to
/// the outside world, but can actually be made up of an arbitrary amount of components (both other
/// <see cref="ComplexComponent"/>s, as well as <see cref="PrimitiveComponent"/>s) on the inside.
/// <br/>
/// Complex components can thus look like anything, and also be arbitrarily big. The layouting for
/// the components they consist of is done inside of them under the hood - from outside code, they
/// can be dealt with just like a normal component.<br/>
/// The <see cref="Component.Width"/> and <see cref="Component.Height"/> of a complex component will
/// always encompass all parts (components) it is made of, and thus the rectangle returned by
/// <see cref="Component.GetRect"/> will, too.
/// </summary>
public abstract class ComplexComponent : Component {
	
	/* ---------- Methods ---------- */

	/// <summary>
	/// Layouts the components that this complex component is made of. Layouting them needs to be
	/// done so that they correctly make up the complex component, and also for it to have the right
	/// width and height in the end.<br/>
	/// Layouting the components usually includes:
	/// <list type="bullet">
	/// <item><description>
	/// Position the components correctly relatively to each other. This might include re-scaling
	/// some of them, like changing their <see cref="Component.Width"/> if they are to be placed
	/// next to each other, or if the width of the complex component is different from their own.
	/// </description></item>
	/// <item><description>
	/// Calculate the "local" position of each component, and store it in some variable. The local
	/// position is their position within the complex component as a whole, with (0,0) being the
	/// upper left corner.
	/// <br/>
	/// This is required so that the complex component can be drawn correctly - each individual
	/// component that is making it up can be drawn by adding its local position to the "absolute"
	/// position given to the <see cref="Component.Draw"/> method.
	/// </description></item>
	/// </list>
	/// This method, in most cases, needs to be called at the end of construction of the complex
	/// component (after all members have been initialized). After that, it might be necessary to
	/// also calculate a new <see cref="Component.Height"/> for the complex component.<br/>
	/// Additionally, if the complex component features any methods to change its width, height, or
	/// both, at any point after construction is done, it is likely that the components need to be
	/// layouted again afterwards.
	/// </summary>
	/// <remarks>
	/// The code for the whole layouting process might become arbitrarily complex, since a complex
	/// component can contain other complex components, which then also need to re-layout themselves
	/// potentially, and so on. In the worst case, for some types of complex components, this method
	/// might also be called several times per second.<br/>
	/// The layouting code should thus be be kept at least as simple as possible. In extreme cases,
	/// it might be necessary to make two methods for the layouting, where one features a simple
	/// algorithm and the other one a complex one - the complex one is then only called during
	/// construction and when no new layouting calls are necessary for e.g. a second or longer, and
	/// otherwise the simple one is used.
	/// </remarks>
	/// <exception cref="LayoutException">
	/// There are a number of reasons why the layouting process could not be finished successfully.
	/// One of them would be that the current <see cref="Component.Width"/> of the complex component
	/// is too small to fit all its components into it, even after after those that can be resized
	/// have already been resized down as much as possible.<br/>
	/// For any possible reason of a failure of the layouting process, this exception will be
	/// thrown. Any methods which directly call this method should also either catch and rethrow
	/// this exception, or just throw it upwards.
	/// </exception>
	protected abstract void LayoutComponents();
}

}

}

}