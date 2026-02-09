using System;
using System.Numerics;

namespace UI.Numerics;

/// <summary>
/// A wrapper for numeric types that provides saturating (clamping) arithmetic.
/// Any operation that would normally overflow or underflow will instead clamp
/// to the maximum or minimum value of the underlying type.
///
/// This is the C# equivalent of the C++ ClampedNumeric class, designed to be
/// idiomatic and performant in .NET. It uses generic math interfaces (`INumber<T>`)
/// to provide a safe, generic, and efficient implementation of saturating arithmetic.
/// </summary>
/// <typeparam name="T">An arithmetic type, such as int, float, or double.</typeparam>
public readonly struct ClampedNumeric<T> : IEquatable<ClampedNumeric<T>>
    where T : struct, IBinaryInteger<T> // Restrict to integer types for now
{
    private readonly T _value;

    /// <summary>
    /// Gets the raw underlying value.
    /// </summary>
    public T RawValue => _value;

    /// <summary>
    /// Constructs a ClampedNumeric. The value is saturated on construction.
    /// </summary>
    public ClampedNumeric(T value)
    {
        // Although operators saturate, values can also be constructed from raw T,
        // so we must saturate here for consistency.
        _value = Conversion.SaturatedCast<T, T>(value);
    }

    public override string ToString() => _value.ToString() ?? string.Empty;
    public override bool Equals(object? obj) => obj is ClampedNumeric<T> other && Equals(other);
    public bool Equals(ClampedNumeric<T> other) => _value.Equals(other._value);
    public override int GetHashCode() => _value.GetHashCode();

    // --- Comparison Operators ---
    public static bool operator ==(ClampedNumeric<T> left, ClampedNumeric<T> right) => left.Equals(right);
    public static bool operator !=(ClampedNumeric<T> left, ClampedNumeric<T> right) => !left.Equals(right);
    public static bool operator <(ClampedNumeric<T> left, ClampedNumeric<T> right) => left._value < right._value;
    public static bool operator <=(ClampedNumeric<T> left, ClampedNumeric<T> right) => left._value <= right._value;
    public static bool operator >(ClampedNumeric<T> left, ClampedNumeric<T> right) => left._value > right._value;
    public static bool operator >=(ClampedNumeric<T> left, ClampedNumeric<T> right) => left._value >= right._value;

    // --- Implicit and Explicit Conversions ---
    public static implicit operator ClampedNumeric<T>(T value) => new(value);
    public static explicit operator T(ClampedNumeric<T> value) => value.RawValue;

    // --- Unary Operators ---
    public static ClampedNumeric<T> operator +(ClampedNumeric<T> value) => value; // Unary plus is a no-op

    public static ClampedNumeric<T> operator -(ClampedNumeric<T> value)
    {
        var val = value._value;
        if (val == T.MinValue) return new ClampedNumeric<T>(T.MaxValue);
        return new ClampedNumeric<T>(-val);
    }

    public static ClampedNumeric<T> operator ++(ClampedNumeric<T> value) => value + One;
    public static ClampedNumeric<T> operator --(ClampedNumeric<T> value) => value - One;

    // --- Binary Operators ---
    public static ClampedNumeric<T> operator +(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left._value;
        var b = right._value;
        if (T.IsPositive(b)) { if (a > T.MaxValue - b) return MaxValue; }
        else { if (a < T.MinValue - b) return MinValue; }
        return new ClampedNumeric<T>(a + b);
    }

    public static ClampedNumeric<T> operator -(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left._value;
        var b = right._value;
        if (T.IsNegative(b)) { if (a > T.MaxValue + b) return MaxValue; }
        else { if (a < T.MinValue + b) return MinValue; }
        return new ClampedNumeric<T>(a - b);
    }

    public static ClampedNumeric<T> operator *(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left._value;
        var b = right._value;
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
        var a = left._value;
        var b = right._value;
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
        var a = left._value;
        var b = right._value;
        if (T.IsZero(b) || (a == T.MinValue && b == -One)) return left; // Match C++ failure case
        return new ClampedNumeric<T>(a % b);
    }

    public static ClampedNumeric<T> operator &(ClampedNumeric<T> left, ClampedNumeric<T> right) => new(left._value & right._value);
    public static ClampedNumeric<T> operator |(ClampedNumeric<T> left, ClampedNumeric<T> right) => new(left._value | right._value);
    public static ClampedNumeric<T> operator ^(ClampedNumeric<T> left, ClampedNumeric<T> right) => new(left._value ^ right._value);

    public static ClampedNumeric<T> operator <<(ClampedNumeric<T> left, int shift)
    {
        var val = left._value;
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
        return new ClampedNumeric<T>(left._value >> shift);
    }

    // -- Constants --
    public static ClampedNumeric<T> MaxValue => new(T.MaxValue);
    public static ClampedNumeric<T> MinValue => new(T.MinValue);
    public static ClampedNumeric<T> Zero => new(T.Zero);
    public static ClampedNumeric<T> One => new(T.One);
    private static readonly ClampedNumeric<T> NegativeOne = new(-T.One);
}
