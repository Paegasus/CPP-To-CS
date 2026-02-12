using SkiaSharp;

using static UI.Numerics.MathExtras;

namespace UI.Geometry;

public struct FloatSize
{
    // Machine epsilon for IEEE-754 float
	// Equivalent to std::numeric_limits<float>::epsilon()
    private static readonly float epsilon = (float)Math.Pow(2, -23);

    private float m_Width, m_Height;

    public float Width { readonly get => m_Width; set => m_Width = value; }
    public float Height { readonly get => m_Height; set => m_Height = value; }

    public FloatSize()
    {
        m_Width = 0;
        m_Height = 0;
    }

    public FloatSize(float width, float height)
    {
        m_Width = width;
        m_Height = height;
    }

    public FloatSize(in IntSize size)
    {
        m_Width = size.Width;
        m_Height = size.Height;
    }

    public FloatSize(in SKSize size)
    {
        m_Width = size.Width;
        m_Height = size.Height;
    }

    public FloatSize(in LayoutSize size)
    {
        m_Width = size.Width.ToFloat();
        m_Height = size.Height.ToFloat();
    }

    public static FloatSize NarrowPrecision(double width, double height)
    {
        return new FloatSize(ClampTo<float>(width), ClampTo<float>(height));
    }

    public readonly bool IsEmpty() => m_Width <= 0 || m_Height <= 0;

    public readonly bool IsZero()
    {
        return Math.Abs(m_Width) < epsilon && Math.Abs(m_Height) < epsilon;
    }

    public readonly bool IsExpressibleAsIntSize()
    {
        return IsWithinIntRange(m_Width) && IsWithinIntRange(m_Height);
    }

    public readonly float AspectRatio() => m_Width / m_Height;

    public void Expand(float width, float height)
    {
        m_Width += width;
        m_Height += height;
    }

    public void Scale(float s) => Scale(s, s);

    public void Scale(float scaleX, float scaleY)
    {
        m_Width *= scaleX;
        m_Height *= scaleY;
    }

    public void ScaleAndFloor(float scale)
    {
        m_Width = MathF.Floor(m_Width * scale);
        m_Height = MathF.Floor(m_Height * scale);
    }

    public readonly FloatSize ExpandedTo(in FloatSize other)
    {
        return new FloatSize(Math.Max(m_Width, other.m_Width), Math.Max(m_Height, other.m_Height));
    }

    public readonly FloatSize ShrunkTo(in FloatSize other)
    {
        return new FloatSize(Math.Min(m_Width, other.m_Width), Math.Min(m_Height, other.m_Height));
    }

    public readonly float DiagonalLength() => float.Hypot(m_Width, m_Height);
    public readonly float DiagonalLengthSquared() => m_Width * m_Width + m_Height * m_Height;

    public readonly FloatSize TransposedSize() => new(m_Height, m_Width);

    public readonly FloatSize ScaledBy(float scale) => ScaledBy(scale, scale);
    public readonly FloatSize ScaledBy(float scaleX, float scaleY) => new(m_Width * scaleX, m_Height * scaleY);

    public override readonly bool Equals(object? obj) => obj is FloatSize size && this == size;

    public override readonly int GetHashCode() => HashCode.Combine(m_Width, m_Height);

    public static FloatPoint operator +(IntPoint a, FloatSize b) => new(a.X + b.Width, a.Y + b.Height);
    public static FloatSize operator +(FloatSize a, FloatSize b) => new(a.Width + b.Width, a.Height + b.Height);
    public static FloatSize operator -(FloatSize a, FloatSize b) => new(a.Width - b.Width, a.Height - b.Height);
    public static FloatSize operator -(FloatSize size) => new(-size.Width, -size.Height);
    public static FloatSize operator *(FloatSize a, float b) => new(a.Width * b, a.Height * b);
    public static FloatSize operator *(float a, FloatSize b) => new(a * b.Width, a * b.Height);

    public static bool operator ==(FloatSize a, FloatSize b) => a.Width == b.Width && a.Height == b.Height;
    public static bool operator !=(FloatSize a, FloatSize b) => !(a == b);

    public static IntSize RoundedIntSize(in FloatSize p) => new(ClampTo<int>(MathF.Round(p.Width)), ClampTo<int>(MathF.Round(p.Height)));
    public static IntSize FlooredIntSize(in FloatSize p) => new(ClampTo<int>(MathF.Floor(p.Width)), ClampTo<int>(MathF.Floor(p.Height)));
    public static IntSize ExpandedIntSize(in FloatSize p) => new(ClampTo<int>(MathF.Ceiling(p.Width)), ClampTo<int>(MathF.Ceiling(p.Height)));
    public static IntPoint FlooredIntPoint(in FloatSize p) => new(ClampTo<int>(MathF.Floor(p.Width)), ClampTo<int>(MathF.Floor(p.Height)));

    public static implicit operator SKSize(in FloatSize size)
    {
        return new SKSize(size.m_Width, size.m_Height);
    }
}
