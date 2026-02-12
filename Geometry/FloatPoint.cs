using UI.Numerics;
using SkiaSharp;

namespace UI.Geometry;

public struct FloatPoint
{
    private float m_X, m_Y;

    public float X { readonly get => m_X; set => m_X = value; }
    public float Y { readonly get => m_Y; set => m_Y = value; }

    public FloatPoint()
    {
        m_X = 0;
        m_Y = 0;
    }

    public FloatPoint(float x, float y)
    {
        m_X = x;
        m_Y = y;
    }

    public FloatPoint(in IntPoint p)
    {
        m_X = p.X;
        m_Y = p.Y;
    }
    
    public FloatPoint(in DoublePoint p)
    {
        m_X = (float) p.X;
        m_Y = (float) p.Y;
    }
    
    public FloatPoint(in LayoutPoint p)
    {
        m_X = p.X.ToFloat();
        m_Y = p.Y.ToFloat();
    }

    public FloatPoint(in FloatSize size)
    {
        m_X = size.Width;
        m_Y = size.Height;
    }
    
    public FloatPoint(in LayoutSize size)
    {
        m_X = size.Width.ToFloat();
        m_Y = size.Height.ToFloat();
    }

    public FloatPoint(in IntSize size)
    {
        m_X = size.Width;
        m_Y = size.Height;
    }

    public static FloatPoint Zero() => new();

    public static FloatPoint NarrowPrecision(double x, double y) => new((float)x, (float)y);

    public void Set(float x, float y)
    {
        m_X = x;
        m_Y = y;
    }
    
    public void Move(float dx, float dy)
    {
        m_X += dx;
        m_Y += dy;
    }

    public void Move(in IntSize s)
    {
        m_X += s.Width;
        m_Y += s.Height;
    }
    
    public void Move(in LayoutSize s)
    {
        m_X += s.Width.ToFloat();
        m_Y += s.Height.ToFloat();
    }

    public void Move(in FloatSize s)
    {
        m_X += s.Width;
        m_Y += s.Height;
    }

    public void MoveBy(in IntPoint p)
    {
        m_X += p.X;
        m_Y += p.Y;
    }
    
    public void MoveBy(in LayoutPoint p)
    {
        m_X += p.X.ToFloat();
        m_Y += p.Y.ToFloat();
    }

    public void MoveBy(in FloatPoint p)
    {
        m_X += p.X;
        m_Y += p.Y;
    }

    public void Scale(float sx, float sy)
    {
        m_X *= sx;
        m_Y *= sy;
    }

    public readonly float Dot(in FloatPoint other) => m_X * other.X + m_Y * other.Y;

    public readonly float SlopeAngleRadians() => (float)Math.Atan2(m_Y, m_X);
    public readonly float Length() => (float)Math.Sqrt(LengthSquared());
    public readonly float LengthSquared() => m_X * m_X + m_Y * m_Y;

    public readonly FloatPoint ExpandedTo(in FloatPoint other) => new(Math.Max(m_X, other.X), Math.Max(m_Y, other.Y));
    public readonly FloatPoint ShrunkTo(in FloatPoint other) => new(Math.Min(m_X, other.X), Math.Min(m_Y, other.Y));

    public readonly FloatPoint TransposedPoint() => new(m_Y, m_X);

    public readonly FloatPoint ScaledBy(float scale) => new(m_X * scale, m_Y * scale);

    public override readonly bool Equals(object? obj) => obj is FloatPoint point && this == point;

    public override readonly int GetHashCode() => HashCode.Combine(m_X, m_Y);

    public static FloatPoint operator +(FloatPoint a, FloatSize b) => new(a.X + b.Width, a.Y + b.Height);
    public static FloatPoint operator +(FloatPoint a, IntSize b) => new(a.X + b.Width, a.Y + b.Height);
    public static FloatPoint operator +(FloatPoint a, FloatPoint b) => new(a.X + b.X, a.Y + b.Y);
    public static FloatPoint operator +(FloatPoint a, IntPoint b) => new(a.X + b.X, a.Y + b.Y);

    public static FloatPoint operator -(FloatPoint a, FloatSize b) => new(a.X - b.Width, a.Y - b.Height);
    public static FloatSize operator -(FloatPoint a, FloatPoint b) => new(a.X - b.X, a.Y - b.Y);
    public static FloatSize operator -(FloatPoint a, IntPoint b) => new(a.X - b.X, a.Y - b.Y);
    public static FloatPoint operator -(FloatPoint a) => new(-a.X, -a.Y);

    public static bool operator ==(FloatPoint a, FloatPoint b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(FloatPoint a, FloatPoint b) => !(a == b);

    public static float operator *(FloatPoint a, FloatPoint b) => a.Dot(b);
    
    public static IntPoint RoundedIntPoint(in FloatPoint p) => new(MathExtras.ClampTo(MathF.Round(p.X)), MathExtras.ClampTo(MathF.Round(p.Y)));
    public static IntSize RoundedIntSize(in FloatPoint p) => new(MathExtras.ClampTo(MathF.Round(p.X)), MathExtras.ClampTo(MathF.Round(p.Y)));
    public static IntPoint FlooredIntPoint(in FloatPoint p) => new(MathExtras.ClampTo(MathF.Floor(p.X)), MathExtras.ClampTo(MathF.Floor(p.Y)));
    public static IntPoint CeiledIntPoint(in FloatPoint p) => new(MathExtras.ClampTo(MathF.Ceiling(p.X)), MathExtras.ClampTo(MathF.Ceiling(p.Y)));
    public static IntSize FlooredIntSize(in FloatPoint p) => new(MathExtras.ClampTo(MathF.Floor(p.X)), MathExtras.ClampTo(MathF.Floor(p.Y)));

    public static FloatSize ToFloatSize(in FloatPoint a) => new(a.X, a.Y);

    public static implicit operator SKPoint(in FloatPoint point)
    {
        return new SKPoint(point.m_X, point.m_Y);
    }

    public override readonly string ToString()
    {
        return $"FloatPoint({m_X}, {m_Y})";
    }
}
