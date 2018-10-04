using System;


namespace VirtualSelf.Utility.Editor {


/// <summary>
/// A "fixed" component is a component that has its dimensions (both its width and height) specified
/// by the user upon its creation. This means that it does not calculate its own dimensions, at all.
/// It does not even have an idea about how to do that, as its content is of a nature that does not
/// really allow any "automatic" calculation of its dimensions.<br/>
/// Because of this, fixed components just have two methods to change each their width and their
/// height (and a third to change both at once). Concrete fixed components usually do not have to
/// specialize these, either.<br/>
/// It should be noted that for fixed components, their width and height are each independent from
/// one another - modifying either does not affect the other.
/// </summary>
public abstract class FixedPrimitiveComponent : PrimitiveComponent {

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
    
    /// <summary>
    /// The minimum height for this component. If the component would have a
    /// <see cref="Component.Height"/> smaller than this, it can not be reasonably drawn and/or used
    /// anymore.<br/>
    /// This value is supposed to be set either at construction per-instance, or directly on
    /// implementation of this property, e.g. from a static variable.<br/>
    /// If either at construction, or at some point in the future when <see cref="SetNewHeight"/> is
    /// called, the width of this component is smaller than the minimum height, a
    /// <see cref="LayoutException"/> should be thrown.<br/>
    /// On the question of what exactly a "reasonable" minimal height is, especially for potential
    /// different stylings of components, can only ever be an estimation, and might turn out bad in
    /// practice.
    /// </summary>
    public abstract float MinimumHeight { get; protected set; }
    
    
    /* ---------- Methods ---------- */
    
    /// <summary>
    /// Sets the <see cref="Component.Width"/> of this component to a new value, given by
    /// <paramref name="newWidth"/>.<br/>
    /// As stated in the class description, generally this will not affect the height of the
    /// component at all.
    /// </summary>
    /// <param name="newWidth">The new width to set for this component.</param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="newWidth"/> is not positive (>0).
    /// </exception>
    public virtual void SetNewWidth(float newWidth) {

        ComponentUtils.AssertGreaterThanZero(newWidth, "width");
        ComponentUtils.AssertMinimumWidth(newWidth, MinimumWidth);

        Width = newWidth;
    }
    
    /// <summary>
    /// Sets the <see cref="Component.Height"/> of this component to a new value, given by
    /// <paramref name="newHeight"/>.<br/>
    /// As stated in the class description, generally this will not affect the width of the
    /// component at all.
    /// </summary>
    /// <param name="newHeight">The new height to set for this component.</param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="newHeight"/> is not positive (>0).
    /// </exception>
    public virtual void SetNewHeight(float newHeight) {

        ComponentUtils.AssertGreaterThanZero(newHeight, "height");
        ComponentUtils.AssertMinimumHeight(newHeight, MinimumHeight);
        
        Height = newHeight;
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
    /// If <paramref name="newWidth"/> or <paramref name="newHeight"/> are not positive (>0).
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