using UI.Numerics;

namespace UI.Geometry;

public struct DoubleSize
{
    private double m_width, m_height;

    public DoubleSize()
    {
        m_width = 0;
        m_height = 0;
    }

    public DoubleSize(double width, double height)
    {
        m_width = width;
        m_height = height;
    }

    public DoubleSize(IntSize p)
    {
        m_width = p.Width;
        m_height = p.Height;
    }

    public DoubleSize(FloatSize p)
    {
        m_width = p.Width;
        m_height = p.Height;
    }

    public double Width { readonly get => m_width; set => m_width = value; }
    public double Height { readonly get => m_height; set => m_height = value; }

    public readonly bool IsEmpty() => m_width <= 0 || m_height <= 0;
    public readonly bool IsZero() => m_width == 0 && m_height == 0;

    public readonly DoubleSize ExpandedTo(DoubleSize other)
    {
        return new DoubleSize(Math.Max(m_width, other.m_width), Math.Max(m_height, other.m_height));
    }

    public readonly DoubleSize ShrunkTo(DoubleSize other)
    {
        return new DoubleSize(Math.Min(m_width, other.m_width), Math.Min(m_height, other.m_height));
    }

    public void Scale(float s) { Scale(s, s); }
    public void Scale(float sx, float sy)
    {
        m_width *= sx;
        m_height *= sy;
    }

    public override readonly bool Equals(object? obj)
    {
        if(obj is null) return false;
        
        return obj is DoubleSize size && this == size;
    }

    public override readonly int GetHashCode() => HashCode.Combine(m_width, m_height);

    public static DoubleSize operator +(DoubleSize a, DoubleSize b)
    {
        return new DoubleSize(a.Width + b.Width, a.Height + b.Height);
    }

    public static DoubleSize operator -(DoubleSize a, DoubleSize b)
    {
        return new DoubleSize(a.Width - b.Width, a.Height - b.Height);
    }

    public static DoubleSize operator -(DoubleSize size)
    {
        return new DoubleSize(-size.Width, -size.Height);
    }

    public static bool operator ==(DoubleSize a, DoubleSize b)
    {
        return a.Width == b.Width && a.Height == b.Height;
    }

    public static bool operator !=(DoubleSize a, DoubleSize b)
    {
        return !(a == b);
    }

    public static FloatSize ToFloatSize(DoubleSize a)
    {
        return new FloatSize((float)a.Width, (float)a.Height);
    }

    public static IntSize RoundedIntSize(DoubleSize p)
    {
        return new IntSize(MathExtras.ClampTo(Math.Round(p.Width)), MathExtras.ClampTo(Math.Round(p.Height)));
    }

    public static IntSize FlooredIntSize(DoubleSize p)
    {
        return new IntSize(MathExtras.ClampTo(Math.Floor(p.Width)), MathExtras.ClampTo(Math.Floor(p.Height)));
    }

    public static IntSize ExpandedIntSize(DoubleSize p)
    {
        return new IntSize(MathExtras.ClampTo(Math.Ceiling(p.Width)), MathExtras.ClampTo(Math.Ceiling(p.Height)));
    }
}
