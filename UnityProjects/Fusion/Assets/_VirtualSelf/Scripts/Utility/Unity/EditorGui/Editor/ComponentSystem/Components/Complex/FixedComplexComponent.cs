using System;


namespace VirtualSelf.Utility.Editor {


/// <summary>
/// A fixed (complex) component is, like its primitive version
/// (<see cref="FixedPrimitiveComponent"/>), a component whose <see cref="Component.Width"/> and
/// <see cref="Component.Height"/> both have to be specific explicitly by the user - neither of them
/// can be determined from the contents of the component alone.<br/>
/// The component's width and height can be changed at any time, individually. Doing so might make
/// a fixed complex component have to re-layout all the components it is made up of internally
/// (which might be an arbitrarily complex operation).<br/>
/// As long as a complex component contains at least one fixed component (whether primitive or
/// complex), it probably needs to be a fixed complex component.
/// </summary>
public abstract class FixedComplexComponent : ComplexComponent {

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
    /// In general, any concrete fixed complex component is supposed to set a "reasonable" minimum
    /// width. Reasonable here means that the component (at this minimal width) is still properly
    /// visible and usable. In most cases, this should already be governed by the minimum widths of
    /// the components contained inside the component.
    /// </summary>
    public abstract float MinimumWidth { get; protected set; }
    
    /// <summary>
    /// The minimal height that this complex component can have, before it can no longer be layouted
    /// and drawn at all.<br/>
    /// This value is mostly determined by the following things:
    /// <list type="bullet">
    /// <item><description>
    /// Automatic components (whether they are primitive or complex) within this component, as these
    /// cannot be resized at all.
    /// </description></item>
    /// <item><description>
    /// The <c>MinimumHeight</c> values of components within this component (whether they are
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
    /// <see cref="SetNewHeight"/> is called, the height of this complex component is smaller than
    /// the minimum height, a <see cref="LayoutException"/> should be thrown.<br/><br/>
    /// In general, any concrete fixed complex component is supposed to set a "reasonable" minimum
    /// height. Reasonable here means that the component (at this minimal height) is still properly
    /// visible and usable. In most cases, this should already be governed by the minimum heights of
    /// the components contained inside the component.
    /// </summary>
    public abstract float MinimumHeight { get; protected set; }

    
    /* ---------- Methods ---------- */
    
    /// <summary>
    /// Sets the <see cref="Component.Width"/> of this complex component to a new value, given by
    /// <paramref name="newWidth"/>.<br/>
    /// As stated in the class description, in most cases, this will cause the component to then
    /// re-layout the components it is made up of (which means that
    /// <see cref="ComplexComponent.LayoutComponents"/> needs to be called).
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
        ComponentUtils.AssertMinimumHeight(newWidth, MinimumWidth);
        
        Width = newWidth;
        
        LayoutComponents();
    }
    
    /// <summary>
    /// Sets the <see cref="Component.Height"/> of this complex component to a new value, given by
    /// <paramref name="newHeight"/>.<br/>
    /// As stated in the class description, in most cases, this will cause the component to then
    /// re-layout the components it is made up of (which means that
    /// <see cref="ComplexComponent.LayoutComponents"/> needs to be called).
    /// </summary>
    /// <param name="newHeight">The new height to set for this component.</param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="newHeight"/> is not positive (>0).
    /// </exception>
    /// <exception cref="LayoutException">
    /// If <paramref name="newHeight"/> is smaller than <see cref="MinimumHeight"/>.
    /// </exception>
    public virtual void SetNewHeight(float newHeight) {

        ComponentUtils.AssertGreaterThanZero(newHeight, "height");
        ComponentUtils.AssertMinimumHeight(newHeight, MinimumHeight);
        
        Height = newHeight;
        
        LayoutComponents();
    }
    
    /// <summary>
    /// Convenience method to set both the <see cref="Component.Width"/> and
    /// <see cref="Component.Height"/> of this component to new values with a single method call.
    /// <br/>
    /// This method behaves identically to calling <see cref="SetNewWidth"/> and
    /// <see cref="SetNewHeight"/> each individually one after the other.
    /// </summary>
    /// <param name="newWidth">The new width to set for this component.</param>
    /// <param name="newHeight">The new height to set for this component.</param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="newWidth"/> or <paramref name="newHeight"/> are not positive.
    /// </exception>
    /// <exception cref="LayoutException">
    /// Whenever <see cref="SetNewWidth"/> or <see cref="SetNewHeight"/> would throw this exception.
    /// </exception>
    public virtual void SetNewDimensions(float newWidth, float newHeight) {
        
        SetNewWidth(newHeight);
        SetNewHeight(newHeight);
    }
}

}