using System.Diagnostics;
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

	public readonly int ClampRawValue<T>(T rawValue) where T : IBinaryInteger<T>
	{
        return Conversion.SaturatedCast<int, T>(rawValue);
	}
}
