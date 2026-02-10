using System;

namespace UI.Geometry;

public struct IntSize
{
    private int m_width, m_height;

    public IntSize()
    {
        m_width = 0;
        m_height = 0;
    }

    public IntSize(int width, int height)
    {
        m_width = width;
        m_height = height;
    }

    public int Width { get { return m_width; } set { m_width = value; } }
    public int Height { get { return m_height; } set { m_height = value; } }

    public bool IsEmpty() => m_width <= 0 || m_height <= 0;
    public bool IsZero() => m_width == 0 && m_height == 0;

    public float AspectRatio() => (float)m_width / m_height;

    public void Expand(int width, int height)
    {
        m_width += width;
        m_height += height;
    }

    public void Scale(float widthScale, float heightScale)
    {
        m_width = (int)(m_width * widthScale);
        m_height = (int)(m_height * heightScale);
    }

    public void Scale(float scale)
    {
        Scale(scale, scale);
    }

    public IntSize ExpandedTo(IntSize other)
    {
        return new IntSize(Math.Max(m_width, other.m_width), Math.Max(m_height, other.m_height));
    }

    public IntSize ShrunkTo(IntSize other)
    {
        return new IntSize(Math.Min(m_width, other.m_width), Math.Min(m_height, other.m_height));
    }

    public void ClampNegativeToZero()
    {
        this = ExpandedTo(new IntSize());
    }

    public void ClampToMinimumSize(IntSize minimumSize)
    {
        if (m_width < minimumSize.Width)
            m_width = minimumSize.Width;
        if (m_height < minimumSize.Height)
            m_height = minimumSize.Height;
    }

    public ulong Area()
    {
        return (ulong)m_width * (ulong)m_height;
    }

    public int DiagonalLengthSquared()
    {
        return m_width * m_width + m_height * m_height;
    }

    public IntSize TransposedSize()
    {
        return new IntSize(m_height, m_width);
    }

    public override bool Equals(object obj)
    {
        return obj is IntSize && this == (IntSize)obj;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public static IntSize operator +(IntSize a, IntSize b)
    {
        return new IntSize(a.Width + b.Width, a.Height + b.Height);
    }

    public static IntSize operator -(IntSize a, IntSize b)
    {
        return new IntSize(a.Width - b.Width, a.Height - b.Height);
    }

    public static IntSize operator -(IntSize size)
    {
        return new IntSize(-size.Width, -size.Height);
    }

    public static bool operator ==(IntSize a, IntSize b)
    {
        return a.Width == b.Width && a.Height == b.Height;
    }

    public static bool operator !=(IntSize a, IntSize b)
    {
        return !(a == b);
    }
}
