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

        if (T.IsZero(b))
            return left;

        // For floating-point types, standard addition works as it produces +/- Infinity on overflow.
        if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal))
        {
            return new ClampedNumeric<T>(a + b);
        }

        // For integer types, perform saturating addition.
        // The logic from C++ `ClampedAddOp` is implemented here directly.
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
        // Subtraction is implemented as addition of the negated value, which reuses the saturation logic.
        return left + -right;
    }
}