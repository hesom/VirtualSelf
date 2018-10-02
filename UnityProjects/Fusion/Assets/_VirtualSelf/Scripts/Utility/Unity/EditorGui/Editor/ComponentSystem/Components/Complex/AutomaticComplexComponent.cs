namespace VirtualSelf.Utility.Editor {


/// <summary>
/// An automatic complex component is similar to its "primitive" component equivalent,
/// <see cref="AutomaticPrimitiveComponent"/>. A complex component can only be automatic if it only
/// consists of automatic components (both primitive and complex). Only then, its
/// <see cref="Component.Width"/> and <see cref="Component.Height"/> are both unchangeable, and can
/// be calculated completely just from the components it is made of.<br/>
/// Because of this, an automatic complex component is generally also cheap to construct and to
/// maintain. It only needs to layout its internal components and calculate its dimensions exactly
/// once, during construction, and this operation is generally cheap compared to other types of
/// complex components. After this, it is fixed and no more calculations or layouting is necessary.
/// </summary>
public abstract class AutomaticComplexComponent : ComplexComponent {

	/// <summary>
	/// Calculates the dimensions (width and height) of this component.<br/>
	/// This method is supposed to calculate the correct width and height of this component, and to
	/// then initialize <see cref="Component.Width"/> and <see cref="Component.Height"/> with those
	/// values.<br/>
	/// Since the dimensions only depend on the contents of the component, this method can be run
	/// right during construction of the component (after all other members have been initialized).
	/// It should also not need to be called more than once during the entire lifetime of the
	/// component.<br/>
	/// In most cases, before the dimensions can be calculated,
	/// <see cref="ComplexComponent.LayoutComponents"/> will have to be called.
	/// </summary>
	protected abstract void CalculateDimensions();
}

}