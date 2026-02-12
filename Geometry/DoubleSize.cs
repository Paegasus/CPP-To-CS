using UI.Numerics;

namespace UI.Geometry;

public struct DoubleSize
{
    private double m_Width, m_Height;

    public double Width { readonly get => m_Width; set => m_Width = value; }
    public double Height { readonly get => m_Height; set => m_Height = value; }

    public DoubleSize()
    {
        m_Width = 0;
        m_Height = 0;
    }

    public DoubleSize(double width, double height)
    {
        m_Width = width;
        m_Height = height;
    }

    public DoubleSize(in IntSize p)
    {
        m_Width = p.Width;
        m_Height = p.Height;
    }

    public DoubleSize(in FloatSize p)
    {
        m_Width = p.Width;
        m_Height = p.Height;
    }

    public readonly bool IsEmpty() => m_Width <= 0 || m_Height <= 0;
    public readonly bool IsZero() => m_Width == 0 && m_Height == 0;

    public readonly DoubleSize ExpandedTo(in DoubleSize other)
    {
        return new DoubleSize(Math.Max(m_Width, other.m_Width), Math.Max(m_Height, other.m_Height));
    }

    public readonly DoubleSize ShrunkTo(in DoubleSize other)
    {
        return new DoubleSize(Math.Min(m_Width, other.m_Width), Math.Min(m_Height, other.m_Height));
    }

    public void Scale(float s) { Scale(s, s); }
    public void Scale(float sx, float sy)
    {
        m_Width *= sx;
        m_Height *= sy;
    }

    public override readonly bool Equals(object? obj) => obj is DoubleSize size && this == size;

    public override readonly int GetHashCode() => HashCode.Combine(m_Width, m_Height);

    public static implicit operator DoubleSize(in IntSize s) => new(s);
    public static implicit operator DoubleSize(in FloatSize s) => new(s);
    public static implicit operator DoubleSize(in LayoutSize s) => new(s);

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

    public static FloatSize ToFloatSize(in DoubleSize a)
    {
        return new FloatSize((float)a.Width, (float)a.Height);
    }

    public static IntSize RoundedIntSize(in DoubleSize p)
    {
        return new IntSize(MathExtras.ClampTo(Math.Round(p.Width)), MathExtras.ClampTo(Math.Round(p.Height)));
    }

    public static IntSize FlooredIntSize(in DoubleSize p)
    {
        return new IntSize(MathExtras.ClampTo(Math.Floor(p.Width)), MathExtras.ClampTo(Math.Floor(p.Height)));
    }

    public static IntSize ExpandedIntSize(in DoubleSize p)
    {
        return new IntSize(MathExtras.ClampTo(Math.Ceiling(p.Width)), MathExtras.ClampTo(Math.Ceiling(p.Height)));
    }
}
