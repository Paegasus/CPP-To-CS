using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace UI.Numerics;

/// <summary>
/// A wrapper for a numeric type that ensures all arithmetic operations
/// are clamped within the range of the underlying type T.
/// </summary>
public readonly struct ClampedNumeric<T> : INumber<ClampedNumeric<T>>, IEquatable<ClampedNumeric<T>> where T : 
IBinaryInteger<T>,
IMinMaxValue<T>
{
    private readonly T _value;

    public ClampedNumeric(T value)
    {
        _value = value;
    }

    public static ClampedNumeric<T> One => new(T.One);

    public static int Radix => T.Radix;

    public static ClampedNumeric<T> Zero => new(T.Zero);

    public static ClampedNumeric<T> AdditiveIdentity => new(T.AdditiveIdentity);

    public static ClampedNumeric<T> MultiplicativeIdentity => new(T.MultiplicativeIdentity);

    public static ClampedNumeric<T> operator +(ClampedNumeric<T> value)
    {
        return value; // Unary plus is a no-op.
    }

    public static ClampedNumeric<T> operator +(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var result = T.CreateSaturating(left._value + right._value);

        return new ClampedNumeric<T>(result);
    }

    public static ClampedNumeric<T> operator -(ClampedNumeric<T> value)
    {
        var result = T.CreateSaturating(T.Zero - value._value);

        return new ClampedNumeric<T>(result);
    }

    public static ClampedNumeric<T> operator -(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var result = T.CreateSaturating(left._value - right._value);

        return new ClampedNumeric<T>(result);
    }

    public static ClampedNumeric<T> operator ++(ClampedNumeric<T> value)
    {
        var result = T.CreateSaturating(value._value + T.One);
        return new ClampedNumeric<T>(result);
    }

    public static ClampedNumeric<T> operator --(ClampedNumeric<T> value)
    {
        var result = T.CreateSaturating(value._value - T.One);
        return new ClampedNumeric<T>(result);
    }

    public static ClampedNumeric<T> operator *(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        var result = T.CreateSaturating(left._value * right._value);
        return new ClampedNumeric<T>(result);
    }

    public static ClampedNumeric<T> operator /(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        if (T.IsNegative(T.MinValue) &&
            right._value == (T.Zero - T.One) &&
            left._value == T.MinValue)
        {
            return new ClampedNumeric<T>(T.MaxValue);
        }

        return new ClampedNumeric<T>(left._value / right._value);
    }

    public static ClampedNumeric<T> operator %(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        return new ClampedNumeric<T>(left._value % right._value);
    }

    public static bool operator >(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        return left._value > right._value;
    }

    public static bool operator >=(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        return left._value >= right._value;
    }

    public static bool operator <(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        return left._value < right._value;
    }

    public static bool operator <=(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        return left._value <= right._value;
    }

    public static bool operator ==(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ClampedNumeric<T> left, ClampedNumeric<T> right)
    {
        return !left.Equals(right);
    }

    static ClampedNumeric<T> INumber<ClampedNumeric<T>>.Clamp(ClampedNumeric<T> value, ClampedNumeric<T> min, ClampedNumeric<T> max)
    {
        return new ClampedNumeric<T>(T.Clamp(value._value, min._value, max._value));
    }

    static ClampedNumeric<T> INumberBase<ClampedNumeric<T>>.Abs(ClampedNumeric<T> value)
    {
        return new ClampedNumeric<T>(T.Abs(value._value));
    }

    public int CompareTo(object? obj)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(ClampedNumeric<T> other)
    {
        throw new NotImplementedException();
    }

    public static bool IsCanonical(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsComplexNumber(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsEvenInteger(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsFinite(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsImaginaryNumber(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsInfinity(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsInteger(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNaN(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNegative(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNegativeInfinity(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsNormal(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsOddInteger(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsPositive(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsPositiveInfinity(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsRealNumber(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsSubnormal(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static bool IsZero(ClampedNumeric<T> value)
    {
        throw new NotImplementedException();
    }

    public static ClampedNumeric<T> MaxMagnitude(ClampedNumeric<T> x, ClampedNumeric<T> y)
    {
        throw new NotImplementedException();
    }

    public static ClampedNumeric<T> MaxMagnitudeNumber(ClampedNumeric<T> x, ClampedNumeric<T> y)
    {
        throw new NotImplementedException();
    }

    public static ClampedNumeric<T> MinMagnitude(ClampedNumeric<T> x, ClampedNumeric<T> y)
    {
        throw new NotImplementedException();
    }

    public static ClampedNumeric<T> MinMagnitudeNumber(ClampedNumeric<T> x, ClampedNumeric<T> y)
    {
        throw new NotImplementedException();
    }

    public static ClampedNumeric<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static ClampedNumeric<T> Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out ClampedNumeric<T> result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out ClampedNumeric<T> result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out ClampedNumeric<T> result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToChecked<TOther>(ClampedNumeric<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToSaturating<TOther>(ClampedNumeric<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToTruncating<TOther>(ClampedNumeric<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out ClampedNumeric<T> result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out ClampedNumeric<T> result)
    {
        throw new NotImplementedException();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        throw new NotImplementedException();
    }

    public static ClampedNumeric<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out ClampedNumeric<T> result)
    {
        throw new NotImplementedException();
    }

    public static ClampedNumeric<T> Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ClampedNumeric<T> result)
    {
        throw new NotImplementedException();
    }

    public bool Equals(ClampedNumeric<T> other)
    {
        throw new NotImplementedException();
    }

    // Necessary for == and != operators
    public override bool Equals(object? other)
    {
        return other is ClampedNumeric<T> numeric && Equals(numeric);
    }

    // Necessary for == and != operators
    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
