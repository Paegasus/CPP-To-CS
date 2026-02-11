namespace UI.Geometry;

public struct IntSize
{
    private int m_Width, m_Height;

    public int Width { readonly get => m_Width; set => m_Width = value; }
    public int Height { readonly get => m_Height; set => m_Height = value; }

    public IntSize()
    {
        m_Width = 0;
        m_Height = 0;
    }

    public IntSize(int width, int height)
    {
        m_Width = width;
        m_Height = height;
    }

    public readonly bool IsEmpty() => m_Width <= 0 || m_Height <= 0;
    public readonly bool IsZero() => m_Width == 0 && m_Height == 0;
    public readonly float AspectRatio() => (float)m_Width / m_Height;

    public void Expand(int width, int height)
    {
        m_Width += width;
        m_Height += height;
    }

    public void Scale(float widthScale, float heightScale)
    {
        m_Width = (int)(m_Width * widthScale);
        m_Height = (int)(m_Height * heightScale);
    }

    public void Scale(float scale)
    {
        Scale(scale, scale);
    }

    public readonly IntSize ExpandedTo(in IntSize other)
    {
        return new IntSize(Math.Max(m_Width, other.m_Width), Math.Max(m_Height, other.m_Height));
    }

    public readonly IntSize ShrunkTo(in IntSize other)
    {
        return new IntSize(Math.Min(m_Width, other.m_Width), Math.Min(m_Height, other.m_Height));
    }

    public void ClampNegativeToZero()
    {
        this = ExpandedTo(new IntSize());
    }

    public void ClampToMinimumSize(in IntSize minimumSize)
    {
        if (m_Width < minimumSize.Width) m_Width = minimumSize.Width;
        if (m_Height < minimumSize.Height) m_Height = minimumSize.Height;
    }

    public readonly ulong Area()
    {
        return (ulong)m_Width * (ulong)m_Height;
    }

    public readonly int DiagonalLengthSquared()
    {
        return m_Width * m_Width + m_Height * m_Height;
    }

    public readonly IntSize TransposedSize()
    {
        return new IntSize(m_Height, m_Width);
    }

    public override readonly bool Equals(object? obj)
    {
        if(obj is null) return false;
        
        return obj is IntSize size && this == size;
    }

    public override readonly int GetHashCode()
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

    public static bool operator !=(IntSize a, IntSize b) { return !(a == b); }
}
