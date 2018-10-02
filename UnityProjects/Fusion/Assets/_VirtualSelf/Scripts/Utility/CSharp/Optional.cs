using System;
using System.Collections;
using System.Collections.Generic;


namespace VirtualSelf {

namespace Utility {

    
/// <summary>
/// A simple implementation of an "optional" type. This is also called a "maybe monad".<br/>
/// This is a utility class to be used for passing around values which might or might not exist. It
/// should be used over the <c>null</c> value.<br/>
/// For reasons of why to follow the advice given above, and why to generally use this class,
/// consider for example:<br/>
/// http://www.oracle.com/technetwork/articles/java/java8-optional-2175753.html <br/>
/// https://softwareengineering.stackexchange.com/q/309134/277218 <br/>
/// (While these are about Java, almost the exact same principle apply to C# in just the same way,
/// as its type system and the existence of "null" is very similar to Java, at least for all
/// reference types.)
/// <br/><br/>
/// The class and method naming and design for this class were inspired by Java's Optional class:
/// https://docs.oracle.com/javase/9/docs/api/java/util/Optional.html <br/>
/// This class can also be used (in a limited way) within LINQ statements. This also makes it
/// possible to use it without branching <c>if</c> paths to check for an existing value. This is
/// made possible by implementing the <see cref="IEnumerable{T}"/> interface. For the origin of this
/// idea, and details about it, see here:<br/>
/// http://codinghelmet.com/?path=howto/reduce-cyclomatic-complexity-option-functional-type <br/>
/// Some further ideas were taken from here: <br/>
/// https://softwareengineering.stackexchange.com/q/308211/277218
/// </summary>
/// <typeparam name="T">The type of this optional type.</typeparam>
public sealed class Optional<T> : IEnumerable<T> {

    /* ---------- Variables & Properties ---------- */
    
    /// <summary>
    /// The value this <see cref="Optional{T}"/> (maybe) contains. If it contains no value, this has
    /// a length of <c>0</c>, if it does this has a length of <c>1</c>.<br/>
    /// This is an array because it can work together with the <see cref="IEnumerable{T}"/>
    /// interface to make this class usable in LINQ statements.
    /// </summary>
    private readonly T[] value;
    
    
    /* ---------- Constructors ---------- */

    /// <summary>
    /// This constructor is private, because <see cref="Optional{T}"/> instances are not created
    /// directly. They are only created through the static factory methods of this class.<br/>
    /// This constructor also does not do any kind of checking - it relies on the static factory
    /// methods only calling it in a safe way. */
    /// </summary>
    /// <param name="value">The value of this <see cref="Optional{T}"/>.</param>
    private Optional (T[] value) {

        this.value = value;
    }

    
    /* ---------- Methods ---------- */

    /// <summary>
    /// Creates an <see cref="Optional{T}"/> of type <c>T</c> containing <paramref name="value"/>,
    /// if <paramref name="value"/> is not <c>null</c>.<br/>
    /// If it is <c>null</c>, throws an exception.
    /// </summary>
    /// <param name="value">
    /// The value to create an <see cref="Optional{T}"/> of. Must not be <c>null</c>.
    /// </param>
    /// <returns>A new <see cref="Optional{T}"/> containing <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="value"/>is <c>null</c>.
    /// </exception>
    public static Optional<T> Of(T value)  {

        if (value == null) {
            
            throw new ArgumentNullException(
                    "Optional cannot be constructed from a null value. " + 
                    "(Use \"OfNullable()\" instead.)");
        }
        
        return (new Optional<T>(new[] {value}));
    }
    
    /// <summary>
    /// Creates an <see cref="Optional{T}"/> of type <c>T</c> containing <paramref name="value"/>,
    /// if <paramref name="value"/>is not <c>null</c>.<br/>
    /// If it is <c>null</c>, creates an empty <see cref="Optional{T}"/> of type <c>T</c>. Empty
    /// means that the created <see cref="Optional{T}"/> will have no value present.
    /// </summary>
    /// <param name="value">The value to create an <see cref="Optional{T}"/> of.</param>
    /// <returns>
    /// A new <see cref="Optional{T}"/> containing <paramref name="value"/>, or an empty
    /// <see cref="Optional{T}"/>.
    /// </returns>
    public static Optional<T> OfNullable(T value) {

        if (value == null) { return (Empty()); }
        
        return (Of(value));
    }

    /// <summary>
    /// Creates an empty <see cref="Optional{T}"/> of type <c>T</c>. Empty means that the created
    /// <see cref="Optional{T}"/> will have no value present.
    /// </summary>
    /// <returns>An empty <c>Optional</c> (of type <c>T</c>).</returns>
    public static Optional<T> Empty() {

        return (new Optional<T>(new T[0]));
    }

    /// <summary>
    /// Returns the value of this <see cref="Optional{T}"/>, if present.<br/>
    /// Otherwise, throws an exception.
    /// </summary>
    /// <returns>The value of this <see cref="Optional{T}"/>, if present.</returns>
    /// <exception cref="InvalidOperationException">
    /// If no value is present (the <see cref="Optional{T}"/> is empty).
    /// </exception>
    public T Get() {

        if (IsPresent() == true) { return (value[0]); }
        
        throw new InvalidOperationException("Optional does not contain a value (is empty).");
    }

    /// <summary>
    /// Returns the value of this <see cref="Optional{T}"/> if it is present,
    /// or <paramref name="other"/> otherwise.
    /// </summary>
    /// <param name="other">
    /// The value to return if this <see cref="Optional{T}"/> does not contain a value.
    /// </param>
    /// <returns>
    /// The value of this <see cref="Optional{T}"/> if it is present, or <paramref name="other"/>
    /// otherwise.
    /// </returns>
    public T OrElse(T other) {

        if (IsPresent() == true) { return (value[0]); }
        
        return (other);
    }

    /// <summary>
    /// Returns <c>true</c> if a value is present within this <see cref="Optional{T}"/>, and
    /// <c>false</c> otherwise.
    /// </summary>
    /// <returns>
    /// <c>true</c> if a value is present within this <see cref="Optional{T}"/>, and
    /// <c>false</c> otherwise.
    /// </returns>
    public bool IsPresent() {

        return (value.Length > 0);
    }

    
    /* ---------- Overrides ---------- */

    /// <summary>
    /// Returns whether this <see cref="Optional{T}"/> is equal to <paramref name="obj"/>. They are
    /// equal if:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// The given object is also an <see cref="Optional{T}"/>, and:
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Both this <see cref="Optional{T}"/> and the given <see cref="Optional{T}"/> are empty (have
    /// no value), or:
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Both this <see cref="Optional{T}"/> and the given <see cref="Optional{T}"/> have values
    /// present, and the values compare equal via their <see cref="Equals"/> methods.
    /// </description>
    /// </item>
    /// </list>
    /// (In any other case, they are not equal.)
    /// </summary>
    /// <param name="obj">The object to compare this <see cref="Optional{T}"/>with.</param>
    /// <returns><c>true</c> if both objects are equal, <c>false</c> otherwise.</returns>
    public override bool Equals(object obj) {

        if (ReferenceEquals(null, obj)) { return (false); }
        if (ReferenceEquals(this, obj)) { return (true); }
        
        if (GetType() != obj.GetType()) { return (false); }

        Optional<T> other = (Optional<T>) obj;

        if (IsPresent() != other.IsPresent()) { return (false); }
        if ((IsPresent() == false) && (other.IsPresent() == false)) { return (true); }
        
        return (value[0].Equals(other.value[0]));
    }

    /// <summary>
    /// Returns the hash code of the value present in this <see cref="Optional{T}"/>, or <c>0</c> if
    /// no value is present.
    /// </summary>
    /// <returns>The hash code of this <see cref="Optional{T}"/>.</returns>
    public override int GetHashCode() {

        if (IsPresent() == false) { return (0); }
        
        return (value[0].GetHashCode());
    }

    /// <summary>
    /// Returns a string representation for this <see cref="Optional{T}"/>.<br/>
    /// If it contains a value, the representation will include the string representation of the
    /// value, as well (its <see cref="object.ToString"/> method will be used). If it doesn't, the
    /// string representation will not be unique. In any case, the type <typeparamref name="T"/>
    /// will be included in the representation, as well.
    /// </summary>
    /// <returns>A string representation for this <see cref="Optional{T}"/>.</returns>
    public override string ToString() {

        string description = "Optional<" + typeof(T) + ">(";

        if (IsPresent() == false) { description += "Empty"; }
        else { description += Get(); }
        description += ")";

        return (description);
    }
    
    
    /* ---------- Operator Overloads ---------- */

    /// <summary>
    /// Returns whether the two <see cref="Optional{T}"/> instances are equal.<br/>
    /// This overload uses the <see cref="Equals"/> overridden method, and returns the same result.
    /// </summary>
    /// <param name="left">The first <see cref="Optional{T}"/> instance for the comparison.</param>
    /// <param name="right">The second <see cref="Optional{T}"/> instance for the comparison.</param>
    /// <returns>Whether the two <see cref="Optional{T}"/> instances are equal.</returns>
    public static bool operator ==(Optional<T> left, Optional<T> right) {
        
        return (Equals(left, right));
    }

    /// <summary>
    /// Returns whether the two <see cref="Optional{T}"/> instances are <i>not</i> equal.<br/>
    /// This overload uses the <see cref="Equals"/> overridden method, and returns the same result.
    /// </summary>
    /// <param name="left">The first <see cref="Optional{T}"/> instance for the comparison.</param>
    /// <param name="right">The second <see cref="Optional{T}"/> instance for the comparison.</param>
    /// <returns>Whether the two <see cref="Optional{T}"/> instances are <i>not</i> equal.</returns>
    public static bool operator !=(Optional<T> left, Optional<T> right) {
        
        return (!Equals(left, right));
    }
    

    /* ---------- Interface Implementations ---------- */

    public IEnumerator<T> GetEnumerator() {

        IEnumerable<T> enumerable = value;
        return (enumerable.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator() {

        return (value.GetEnumerator());
    }
}

}

}


