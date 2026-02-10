using System;
using UI.Numerics;

namespace UI.Geometry;

public struct FloatSize
{
    private float m_width, m_height;

    public FloatSize()
    {
        m_width = 0;
        m_height = 0;
    }

    public FloatSize(float width, float height)
    {
        m_width = width;
        m_height = height;
    }

    public FloatSize(IntSize size)
    {
        m_width = size.Width;
        m_height = size.Height;
    }

    public FloatSize(LayoutSize size)
    {
        m_width = size.Width.ToFloat();
        m_height = size.Height.ToFloat();
    }

    public static FloatSize NarrowPrecision(double width, double height)
    {
        return new FloatSize((float)width, (float)height);
    }

    public float Width { get => m_width; set => m_width = value; }
    public float Height { get => m_height; set => m_height = value; }

    public bool IsEmpty() => m_width <= 0 || m_height <= 0;
    public bool IsZero() => m_width == 0 && m_height == 0;
    public bool IsExpressibleAsIntSize() => m_width == (int)m_width && m_height == (int)m_height;

    public float AspectRatio() => m_width / m_height;

    public void Expand(float width, float height)
    {
        m_width += width;
        m_height += height;
    }

    public void Scale(float s) => Scale(s, s);

    public void Scale(float scaleX, float scaleY)
    {
        m_width *= scaleX;
        m_height *= scaleY;
    }

    public void ScaleAndFloor(float scale)
    {
        m_width = MathF.Floor(m_width * scale);
        m_height = MathF.Floor(m_height * scale);
    }

    public FloatSize ExpandedTo(FloatSize other)
    {
        return new FloatSize(Math.Max(m_width, other.m_width), Math.Max(m_height, other.m_height));
    }

    public FloatSize ShrunkTo(FloatSize other)
    {
        return new FloatSize(Math.Min(m_width, other.m_width), Math.Min(m_height, other.m_height));
    }

    public float DiagonalLength() => MathF.Sqrt(DiagonalLengthSquared());
    public float DiagonalLengthSquared() => m_width * m_width + m_height * m_height;

    public FloatSize TransposedSize() => new(m_height, m_width);

    public FloatSize ScaledBy(float scale) => ScaledBy(scale, scale);
    public FloatSize ScaledBy(float scaleX, float scaleY) => new(m_width * scaleX, m_height * scaleY);

    public override bool Equals(object obj) => obj is FloatSize size && this == size;
    public override int GetHashCode() => HashCode.Combine(m_width, m_height);

    public static FloatSize operator +(FloatSize a, FloatSize b) => new(a.Width + b.Width, a.Height + b.Height);
    public static FloatSize operator -(FloatSize a, FloatSize b) => new(a.Width - b.Width, a.Height - b.Height);
    public static FloatSize operator -(FloatSize size) => new(-size.Width, -size.Height);
    public static FloatSize operator *(FloatSize a, float b) => new(a.Width * b, a.Height * b);
    public static FloatSize operator *(float a, FloatSize b) => new(a * b.Width, a * b.Height);

    public static bool operator ==(FloatSize a, FloatSize b) => a.Width == b.Width && a.Height == b.Height;
    public static bool operator !=(FloatSize a, FloatSize b) => !(a == b);

    public static IntSize RoundedIntSize(FloatSize p) => new(MathExtras.ClampTo(MathF.Round(p.Width)), MathExtras.ClampTo(MathF.Round(p.Height)));
    public static IntSize FlooredIntSize(FloatSize p) => new(MathExtras.ClampTo(MathF.Floor(p.Width)), MathExtras.ClampTo(MathF.Floor(p.Height)));
    public static IntSize ExpandedIntSize(FloatSize p) => new(MathExtras.ClampTo(MathF.Ceiling(p.Width)), MathExtras.ClampTo(MathF.Ceiling(p.Height)));
    public static IntPoint FlooredIntPoint(FloatSize p) => new(MathExtras.ClampTo(MathF.Floor(p.Width)), MathExtras.ClampTo(MathF.Floor(p.Height)));
}
