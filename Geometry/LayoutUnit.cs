using System.Diagnostics;
using System.Numerics;
using UI.Numerics;

namespace UI.Geometry;

// LayoutUnit implements the IFixedPoint interface,
// storing multiples of 1/64 of a pixel in an int (System.Int32).
//
// LayoutUnit is a subpixel unit used by the rendering tree,
// to represent locations and sizes instead of integers,
// which results in bette zooming support (both on desktop and mobile).
//
// LayoutUnit is an abstraction used to represent the location or size of a render object in fractions of a logical pixel,
// it is used primarily for layout and hit testing.
// The current implementation represents values as multiples of 1/64th pixel.
// This allows us to use integer math and avoids floating point imprecision.
// Even though layout calculations are done using LayoutUnits
// the values are aligned to integer pixel values at paint time to line up with device pixels.
// While most modern graphics libraries support painting with subpixel precision,
// this results in unwanted anti-aliasing.
// When aligning to device pixels the edges are aligned to the nearest pixel and then the size is adjusted accordingly.
// This ensures that the bottom/right edge and the total width/height is at most off-by-one.
public struct LayoutUnit : IFixedPoint<int, uint>, IEquatable<LayoutUnit>
{
	public const int FractionalBits = 6;
    public const int IntegralBits = sizeof(int) * 8 - FractionalBits;
    public const int FixedPointDenominator = 1 << FractionalBits;
	public const int RawValueMax = int.MaxValue;
    public const int RawValueMin = int.MinValue;
    public const int IntegerMax = RawValueMax / FixedPointDenominator;
    public const int IntegerMin = RawValueMin / FixedPointDenominator;

    // kIndefiniteSize is a special value used within layout code.
    // It is typical within layout to have sizes which are only allowed to be non-negative or "indefinite".
    // We use the value of "-1" to represent these indefinite values.
    public static readonly LayoutUnit IndefiniteSize = new(-1);

    public static readonly LayoutUnit Max = FromRawValue(RawValueMax);
    public static readonly LayoutUnit Min = FromRawValue(RawValueMin);
    public static readonly LayoutUnit NearlyMax = FromRawValue(RawValueMax - FixedPointDenominator / 2);
    public static readonly LayoutUnit NearlyMin = FromRawValue(RawValueMin + FixedPointDenominator / 2);
	
	private int m_Value;

	public LayoutUnit() { m_Value = 0; }

    // Constructors from signed integral types <= sizeof(int)
    public LayoutUnit(int value) => SaturatedSet(value);
    public LayoutUnit(short value) => SaturatedSet(value);
    public LayoutUnit(sbyte value) => SaturatedSet(value);

    // Constructors from unsigned integral types <= sizeof(int)
    public LayoutUnit(uint value) => SaturatedSet(value);
    public LayoutUnit(ushort value) => SaturatedSet(value);
    public LayoutUnit(byte value) => SaturatedSet(value);

    // Constructors from integral types larger than int
    public LayoutUnit(long value) { m_Value = ClampRawValue(value * FixedPointDenominator); }
    public LayoutUnit(ulong value) { m_Value = ClampRawValue(value * FixedPointDenominator); }

    // Constructors from floating-point types
    public LayoutUnit(float value) { m_Value = ClampRawValue(value * FixedPointDenominator); }
    public LayoutUnit(double value) { m_Value = ClampRawValue(value * FixedPointDenominator); }

    public readonly int RawValue() => m_Value;

	public void SetRawValue(int value) => m_Value = value;

	public void SetRawValue(long value)
	{
#if DEBUG
        Debug.WriteLineIf(value > int.MaxValue || value < int.MinValue, $"LayoutUnit overflow: {value} is out of range for a 32-bit integer.");
#endif
		m_Value = (int)value;
	}

    public void SaturatedSet(int value)
    {
        if (value > IntegerMax)
        {
            m_Value = RawValueMax;
        }
        else if (value < IntegerMin)
        {
            m_Value = RawValueMin;
        }
        else
        {
            m_Value = value << FractionalBits;
        }
    }

    public void SaturatedSet(uint value)
    {
        // Unsigned values can't be negative, so we only need to check the upper bound.
        if (value >= (uint)IntegerMax)
        {
            m_Value = RawValueMax;
        }
        else
        {
            m_Value = (int)(value << FractionalBits);
        }
    }

	public readonly int ToInteger()
	{
		return m_Value / FixedPointDenominator;
	}

	public readonly uint ToUnsignedInteger()
	{
        // unchecked is required here to handle the conversion of negative values correctly
		unchecked
		{
			return (uint)(m_Value / FixedPointDenominator);
		}
	}

	public readonly float ToFloat()
	{
		return (float)m_Value / FixedPointDenominator;
	}

	public readonly double ToDouble()
	{
		return (double)m_Value / FixedPointDenominator;
	}

    

	// Note: Original C++ has this function here as well, but since it's static and only calls Conversion.SaturatedCast(),
	// I'm not sure if we should keep it in LayoutUnit, there might be a better place for it.
	public static int ClampRawValue<T>(T value) where T : IBinaryInteger<T>
	{
        return Conversion.SaturatedCast<int, T>(value);
	}

    public static int ClampRawValue(float value)
    {
        return Conversion.SaturatedCast<int, float>(value);
    }

    public static int ClampRawValue(double value)
    {
        return Conversion.SaturatedCast<int, double>(value);
    }

	public static LayoutUnit FromRawValue(int value)
	{
		LayoutUnit unit = new()
		{
			m_Value = value
		};
		
		return unit;
	}
	
	public static LayoutUnit FromRawValueWithClamp<T>(T raw_value) where T : IBinaryInteger<T>
	{
		// Note: Might be better to just call Conversion.SaturatedCast() directly and avoid the indirect call to ClampRawValue()
		// return FromRawValue(Conversion.SaturatedCast<int, T>(raw_value));
		return FromRawValue(ClampRawValue<T>(raw_value));
	}

    // The specified `value` is rounded up to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static LayoutUnit FromFloatCeil(float value)
    {
        return FromRawValue(ClampRawValue(MathF.Ceiling(value * FixedPointDenominator)));
    }

    // The specified `value` is truncated to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static LayoutUnit FromFloatFloor(float value)
    {
        return FromRawValue(ClampRawValue(MathF.Floor(value * FixedPointDenominator)));
    }

    public static LayoutUnit Clamp(double value)
    {
        return FromFloatFloor((float)value);
    }

    // The specified `value` is rounded to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static LayoutUnit FromFloatRound(float value)
    {
        return FromRawValue(ClampRawValue(MathF.Round(value * FixedPointDenominator)));
    }

    // The specified `value` is rounded to a multiple of `Epsilon()`, and is
    // clamped by `Min()` and `Max()`. A NaN `value` produces `FixedPoint(0)`.
    public static LayoutUnit FromDoubleRound(double value)
    {
        return FromRawValue(ClampRawValue(Math.Round(value * FixedPointDenominator)));
    }

    public readonly bool HasFraction => (m_Value % FixedPointDenominator) != 0;

    public readonly bool IsInteger => (m_Value % FixedPointDenominator) == 0;

    public readonly LayoutUnit Fraction()
    {
        // Compute fraction using the mod operator to preserve the sign of the value as it may affect rounding.
        return FromRawValue(m_Value % FixedPointDenominator);
    }

    public readonly bool MightBeSaturated()
    {
        return m_Value == RawValueMax || m_Value == RawValueMin;
    }

    public static float Epsilon() => 1.0f / FixedPointDenominator;

    public readonly LayoutUnit AddEpsilon()
    {
        return FromRawValue(m_Value < RawValueMax ? m_Value + 1 : m_Value);
    }

    public readonly int Ceil()
    {
        if (m_Value >= RawValueMax - FixedPointDenominator + 1)
            return IntegerMax;

        if (m_Value >= 0)
            return (m_Value + FixedPointDenominator - 1) / FixedPointDenominator;

        return ToInteger();
    }

    public readonly int Round()
    {
        return ToInteger() + ((Fraction().RawValue() + (FixedPointDenominator / 2)) >> FractionalBits);
    }

    public readonly int Floor() 
    {
        if (m_Value <= RawValueMin + FixedPointDenominator - 1)
            return IntegerMin;
        
        return m_Value >> FractionalBits;
    }

    public readonly LayoutUnit Abs()
    {
        // Make the operation explicitly unchecked (Even with "checked" compiler flag on)
        unchecked
        {
            // Not using Math.Abs() here because it throws a overflow exception if value < 0
            return FromRawValue(m_Value < 0 ? -m_Value : m_Value);
        }
    }

    public readonly LayoutUnit ClampNegativeToZero()
    {
        return m_Value < 0 ? new LayoutUnit() : this;
    }

    public readonly LayoutUnit ClampPositiveToZero()
    {
        return m_Value > 0 ? new LayoutUnit() : this;
    }

    public readonly LayoutUnit ClampIndefiniteToZero()
    {
        if (m_Value == -FixedPointDenominator)
            return new LayoutUnit();

#if DEBUG
        Debug.Assert(m_Value >= 0, "ClampIndefiniteToZero called on a negative value that is not the 'indefinite' sentinel.");
#endif

        return this;
    }

    public override readonly string ToString()
    {
        string formatted = ToDouble().ToString("G14");

        if (m_Value == Max.RawValue()) return $"Max({formatted})";
        if (m_Value == Min.RawValue()) return $"Min({formatted})";
        if (m_Value == NearlyMax.RawValue()) return $"NearlyMax({formatted})";
        if (m_Value == NearlyMin.RawValue()) return $"NearlyMin({formatted})";

        return formatted;
    }

    public override readonly bool Equals(object? obj) => obj is LayoutUnit other && Equals(other);

    public readonly bool Equals(LayoutUnit other) => m_Value == other.m_Value;

    public override readonly int GetHashCode() => m_Value.GetHashCode();

    public static bool operator ==(LayoutUnit left, LayoutUnit right) => left.Equals(right);
    public static bool operator !=(LayoutUnit left, LayoutUnit right) => !left.Equals(right);
    public static bool operator <(LayoutUnit left, LayoutUnit right) => left.m_Value < right.m_Value;
    public static bool operator <=(LayoutUnit left, LayoutUnit right) => left.m_Value <= right.m_Value;
    public static bool operator >(LayoutUnit left, LayoutUnit right) => left.m_Value > right.m_Value;
    public static bool operator >=(LayoutUnit left, LayoutUnit right) => left.m_Value >= right.m_Value;

    public static bool operator <=(LayoutUnit a, int b) => a <= new LayoutUnit(b);
    public static bool operator <=(int a, LayoutUnit b) => new LayoutUnit(a) <= b;
    public static bool operator >=(LayoutUnit a, int b) => a >= new LayoutUnit(b);
    public static bool operator >=(int a, LayoutUnit b) => new LayoutUnit(a) >= b;
    public static bool operator <(LayoutUnit a, int b) => a < new LayoutUnit(b);
    public static bool operator <(int a, LayoutUnit b) => new LayoutUnit(a) < b;
    public static bool operator >(LayoutUnit a, int b) => a > new LayoutUnit(b);
    public static bool operator >(int a, LayoutUnit b) => new LayoutUnit(a) > b;
    public static bool operator ==(LayoutUnit a, int b) => a == new LayoutUnit(b);
    public static bool operator ==(int a, LayoutUnit b) => new LayoutUnit(a) == b;
    public static bool operator !=(LayoutUnit a, int b) => a != new LayoutUnit(b);
    public static bool operator !=(int a, LayoutUnit b) => new LayoutUnit(a) != b;

    public static double operator *(LayoutUnit a, double b) => a.ToDouble() * b;
    public static float operator *(LayoutUnit a, float b) => a.ToFloat() * b;
    public static float operator *(float a, LayoutUnit b) => a * b.ToFloat();
    public static double operator *(double a, LayoutUnit b) => a * b.ToDouble();
    public static float operator /(LayoutUnit a, float b) => a.ToFloat() / b;
    public static double operator /(LayoutUnit a, double b) => a.ToDouble() / b;
    public static float operator /(float a, LayoutUnit b) => a / b.ToFloat();
    public static double operator /(double a, LayoutUnit b) => a / b.ToDouble();
    public static double operator +(LayoutUnit a, double b) => a.ToDouble() + b;
    public static float operator +(float a, LayoutUnit b) => a + b.ToFloat();
    public static double operator +(double a, LayoutUnit b) => a + b.ToDouble();
    public static float operator -(LayoutUnit a, float b) => a.ToFloat() - b;
    public static double operator -(LayoutUnit a, double b) => a.ToDouble() - b;
    public static float operator -(float a, LayoutUnit b) => a - b.ToFloat();

    public static LayoutUnit operator +(LayoutUnit a, LayoutUnit b)
    {
        ClampedNumeric<int> result = new ClampedNumeric<int>(a.RawValue()) + new ClampedNumeric<int>(b.RawValue());
        return FromRawValue(result.RawValue);
    }

    public static LayoutUnit operator -(LayoutUnit a, LayoutUnit b)
    {
        ClampedNumeric<int> result = new ClampedNumeric<int>(a.RawValue()) - new ClampedNumeric<int>(b.RawValue());
        return FromRawValue(result.RawValue);
    }

    public static LayoutUnit operator *(LayoutUnit a, int b)
    {
        ClampedNumeric<int> result = new ClampedNumeric<int>(a.RawValue()) * new ClampedNumeric<int>(b);
        return FromRawValue(result.RawValue);
    }

    public static LayoutUnit operator *(int a, LayoutUnit b) => b * a;

    public static LayoutUnit operator /(LayoutUnit a, int b)
    {
        ClampedNumeric<int> result = new ClampedNumeric<int>(a.RawValue()) / new ClampedNumeric<int>(b);
        return FromRawValue(result.RawValue);
    }

    public static LayoutUnit operator /(int a, LayoutUnit b) => new LayoutUnit(a) / b;

    public static LayoutUnit operator +(LayoutUnit a, int b) => a + new LayoutUnit(b);
    public static LayoutUnit operator +(int a, LayoutUnit b) => new LayoutUnit(a) + b;
    public static LayoutUnit operator -(LayoutUnit a, int b) => a - new LayoutUnit(b);
    public static LayoutUnit operator -(int a, LayoutUnit b) => new LayoutUnit(a) - b;

    public static LayoutUnit operator *(LayoutUnit a, LayoutUnit b)
    {
        long result = ((long)a.RawValue() * b.RawValue()) >> FractionalBits;
        return FromRawValue(ClampRawValue(result));
    }

    public static LayoutUnit operator /(LayoutUnit a, LayoutUnit b)
    {
        if (b.RawValue() == 0)
            return FromRawValue(a.RawValue() >= 0 ? RawValueMax : RawValueMin);
        
        long result = ((long)a.RawValue() << FractionalBits) / b.RawValue();
        return FromRawValue(ClampRawValue(result));
    }

    /// <summary>
    /// Returns the remainder after a division with integer results.
    /// This calculates the modulo so that:
    /// a = (int)(a / b) * b + IntMod(a, b).
    /// </summary>
    public static LayoutUnit IntMod(LayoutUnit a, LayoutUnit b)
    {
        return FromRawValue(a.RawValue() % b.RawValue());
    }
}