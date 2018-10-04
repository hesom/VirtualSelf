namespace VirtualSelf.Utility.Editor {


/// <summary>
/// A small helper class that defines a set of margin values for a Unity Editor GUI component.<br/>
/// The "margins" of a component are the (empty) space between the component itself and any other
/// GUI components/elements surrounding it. They consist of margin values for the top, the bottom,
/// the left and the right side of the component.<br/>
/// <remarks>This is an immutable class.</remarks>
/// </summary>
public sealed class Margins {

    /* ---------- Variables & Properties ---------- */
    
    /// <summary>
    /// The top margin value. This may also be a negative number.
    /// </summary>
    public float Top { get; }
    
    /// <summary>
    /// The bottom margin value. This may also be a negative number.
    /// </summary>
    public float Bottom { get; }
    
    /// <summary>
    /// The left margin value. This may also be a negative number.
    /// </summary>
    public float Left { get; }
    
    /// <summary>
    /// The right margin value. This may also be a negative number.
    /// </summary>
    public float Right { get; }
    
    
    /* ---------- Constructors ---------- */

    /// <summary>
    /// Constructs a <see cref="Margins"/> instance with all margin values set to 0.
    /// </summary>
    public Margins() { }

    /// <summary>
    /// Constructs a <see cref="Margins"/> instance with all margin values set to the given value.
    /// </summary>
    /// <param name="allMargins">
    /// The value to set all margin values to. This may also be a negative value.
    /// </param>
    public Margins(float allMargins) {

        Top = allMargins;
        Bottom = allMargins;
        Left = allMargins;
        Right = allMargins;
    }

    /// <summary>
    /// Constructs a <see cref="Margins"/> instance with the horizontal margin values and the
    /// vertical margin values each set to the given values.
    /// </summary>
    /// <param name="horizontalMargins">
    /// The value to set the horizontal margin values (left and right) to.
    /// </param>
    /// <param name="verticalMargins">
    /// The value to set the vertical margin values (top and bottom) to.
    /// </param>
    public Margins(float verticalMargins, float horizontalMargins) {

        Top = verticalMargins;
        Bottom = verticalMargins;
        Left = horizontalMargins;
        Right = horizontalMargins;
    }
    
    /// <summary>
    /// Constructs a <see cref="Margins"/> instance with all margin values set to their respective
    /// given values. All of these values may also be negative values.
    /// </summary>
    /// <param name="topMargin">The value to set the top margin value to.</param>
    /// <param name="bottomMargin">The value to set the bottom margin value to.</param>
    /// <param name="leftMargin">The value to set the left margin value to.</param>
    /// <param name="rightMargin">The value to set the right margin value to.</param>
    public Margins(float topMargin, float bottomMargin, float leftMargin, float rightMargin) {

        Top = topMargin;
        Bottom = bottomMargin;
        Left = leftMargin;
        Right = rightMargin;
    }
    
    
    /* ---------- Methods ---------- */

    /// <summary>
    /// Returns the total vertical margins value for this <see cref="Margins"/> instance. This is
    /// the sum of the top and bottom margin values.
    /// </summary>
    /// <returns>
    /// The total vertical margins value for this <see cref="Margins"/> instance.
    /// </returns>
    public float GetTotalVerticalMargins() {

        return (Top + Bottom);
    }
    
    /// <summary>
    /// Returns the total horizontal margins value for this <see cref="Margins"/> instance. This is
    /// the sum of the left and right margin values.
    /// </summary>
    /// <returns>
    /// The total horizontal margins value for this <see cref="Margins"/> instance.
    /// </returns>
    public float GetTotalHorizontalMargins() {

        return (Left + Right);
    }

    /// <summary>
    /// Returns whether all margin values of this <see cref="Margins"/> instance are equal to each
    /// other.
    /// </summary>
    /// <returns>
    /// whether all margin values of this <see cref="Margins"/> instance are equal to each other.
    /// </returns>
    public bool AreAllMarginsEqual(float tolerance) {

        return (ComponentUtils.Equal(Top, Bottom) && ComponentUtils.Equal(Bottom, Left) &&
                ComponentUtils.Equal(Left, Right));
    }
    
    /// <summary>
    /// Returns whether the vertical margin values of this <see cref="Margins"/> instance are equal
    /// to each other.
    /// </summary>
    /// <returns>
    /// whether the vertical margin values (top and bottom) of this <see cref="Margins"/> instance
    /// are equal to each other.
    /// </returns>
    public bool AreVerticalMarginsEqual() {

        return (ComponentUtils.Equal(Top, Bottom));
    }

    /// <summary>
    /// Returns whether the horizontal margin values of this <see cref="Margins"/> instance are
    /// equal to each other.
    /// </summary>
    /// <returns>
    /// whether the horizontal margin values (left and right) of this <see cref="Margins"/> instance
    /// are equal to each other.
    /// </returns>
    public bool AreHorizontalMarginsEqual() {

        return (ComponentUtils.Equal(Left, Right));
    }
    

    /* ---------- Overrides ---------- */

    public override string ToString() {

        string marginsString = 
                ("Margins (Left: " + Left + ", Right: " + Right + ", Top: " + Top + ", Bottom: " + 
                Bottom + ")");

        return (marginsString);
    }

    /// <summary>
    /// Two <see cref="Margins"/> instances are equal to each other if and only if all of their
    /// margin values are equal.
    /// </summary>
    public override bool Equals(object obj) {
        
        if (ReferenceEquals(null, obj)) return (false);
        if (ReferenceEquals(this, obj)) return (true);

        var margins = obj as Margins;
        return ((margins != null) && (Equals(margins)));
    }

    public override int GetHashCode() {
        
        unchecked {
            
            int hashCode = Top.GetHashCode();
            hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
            hashCode = (hashCode * 397) ^ Left.GetHashCode();
            hashCode = (hashCode * 397) ^ Right.GetHashCode();
            
            return (hashCode);
        }
    }
    
    private bool Equals(Margins other) {
        
        return (ComponentUtils.Equal(Top, other.Top) && ComponentUtils.Equal(Bottom, other.Bottom) && 
                ComponentUtils.Equal(Left, other.Left) && ComponentUtils.Equal(Right, other.Right));
    }
    
    
    /* ---------- Operator Overloads ---------- */
    
    /// <summary>
    /// The returned value of this operator is the same as when using <see cref="Equals"/> for this
    /// class.
    /// </summary>
    public static bool operator ==(Margins left, Margins right) {
        
        return (Equals(left, right));
    }

    public static bool operator !=(Margins left, Margins right) {
        
        return (!Equals(left, right));
    }
}

}