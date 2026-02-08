using System.Numerics;
using UI.Numerics;

namespace UI.Geometry;

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
		// We should report overflow if outside range.
		//if (value > int.MaxValue || value < int.MinValue)
		//	throw new OverflowException("Raw value out of range for LayoutUnit.");

		m_Value = (int)value;
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

    // Original C++ version of ClampRawValue()
    /*
    template <typename T>
    static constexpr Storage ClampRawValue(T raw_value)
    {
	    return base::saturated_cast<Storage>(raw_value);
    }
    */
	public readonly int ClampRawValue<T>(T rawValue) where T : IBinaryInteger<T>
	{
        return Conversion.SaturatedCast<int, T>(rawValue);
	}
}
