namespace UI.Geometry;

public struct IntPoint
{
    private int m_X, m_Y;

    public int X { readonly get => m_X; set => m_X = value; }
    public int Y { readonly get => m_Y; set => m_Y = value; }

    public IntPoint()
    {
        m_X = 0;
        m_Y = 0;
    }

    public IntPoint(int x, int y)
    {
        m_X = x;
        m_Y = y;
    }

    public IntPoint(in IntSize size)
    {
        m_X = size.Width;
        m_Y = size.Height;
    }

    public static IntPoint Zero() { return new IntPoint(); }

    public void Move(in IntSize s)
    {
        m_X += s.Width;
        m_Y += s.Height;
    }

    public void MoveBy(in IntPoint offset)
    {
        m_X += offset.X;
        m_Y += offset.Y;
    }

    public void Move(int dx, int dy)
    {
        m_X += dx;
        m_Y += dy;
    }

    public void Scale(float sx, float sy)
    {
        m_X = (int)Math.Round(m_X * sx);
        m_Y = (int)Math.Round(m_Y * sy);
    }

    public readonly IntPoint ExpandedTo(in IntPoint other)
    {
        return new IntPoint(Math.Max(m_X, other.m_X), Math.Max(m_Y, other.m_Y));
    }

    public readonly IntPoint ShrunkTo(in IntPoint other)
    {
        return new IntPoint(Math.Min(m_X, other.m_X), Math.Min(m_Y, other.m_Y));
    }

    public readonly int DistanceSquaredToPoint(in IntPoint other)
    {
        return (this - other).DiagonalLengthSquared();
    }

    public void ClampNegativeToZero()
    {
        this = ExpandedTo(Zero());
    }

    public readonly IntPoint TransposedPoint()
    {
        return new IntPoint(m_Y, m_X);
    }
    
    public static IntSize ToIntSize(in IntPoint a)
    {
        return new IntSize(a.X, a.Y);
    }

    public override readonly bool Equals(object? obj) => obj is IntPoint point && this == point;

    public override readonly int GetHashCode()
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
}
