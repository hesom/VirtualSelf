using System;
using UnityEditor;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {

    
/// <summary>
/// A <see cref="Component"/> corresponding to
/// <see cref="EditorGUI.Button(UnityEngine.Rect,UnityEngine.GUIContent)"/> (and all related
/// overloads).<br/>
/// A button is a simple component, which is clickable and invokes a function (more precisely, an
/// <see cref="Action"/>) for each click. Apart from that, it just features some label text.
/// </summary>
public sealed class Button : DynamicPrimitiveComponent {

    /* ---------- Variables & Properties ---------- */

    /// <inheritdoc />
    public override float MinimumWidth { get; protected set; }
    
    /// <summary>
    /// The label text that is shown within this button (this may, of course, be empty). The button
    /// may not be smaller than the minimum size to fit this text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// The function which is called when the button is pressed.
    /// </summary>
    public Action Function { get; }


    /* ---------- Constructors ---------- */

    /// <summary>
    /// Creates a <see cref="Button"/> with the given label text and width, and connected to the
    /// given function.
    /// </summary>
    /// <param name="width">
    /// The width of this button component. Must be positive (>0), and big enough to hold at least
    /// <paramref name="text"/> (without it overflowing outside of the button).
    /// </param>
    /// <param name="text">
    /// The label text displayed inside of the button component. May be empty.
    /// </param>
    /// <param name="function">
    /// The function attached to this button component, which will be called when the button is
    /// pressed.
    /// </param>
    /// <param name="guiStyle">
    /// The GUI style that the component will use to calculate its own dimensions.
    /// </param>
    /// <exception cref="LayoutException">
    /// If <paramref name="width"/> is not big enough to make, at least, the button hold
    /// <paramref name="text"/>.
    /// </exception>
    public Button(float width, string text, Action function, GUIStyle guiStyle) {

        ComponentUtils.AssertGreaterThanZero(width, "width");

        Width = width;
        
        Text = text;
        Function = function;
        GuiStyle = guiStyle;

        float minWidth, maxWidth;
        GuiStyle.CalcMinMaxWidth(new GUIContent(text), out minWidth, out maxWidth);
        MinimumWidth = minWidth;
        
        ComponentUtils.AssertMinimumWidth(width, MinimumWidth);
        
        CalculateHeight();
    }
    
    /// <summary>
    /// Creates a <see cref="Button"/> with the given label text and width, and connected to the
    /// given function.
    /// </summary>
    /// <param name="width">
    /// The width of this button component. Must be positive (>0), and big enough to hold at least
    /// <paramref name="text"/> (without it overflowing outside of the button).
    /// </param>
    /// <param name="text">
    /// The label text displayed inside of the button component. May be empty.
    /// </param>
    /// <param name="function">
    /// The function attached to this button component, which will be called when the button is
    /// pressed.
    /// </param>
    /// <exception cref="LayoutException">
    /// If <paramref name="width"/> is not big enough to make, at least, the button hold
    /// <paramref name="text"/>.
    /// </exception>
    public Button(float width, string text, Action function) : 
            this(width, text, function, EditorStyles.label) {}


    /* ---------- Overrides ---------- */

    /// <inheritdoc />
    public override void Draw(float positionX, float positionY) {

        if (GUI.Button(GetRect(positionX, positionY), Text) == true) {
            
            Function.Invoke();
        }
    }

    /// <inheritdoc />
    public override GUIStyle GetDefaultGuiStyle() {

        return (EditorStyles.label);
    }

    /// <summary>
    /// The height of a button depends on its label text (<see cref="Text"/>), plus a small
    /// additional padding.
    /// </summary>
    protected override void CalculateHeight() {

        Height = (GuiStyle.CalcHeight(new GUIContent(Text), Width) + 5.0f);
    }
}

}