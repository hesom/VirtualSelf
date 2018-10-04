using System;
using UnityEditor;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {


/// <summary>
/// A <see cref="Component"/> corresponding to <see cref="EditorGUI.Toggle(UnityEngine.Rect,bool)"/>
/// and <see cref="EditorGUI"/>.<see cref="EditorGUI.ToggleLeft(UnityEngine.Rect,string,bool)"/>
/// (and all related overloads).<br/>
/// As outlined there, this component features a label and a little checkbox at its side (the right
/// side for <c>Toggle</c> and the left side for <c>ToggleLeft</c>). The component controls a single
/// boolean value, and the checkbox can be clicked to change that value.
/// </summary>
public sealed class Toggle : AutomaticPrimitiveComponent {
    
    /* ---------- Enumerations ---------- */

    /// <summary>
    /// An enum describing the different ways to place the checkbox of a toggle component, in
    /// relation to its label text.
    /// </summary>
    public enum CheckboxPlacement {

        /// <summary>
        /// The checkbox is placed on the left side of the label text. This corresponds to
        /// <see cref="EditorGUI.ToggleLeft(UnityEngine.Rect,string,bool)"/>.
        /// </summary>
        Left,
        /// <summary>
        /// The checkbox is placed on the left side of the label text. This corresponds to
        /// <see cref="EditorGUI.Toggle(UnityEngine.Rect,bool)"/>.
        /// </summary>
        Right
    }
        
    
    /* ---------- Variables & Properties ---------- */
    
    /// <summary>
    /// Where to place the checkbox of this toggle component, in relation to its label text.
    /// </summary>
    public CheckboxPlacement CheckboxPlace { get; }
    
    /// <summary>
    /// The label text of this toggle component. The label text is shown next to it, the side
    /// depending on the value of <see cref="CheckboxPlace"/>.
    /// </summary>
    public string LabelText { get; }
    
    /// <summary>
    /// The value of this toggle component. <c>true</c> means its checkbox is ticked, <c>false</c>
    /// means it is not.
    /// </summary>
    public bool Value { get; set; }
 

    /* ---------- Constructors ---------- */

    /// <summary>
    /// Creates a <see cref="Toggle"/> from a label text and a GUI style, and with its (initial)
    /// state set to <paramref name="value"/>.
    /// </summary>
    /// <param name="labelText">The label text that this toggle component should have.</param>
    /// <param name="value">The value of this toggle component.</param>
    /// <param name="checkboxPlacement">
    /// Where to place the checkbox of this toggle component, in relation to its label text.
    /// </param>
    /// <param name="guiStyle">
    /// The GUI style that this toggle component will use to calculate its own dimensions and to
    /// draw itself.
    /// </param>
    public Toggle(string labelText, bool value,
                  CheckboxPlacement checkboxPlacement, GUIStyle guiStyle) {

        LabelText = labelText;
        Value = value;
        CheckboxPlace = checkboxPlacement;
        GuiStyle = guiStyle;
        
        CalculateDimensions();
    }

    /// <summary>
    /// Creates a <see cref="Toggle"/> from a label text, and with its (initial) state set to
    /// <paramref name="value"/>. It will use the default GUI style of Unity for the <c>Toggle</c>
    /// GUI component.
    /// </summary>
    /// <param name="labelText">The label text that this toggle component should have.</param>
    /// <param name="value">The value of this toggle component.</param>
    /// <param name="checkboxPlacement">
    /// Where to place the checkbox of this toggle component, in relation to its label text.
    /// </param>
    public Toggle(string labelText, bool value, CheckboxPlacement checkboxPlacement) : 
                  this(labelText, value, checkboxPlacement, EditorStyles.toggle) { }
    
    
    /* ---------- Overrides ---------- */
    
    /// <inheritdoc/>
    public override void Draw(float positionX, float positionY) {

        bool result;

        if (CheckboxPlace == CheckboxPlacement.Left) {
            
            result = EditorGUI.ToggleLeft(GetRect(positionX, positionY), LabelText, Value);
        }
        else if (CheckboxPlace == CheckboxPlacement.Right) {

            result = EditorGUI.Toggle(GetRect(positionX, positionY), LabelText, Value);
        }
        else {
            
            throw new NotImplementedException(
                "This placement for a \"Toggle\" component checkbox has not been implemented yet.");
        }

        Value = result;
    }

    /// <inheritdoc/>
    public override GUIStyle GetDefaultGuiStyle() {

        return (EditorStyles.toggle);
    }

    /// <summary>
    /// Since a "Toggle" component is just a checkbox with a label, its dimensions are easy to
    /// calculate. It will always have the height of a single line of text, and its width are just
    /// the fixed width of a checkbox, a little bit of padding space, and the width of the label
    /// text.
    /// </summary>
    protected override void CalculateDimensions() {
        
        float widthMin, widthMax;
        GUIContent guiContent = new GUIContent(LabelText);
        GuiStyle.CalcMinMaxWidth(guiContent, out widthMin, out widthMax);

        float height = GuiStyle.CalcHeight(guiContent, widthMax);

        Width = widthMax;
        Height = height;
    }
}

}
