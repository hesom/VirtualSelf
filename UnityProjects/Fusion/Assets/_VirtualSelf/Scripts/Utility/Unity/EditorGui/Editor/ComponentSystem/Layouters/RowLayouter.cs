using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace VirtualSelf.Utility.Editor {

    
/// <summary>
/// This class models a "row layouter" for <see cref="Component"/>s. It is able to take a set of
/// components and layout them in one or multiple rows, in accordance with its own layout settings.
/// The components can then be drawn, processed further, etc., with using the layouting rectangles
/// (<see cref="Rect"/> instances) that this layouter returns. They can also be drawn directly after
/// layouting.<br/>
/// More specifically: This layouter can layout an arbitrary number of components given to it. The
/// components are layouted in a row, in the given order, from left to right. The width of the row
/// has to be specified. Once a row is "filled", a new row is started below it, until all components
/// have been "processed".<br/>
/// This layouter does currently not, however, retain any "memory" of components, it does not store
/// them internally.
/// </summary>
public class RowLayouter {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// The margins in all directions this layout will use when layouting its given components. The
    /// margins specify the empty space between the "outsides" of the components and the surrounding
    /// components (which are not included in the layout calls for this layouter).<br/>
    /// The margins will be included in the return value of <see cref="LayoutComponents"/>.<br/>
    /// The margins may also contain negative values.
    /// </summary>
    public Margins Margins { get; set; } = new Margins(5.0f);

    /// <summary>
    /// The horizontal padding this layouter should use when layouting its given components. The
    /// padding describes the space between each component and its neighbouring ones.<br/>
    /// The value of this is ignored if <see cref="LayoutEvenly"/> is set to <c>true</c>.
    /// </summary>
    public float PaddingHorizontal { get; set; } = 10.0f;

    /// <summary>
    /// The vertical padding this layouter should use when layouting its given components. The
    /// padding describes the space between each component and its neighbouring ones.
    /// </summary>
    public float PaddingVertical { get; set; } = 5.0f;

    /// <summary>
    /// The minimal horizontal padding that this layouter will use when layouting its given
    /// components evenly - this happens when <see cref="LayoutEvenly"/> is set to <c>true</c>.
    /// If the padding in a row would get smaller than this value, the layouter will start a new row
    /// instead.
    /// </summary>
    public float MinPaddingHorizontal { get; set; } = 10.0f;

    /// <summary>
    /// The horizontal alignment that this layouter will use for its given components when layouting
    /// them.<br/>
    /// The value of this is ignored if <see cref="LayoutEvenly"/> is set to <c>true</c>.
    /// </summary>
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;

    /// <summary>
    /// The vertical alignment that this layouter will use for its given components when layouting
    /// them.
    /// </summary>
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

    /// <summary>
    /// Whether this layouter should layout its given components evenly. This means that they will
    /// always be taking up the whole available width of the row, by being spaced out evenly over
    /// it. They will have equal paddings between each other, and also at the left and right
    /// borders.<br/>
    /// If this is <c>true</c>, the values of <see cref="PaddingHorizontal"/> and
    /// <see cref="HorizontalAlignment"/> will be ignored.<br/>
    /// If this is <c>false</c>, the value of <see cref="MinPaddingHorizontal"/> will be ignored.
    /// </summary>
    public bool LayoutEvenly { get; set; }
    
    
    /* ---------- Methods ---------- */

    /// <summary>
    /// Layouts, and then draws, all the components given by <paramref name="components"/>.<br/>
    /// This method is a convenience method for cases where the components are just layouted here to
    /// then be drawn right away. In this case, instead of calling <see cref="LayoutComponents"/>,
    /// this method can be called directly. Internally, it just calls <see cref="LayoutComponents"/>
    /// by itself and then draws all the components in a loop.
    /// </summary>
    /// <param name="components">
    /// The collection of components to be layouted and then drawn. See
    /// <see cref="LayoutComponents"/> for details.
    /// </param>
    /// <param name="startingPositionX">
    /// The X-position from where to start the layouting and the drawing. See
    /// <see cref="LayoutComponents"/> for details.
    /// </param>
    /// <param name="startingPositionY">
    /// The Y-position from where to start the layouting and the drawing. See
    /// <see cref="LayoutComponents"/> for details.
    /// </param>
    /// <param name="availableWidth">
    /// The available width for the row(s) of components to be layouted and then drawn. See
    /// <see cref="LayoutComponents"/> for details.
    /// </param>
    /// <returns>
    /// A <see cref="Rect"/> instance encompassing all the layouted components. See
    /// <see cref="LayoutComponents"/> for details.
    /// </returns>
    /// <exception cref="LayoutException">
    /// If <see cref="LayoutComponents"/> throws this same exception. See that method for details.
    /// </exception>
    /// <seealso cref="LayoutComponents"/>
    public Rect DrawComponents(
            IList<Component> components,
            float startingPositionX, float startingPositionY, float availableWidth) {
        
        IList<Rect> rects;

        Rect finalRect = LayoutComponents(
                components,
                startingPositionX, startingPositionY,
                availableWidth,
                out rects);
      
        for (int i = 0; i < components.Count; i++) {

            Rect currentRect = rects[i];
            components[i].Draw(currentRect.x, currentRect.y);
        }

        return (finalRect);
    }
    
    /// <summary>
    /// Layout all the components given by <paramref name="components"/>. The components will be
    /// layouted according to their attributes and the settings of this layouter instance.<br/>
    /// The method will return a layouting rectangle for each component, as well as a rectangle
    /// encompassing all of the layouted components (including their margins, and so on).
    /// </summary>
    /// <param name="components">
    /// The collection of components to be layouted. The components themselves will not be modified.
    /// </param>
    /// <param name="startingPositionX">
    /// The X-position from where to start the layouting.
    /// </param>
    /// <param name="startingPositionY">
    /// The Y-position from where to start the layouting.
    /// </param>
    /// <param name="availableWidth">
    /// The available width for the row(s) of components to be layouted. No component will go over
    /// the x-axis value of (<paramref name="startingPositionX"/> + this).
    /// </param>
    /// <param name="layoutRects">
    /// A collection of <see cref="Rect"/> instances, one for each component to layout. They can be
    /// used directly in Unity draw calls for the components.
    /// </param>
    /// <returns>
    /// A <see cref="Rect"/> instance encompassing all the layouted components - this includes their
    /// margin values (wich are set inside of this class), as well. The rect will thus specify the
    /// "boundaries" of the whole collection of layouted components that were given to this method.
    /// <br/>
    /// The rectangle will always start at the point (<paramref name="startingPositionX"/>,
    /// <paramref name="startingPositionY"/>).<br/>
    /// If <paramref name="components"/> is empty, the <c>width</c> and <c>height</c> of the
    /// returned rect will both be <c>0</c>.
    /// </returns>
    /// <exception cref="LayoutException">
    /// If <paramref name="availableWidth"/> is smaller or equal to <c>0</c>, or if even a single
    /// component is too wide to fit into a single row, even if all on its own (its <c>width</c> is
    /// greater than <paramref name="availableWidth"/>, together with the horizontal margins).
    /// </exception>
    public Rect LayoutComponents(
            IList<Component> components,
            float startingPositionX, float startingPositionY, float availableWidth,
            out IList<Rect> layoutRects) {

        layoutRects = new List<Rect>();
        
        if (components.Any() == false) {
            
            return (new Rect(startingPositionX, startingPositionY, 0.0f, 0.0f));
        }

        if (ComponentUtils.GreaterThanZero(availableWidth) == false) {
            
            throw new LayoutException(
                    "The provided available width (" + availableWidth + ") is 0 or negative.");
        }

        float greatestCompWidth = components.Max(comp => comp.Width);

        if ((greatestCompWidth + Margins.GetTotalHorizontalMargins()) > availableWidth) {
            
            throw new LayoutException(
                    "At least one of the provided components does not fit into the provided " + 
                    "available width (" + availableWidth + ") because its own width (combined " + 
                    "with the set horizontal margins) of " + greatestCompWidth + " is too high.");
        }

        /* At first, we have to "distribute" the components into rows. We will "fill" each row with
         * components, and when no more fit, start over at the next row. We don't have to properly
         * place the components yet (and indeed can't). */

        List<List<Component>> compRows = new List<List<Component>>();
        
        float paddingHorizontal = ((LayoutEvenly == true) ? MinPaddingHorizontal : PaddingHorizontal);
        
        float currentRowWidth = 0.0f;      
        int currentRow = 0;
        compRows.Add(new List<Component>());
        
        foreach (Component comp in components) {

            if ((currentRowWidth + paddingHorizontal + comp.Width + Margins.Right) >
                availableWidth) {

                currentRow++;
                compRows.Add((new List<Component>()));
                
                currentRowWidth = Margins.Left;
                currentRowWidth += comp.Width;
            }
            else {

                currentRowWidth += (paddingHorizontal + comp.Width);
            }
            
            compRows[currentRow].Add(comp);
        }
        
        /* Now, we can go through all the rows and their "assigned" components, and actually place
         * the components. We had to gather all the components for each row first because otherwise
         * we wouldn't be able to property align them, or (if required) calculate their
         * "LayoutEvenly" padding value. */

        float borderLeft = (startingPositionX + Margins.Left);
        float borderRight = (startingPositionX + availableWidth - Margins.Right);
        float borderTop = (startingPositionY + Margins.Top);
        
        float currentYPos = borderTop;
        
        foreach (List<Component> currentRowComps in compRows) {

            float rowCompsTotalWidth = currentRowComps.Sum(comp => comp.Width);
            
            /* Calculate the actual padding value to use for the components in this row. */
            
            float paddingHorizontalFinal;

            if (LayoutEvenly == true) {

                float leftoverSpace =
                        ((availableWidth - Margins.Left - Margins.Right) - rowCompsTotalWidth);
                paddingHorizontalFinal = (leftoverSpace / (currentRowComps.Count + 1));
            }
            else {

                paddingHorizontalFinal = PaddingHorizontal;
            }
            
            float currentXPos;
            
            /* Determine the starting X position for this row. This is different depending on the
             * horizontal alignment value, and whether "LayoutEvenly" is true or false. */

            float paddingsTotalWidth = (paddingHorizontalFinal * (currentRowComps.Count - 1));
            float rowTotalWidth = (rowCompsTotalWidth + paddingsTotalWidth);
            
            if ((LayoutEvenly == true) || (HorizontalAlignment == HorizontalAlignment.Left)) {

                currentXPos = (borderLeft + paddingHorizontalFinal);
            }
            else if (HorizontalAlignment == HorizontalAlignment.Right) {
                
                currentXPos = (borderRight - rowTotalWidth);
            }
            else {

                float middleXPos = (startingPositionX + (availableWidth / 2.0f));
                currentXPos = (middleXPos - (rowTotalWidth / 2.0f));

            }

            float biggestCompHeight = currentRowComps.Max(comp => comp.Height);
            
            /* Now, go through the components of the current row, and layout them. */
            
            foreach (Component comp in currentRowComps) {

                /* Determine the Y position of the current component. This, again, depends on the
                 * (vertical) alignment value. */
                
                float compYPos;

                if (VerticalAlignment == VerticalAlignment.Top) {

                    compYPos = currentYPos;
                }
                else if (VerticalAlignment == VerticalAlignment.Bottom) {

                    compYPos = ((currentYPos + biggestCompHeight) - comp.Height);
                }
                else {

                    compYPos = ((currentYPos + (biggestCompHeight / 2.0f)) - 
                                (comp.Height / 2.0f));
                }

                Rect compRect = new Rect(currentXPos, compYPos, comp.Width, comp.Height);
                layoutRects.Add(compRect);

                /* Update the X position for the next component after this one. */
                
                currentXPos += (comp.Width + paddingHorizontalFinal);
            }

            currentYPos += (biggestCompHeight + PaddingVertical);
        }
        
        /* As a last step, create the rect that encompasses all the layouted components, as well as
         * their margins. */
        
        Rect fullRect = new Rect(
                startingPositionX, startingPositionY,
                availableWidth, 
                ((currentYPos - PaddingVertical + Margins.Bottom)) - startingPositionY);

        return (fullRect);
    }
}

}
