using System;

namespace UI.Geometry;

public struct DoublePoint
{
    private double m_x, m_y;

    public DoublePoint()
    {
        m_x = 0;
        m_y = 0;
    }

    public DoublePoint(double x, double y)
    {
        m_x = x;
        m_y = y;
    }

    public DoublePoint(IntPoint p)
    {
        m_x = p.X;
        m_y = p.Y;
    }

    public DoublePoint(FloatPoint p)
    {
        m_x = p.X;
        m_y = p.Y;
    }

    public DoublePoint(LayoutPoint p)
    {
        m_x = p.X.ToDouble();
        m_y = p.Y.ToDouble();
    }

    public DoublePoint(IntSize size)
    {
        m_x = size.Width;
        m_y = size.Height;
    }

    public DoublePoint(FloatSize size)
    {
        m_x = size.Width;
        m_y = size.Height;
    }

    public DoublePoint(DoubleSize size)
    {
        m_x = size.Width;
        m_y = size.Height;
    }

    public static DoublePoint Zero() => new();

    public DoublePoint ExpandedTo(DoublePoint other) => new(Math.Max(m_x, other.m_x), Math.Max(m_y, other.m_y));
    public DoublePoint ShrunkTo(DoublePoint other) => new(Math.Min(m_x, other.m_x), Math.Min(m_y, other.m_y));

    public double X { get => m_x; set => m_x = value; }
    public double Y { get => m_y; set => m_y = value; }

    public void Move(DoubleSize s)
    {
        m_x += s.Width;
        m_y += s.Height;
    }

    public void Move(double x, double y)
    {
        m_x += x;
        m_y += y;
    }

    public void MoveBy(DoublePoint p)
    {
        m_x += p.X;
        m_y += p.Y;
    }

    public void Scale(float sx, float sy)
    {
        m_x *= sx;
        m_y *= sy;
    }

    public DoublePoint ScaledBy(float scale) => new(m_x * scale, m_y * scale);

    public override bool Equals(object obj) => obj is DoublePoint point && this == point;
    public override int GetHashCode() => HashCode.Combine(m_x, m_y);

    public static bool operator ==(DoublePoint a, DoublePoint b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(DoublePoint a, DoublePoint b) => !(a == b);

    public static DoublePoint operator +(DoublePoint a, DoubleSize b) => new(a.X + b.Width, a.Y + b.Height);
    public static DoubleSize operator -(DoublePoint a, DoublePoint b) => new(a.X - b.X, a.Y - b.Y);
    public static DoublePoint operator -(DoublePoint a) => new(-a.X, -a.Y);
    public static DoublePoint operator -(DoublePoint a, DoubleSize b) => new(a.X - b.Width, a.Y - b.Height);

    public static IntPoint RoundedIntPoint(DoublePoint p) => new(Numerics.Conversion.ClampTo<int>(Math.Round(p.X)), Numerics.Conversion.ClampTo<int>(Math.Round(p.Y)));
    public static IntPoint CeiledIntPoint(DoublePoint p) => new(Numerics.Conversion.ClampTo<int>(Math.Ceiling(p.X)), Numerics.Conversion.ClampTo<int>(Math.Ceiling(p.Y)));
    public static IntPoint FlooredIntPoint(DoublePoint p) => new(Numerics.Conversion.ClampTo<int>(Math.Floor(p.X)), Numerics.Conversion.ClampTo<int>(Math.Floor(p.Y)));

    public static FloatPoint ToFloatPoint(DoublePoint a) => new((float)a.X, (float)a.Y);
    public static DoubleSize ToDoubleSize(DoublePoint a) => new(a.X, a.Y);
}
