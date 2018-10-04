using UnityEditor;
using UnityEngine;


namespace VirtualSelf {

namespace Utility {

namespace Editor {


/// <summary>
/// This class models a "component", in its most abstract form. A component, in this case, refers to
/// GUI elements/forms/components of Unity's
/// <a href="https://docs.unity3d.com/Manual/GUIScriptingGuide.html">Immediate Mode GUI (IMGUI)</a>".
/// It is thus not related to "ingame" GUIs of any kind, but instead used for creating GUIs for
/// Unity itself, as stated in the previous link (to the Unity manual).<br/>
/// <br/>
/// This class is of a very high abstraction level, and only offers very basic capabilities and
/// attributes refering to a "component". Basically, a component (on this abstraction level) can be
/// anything that can be drawn to a GUI. What this is more specifically is specified further down
/// the class hierarchy - this class is only concerned with the (very few) things that everything
/// that can be drawn has in common.<br/>
/// <br/>
/// Each possible component features a width and height (its dimensions). This also means that each
/// component can fit into a rectangle of these dimensions. Naturally, each component can also be
/// drawn (although some concrete components might be effectively invisible), which requires the
/// user to additionally specifiy an actual position (in 2D space) to draw the component at.
/// <remarks>
/// This class is part of a larger "component system", that interfaces with Unity's IMGUI system
/// (and, for the most part, uses it under the hood).<br/>
/// This is necessary because, as the name states, Unity's system is entirely "immediate mode" based
/// - every GUI component is just a method call, no state is stored/retained between frames. This
/// makes it very difficult and verbose to create and manage a more complex GUI, especially if it
/// needs to retain a lot of state over time and has many different states on its own. While Unity
/// offers <c>Layout</c> versions of most of their drawing methods, which automatically lay out the
/// components they draw, these only offer a pretty bare-bones way of layouting the components, are
/// barely configurable, and do not resolve any other problems of such a system (like retaining
/// state). They also cannot be used at all in the "Custom Property Drawers".<br/>
/// <br/>
/// This component system basically implements a retained mode GUI on top of Unity's immediate mode
/// one, like it is usually known and used by GUI frameworks. It features classes for all actual
/// components, some classes that help with layouting, and more.
/// </remarks>
/// </summary>
public abstract class Component {

    /* ---------- Variables & Properties ---------- */
    
    /// <summary>
    /// The width of this component. This will always be a positive (>0) value.
    /// </summary>
    public float Width { get; protected set; }
    
    /// <summary>
    /// The height of this component. This will always be a positive (>0) value.
    /// </summary>
    public float Height { get; protected set; }
    
    
    /* ---------- Methods ---------- */

    /// <summary>
    /// Draws this component. Depending on the concrete component, this might be an arbitrarily
    /// complex procedure, and might consist of many drawing calls.<br/>
    /// Generally, all actual drawing is done using the drawing methods Unity's
    /// <see cref="EditorGUI"/> offers.<br/>
    /// For more information, see the more concrete classes further down in this class hierarchy.
    /// </summary>
    /// <param name="positionX">
    /// The x-position that the component will be drawn at (this refers to the upper left corner of
    /// the rectangle encompassing the component).
    /// </param>
    /// <param name="positionY">
    /// The y-position that the component will be drawn at (this refers to the upper left corner of
    /// the rectangle encompassing the component).
    /// </param>
    public abstract void Draw(float positionX, float positionY);

    /// <summary>
    /// Returns a correctly sized <see cref="Rect"/> to position this component at the specified
    /// position.
    /// </summary>
    /// <param name="positionX">
    /// The x-position that the returned <see cref="Rect"/> should have (this refers to the upper
    /// left corner of the rectangle).
    /// </param>
    /// <param name="positionY">
    /// The y-position that the returned  <see cref="Rect"/> should have (this refers to the upper
    /// left corner of the rectangle).
    /// </param>
    /// <returns>
    /// A correctly sized <see cref="Rect"/> to position this component at the specified position.
    /// </returns>
    public Rect GetRect(float positionX, float positionY) {

        return (new Rect(positionX, positionY, Width, Height));
    }

    /// <summary>
    /// Returns whether the specified component <paramref name="other"/> fits into this component.
    /// <br/>
    /// "Fitting" in this case means that both the other component's width and height are smaller
    /// or, at most, equal, to those of this component.
    /// </summary>
    /// <param name="other">The component to check for fitting into this one.</param>
    /// <returns>Whether the specified component fits into this component.</returns>
    public bool FitsIntoThis(Component other) {

        /* We use the negation of "smaller or equal" and "and" here, so that we can use the simpler
         * "greater than" operator instead. */
        
        return ((ComponentUtils.GreaterThan(other.Width, Width) ||
                ComponentUtils.GreaterThan(other.Height, Height)) == false);
    }
    
    /// <summary>
    /// Returns whether this component fits into the specified component <paramref name="other"/>.
    /// <br/>
    /// "Fitting" in this case means that both this component's width and height are smaller or, at
    /// most, equal, to those of the other component.
    /// </summary>
    /// <param name="other">The component to check this component for fitting into for.</param>
    /// <returns>Wwhether this component fits into the specified component.</returns>
    public bool FitsIntoOther(Component other) {
        
        /* We use the negation of "smaller or equal" and "and" here, so that we can use the simpler
         * "greater than" operator instead. */
        
        return ((ComponentUtils.GreaterThan(Width, other.Width) ||
                 ComponentUtils.GreaterThan(Height, other.Height)) == false);
    }
    
    
    /* ---------- Overrides ---------- */

    /// <summary>
    /// Returns a simple string representation of this component.<br/>
    /// As the abstraction level of this class is so high, it does not contain a lot of useful
    /// information to build a string representation. For this reason, the representation is pretty
    /// bare-bones.
    /// </summary>
    /// <returns>
    /// A simple string representation of this component, featuring its dimensions.
    /// </returns>
    public override string ToString() {

        return ("Component with dimensions of (" + Width + ", " + Height + ")");
    }
}

}

}

}

