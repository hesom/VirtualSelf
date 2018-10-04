using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {


/// <summary>
/// A <see cref="Component"/> corresponding to
/// <see cref="EditorGUI"/>.<see cref="EditorGUI.HelpBox"/> (and all related overloads).<br/>
/// A "help box" is a box featuring some text, and is generally used as a "message box" to inform,
/// warn or notify the user of something. The box can also feature an icon that shows the "nature"
/// or "type" of the message.<br/>
/// As the box just contains text to display, like a label, it does not manage any value or hold any
/// state.
/// </summary>
public sealed class HelpBox : DynamicPrimitiveComponent {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// A list of estimated widths for the icon that is displayed inside of a help nox box
    /// component, if <see cref="MessageType"/> is anything except
    /// <see cref="UnityEditor.MessageType.None"/>, depending on the amount of lines the text inside
    /// of the message box needs to be drawn (the line amount is encoded by the list position of
    /// each respective width value).<br/>
    /// The icon of a <c>HelpBox</c> has a fixed maximum size, but shrinks depending on the height
    /// of the box. This in turn determines how much text fits into one line, which in turn
    /// determines the required height to draw all the text...<br/>
    /// This list is needed to make an estimate for the height of this component. For more
    /// information, see <see cref="CalculateHeight"/>.
    /// </summary>
    private static readonly IList<float> IconWidthEstimations = 
            new ReadOnlyCollection<float>(new List<float> {
                
                15.0f, 22.0f, 35.0f
    });

    /// <summary>
    /// The minimum reasonable width for a help box component.
    /// </summary>
    private const float MinimumWidthHelpBoxes = 100.0f;

    /// <inheritdoc/>
    public override float MinimumWidth { get; protected set; } = MinimumWidthHelpBoxes;
    
    /// <summary>
    /// The message text of this HelpBox component. This text is shown inside of the box. The text
    /// is used to calculate the height of the component.
    /// </summary>
    public string MessageText { get; }
    
    /// <summary>
    /// The message type of this HelpBox component. This determines the icon shown on the left of
    /// the box (if any).
    /// </summary>
    public MessageType MessageType { get; }


    /* ---------- Constructors ---------- */

    /// <summary>
    /// Creates a <see cref="HelpBox"/> from a message text and a GUI style, with a given
    /// width and message type.
    /// </summary>
    /// <param name="width">
    /// The width of this help box component. This must be a positive (>0) value.
    /// </param>
    /// <param name="messageText">
    /// The message text that will be shown inside of this help box component.
    /// </param>
    /// <param name="messageType">
    /// The message type of this help box component, which determines its icon (if any).
    /// </param>
    /// <param name="guiStyle">
    /// The GUI style that the component will use to calculate its own dimensions.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="width"/> is not positive (>0).
    /// </exception>
    /// <exception cref="LayoutException">
    /// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
    /// </exception>
    public HelpBox(
            float width, string messageText, MessageType messageType, GUIStyle guiStyle) {

        ComponentUtils.AssertGreaterThanZero(width, "width");
        ComponentUtils.AssertMinimumWidth(width, MinimumWidth);
        
        Width = width;
        Height = -1;
        
        GuiStyle = guiStyle;
        MessageText = messageText;
        MessageType = messageType;
        
        CalculateHeight();
    }

    /// <summary>
    /// Creates a <see cref="HelpBox"/> from a message text, with a given width and message
    /// type. It will use the default GUI style of Unity for <c>HelpBox</c> components.
    /// </summary>
    /// <param name="width">
    /// The width of this help box component. This must be a positive (>=0) value.
    /// </param>
    /// <param name="messageText">
    /// The message text that will be shown inside of this help box component.
    /// </param>
    /// <param name="messageType">
    /// The message type of this help box component, which determines its icon (if any).
    /// </param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="width"/> is not positive (>0).
    /// </exception>
    /// <exception cref="LayoutException">
    /// If <paramref name="width"/> is smaller than <see cref="MinimumWidth"/>.
    /// </exception>
    public HelpBox(float width, string messageText, MessageType messageType) : 
                   this(width, messageText, messageType, EditorStyles.helpBox) { }
    
    
    /* ---------- Overrides ---------- */

    /// <inheritdoc/>
    public override void Draw(float positionX, float positionY) {
        
        EditorGUI.HelpBox(GetRect(positionX, positionY), MessageText, MessageType);
    }

    /// <inheritdoc/>
    public override GUIStyle GetDefaultGuiStyle() {

        return (EditorStyles.helpBox);
    }

    /// <summary>
    /// The height of a <c>HelpBox</c> depends on the text inside. However, there is a problem:<br/>
    /// <see cref="GUIStyle.CalcHeight"/> can calculate the needed height for the box depending on
    /// the text inside just fine <i>if</i> the <see cref="UnityEditor.MessageType"/> of the box is
    /// <see cref="UnityEditor.MessageType.None"/>. For all other message types, it returns
    /// incorrect results in some cases because it does not seem to correctly factor in the size of
    /// the icon inside the box.<br/>
    /// Because of this, we have to calculate the height on our own, which requires making some
    /// estimates/guesses about e.g. the size of the icon (as the icon has a fixed original size,
    /// but is scaled down if the box height is lower than the size of the icon - which in turn
    /// changes how much text fits into one line, which in turn influences the height of the box,
    /// and so on...).<br/>
    /// The estimates we make are quite primitive, and so I hope that this works for all corner
    /// cases ocurring in practice.
    /// </summary>
    protected override void CalculateHeight() {

        /* Calculate the "automatic height" for the box. */
        
        float autoHeight = GuiStyle.CalcHeight(new GUIContent(MessageText), Width);
        
        /* The automatic height is only correct if the message type of the box is "None". Otherwise,
         * we have to estimate the "true" required height. */
        
        if (MessageType == MessageType.None) {

            Height = autoHeight;
            return;
        }
        
        /*
         * Algorithm for the height estimation:
         * 1. Determine the amount of lines inside the box the message text would need if there was
         *    no icon. We do not do proper word wrapping here, but just split the lines by the
         *    available width if they are too long - since this is just an estimation anyway, this
         *    should be enough.
         * 2. Subtract some width for the icon from the available width, depending on the amount of
         *    lines - we just estimate how big the icon will probably be with these many lines,
         *    based on observations in the Unity Inspector.
         * 3. Use Unity functions to calculate the height of the box, now using the new, adjusted
         *    available width.
         */

        string[] messageLines = MessageText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        int linesCount = messageLines.Length;
      
        foreach (string line in messageLines) {
            
            float minWidth, maxWidth;
            GuiStyle.CalcMinMaxWidth(new GUIContent(line), out minWidth, out maxWidth);

            if (maxWidth > Width) { linesCount += Mathf.RoundToInt(maxWidth / Width); }
        }
        
        float iconWidth = (linesCount < IconWidthEstimations.Count)
                        ? IconWidthEstimations[linesCount]
                        : IconWidthEstimations.Last();

        float finalHeight = GuiStyle.CalcHeight(new GUIContent(MessageText), (Width - iconWidth));

        Height = finalHeight;
    }
}

}
