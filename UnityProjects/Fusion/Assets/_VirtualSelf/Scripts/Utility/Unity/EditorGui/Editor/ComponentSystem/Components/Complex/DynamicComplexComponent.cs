using System;


namespace VirtualSelf.Utility.Editor {


/// <summary>
/// A dynamic complex component is similar to its "primitive" component equivalent,
/// <see cref="DynamicPrimitiveComponent"/>. Its <see cref="Component.Width"/> is given to it on
/// construction (and can also be changed to a different value later on), and its
/// <see cref="Component.Height"/> is dynamically calculated based on its contents and its width.
/// <br/>
/// For a complex component, changing the width might also make it require to re-layout all the
/// components it is made up of, making changing the width an arbitrarily complex operation (even
/// more so since the component made be made up of more complex components, which then also need
/// to re-layout their contents, and so on...).
/// </summary>
public abstract class DynamicComplexComponent : ComplexComponent {

    /* ---------- Variables & Properties ---------- */
    
    /// <summary>
    /// The minimal width that this complex component can have, before it can no longer be layouted
    /// and drawn at all.<br/>
    /// This value is mostly determined by the following things:
    /// <list type="bullet">
    /// <item><description>
    /// Automatic components (whether they are primitive or complex) within this component, as these
    /// cannot be resized at all.
    /// </description></item>
    /// <item><description>
    /// The <c>MinimumWidth</c> values of components within this component (whether they are
    /// primitive or complex ones).
    /// </description></item>
    /// <item><description>
    /// Possible padding values, etc. These should be seen as absolute, and not manipulated.
    /// </description></item>
    /// </list>
    /// <br/>
    /// This value is supposed to be set at construction, likely inside of
    /// <see cref="ComplexComponent.LayoutComponents"/>, so that all layouting decisions are already
    /// known. If then either at this time, or at some point in the future when
    /// <see cref="SetNewWidth"/> is called, the width of this complex component is smaller than
    /// the minimum width, a <see cref="LayoutException"/> should be thrown.<br/><br/>
    /// In general, any concrete dynamic complex component is supposed to set a "reasonable" minimum
    /// width. Reasonable here means that the component (at this minimal width) is still properly
    /// visible and usable. In most cases, this should already be governed by the minimum widths of
    /// the components contained inside the component.
    /// </summary>
    public abstract float MinimumWidth { get; protected set; }
    
    
    /* ---------- Methods ---------- */
    
    /// <summary>
    /// Sets the <see cref="Component.Width"/> of this complex component to a new value, given by
    /// <paramref name="newWidth"/>.<br/>
    /// As stated in the class description, in most cases, this will cause the component to then
    /// both re-layout the components it is made up of (which means that
    /// <see cref="ComplexComponent.LayoutComponents"/> needs to be called), as well as
    /// re-calculating its own <see cref="Component.Height"/>, which means that
    /// <see cref="CalculateHeight"/> needs to be called.
    /// </summary>
    /// <param name="newWidth">The new width to set for this component.</param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="newWidth"/> is not positive.
    /// </exception>
    /// <exception cref="LayoutException">
    /// If <paramref name="newWidth"/> is smaller than <see cref="MinimumWidth"/>.
    /// </exception>
    public virtual void SetNewWidth(float newWidth) {

        ComponentUtils.AssertGreaterThanZero(newWidth, "width");
        ComponentUtils.AssertMinimumWidth(newWidth, MinimumWidth);
        
        Width = newWidth;
        
        LayoutComponents();
        CalculateHeight();
    }
    
    /// <summary>
    /// (Re-)Calculates the <see cref="Component.Height"/> of this complex component, depending on
    /// its <see cref="Component.Width"/>, as stated in the class description.<br/>
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