namespace UI.Geometry;

public enum AspectRatioFit
{
    Shrink,
    Grow
}

public struct LayoutSize
{
    private LayoutUnit m_width, m_height;

    public LayoutSize()
    {
        m_width = new LayoutUnit();
        m_height = new LayoutUnit();
    }

    public LayoutSize(IntSize size)
    {
        m_width = new LayoutUnit(size.Width);
        m_height = new LayoutUnit(size.Height);
    }

    public LayoutSize(LayoutUnit width, LayoutUnit height)
    {
        m_width = width;
        m_height = height;
    }

    public LayoutSize(FloatSize size)
    {
        m_width = new LayoutUnit(size.Width);
        m_height = new LayoutUnit(size.Height);
    }

    public LayoutSize(DoubleSize size)
    {
        m_width = new LayoutUnit(size.Width);
        m_height = new LayoutUnit(size.Height);
    }

    public LayoutUnit Width { get { return m_width; } set { m_width = value; } }
    public LayoutUnit Height { get { return m_height; } set { m_height = value; } }

    public bool IsEmpty() => m_width.RawValue() <= 0 || m_height.RawValue() <= 0;
    public bool IsZero() => m_width.RawValue() == 0 && m_height.RawValue() == 0;

    public float AspectRatio() => m_width.ToFloat() / m_height.ToFloat();

    public void Expand(LayoutUnit width, LayoutUnit height)
    {
        m_width += width;
        m_height += height;
    }

    public void Shrink(LayoutUnit width, LayoutUnit height)
    {
        m_width -= width;
        m_height -= height;
    }

    public void Scale(float scale)
    {
        m_width *= scale;
        m_height *= scale;
    }

    public void Scale(float widthScale, float heightScale)
    {
        m_width *= widthScale;
        m_height *= heightScale;
    }

    public LayoutSize ExpandedTo(LayoutSize other)
    {
        return new LayoutSize(LayoutUnit.Max(m_width, other.m_width), LayoutUnit.Max(m_height, other.m_height));
    }

    public LayoutSize ExpandedTo(IntSize other)
    {
        return new LayoutSize(
            m_width > other.Width ? m_width : new LayoutUnit(other.Width),
            m_height > other.Height ? m_height : new LayoutUnit(other.Height));
    }

    public LayoutSize ShrunkTo(LayoutSize other)
    {
        return new LayoutSize(LayoutUnit.Min(m_width, other.m_width), LayoutUnit.Min(m_height, other.m_height));
    }

    public void ClampNegativeToZero()
    {
        this = ExpandedTo(new LayoutSize());
    }

    public void ClampToMinimumSize(LayoutSize minimumSize)
    {
        if (m_width < minimumSize.Width)
            m_width = minimumSize.Width;
        if (m_height < minimumSize.Height)
            m_height = minimumSize.Height;
    }

    public LayoutSize TransposedSize()
    {
        return new LayoutSize(m_height, m_width);
    }

    public LayoutSize FitToAspectRatio(LayoutSize aspectRatio, AspectRatioFit fit)
    {
        float heightScale = Height.ToFloat() / aspectRatio.Height.ToFloat();
        float widthScale = Width.ToFloat() / aspectRatio.Width.ToFloat();
        if ((widthScale > heightScale) != (fit == AspectRatioFit.Grow))
            return new LayoutSize(Height * aspectRatio.Width / aspectRatio.Height, Height);
        return new LayoutSize(Width, Width * aspectRatio.Height / aspectRatio.Width);
    }

    public LayoutSize Fraction()
    {
        return new LayoutSize(m_width.Fraction(), m_height.Fraction());
    }

    public override bool Equals(object obj)
    {
        return obj is LayoutSize && this == (LayoutSize)obj;
    }

    public override int GetHashCode()
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
