namespace UI.Geometry;

public struct IntPoint
{
    private int m_x, m_y;

    public IntPoint()
    {
        m_x = 0;
        m_y = 0;
    }

    public IntPoint(int x, int y)
    {
        m_x = x;
        m_y = y;
    }

    public IntPoint(IntSize size)
    {
        m_x = size.Width;
        m_y = size.Height;
    }

    public static IntPoint Zero() { return new IntPoint(); }

    public int X { get { return m_x; } set { m_x = value; } }
    public int Y { get { return m_y; } set { m_y = value; } }

    public void Move(IntSize s)
    {
        m_x += s.Width;
        m_y += s.Height;
    }

    public void MoveBy(IntPoint offset)
    {
        m_x += offset.X;
        m_y += offset.Y;
    }

    public void Move(int dx, int dy)
    {
        m_x += dx;
        m_y += dy;
    }

    public void Scale(float sx, float sy)
    {
        m_x = (int)Math.Round(m_x * sx);
        m_y = (int)Math.Round(m_y * sy);
    }

    public IntPoint ExpandedTo(IntPoint other)
    {
        return new IntPoint(Math.Max(m_x, other.m_x), Math.Max(m_y, other.m_y));
    }

    public IntPoint ShrunkTo(IntPoint other)
    {
        return new IntPoint(Math.Min(m_x, other.m_x), Math.Min(m_y, other.m_y));
    }

    public int DistanceSquaredToPoint(IntPoint other)
    {
        return (this - other).DiagonalLengthSquared();
    }

    public void ClampNegativeToZero()
    {
        this = ExpandedTo(Zero());
    }

    public IntPoint TransposedPoint()
    {
        return new IntPoint(m_y, m_x);
    }

    public override bool Equals(object obj)
    {
        return obj is IntPoint && this == (IntPoint)obj;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static IntPoint operator +(IntPoint a, IntSize b)
    {
        return new IntPoint(a.X + b.Width, a.Y + b.Height);
    }

    public static IntPoint operator +(IntPoint a, IntPoint b)
    {
        return new IntPoint(a.X + b.X, a.Y + b.Y);
    }

    public static IntSize operator -(IntPoint a, IntPoint b)
    {
        return new IntSize(a.X - b.X, a.Y - b.Y);
    }

    public static IntPoint operator -(IntPoint a, IntSize b)
    {
        return new IntPoint(a.X - b.Width, a.Y - b.Height);
    }

    public static IntPoint operator -(IntPoint point)
    {
        return new IntPoint(-point.X, -point.Y);
    }

    public static bool operator ==(IntPoint a, IntPoint b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(IntPoint a, IntPoint b)
    {
        return !(a == b);
    }

    public static IntSize ToIntSize(IntPoint a)
    {
        return new IntSize(a.X, a.Y);
    }
}
