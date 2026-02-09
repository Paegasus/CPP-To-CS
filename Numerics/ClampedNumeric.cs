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
    where T : struct, INumber<T>
{
    private readonly T _value;

    /// <summary>
    /// Gets the raw underlying value.
    /// </summary>
    public T RawValue => _value;

    /// <summary>
    /// Constructs a ClampedNumeric. The constructor is private to enforce
    /// creation via the factory method or implicit conversion, which makes
    /// the source of values clearer.
    /// </summary>
    private ClampedNumeric(T value)
    {
        _value = value;
    }
    
    public override string ToString() => _value.ToString() ?? string.Empty;
    public override bool Equals(object? obj) => obj is ClampedNumeric<T> other && Equals(other);
    public bool Equals(ClampedNumeric<T> other) => _value.Equals(other._value);
    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(ClampedNumeric<T> left, ClampedNumeric<T> right) => left.Equals(right);
    public static bool operator !=(ClampedNumeric<T> left, ClampedNumeric<T> right) => !left.Equals(right);
    public static bool operator <(ClampedNumeric<T> left, ClampedNumeric<T> right) => left._value < right._value;
    public static bool operator <=(ClampedNumeric<T> left, ClampedNumeric<T> right) => left._value <= right._value;
    public static bool operator >(ClampedNumeric<T> left, ClampedNumeric<T> right) => left._value > right._value;
    public static bool operator >=(ClampedNumeric<T> left, ClampedNumeric<T> right) => left._value >= right._value;

    // Implicit conversions for convenience
    public static implicit operator ClampedNumeric<T>(T value) => new(value);
    public static explicit operator T(ClampedNumeric<T> value) => value.RawValue;
    
    // --- Arithmetic Operators ---

    /// <summary>
    /// Performs saturating negation.
    /// </summary>
    public static ClampedNumeric<T> operator -(ClampedNumeric<T> value)
    {
        var val = value._value;
        // For signed integers, the only case where negation saturates is negating the minimum value.
        if (val == T.MinValue)
        {
            return new ClampedNumeric<T>(T.MaxValue);
        }
        return new ClampedNumeric<T>(-val);
    }

    /// <summary>
    /// Performs saturating addition.
    /// </summary>
    public static ClampedNumeric<T> operator +(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left._value;
        var b = right._value;

        // Standard addition for floating-point types already saturates to Infinity.
        if (T.IsFloatingPoint(a))
        {
            return new ClampedNumeric<T>(a + b);
        }

        // Integer saturating addition logic.
        if (T.IsPositive(b))
        {
            if (a > T.MaxValue - b)
                return new ClampedNumeric<T>(T.MaxValue);
        }
        else
        {
            if (a < T.MinValue - b)
                return new ClampedNumeric<T>(T.MinValue);
        }
        return new ClampedNumeric<T>(a + b);
    }

    /// <summary>
    /// Performs saturating subtraction.
    /// </summary>
    public static ClampedNumeric<T> operator -(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left._value;
        var b = right._value;

        if (T.IsFloatingPoint(a))
        {
            return new ClampedNumeric<T>(a - b);
        }

        // Integer saturating subtraction logic.
        if (T.IsNegative(b))
        {
            // Subtracting a negative is like adding a positive.
            if (a > T.MaxValue + b) // Note: b is negative, so this is `T.MaxValue - abs(b)`
                return new ClampedNumeric<T>(T.MaxValue);
        }
        else
        {
            // Subtracting a positive.
            if (a < T.MinValue + b)
                return new ClampedNumeric<T>(T.MinValue);
        }
        return new ClampedNumeric<T>(a - b);
    }

    /// <summary>
    /// Performs saturating multiplication.
    /// </summary>
    public static ClampedNumeric<T> operator *(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left._value;
        var b = right._value;

        if (T.IsFloatingPoint(a))
        {
            return new ClampedNumeric<T>(a * b);
        }

        // Integer saturating multiplication logic.
        if (T.IsZero(a) || T.IsZero(b)) return new ClampedNumeric<T>(T.Zero);
        if (a == T.One) return right;
        if (b == T.One) return left;

        if (a == T.MinValue && b == -T.One)
            return new ClampedNumeric<T>(T.MaxValue);
        if (b == T.MinValue && a == -T.One)
             return new ClampedNumeric<T>(T.MaxValue);

        T result = a * b;
        // Check for overflow by seeing if the division gets us back to the original number.
        if (a != result / b)
        {
            // Overflow occurred. Determine saturation direction.
            return (T.IsNegative(a) ^ T.IsNegative(b)) 
                ? new ClampedNumeric<T>(T.MinValue) 
                : new ClampedNumeric<T>(T.MaxValue);
        }

        return new ClampedNumeric<T>(result);
    }

    /// <summary>
    /// Performs saturating division.
    /// </summary>
    public static ClampedNumeric<T> operator /(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var a = left._value;
        var b = right._value;

        if (T.IsFloatingPoint(a))
        {
            // Floating point division by zero yields Infinity, which is a form of saturation.
            return new ClampedNumeric<T>(a / b);
        }

        if (T.IsZero(b))
        {
            // For integer division by zero, saturate to MaxValue if positive, MinValue if negative, or 0 if a is 0.
            if (T.IsZero(a)) return new ClampedNumeric<T>(T.Zero);
            return T.IsNegative(a) ? new ClampedNumeric<T>(T.MinValue) : new ClampedNumeric<T>(T.MaxValue);
        }

        // The only integer overflow case for division is `MinValue / -1`
        if (a == T.MinValue && b == -T.One)
        {
            return new ClampedNumeric<T>(T.MaxValue);
        }

        return new ClampedNumeric<T>(a / b);
    }
}