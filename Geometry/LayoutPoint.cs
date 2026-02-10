namespace UI.Geometry;

public struct LayoutPoint
{
    private LayoutUnit m_x, m_y;

    public LayoutPoint()
    {
        m_x = new LayoutUnit();
        m_y = new LayoutUnit();
    }

    public LayoutPoint(LayoutUnit x, LayoutUnit y)
    {
        m_x = x;
        m_y = y;
    }

    public LayoutPoint(IntPoint point)
    {
        m_x = new LayoutUnit(point.X);
        m_y = new LayoutUnit(point.Y);
    }

    public LayoutPoint(FloatPoint point)
    {
        m_x = new LayoutUnit(point.X);
        m_y = new LayoutUnit(point.Y);
    }

    public LayoutPoint(DoublePoint point)
    {
        m_x = new LayoutUnit(point.X);
        m_y = new LayoutUnit(point.Y);
    }

    public LayoutPoint(LayoutSize size)
    {
        m_x = size.Width;
        m_y = size.Height;
    }

    public static LayoutPoint Zero() { return new LayoutPoint(); }

    public LayoutUnit X { get { return m_x; } set { m_x = value; } }
    public LayoutUnit Y { get { return m_y; } set { m_y = value; } }

    public void Move(LayoutSize s)
    {
        m_x += s.Width;
        m_y += s.Height;
    }

    public void Move(IntSize s)
    {
        m_x += s.Width;
        m_y += s.Height;
    }

    public void MoveBy(LayoutPoint offset)
    {
        m_x += offset.X;
        m_y += offset.Y;
    }

    public void Move(LayoutUnit dx, LayoutUnit dy)
    {
        m_x += dx;
        m_y += dy;
    }

    public void Scale(float sx, float sy)
    {
        m_x *= sx;
        m_y *= sy;
    }

    public LayoutPoint ExpandedTo(LayoutPoint other)
    {
        return new LayoutPoint(Math.Max(m_x, other.m_x), Math.Max(m_y, other.m_y));
    }

    public LayoutPoint ShrunkTo(LayoutPoint other)
    {
        return new LayoutPoint(Math.Min(m_x, other.m_x), Math.Min(m_y, other.m_y));
    }

    public void ClampNegativeToZero()
    {
        this = ExpandedTo(Zero());
    }

    public LayoutPoint TransposedPoint()
    {
        return new LayoutPoint(m_y, m_x);
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

    public override bool Equals(object obj)
    {
        return obj is LayoutPoint && this == (LayoutPoint)obj;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static LayoutPoint ToPoint(LayoutSize size)
    {
        return new LayoutPoint(size.Width, size.Height);
    }

    public static LayoutPoint ToLayoutPoint(LayoutSize p)
    {
        return new LayoutPoint(p.Width, p.Height);
    }

    public static LayoutSize ToSize(LayoutPoint a)
    {
        return new LayoutSize(a.X, a.Y);
    }

    public static IntPoint FlooredIntPoint(LayoutPoint point)
    {
        return new IntPoint(point.X.Floor(), point.Y.Floor());
    }

    public static IntPoint RoundedIntPoint(LayoutPoint point)
    {
        return new IntPoint(point.X.Round(), point.Y.Round());
    }

    public static IntPoint CeiledIntPoint(LayoutPoint point)
    {
        return new IntPoint(point.X.Ceil(), point.Y.Ceil());
    }

    public static LayoutPoint FlooredLayoutPoint(FloatPoint p)
    {
        return new LayoutPoint(LayoutUnit.FromFloatFloor(p.X), LayoutUnit.FromFloatFloor(p.Y));
    }

    public static LayoutPoint CeiledLayoutPoint(FloatPoint p)
    {
        return new LayoutPoint(LayoutUnit.FromFloatCeil(p.X), LayoutUnit.FromFloatCeil(p.Y));
    }

    public static IntSize PixelSnappedIntSize(LayoutSize s, LayoutPoint p)
    {
        return new IntSize(LayoutUnit.SnapSizeToPixel(s.Width, p.X), LayoutUnit.SnapSizeToPixel(s.Height, p.Y));
    }

    public static LayoutPoint RoundedLayoutPoint(FloatPoint p)
    {
        return new LayoutPoint(p);
    }

    public static IntSize RoundedIntSize(LayoutPoint p)
    {
        return new IntSize(p.X.Round(), p.Y.Round());
    }

    public static LayoutSize ToLayoutSize(LayoutPoint p)
    {
        return new LayoutSize(p.X, p.Y);
    }

    public static LayoutPoint FlooredLayoutPoint(FloatSize s)
    {
        return FlooredLayoutPoint(new FloatPoint(s));
    }
}
