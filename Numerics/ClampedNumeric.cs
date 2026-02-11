using System.Numerics;

namespace UI.Numerics;

/// <summary>
/// A wrapper for numeric types that provides saturating (clamping) arithmetic.
/// Any operation that would normally overflow or underflow will instead clamp
/// to the maximum or minimum value of the underlying type.
///
/// This is the C# equivalent of the C++ ClampedNumeric class, designed to be
/// idiomatic and performant in .NET. It uses generic math interfaces (`IBinaryInteger<T>`
/// and `IMinMaxValue<T>`) to provide a safe, generic, and efficient implementation of
/// saturating arithmetic for integer types.
/// </summary>
/// <typeparam name="T">An integer type that implements IBinaryInteger<T> and IMinMaxValue<T>.</typeparam>
public readonly struct ClampedNumeric<T> : IEquatable<ClampedNumeric<T>>
    where T :
     struct,
     IBinaryInteger<T>,
     IMinMaxValue<T> // Restrict to integer types for now
{
    private readonly T m_Value;

    /// <summary>
    /// Gets the raw underlying value.
    /// </summary>
    public T RawValue => m_Value;

    /// <summary>
    /// Constructs a ClampedNumeric. The value is saturated on construction.
    /// </summary>
    public ClampedNumeric(T value)
    {
        // Although operators saturate, values can also be constructed from raw T,
        // so we must saturate here for consistency.
        m_Value = Conversion.SaturatedCast<T, T>(value);
    }

    public override string ToString() => m_Value.ToString() ?? string.Empty;
    public override bool Equals(object? obj) => obj is ClampedNumeric<T> other && Equals(other);
    public bool Equals(ClampedNumeric<T> other) => m_Value.Equals(other.m_Value);
    public override int GetHashCode() => m_Value.GetHashCode();

    // --- Comparison Operators ---
    public static bool operator ==(ClampedNumeric<T> left, ClampedNumeric<T> right) => left.Equals(right);
    public static bool operator !=(ClampedNumeric<T> left, ClampedNumeric<T> right) => !left.Equals(right);
    public static bool operator <(ClampedNumeric<T> left, ClampedNumeric<T> right) => left.m_Value < right.m_Value;
    public static bool operator <=(ClampedNumeric<T> left, ClampedNumeric<T> right) => left.m_Value <= right.m_Value;
    public static bool operator >(ClampedNumeric<T> left, ClampedNumeric<T> right) => left.m_Value > right.m_Value;
    public static bool operator >=(ClampedNumeric<T> left, ClampedNumeric<T> right) => left.m_Value >= right.m_Value;

    // --- Implicit and Explicit Conversions ---
    public static implicit operator ClampedNumeric<T>(T value) => new(value);
    public static explicit operator T(ClampedNumeric<T> value) => value.RawValue;

    // --- Unary Operators ---
    public static ClampedNumeric<T> operator +(ClampedNumeric<T> value) => value; // Unary plus is a no-op

    public static ClampedNumeric<T> operator -(ClampedNumeric<T> value)
    {
        var val = value.m_Value;
        if (val == T.MinValue) return new ClampedNumeric<T>(T.MaxValue);
        return new ClampedNumeric<T>(-val);
    }

    public static ClampedNumeric<T> operator ++(ClampedNumeric<T> value) => value + One;
    public static ClampedNumeric<T> operator --(ClampedNumeric<T> value) => value - One;

    // --- Binary Operators ---
    public static ClampedNumeric<T> operator +(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left.m_Value;
        var b = right.m_Value;
        if (T.IsPositive(b)) { if (a > T.MaxValue - b) return MaxValue; }
        else { if (a < T.MinValue - b) return MinValue; }
        return new ClampedNumeric<T>(a + b);
    }

    public static ClampedNumeric<T> operator -(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left.m_Value;
        var b = right.m_Value;
        if (T.IsNegative(b)) { if (a > T.MaxValue + b) return MaxValue; }
        else { if (a < T.MinValue + b) return MinValue; }
        return new ClampedNumeric<T>(a - b);
    }

    public static ClampedNumeric<T> operator *(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left.m_Value;
        var b = right.m_Value;
        if (T.IsZero(a) || T.IsZero(b)) return Zero;
        if (a == One) return right;
        if (b == One) return left;
        if (a == T.MinValue && b == -One) return MaxValue;
        if (b == T.MinValue && a == -One) return MaxValue;

        T result = a * b;
        if (a != result / b) // Overflow check
        {
            return (T.IsNegative(a) ^ T.IsNegative(b)) ? MinValue : MaxValue;
        }
        return new ClampedNumeric<T>(result);
    }

    public static ClampedNumeric<T> operator /(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left.m_Value;
        var b = right.m_Value;
        if (T.IsZero(b)) 
        {
            if (T.IsZero(a)) return Zero;
            return T.IsNegative(a) ? MinValue : MaxValue;
        }
        if (a == T.MinValue && b == -One) return MaxValue;
        return new ClampedNumeric<T>(a / b);
    }

    public static ClampedNumeric<T> operator %(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left.m_Value;
        var b = right.m_Value;
        if (T.IsZero(b) || (a == T.MinValue && b == -One)) return left; // Match C++ failure case
        return new ClampedNumeric<T>(a % b);
    }

    public static ClampedNumeric<T> operator &(ClampedNumeric<T> left, ClampedNumeric<T> right) => new(left.m_Value & right.m_Value);
    public static ClampedNumeric<T> operator |(ClampedNumeric<T> left, ClampedNumeric<T> right) => new(left.m_Value | right.m_Value);
    public static ClampedNumeric<T> operator ^(ClampedNumeric<T> left, ClampedNumeric<T> right) => new(left.m_Value ^ right.m_Value);

    public static ClampedNumeric<T> operator <<(ClampedNumeric<T> left, int shift)
    {
        var val = left.m_Value;
        if (shift <= 0) return left;
        int bitWidth = val.GetByteCount() * 8;
        if (shift >= bitWidth) return T.IsNegative(val) ? MinValue : MaxValue;
        
        var shifted = val << shift;
        if ((shifted >> shift) != val) // Overflow check
        {
            return T.IsNegative(val) ? MinValue : MaxValue;
        }
        return new ClampedNumeric<T>(shifted);
    }

    public static ClampedNumeric<T> operator >>(ClampedNumeric<T> left, int shift)
    {
        if (shift <= 0) return left;
        return new ClampedNumeric<T>(left.m_Value >> shift);
    }

    // -- Constants --
    public static ClampedNumeric<T> MaxValue => new(T.MaxValue);
    public static ClampedNumeric<T> MinValue => new(T.MinValue);
    public static ClampedNumeric<T> Zero => new(T.Zero);
    public static ClampedNumeric<T> One => new(T.One);
    private static readonly ClampedNumeric<T> NegativeOne = new(-T.One);
}
