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
public struct LayoutUnit : IFixedPoint<int, uint>
{
	public const int FractionalBits = 6;
    public const int IntegralBits = sizeof(int) * 8 - FractionalBits;
    public const int FixedPointDenominator = 1 << FractionalBits;
	public const int RawValueMax = int.MaxValue;
    public const int RawValueMin = int.MinValue;
    public const int IntegerMax = RawValueMax / FixedPointDenominator;
    public const int IntegerMin = RawValueMin / FixedPointDenominator;
	
	private int m_Value;

	public LayoutUnit()
	{
		m_Value = 0;
	}

    public readonly int RawValue()
    {
        return m_Value;
    }

	public void SetRawValue(int value)
	{
		m_Value = value;
	}

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
		unchecked
		{
			return m_Value / FixedPointDenominator;
		}
	}

	public readonly uint ToUnsignedInteger()
	{
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
		return FromRawValue(LayoutUnit.ClampRawValue<T>(raw_value));
	}
}
