namespace UI.Geometry;

public enum AspectRatioFit
{
    Shrink,
    Grow
}

public struct LayoutSize
{
    private LayoutUnit m_Width, m_Height;

    public LayoutUnit Width { readonly get => m_Width; set => m_Width = value; }
    public LayoutUnit Height { readonly get => m_Height; set => m_Height = value; }

    public LayoutSize()
    {
        m_Width = new LayoutUnit();
        m_Height = new LayoutUnit();
    }

    public LayoutSize(IntSize size)
    {
        m_Width = new LayoutUnit(size.Width);
        m_Height = new LayoutUnit(size.Height);
    }

    public LayoutSize(LayoutUnit width, LayoutUnit height)
    {
        m_Width = width;
        m_Height = height;
    }

    public LayoutSize(FloatSize size)
    {
        m_Width = new LayoutUnit(size.Width);
        m_Height = new LayoutUnit(size.Height);
    }

    public LayoutSize(DoubleSize size)
    {
        m_Width = new LayoutUnit(size.Width);
        m_Height = new LayoutUnit(size.Height);
    }

    public readonly bool IsEmpty() => m_Width.RawValue() <= 0 || m_Height.RawValue() <= 0;
    public readonly bool IsZero() => m_Width.RawValue() == 0 && m_Height.RawValue() == 0;

    public readonly float AspectRatio() => m_Width.ToFloat() / m_Height.ToFloat();

    public void Expand(LayoutUnit width, LayoutUnit height)
    {
        m_Width += width;
        m_Height += height;
    }

    public void Shrink(LayoutUnit width, LayoutUnit height)
    {
        m_Width -= width;
        m_Height -= height;
    }

    public void Scale(float scale)
    {
        m_Width *= scale;
        m_Height *= scale;
    }

    public void Scale(float widthScale, float heightScale)
    {
        m_Width *= widthScale;
        m_Height *= heightScale;
    }

    public readonly LayoutSize ExpandedTo(LayoutSize other)
    {
        return new LayoutSize(LayoutUnit.Max(m_Width, other.m_Width), LayoutUnit.Max(m_Height, other.m_Height));
    }

    public readonly LayoutSize ExpandedTo(IntSize other)
    {
        return new LayoutSize(
            m_Width > other.Width ? m_Width : new LayoutUnit(other.Width),
            m_Height > other.Height ? m_Height : new LayoutUnit(other.Height));
    }

    public readonly LayoutSize ShrunkTo(LayoutSize other)
    {
        return new LayoutSize(LayoutUnit.Min(m_Width, other.m_Width), LayoutUnit.Min(m_Height, other.m_Height));
    }

    public void ClampNegativeToZero()
    {
        this = ExpandedTo(new LayoutSize());
    }

    public void ClampToMinimumSize(LayoutSize minimumSize)
    {
        if (m_Width < minimumSize.Width) m_Width = minimumSize.Width;
        if (m_Height < minimumSize.Height) m_Height = minimumSize.Height;
    }

    public readonly LayoutSize TransposedSize()
    {
        return new LayoutSize(m_Height, m_Width);
    }

    public readonly LayoutSize FitToAspectRatio(LayoutSize aspectRatio, AspectRatioFit fit)
    {
        float heightScale = Height.ToFloat() / aspectRatio.Height.ToFloat();
        float widthScale = Width.ToFloat() / aspectRatio.Width.ToFloat();
        if ((widthScale > heightScale) != (fit == AspectRatioFit.Grow))
            return new LayoutSize(Height * aspectRatio.Width / aspectRatio.Height, Height);
        return new LayoutSize(Width, Width * aspectRatio.Height / aspectRatio.Width);
    }

    public readonly LayoutSize Fraction()
    {
        return new LayoutSize(m_Width.Fraction(), m_Height.Fraction());
    }

    public override readonly bool Equals(object? obj)
    {
        if(obj is null) return false;
        
        return obj is LayoutSize size && this == size;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public static LayoutSize operator +(LayoutSize a, LayoutSize b)
    {
        return new LayoutSize(a.Width + b.Width, a.Height + b.Height);
    }

    public static LayoutSize operator +(LayoutSize a, IntSize b)
    {
        return new LayoutSize(a.Width + b.Width, a.Height + b.Height);
    }

    public static LayoutSize operator -(LayoutSize a, LayoutSize b)
    {
        return new LayoutSize(a.Width - b.Width, a.Height - b.Height);
    }

    public static LayoutSize operator -(LayoutSize a, IntSize b)
    {
        return new LayoutSize(a.Width - b.Width, a.Height - b.Height);
    }

    public static LayoutSize operator -(LayoutSize size)
    {
        return new LayoutSize(-size.Width, -size.Height);
    }

    public static bool operator ==(LayoutSize a, LayoutSize b)
    {
        return a.Width == b.Width && a.Height == b.Height;
    }

    public static bool operator ==(LayoutSize a, IntSize b)
    {
        return a.Width == b.Width && a.Height == b.Height;
    }

    public static bool operator !=(LayoutSize a, LayoutSize b)
    {
        return !(a == b);
    }
    
    public static bool operator !=(LayoutSize a, IntSize b)
    {
        return !(a == b);
    }

    public static FloatPoint operator +(FloatPoint a, LayoutSize b)
    {
        return new FloatPoint(a.X + b.Width, a.Y + b.Height);
    }

    public static IntSize FlooredIntSize(LayoutSize s)
    {
        return new IntSize(s.Width.Floor(), s.Height.Floor());
    }

    public static IntSize RoundedIntSize(LayoutSize s)
    {
        return new IntSize(s.Width.Round(), s.Height.Round());
    }

    public static LayoutSize RoundedLayoutSize(FloatSize s)
    {
        return new LayoutSize(s);
    }
}
