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
}
