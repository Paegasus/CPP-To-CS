namespace UI.GFX.Geometry;

using static Numerics.ClampedMath;

// A point has an x and y coordinate.
public struct Point : IComparable<Point>
{
    private int x_, y_;

    public int x { readonly get => x_; set => x_ = value; }
    public int y { readonly get => y_; set => y_ = value; }

    public Point()
    {
        x_ = 0;
        y_ = 0;
    }
    
    public Point(int x, int y)
    {
        x_ = x;
        y_ = y;
    }

    public void SetPoint(int x, int y)
    {
        x_ = x;
        y_ = y;
    }

    public void Offset(int delta_x, int delta_y)
    {
        x_ = ClampAdd(x_, delta_x);
        y_ = ClampAdd(y_, delta_y);
    }

    public static Point operator + (Point point, in  Vector2D vector)
    {
        return new Point
        {
            x_ = ClampAdd(point.x_, vector.x),
            y_ = ClampAdd(point.y_, vector.y)
        };
    }

    public static Point operator - (Point point, in  Vector2D vector)
    {
        return new Point
        {
            x_ = ClampSub(point.x_, vector.x),
            y_ = ClampSub(point.y_, vector.y)
        };
    }

    public void SetToMin(in Point other)
    {
        x_ = Math.Min(x_, other.x_);
        y_ = Math.Min(y_, other.y_);
    }

    public void SetToMax(in Point other)
    {
        x_ = Math.Max(x_, other.x_);
        y_ = Math.Max(y_, other.y_);
    }

    public readonly bool IsOrigin()
    {
        return x_ == 0 && y_ == 0;
    }

    public readonly Vector2D OffsetFromOrigin()
    {
        return new Vector2D(x_, y_);
    }

    public void Transpose()
    {
        // Swap x_ and y_ (using tuple deconstruction swap)
        (x_, y_) = (y_, x_);
    }

    public override readonly string ToString()
    {
        return $"{x_},{y_}";
    }

    // For use in collections (SortedSet, Dictionary keys, etc.)
    public override readonly int GetHashCode() => HashCode.Combine(y_, x_);

    public readonly int CompareTo(Point other)
    {
        int yComparison = y_.CompareTo(other.y_);
        return yComparison != 0 ? yComparison : x_.CompareTo(other.x_);
    }

    public override readonly bool Equals(object? obj) => obj is Point other && CompareTo(other) == 0;
    
    // A point is less than another point if its y-value is closer to the origin.
    // If the y-values are the same, then point with the x-value closer to the origin is considered less than the other.
    // This comparison is required to use Point in sets, or sorted vectors.
    public static bool operator <(Point left, Point right) => left.CompareTo(right) < 0;
    public static bool operator >(Point left, Point right) => left.CompareTo(right) > 0;
    public static bool operator <=(Point left, Point right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Point left, Point right) => left.CompareTo(right) >= 0;
}
