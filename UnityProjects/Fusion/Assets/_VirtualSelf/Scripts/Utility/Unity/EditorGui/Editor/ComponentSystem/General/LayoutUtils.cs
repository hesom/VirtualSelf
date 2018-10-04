using UnityEngine;


namespace VirtualSelf.Utility.Editor {

	
/// <summary>
/// A collection of static utility methods to help with basic layouting tasks for
/// <see cref="Component"/>s of different kinds and in different combinations.<br/>
/// For more complex layouting tasks, the dedicated layouting classes like <see cref="RowLayouter"/>
/// should be used.
/// </summary>
public static class LayoutUtils {

	/// <summary>
	/// Layout a <see cref="Label"/> component together with another (generic) component. The label
	/// and the other component can be placed in any relation to one another, depending on the
	/// given values for the method arguments.
	/// <remarks>
	/// This method assumes that the label is smaller (or equal) in both its width and height
	/// compared to the other component. While it will not result in erroneous behavior if this is
	/// untrue, the values for <paramref name="labelHorizAlign"/> and
	/// <paramref name="labelVertAlign"/> will then have no effect.
	/// </remarks>
	/// </summary>
	/// <param name="label">The label component to layout with the other component.</param>
	/// <param name="component">The component to layout with the label.</param>
	/// <param name="padding">
	/// The padding (empty space) that should be between the label and the other component.
	/// </param>
	/// <param name="labelPos">The position of the label relative to the other component.</param>
	/// <param name="labelHorizAlign">
	/// The horizontal alignment of the label relative to the other component, if
	/// <paramref name="labelPos"/> is <see cref="Position.Top"/> or <see cref="Position.Bottom"/>
	/// (otherwise, this value has no effect).
	/// </param>
	/// <param name="labelVertAlign">
	/// The vertical alignment of the label relative to the other component, if
	/// <paramref name="labelPos"/> is <see cref="Position.Left"/> or <see cref="Position.Right"/>
	/// (otherwise, this value has no effect).
	/// </param>
	/// <param name="localPosLabel">The resulting local position of the label component.</param>
	/// <param name="localPosComponent">The resulting local position of the other component.</param>
	public static void LayoutLabelAndComponent(
			Label label, Component component,
			float padding,
			Position labelPos,
			HorizontalAlignment labelHorizAlign, VerticalAlignment labelVertAlign,
			out Vector2 localPosLabel, out Vector2 localPosComponent) {

		Vector2 labelLocalPos, compLocalPos;
		labelLocalPos = compLocalPos = Vector2.zero;

		if ((labelPos == Position.Left) || (labelPos == Position.Right)) {

			compLocalPos.y = 0.0f;
			
			if (label.Height >= component.Height) {

				labelLocalPos.y = 0.0f;
			}
			else {

				if (labelVertAlign == VerticalAlignment.Top) {

					labelLocalPos.y = 0.0f;
				}
				else if (labelVertAlign == VerticalAlignment.Center) {

					labelLocalPos.y = ((component.Height / 2.0f) - (label.Height / 2.0f));
				}
				else if (labelVertAlign == VerticalAlignment.Bottom) {

					labelLocalPos.y = (component.Height - label.Height);
				}
			}

			if (labelPos == Position.Left) {

				labelLocalPos.x = 0.0f;
				compLocalPos.x = (label.Width + padding);
			}
			else if (labelPos == Position.Right) {

				compLocalPos.x = 0.0f;
				labelLocalPos.x = (component.Width + padding);
			}
		}
		else if ((labelPos == Position.Top) || (labelPos == Position.Bottom)) {

			compLocalPos.x = 0.0f;

			if (label.Width >= component.Width) {

				labelLocalPos.x = 0.0f;
			}
			else {

				if (labelHorizAlign == HorizontalAlignment.Left) {

					labelLocalPos.x = 0.0f;
				}
				else if (labelHorizAlign == HorizontalAlignment.Center) {
					
					labelLocalPos.x = ((component.Width / 2.0f) - (label.Width / 2.0f));
				}
				else if (labelHorizAlign == HorizontalAlignment.Right) {

					labelLocalPos.x = (component.Width - label.Width);
				}
			}

			if (labelPos == Position.Top) {

				labelLocalPos.y = 0.0f;
				compLocalPos.y = (label.Height + padding);
			}
			else if (labelPos == Position.Bottom) {

				compLocalPos.y = 0.0f;
				labelLocalPos.y = (component.Height + padding);
			}
		}

		localPosLabel = labelLocalPos;
		localPosComponent = compLocalPos;
	} 

}

}
