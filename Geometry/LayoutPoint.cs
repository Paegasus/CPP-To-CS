namespace UI.Geometry;

public struct LayoutPoint
{
    private LayoutUnit m_X, m_Y;

    public LayoutUnit X { readonly get => m_X; set => m_X = value; }
    public LayoutUnit Y { readonly get => m_Y; set => m_Y = value; }

    public LayoutPoint()
    {
        m_X = new LayoutUnit();
        m_Y = new LayoutUnit();
    }

    public LayoutPoint(in LayoutUnit x, in LayoutUnit y)
    {
        m_X = x;
        m_Y = y;
    }

    public LayoutPoint(in IntPoint point)
    {
        m_X = new LayoutUnit(point.X);
        m_Y = new LayoutUnit(point.Y);
    }

    public LayoutPoint(in FloatPoint point)
    {
        m_X = new LayoutUnit(point.X);
        m_Y = new LayoutUnit(point.Y);
    }

    public LayoutPoint(in DoublePoint point)
    {
        m_X = new LayoutUnit(point.X);
        m_Y = new LayoutUnit(point.Y);
    }

    public LayoutPoint(in LayoutSize size)
    {
        m_X = size.Width;
        m_Y = size.Height;
    }

    public static LayoutPoint Zero() { return new LayoutPoint(); }

    public void Move(in LayoutSize s)
    {
        m_X += s.Width;
        m_Y += s.Height;
    }

    public void Move(in IntSize s)
    {
        m_X += s.Width;
        m_Y += s.Height;
    }

    public void MoveBy(in LayoutPoint offset)
    {
        m_X += offset.X;
        m_Y += offset.Y;
    }

    public void Move(in LayoutUnit dx, in LayoutUnit dy)
    {
        m_X += dx;
        m_Y += dy;
    }

    public void Scale(float sx, float sy)
    {
        m_X *= sx;
        m_Y *= sy;
    }

    public readonly LayoutPoint ExpandedTo(in LayoutPoint other)
    {
        return new LayoutPoint(LayoutUnit.Max(m_X, other.m_X), LayoutUnit.Max(m_Y, other.m_Y));
    }

    public readonly LayoutPoint ShrunkTo(in LayoutPoint other)
    {
        return new LayoutPoint(LayoutUnit.Min(m_X, other.m_X), LayoutUnit.Min(m_Y, other.m_Y));
    }

    public void ClampNegativeToZero()
    {
        this = ExpandedTo(Zero());
    }

    public readonly LayoutPoint TransposedPoint()
    {
        return new LayoutPoint(m_Y, m_X);
    }

    public static LayoutPoint operator +(LayoutPoint a, LayoutSize b)
    {
        return new LayoutPoint(a.X + b.Width, a.Y + b.Height);
    }

    public static LayoutPoint operator +(LayoutPoint a, LayoutPoint b)
    {
        return new LayoutPoint(a.X + b.X, a.Y + b.Y);
    }

    public static LayoutPoint operator +(LayoutPoint a, IntSize b)
    {
        return new LayoutPoint(a.X + b.Width, a.Y + b.Height);
    }

    public static LayoutPoint operator -(LayoutPoint a, LayoutSize b)
    {
        return new LayoutPoint(a.X - b.Width, a.Y - b.Height);
    }

    public static LayoutPoint operator -(LayoutPoint a, IntSize b)
    {
        return new LayoutPoint(a.X - b.Width, a.Y - b.Height);
    }

    public static LayoutSize operator -(LayoutPoint a, LayoutPoint b)
    {
        return new LayoutSize(a.X - b.X, a.Y - b.Y);
    }
    
    public static LayoutSize operator -(LayoutPoint a, IntPoint b)
    {
        return new LayoutSize(a.X - b.X, a.Y - b.Y);
    }

    public static LayoutPoint operator -(LayoutPoint point)
    {
        return new LayoutPoint(-point.X, -point.Y);
    }

    public static bool operator ==(LayoutPoint a, LayoutPoint b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(LayoutPoint a, LayoutPoint b)
    {
        return !(a == b);
    }

    public override readonly bool Equals(object? obj)
    {
        if(obj is null) return false;
        
        return obj is LayoutPoint point && this == point;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static LayoutPoint ToPoint(in LayoutSize size)
    {
        return new LayoutPoint(size.Width, size.Height);
    }

    public static LayoutPoint ToLayoutPoint(in LayoutSize p)
    {
        return new LayoutPoint(p.Width, p.Height);
    }

    public static LayoutSize ToSize(in LayoutPoint a)
    {
        return new LayoutSize(a.X, a.Y);
    }

    public static IntPoint FlooredIntPoint(in LayoutPoint point)
    {
        return new IntPoint(point.X.Floor(), point.Y.Floor());
    }

    public static IntPoint RoundedIntPoint(in LayoutPoint point)
    {
        return new IntPoint(point.X.Round(), point.Y.Round());
    }

    public static IntPoint CeiledIntPoint(in LayoutPoint point)
    {
        return new IntPoint(point.X.Ceil(), point.Y.Ceil());
    }

    public static LayoutPoint FlooredLayoutPoint(in FloatPoint p)
    {
        return new LayoutPoint(LayoutUnit.FromFloatFloor(p.X), LayoutUnit.FromFloatFloor(p.Y));
    }

    public static LayoutPoint CeiledLayoutPoint(in FloatPoint p)
    {
        return new LayoutPoint(LayoutUnit.FromFloatCeil(p.X), LayoutUnit.FromFloatCeil(p.Y));
    }

    public static IntSize PixelSnappedIntSize(in LayoutSize s, LayoutPoint p)
    {
        return new IntSize(LayoutUnit.SnapSizeToPixel(s.Width, p.X), LayoutUnit.SnapSizeToPixel(s.Height, p.Y));
    }

    public static LayoutPoint RoundedLayoutPoint(in FloatPoint p)
    {
        return new LayoutPoint(p);
    }

    public static IntSize RoundedIntSize(in LayoutPoint p)
    {
        return new IntSize(p.X.Round(), p.Y.Round());
    }

    public static LayoutSize ToLayoutSize(in LayoutPoint p)
    {
        return new LayoutSize(p.X, p.Y);
    }

    public static LayoutPoint FlooredLayoutPoint(in FloatSize s)
    {
        return FlooredLayoutPoint(new FloatPoint(s));
    }
}
