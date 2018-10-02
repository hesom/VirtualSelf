using System;
using System.Linq;


namespace VirtualSelf.Utility {
	

/// <summary>
/// A collection of static utility methods for mathematical operations and math-related tasks.<br/>
/// The methods contained here are intended to be an extension for where
/// <see cref="System"/>.<see cref="Math"/> falls short. They should only be used in these cases,
/// because there may be some overlap with the <see cref="System"/> methods, and in these cases the
/// <c>System</c> methods will likely have better performance.
/// </summary>
public static class MathExtensions {

	/* ---------- Public Methods ---------- */

	#region FloatingPoint
	
	#region Equality
	
	/// <summary>
	/// Returns whether the difference between <paramref name="val1"/> and <paramref name="val2"/>
	/// is smaller than the acceptable error <paramref name="epsilon"/>, determined by the larger of
	/// the two.<br/>
	/// If this is the case, the values can be considered "close enough", or "approximately equal,
	/// under the given acceptable error.<br/>
	/// <br/>
	/// For more details, and for where this is taken from, see:<br/>
	/// https://stackoverflow.com/q/3728783/5183713 
	/// <remarks>
	/// This is a weaker equality than <see cref="EssentiallyEqual(float,float,float)"/>. If the
	/// latter is <c>true</c>, this will also always be <c>true</c>, the inverse is not true.
	/// </remarks>
	/// </summary>
	/// <returns>
	/// Whether the two given values are approximately equal under the given acceptable error.
	/// </returns>
	public static bool ApproximatelyEqual(float val1, float val2, float epsilon) {
		
		return (Math.Abs(val1 - val2) <= 
		        ((Math.Abs(val1) < Math.Abs(val2) ? Math.Abs(val2) : Math.Abs(val1)) * epsilon));
	}

	/// <summary>
	/// See the documentation of <see cref="ApproximatelyEqual(float,float,float)"/>.
	/// </summary>
	/// <returns>
	/// Whether the two given values are approximately equal under the given acceptable error.
	/// </returns>
	public static bool ApproximatelyEqual(double val1, double val2, double epsilon) {
		
		return (Math.Abs(val1 - val2) <= 
		        ((Math.Abs(val1) < Math.Abs(val2) ? Math.Abs(val2) : Math.Abs(val1)) * epsilon));
	}

	/// <summary>
	/// Returns whether the difference between <paramref name="val1"/> and <paramref name="val2"/>
	/// is smaller than the acceptable error <paramref name="epsilon"/>, determined by the smaller
	/// of the two.<br/>
	/// If this is the case, the values differ less than the acceptable difference in any
	/// calculation they are used in, and can thus be considered "essentially equal", under the
	/// the given acceptable error.<br/>
	/// <br/>
	/// For more details, and for where this is taken from, see:<br/>
	/// https://stackoverflow.com/q/3728783/5183713
	/// <remarks>
	/// This is a stronger equality than <see cref="ApproximatelyEqual(float,float,float)"/>. If
	/// this is <c>true</c>, the former will also always be <c>true</c>, the inverse is not true.
	/// </remarks>
	/// </summary>
	/// <returns>
	/// Whether the two given values are essentially equal under the given acceptable error.
	/// </returns>
	public static bool EssentiallyEqual(float val1, float val2, float epsilon) {
		
		return (Math.Abs(val1 - val2) <= 
		        ((Math.Abs(val1) > Math.Abs(val2) ? Math.Abs(val2) : Math.Abs(val1)) * epsilon));
	}

	/// <summary>
	/// See the documentation of <see cref="EssentiallyEqual(float,float,float)"/>.
	/// </summary>
	/// <returns>
	/// Whether the two given values are essentially equal under the given acceptable error.
	/// </returns>
	public static bool EssentiallyEqual(double val1, double val2, double epsilon) {
		
		return (Math.Abs(val1 - val2) <= 
		        ((Math.Abs(val1) > Math.Abs(val2) ? Math.Abs(val2) : Math.Abs(val1)) * epsilon));
	}
	
	#endregion
	
	#region Ordering

	/// <summary>
	/// Returns whether <paramref name="val1"/> is less than <paramref name="val2"/>, under the
	/// acceptable error <paramref name="epsilon"/>.<br/>
	/// <br/>
	/// For more details, and for where this is taken from, see:<br/>
	/// https://stackoverflow.com/a/253874/5183713
	/// </summary>
	/// <returns>
	/// Whether the first given value is less than the second value under the given acceptable
	/// error.
	/// </returns>
	public static bool DefinitelyLessThan(float val1, float val2, float epsilon) {
		
		return ((val2 - val1) > 
		        ((Math.Abs(val1) < Math.Abs(val2) ? Math.Abs(val2) : Math.Abs(val1)) * epsilon));
	}
	
	/// <summary>
	/// See the documentation of <see cref="DefinitelyLessThan(float,float,float)"/>.
	/// </summary>
	/// <returns>
	/// Whether the first given value is less than the second value under the given acceptable
	/// error.
	/// </returns>
	public static bool DefinitelyLessThan(double val1, double val2, double epsilon) {
		
		return ((val2 - val1) > 
		        ((Math.Abs(val1) < Math.Abs(val2) ? Math.Abs(val2) : Math.Abs(val1)) * epsilon));
	}
	
	/// <summary>
	/// Returns whether <paramref name="val1"/> is greater than <paramref name="val2"/>, under the
	/// acceptable error <paramref name="epsilon"/>.<br/>
	/// <br/>
	/// For more details, and for where this is taken from, see:<br/>
	/// https://stackoverflow.com/a/253874/5183713
	/// </summary>
	/// <returns>
	/// Whether the first given value is greater than the second value under the given acceptable
	/// error.
	/// </returns>
	public static bool DefinitelyGreaterThan(float val1, float val2, float epsilon) {
		
		return ((val1 - val2) > 
		        ((Math.Abs(val1) < Math.Abs(val2) ? Math.Abs(val2) : Math.Abs(val1)) * epsilon));
	}
	
	/// <summary>
	/// See the documentation of <see cref="DefinitelyGreaterThan(float,float,float)"/>.
	/// </summary>
	/// <returns>
	/// Whether the first given value is greater than the second value under the given acceptable
	/// error.
	/// </returns>
	public static bool DefinitelyGreaterThan(double val1, double val2, double epsilon) {
		
		return ((val1 - val2) > 
		        ((Math.Abs(val1) < Math.Abs(val2) ? Math.Abs(val2) : Math.Abs(val1)) * epsilon));
	}
	
	/// <summary>
	/// Returns whether <paramref name="val1"/> is less than, or equal to, <paramref name="val2"/>,
	/// under the acceptable error <paramref name="epsilon"/>.
	/// <remarks>
	/// This returns <c>true</c> if <see cref="EssentiallyEqual(float,float,float)"/> or
	/// <see cref="DefinitelyLessThan(float,float,float)"/> returns <c>true</c> for the given values
	/// and acceptable error, and <c>false</c> in any other case.
	/// </remarks>
	/// </summary>
	/// <returns>
	/// Whether the first given value is less than, or equal to, the second value, under the given
	/// acceptable error.
	/// </returns>
	public static bool DefinitelyLessThanOrEqual(float val1, float val2, float epsilon) {

		return (EssentiallyEqual(val1, val2, epsilon) || DefinitelyLessThan(val1, val2, epsilon));
	}
	
	/// <summary>
	/// See the documentation of <see cref="DefinitelyLessThanOrEqual(float,float,float)"/>.
	/// </summary>
	/// <returns>
	/// Whether the first given value is less than, or equal to, the second value, under the given
	/// acceptable error.
	/// </returns>
	public static bool DefinitelyLessThanOrEqual(double val1, double val2, double epsilon) {

		return (EssentiallyEqual(val1, val2, epsilon) || DefinitelyLessThan(val1, val2, epsilon));
	}
	
	/// <summary>
	/// Returns whether <paramref name="val1"/> is greater than, or equal to,
	/// <paramref name="val2"/>, under the acceptable error <paramref name="epsilon"/>.
	/// <remarks>
	/// This returns <c>true</c> if <see cref="EssentiallyEqual(float,float,float)"/> or
	/// <see cref="DefinitelyGreaterThan(float,float,float)"/> returns <c>true</c> for the given
	/// values and acceptable error, and <c>false</c> in any other case.
	/// </remarks>
	/// </summary>
	/// <returns>
	/// Whether the first given value is greater than, or equal to, the second value, under the
	/// given acceptable error.
	/// </returns>
	public static bool DefinitelyGreaterThanOrEqual(float val1, float val2, float epsilon) {

		return (EssentiallyEqual(val1, val2, epsilon) || DefinitelyGreaterThan(val1, val2, epsilon));
	}
	
	/// <summary>
	/// See the documentation of <see cref="DefinitelyGreaterThanOrEqual(float,float,float)"/>.
	/// </summary>
	/// <returns>
	/// Whether the first given value is greater than, or equal to, the second value, under the
	/// given acceptable error.
	/// </returns>
	public static bool DefinitelyGreaterThanOrEqual(double val1, double val2, double epsilon) {

		return (EssentiallyEqual(val1, val2, epsilon) || DefinitelyGreaterThan(val1, val2, epsilon));
	}
	
	#endregion
	
	#endregion
	
	#region Min
	
	#region MinThreeParams
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static sbyte Min(sbyte val1, sbyte val2, sbyte val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static byte Min(byte val1, byte val2, byte val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static short Min(short val1, short val2, short val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static ushort Min(ushort val1, ushort val2, ushort val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static int Min(int val1, int val2, int val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static uint Min(uint val1, uint val2, uint val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static long Min(long val1, long val2, long val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static ulong Min(ulong val1, ulong val2, ulong val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static float Min(float val1, float val2, float val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static double Min(double val1, double val2, double val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the minimum value among the three given values.
	/// </summary>
	/// <returns>The minimum value among the three given values.</returns>
	public static decimal Min(decimal val1, decimal val2, decimal val3) {
		return (Math.Min(Math.Min(val1, val2), val3));
	}
	
	#endregion
	
	#region MinFourParams
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static sbyte Min(sbyte val1, sbyte val2, sbyte val3, sbyte val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static byte Min(byte val1, byte val2, byte val3, byte val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static short Min(short val1, short val2, short val3, short val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static ushort Min(ushort val1, ushort val2, ushort val3, ushort val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static int Min(int val1, int val2, int val3, int val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static uint Min(uint val1, uint val2, uint val3, uint val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static long Min(long val1, long val2, long val3, long val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static ulong Min(ulong val1, ulong val2, ulong val3, ulong val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static float Min(float val1, float val2, float val3, float val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static double Min(double val1, double val2, double val3, double val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the minimum value among the four given values.
	/// </summary>
	/// <returns>The minimum value among the four given values.</returns>
	public static decimal Min(decimal val1, decimal val2, decimal val3, decimal val4) {
		return (Math.Min(Min(val1, val2, val3), val4));
	}
	
	#endregion
	
	#region MinVarParams
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static sbyte Min(params sbyte[] values) { return (values.Min()); }
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static byte Min(params byte[] values) { return (values.Min()); }
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static short Min(params short[] values) { return (values.Min()); }
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static ushort Min(params ushort[] values) { return (values.Min()); }
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static int Min(params int[] values) { return (values.Min()); }
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static uint Min(params uint[] values) { return (values.Min()); }
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static long Min(params long[] values) { return (values.Min()); }
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static ulong Min(params ulong[] values) { return (values.Min()); }
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static float Min(params float[] values) { return (values.Min()); }
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static double Min(params double[] values) { return (values.Min()); }
	
	/// <summary>
	/// Returns the minimum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the minimum of.</param>
	/// <returns>The minimum value among <see cref="values"/>.</returns>
	public static decimal Min(params decimal[] values) { return (values.Min()); }
	
	#endregion
	
	#endregion
	
	#region Max
	
	#region MaxThreeParams
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static sbyte Max(sbyte val1, sbyte val2, sbyte val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static byte Max(byte val1, byte val2, byte val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static short Max(short val1, short val2, short val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static ushort Max(ushort val1, ushort val2, ushort val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static int Max(int val1, int val2, int val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static uint Max(uint val1, uint val2, uint val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static long Max(long val1, long val2, long val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static ulong Max(ulong val1, ulong val2, ulong val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static float Max(float val1, float val2, float val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static double Max(double val1, double val2, double val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	/// <summary>
	/// Returns the maximum value among the three given values.
	/// </summary>
	/// <returns>The maximum value among the three given values.</returns>
	public static decimal Max(decimal val1, decimal val2, decimal val3) {
		return (Math.Max(Math.Max(val1, val2), val3));
	}
	
	#endregion
	
	#region MaxFourParams
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static sbyte Max(sbyte val1, sbyte val2, sbyte val3, sbyte val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static byte Max(byte val1, byte val2, byte val3, byte val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static short Max(short val1, short val2, short val3, short val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static ushort Max(ushort val1, ushort val2, ushort val3, ushort val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static int Max(int val1, int val2, int val3, int val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static uint Max(uint val1, uint val2, uint val3, uint val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static long Max(long val1, long val2, long val3, long val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static ulong Max(ulong val1, ulong val2, ulong val3, ulong val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static float Max(float val1, float val2, float val3, float val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static double Max(double val1, double val2, double val3, double val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	/// <summary>
	/// Returns the maximum value among the four given values.
	/// </summary>
	/// <returns>The maximum value among the four given values.</returns>
	public static decimal Max(decimal val1, decimal val2, decimal val3, decimal val4) {
		return (Math.Max(Max(val1, val2, val3), val4));
	}
	
	#endregion
	
	#region MaxVarParams
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static sbyte Max(params sbyte[] values) { return (values.Max()); }
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static byte Max(params byte[] values) { return (values.Max()); }
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static short Max(params short[] values) { return (values.Max()); }
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static ushort Max(params ushort[] values) { return (values.Max()); }
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static int Max(params int[] values) { return (values.Max()); }
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static uint Max(params uint[] values) { return (values.Max()); }
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static long Max(params long[] values) { return (values.Max()); }
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static ulong Max(params ulong[] values) { return (values.Max()); }
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static float Max(params float[] values) { return (values.Max()); }
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static double Max(params double[] values) { return (values.Max()); }
	
	/// <summary>
	/// Returns the maximum value among <see cref="values"/>.
	/// </summary>
	/// <param name="values">The list of values to return the maximum of.</param>
	/// <returns>The maximum value among <see cref="values"/>.</returns>
	public static decimal Max(params decimal[] values) { return (values.Max()); }
	
	#endregion
	
	#endregion
}

}
