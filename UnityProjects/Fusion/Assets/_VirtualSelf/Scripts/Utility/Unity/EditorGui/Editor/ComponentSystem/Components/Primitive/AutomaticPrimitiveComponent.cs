using UnityEngine;


namespace VirtualSelf {

namespace Utility {

namespace Editor {
    
    
/// <summary>
/// An "automatic" component is a component whose dimensions (width and height) are both dependent
/// entirely on its actual "contents". The component can calculate them both by itself, by only
/// considering its contents (e.g. for a component modeling a "label", this would be the label text)
/// and its <see cref="PrimitiveComponent.GuiStyle"/>. There is no additional user input required,
/// and the component does not change in relation to the "outside world".<br/>
/// Because of this, an automatic component only has a single method (without parameters), which
/// concrete subclasses have to override, and which only has to be called a single time, during
/// construction of the component. This also means that the dimensions of the component are
/// effectively immutable after construction.
/// </summary>
public abstract class AutomaticPrimitiveComponent : PrimitiveComponent {

    /* ---------- Methods ---------- */
    
    /// <summary>
    /// Calculates the dimensions (width and height) of this component.<br/>
    /// This method is supposed to calculate the correct width and height of this component, and to
    /// then initialize <see cref="Component.Width"/> and <see cref="Component.Height"/> with those
    /// values.<br/>
    /// Since the dimensions only depend on the contents of the component, this method can be run
    /// right during construction of the component (after all other members have been initialized).
    /// It should also not need to be called more than once during the entire lifetime of the
    /// component.
    /// </summary>
    protected abstract void CalculateDimensions();
}

}

}

}


