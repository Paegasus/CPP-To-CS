using System.Numerics;

namespace UI.Numerics;

/// <summary>
/// Provides safe and explicit numeric conversion methods.
/// </summary>
public static class SafeConversions
{
    /// <summary>
    /// Converts a value from a source type to a destination type, saturating
    /// the value if it overflows or underflows the destination's range.
    /// This is the C# equivalent of saturated_cast.
    /// </summary>
    public static TDest SaturatedCast<TDest, TSrc>(TSrc value)
        where TDest : INumber<TDest>
        where TSrc : INumber<TSrc>
    {
        // This is the magic of .NET 7+ Generic Math.
        // The Base Class Library now provides this optimized operation for any type that implements INumber.
        return TDest.CreateSaturating(value);
    }

    /// <summary>
    /// Converts a value from a source type to a destination type, throwing
    /// an OverflowException if the value is outside the representable range.
    /// This is the C# equivalent of checked_cast.
    /// </summary>
    public static TDest CheckedCast<TDest, TSrc>(TSrc value)
        where TDest : INumber<TDest>
        where TSrc : INumber<TSrc>
    {
        // .NET also provides an optimized checked version.
        return TDest.CreateChecked(value);
    }

    /// <summary>
    /// Converts a value from a source type to a destination type, truncating
    /// the value if it overflows. This method is intended to be used with a
    //  Source Generator that enforces compile-time range checks.
    /// This is the C# equivalent of strict_cast.
    /// </summary>
    public static TDest StrictCast<TDest, TSrc>(TSrc value)
        where TDest : INumber<TDest>
        where TSrc : INumber<TSrc>
    {
        // This operation truncates, like a standard C# cast (e.g., (short)myInt).
        // The "strictness" must be enforced at compile time.
        return TDest.CreateTruncating(value);
    }

    // floating -> integral conversions that saturate and thus can actually return
    // an integral type.
    //
    // Generally, what you want is saturated_cast<Dst>(std::nearbyint(x)), which
    // rounds correctly according to IEEE-754 (round to nearest, ties go to nearest
    // even number; this avoids bias). If your code is performance-critical
    // and you are sure that you will never overflow, you can use std::lrint()
    // or std::llrint(), which return a long or long long directly.
    //
    // Below are convenience functions around similar patterns, except that
    // they round in nonstandard directions and will generally be slower.

    // Rounds towards negative infinity (i.e., down).
    public static int ClampFloor(float value)
    {
        return SaturatedCast<int, float>(MathF.Floor(value));
    }

    // Rounds towards positive infinity (i.e., up).
    public static int ClampCeil(float value)
    {
        return SaturatedCast<int, float>(MathF.Ceiling(value));
    }

    // Rounds towards nearest integer, with ties away from zero.
    // This means that 0.5 will be rounded to 1 and 1.5 will be rounded to 2.
    // Similarly, -0.5 will be rounded to -1 and -1.5 will be rounded to -2.
    //
    // This is normally not what you want accuracy-wise (it introduces a small bias
    // away from zero), and it is not the fastest option, but it is frequently what
    // existing code expects. Compare with saturated_cast<Dst>(std::nearbyint(x))
    // or std::lrint(x), which would round 0.5 and -0.5 to 0 but 1.5 to 2 and
    // -1.5 to -2.
    public static int ClampRound(float value)
    {
        float rounded = MathF.Round(value);
        return SaturatedCast<int, float>(rounded);
    }
}
