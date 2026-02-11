using UI.Numerics;

namespace UI.Geometry;

public struct DoublePoint
{
    private double m_X, m_Y;

    public double X { readonly get => m_X; set => m_X = value; }
    public double Y { readonly get => m_Y; set => m_Y = value; }

    public DoublePoint()
    {
        m_X = 0;
        m_Y = 0;
    }

    public DoublePoint(double x, double y)
    {
        m_X = x;
        m_Y = y;
    }

    public DoublePoint(IntPoint p)
    {
        m_X = p.X;
        m_Y = p.Y;
    }

    public DoublePoint(FloatPoint p)
    {
        m_X = p.X;
        m_Y = p.Y;
    }

    public DoublePoint(LayoutPoint p)
    {
        m_X = p.X.ToDouble();
        m_Y = p.Y.ToDouble();
    }

    public DoublePoint(IntSize size)
    {
        m_X = size.Width;
        m_Y = size.Height;
    }

    public DoublePoint(FloatSize size)
    {
        m_X = size.Width;
        m_Y = size.Height;
    }

    public DoublePoint(DoubleSize size)
    {
        m_X = size.Width;
        m_Y = size.Height;
    }

    public static DoublePoint Zero() => new();

    public readonly DoublePoint ExpandedTo(DoublePoint other) => new(Math.Max(m_X, other.m_X), Math.Max(m_Y, other.m_Y));
    public readonly DoublePoint ShrunkTo(DoublePoint other) => new(Math.Min(m_X, other.m_X), Math.Min(m_Y, other.m_Y));

    public void Set(double x, double y)
    {
        m_X = x;
        m_Y = y;
    }

    public void Move(DoubleSize s)
    {
        m_X += s.Width;
        m_Y += s.Height;
    }

    public void Move(double x, double y)
    {
        m_X += x;
        m_Y += y;
    }

    public void MoveBy(DoublePoint p)
    {
        m_X += p.X;
        m_Y += p.Y;
    }

    public void Scale(float sx, float sy)
    {
        m_X *= sx;
        m_Y *= sy;
    }

    public readonly DoublePoint ScaledBy(float scale) => new(m_X * scale, m_Y * scale);

    public override readonly bool Equals(object? obj)
    {
        if(obj is null) return false;
        
        return obj is DoublePoint point && this == point;
    }

    public override readonly int GetHashCode() => HashCode.Combine(m_X, m_Y);

    public static bool operator ==(DoublePoint a, DoublePoint b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(DoublePoint a, DoublePoint b) => !(a == b);

    public static DoublePoint operator +(DoublePoint a, DoubleSize b) => new(a.X + b.Width, a.Y + b.Height);
    public static DoubleSize operator -(DoublePoint a, DoublePoint b) => new(a.X - b.X, a.Y - b.Y);
    public static DoublePoint operator -(DoublePoint a) => new(-a.X, -a.Y);
    public static DoublePoint operator -(DoublePoint a, DoubleSize b) => new(a.X - b.Width, a.Y - b.Height);

    public static IntPoint RoundedIntPoint(DoublePoint p) => new(MathExtras.ClampTo(Math.Round(p.X)), MathExtras.ClampTo(Math.Round(p.Y)));
    public static IntPoint CeiledIntPoint(DoublePoint p) => new(MathExtras.ClampTo(Math.Ceiling(p.X)), MathExtras.ClampTo(Math.Ceiling(p.Y)));
    public static IntPoint FlooredIntPoint(DoublePoint p) => new(MathExtras.ClampTo(Math.Floor(p.X)), MathExtras.ClampTo(Math.Floor(p.Y)));

    public static FloatPoint ToFloatPoint(DoublePoint a) => new((float)a.X, (float)a.Y);
    public static DoubleSize ToDoubleSize(DoublePoint a) => new(a.X, a.Y);
}
