using System;


namespace VirtualSelf.Utility.Editor {

    
/// <summary>
/// A collection of static utility methods for the <see cref="Component"/> system, and everything
/// related to it.<br/>
/// The methods contained in here are mainly for convenience, but there are also some important
/// definitions. Especially all the math-related methods in here should, when working with
/// <see cref="Component"/>s, be preferred over the more general math methods anywhere else, as
/// they are more specialized.
/// </summary>
public static class ComponentUtils {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// The epsilon value to use for comparison (and ordering) operations of <see cref="Component"/>
    /// dimension (width, height, etc.) values.<br/>
    /// As these are, necessarily, floating-point values, they are generally not exact. An epsilon
    /// value is required for "correct" comparisons/ordering. This value can also not be chosen
    /// arbitrarily (meaning, from somewhere outside), since different contexts require different
    /// epsilons (e.g., do the values represent units of measurements, monetary values, etc.).<br/>
    /// For GUI elements, a very low epsilon is not really required, as they only have to be sized,
    /// aligned, etc. good enough so that the human eye can't really make out a difference anymore
    /// (and/or the screen in question cannot display it in the first place).
    /// </summary>
    public const float DimsEpsilon = 0.01f;


    /* ---------- Methods ---------- */

    /// <summary>
    /// Whether <paramref name="firstValue"/> and <paramref name="secondValue"/> are "equal" with
    /// respect to <see cref="DimsEpsilon"/>.
    /// </summary>
    /// <returns>Whether the two given values are "equal".</returns>
    public static bool Equal(float firstValue, float secondValue) {

        return (MathExtensions.ApproximatelyEqual(firstValue, secondValue, DimsEpsilon));
    }

    /// <summary>
    /// Whether <paramref name="firstValue"/> is "less than" <paramref name="secondValue"/>, with
    /// respect to <see cref="DimsEpsilon"/>.
    /// </summary>
    /// <returns>Whether the first given value is "less than" the second one.</returns>
    public static bool LessThan(float firstValue, float secondValue) {

        return (MathExtensions.DefinitelyLessThan(firstValue, secondValue, DimsEpsilon));
    }
    
    /// <summary>
    /// Whether <paramref name="firstValue"/> is "greater than" <paramref name="secondValue"/>, with
    /// respect to <see cref="DimsEpsilon"/>.
    /// </summary>
    /// <returns>Whether the first given value is "greater than" the second one.</returns>
    public static bool GreaterThan(float firstValue, float secondValue) {

        return (MathExtensions.DefinitelyGreaterThan(firstValue, secondValue, DimsEpsilon));
    }

    /// <summary>
    /// A convenience method to check whether the given value is "greater than" (with respect to
    /// <see cref="DimsEpsilon"/>) zero (<c>0.0f</c>).
    /// <remarks>
    /// This method just uses <see cref="GreaterThan"/> under the hood.
    /// </remarks>
    /// </summary>
    /// <returns>Whether the given value is "greater than" zero.</returns>
    public static bool GreaterThanZero(float value) {

        return (GreaterThan(value, 0.0f));
    }
    
    /// <summary>
    /// A convenience method that asserts that <paramref name="value"/> is "greater than" (with
    /// respect to <see cref="DimsEpsilon"/>) zero. If the assertion fails, an exception is thrown.
    /// <remarks>
    /// This method just uses <see cref="GreaterThanZero"/> under the hood.
    /// </remarks>
    /// </summary>
    /// <param name="value">The value to assert to be greater than zero.</param>
    /// <param name="name">The name of the value, to be used within the exception message.</param>
    /// <exception cref="ArgumentException">If the assertion fails.</exception>
    public static void AssertGreaterThanZero(float value, string name) {

        if (GreaterThanZero(value) == false) {
            
            throw new ArgumentException(
                "Invalid component " + name + " (" + value + "). The " + name + " of a component " + 
                "must be greater than 0.");
        }
    }
    
    /// <summary>
    /// A convenience method that asserts that the width value <paramref name="width"/> is not
    /// smaller ("less than", with respect to <see cref="DimsEpsilon"/>) than the minimum width
    /// value <paramref name="minimumWidth"/>. If the assertion fails, an exception is thrown.
    /// <remarks>
    /// This method just uses <see cref="LessThan"/> under the hood.
    /// </remarks>
    /// </summary>
    /// <exception cref="LayoutException">If the assertion fails.</exception>
    public static void AssertMinimumWidth(float width, float minimumWidth) {
        
        AssertMinimumDimension(width, minimumWidth, "width");
    }
    
    /// <summary>
    /// A convenience method that asserts that the height value <paramref name="height"/> is not
    /// smaller ("less than", with respect to <see cref="DimsEpsilon"/>) than the minimum height
    /// value <paramref name="minimumHeight"/>. If the assertion fails, an exception is thrown.
    /// <remarks>
    /// This method just uses <see cref="LessThan"/> under the hood.
    /// </remarks>
    /// </summary>
    /// <exception cref="LayoutException">If the assertion fails.</exception>
    public static void AssertMinimumHeight(float height, float minimumHeight) {
        
        AssertMinimumDimension(height, minimumHeight, "height");
    }

    /// <summary>
    /// The actual method used by <see cref="AssertMinimumWidth"/> and
    /// <see cref="AssertMinimumHeight"/>. See those for details.
    /// </summary>
    private static void AssertMinimumDimension(
            float value, float minimumValue, string dimensionName) {

        if (LessThan(value, minimumValue) == true) {
            
            throw new LayoutException(
                "The given " + dimensionName + " (" + value + ") for this component is smaller " + 
                "than its possible minimum " + dimensionName + " (" + minimumValue + ").");       
        }    
    }
}

}