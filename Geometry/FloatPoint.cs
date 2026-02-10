using System;
using UI.Numerics;

namespace UI.Geometry;

public struct FloatPoint
{
    private float m_x, m_y;

    public FloatPoint()
    {
        m_x = 0;
        m_y = 0;
    }

    public FloatPoint(float x, float y)
    {
        m_x = x;
        m_y = y;
    }

    public FloatPoint(IntPoint p)
    {
        m_x = p.X;
        m_y = p.Y;
    }
    
    public FloatPoint(DoublePoint p)
    {
        m_x = (float) p.X;
        m_y = (float) p.Y;
    }
    
    public FloatPoint(LayoutPoint p)
    {
        m_x = p.X.ToFloat();
        m_y = p.Y.ToFloat();
    }

    public FloatPoint(FloatSize size)
    {
        m_x = size.Width;
        m_y = size.Height;
    }
    
    public FloatPoint(LayoutSize size)
    {
        m_x = size.Width.ToFloat();
        m_y = size.Height.ToFloat();
    }

    public FloatPoint(IntSize size)
    {
        m_x = size.Width;
        m_y = size.Height;
    }

    public static FloatPoint Zero() => new();

    public static FloatPoint NarrowPrecision(double x, double y) => new((float)x, (float)y);

    public float X { get => m_x; set => m_x = value; }
    public float Y { get => m_y; set => m_y = value; }

    public void Set(float x, float y)
    {
        m_x = x;
        m_y = y;
    }

    public void Move(float dx, float dy)
    {
        m_x += dx;
        m_y += dy;
    }

    public void Move(IntSize s)
    {
        m_x += s.Width;
        m_y += s.Height;
    }
    
    public void Move(LayoutSize s)
    {
        m_x += s.Width.ToFloat();
        m_y += s.Height.ToFloat();
    }

    public void Move(FloatSize s)
    {
        m_x += s.Width;
        m_y += s.Height;
    }

    public void MoveBy(IntPoint p)
    {
        m_x += p.X;
        m_y += p.Y;
    }
    
    public void MoveBy(LayoutPoint p)
    {
        m_x += p.X.ToFloat();
        m_y += p.Y.ToFloat();
    }

    public void MoveBy(FloatPoint p)
    {
        m_x += p.X;
        m_y += p.Y;
    }

    public void Scale(float sx, float sy)
    {
        m_x *= sx;
        m_y *= sy;
    }

    public float Dot(FloatPoint other) => m_x * other.X + m_y * other.Y;

    public float SlopeAngleRadians() => (float)Math.Atan2(m_y, m_x);
    public float Length() => (float)Math.Sqrt(LengthSquared());
    public float LengthSquared() => m_x * m_x + m_y * m_y;

    public FloatPoint ExpandedTo(FloatPoint other) => new(Math.Max(m_x, other.X), Math.Max(m_y, other.Y));
    public FloatPoint ShrunkTo(FloatPoint other) => new(Math.Min(m_x, other.X), Math.Min(m_y, other.Y));

    public FloatPoint TransposedPoint() => new(m_y, m_x);

    public FloatPoint ScaledBy(float scale) => new(m_x * scale, m_y * scale);

    public override bool Equals(object obj) => obj is FloatPoint point && this == point;
    public override int GetHashCode() => HashCode.Combine(m_x, m_y);

    public static FloatPoint operator +(FloatPoint a, FloatSize b) => new(a.X + b.Width, a.Y + b.Height);
    public static FloatPoint operator +(FloatPoint a, IntSize b) => new(a.X + b.Width, a.Y + b.Height);
    public static FloatPoint operator +(IntPoint a, FloatSize b) => new(a.X + b.Width, a.Y + b.Height);
    public static FloatPoint operator +(FloatPoint a, FloatPoint b) => new(a.X + b.X, a.Y + b.Y);
    public static FloatPoint operator +(FloatPoint a, IntPoint b) => new(a.X + b.X, a.Y + b.Y);

    public static FloatPoint operator -(FloatPoint a, FloatSize b) => new(a.X - b.Width, a.Y - b.Height);
    public static FloatSize operator -(FloatPoint a, FloatPoint b) => new(a.X - b.X, a.Y - b.Y);
    public static FloatSize operator -(FloatPoint a, IntPoint b) => new(a.X - b.X, a.Y - b.Y);
    public static FloatPoint operator -(FloatPoint a) => new(-a.X, -a.Y);

    public static bool operator ==(FloatPoint a, FloatPoint b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(FloatPoint a, FloatPoint b) => !(a == b);

    public static float operator *(FloatPoint a, FloatPoint b) => a.Dot(b);
    
    public static IntPoint RoundedIntPoint(FloatPoint p) => new(MathExtras.ClampTo(MathF.Round(p.X)), MathExtras.ClampTo(MathF.Round(p.Y)));
    public static IntSize RoundedIntSize(FloatPoint p) => new(MathExtras.ClampTo(MathF.Round(p.X)), MathExtras.ClampTo(MathF.Round(p.Y)));
    public static IntPoint FlooredIntPoint(FloatPoint p) => new(MathExtras.ClampTo(MathF.Floor(p.X)), MathExtras.ClampTo(MathF.Floor(p.Y)));
    public static IntPoint CeiledIntPoint(FloatPoint p) => new(MathExtras.ClampTo(MathF.Ceiling(p.X)), MathExtras.ClampTo(MathF.Ceiling(p.Y)));
    public static IntSize FlooredIntSize(FloatPoint p) => new(MathExtras.ClampTo(MathF.Floor(p.X)), MathExtras.ClampTo(MathF.Floor(p.Y)));

    public static FloatSize ToFloatSize(FloatPoint a) => new(a.X, a.Y);
}
