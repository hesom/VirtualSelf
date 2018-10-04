using System;


namespace VirtualSelf.Utility.Editor {


/// <summary>
/// A "dynamic" component is a component that has a variable (hence "dynamic") height, which is
/// (generally) dependent on its width. The width in turn is specified by the user on creation of
/// the component. The width can also be changed later via a method call, which will then usually
/// make the component also re-calculate its height.<br/>
/// How complex the height calculation dependent on the width of the component is depends on the
/// concrete component in question. Generally, each concrete component needs its own implementation
/// of a method to calculate its height, which might be arbitrarily complex (but should be as simple
/// as possible for performance reasons).<br/>
/// In addition to such a method for the height calculation, dynamic components feature the
/// mentioned method to change their width. The width has to, of course, always be a positive value
/// (and greater than 0).<br/>
/// While dynamic components could generally be resized to any (positive) width, in practice there
/// is usually a lower limit for the width under which the component cannot be drawn and/or be used
/// "reasonably" anymore. (For example, a text field with a width of 10 pixels would not be very
/// usable.) For this reason, dynamic components can define a minimum width under which they may
/// refuse to be resized to.
/// </summary>
public abstract class DynamicPrimitiveComponent : PrimitiveComponent {

    /* ---------- Variables & Properties ---------- */
    
    /// <summary>
    /// The minimum width for this component. If the component would have a
    /// <see cref="Component.Width"/> smaller than this, it can not be reasonably drawn and/or used
    /// anymore.<br/>
    /// This value is supposed to be set either at construction per-instance, or directly on
    /// implementation of this property, e.g. from a static variable.<br/>
    /// If either at construction, or at some point in the future when <see cref="SetNewWidth"/> is
    /// called, the width of this component is smaller than the minimum width, a
    /// <see cref="LayoutException"/> should be thrown.<br/>
    /// On the question of what exactly a "reasonable" minimal width is, especially for potential
    /// different stylings of components, can only ever be an estimation, and might turn out bad in
    /// practice.
    /// </summary>
    public abstract float MinimumWidth { get; protected set; }
    
    
    /* ---------- Methods ---------- */
    
    /// <summary>
    /// Sets the <see cref="Component.Width"/> of this component to a new value, given by
    /// <paramref name="newWidth"/>.<br/>
    /// As stated in the class description, in most cases, this will cause the component to then
    /// re-calculate its <see cref="Component.Height"/>, which means that
    /// <see cref="CalculateHeight"/> needs to be called.
    /// </summary>
    /// <param name="newWidth">The new width to set for this component.</param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="newWidth"/> is not positive (>0).
    /// </exception>
    /// <exception cref="LayoutException">
    /// If <paramref name="newWidth"/> is smaller than <see cref="MinimumWidth"/>.
    /// </exception>
    public virtual void SetNewWidth(float newWidth) {

        ComponentUtils.AssertGreaterThanZero(newWidth, "width");
        ComponentUtils.AssertMinimumWidth(newWidth, MinimumWidth);

        Width = newWidth;
        CalculateHeight();
    }

    /// <summary>
    /// (Re-)Calculates the <see cref="Component.Height"/> of this component, depending on its
    /// <see cref="Component.Width"/>, as stated in the class description.<br/>
    /// This method is supposed to, in almost all cases, be called whenever the width of the
    /// component has changed (for any reason), so that its height can change in accordance. It also
    /// obviously needs to be called once during construction of the component, after all its other
    /// members have been initialized, to calculate its initial height.
    /// <remarks>
    /// The code for the height calculation should be kept as simple as possible, since in the worst
    /// case, this method will be called dozens of times per second. In extreme cases, it might be
    /// necessary to make two methods for height calculation, where one features a simple algorithm
    /// and the other one a complex one - the complex one is then only called during construction
    /// and when the width did not change for e.g. a second or longer, otherwise the simple one is
    /// used.
    /// </remarks>
    /// </summary>
    protected abstract void CalculateHeight();
}

}
