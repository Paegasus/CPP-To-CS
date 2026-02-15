namespace UI.GFX.Geometry;

using System.Runtime.CompilerServices;
using static Numerics.ClampedMath;

// A point has an x and y coordinate.
public struct Point : IComparable<Point>, IEquatable<Point>
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

    public readonly bool IsOrigin() => x_ == 0 && y_ == 0;

    public readonly Vector2D OffsetFromOrigin() => new(x_, y_);

    public void Transpose() => (x_, y_) = (y_, x_); // Swap x_ and y_ (using tuple deconstruction swap)

    public override readonly string ToString() => $"{x_},{y_}";

    // For use in collections (SortedSet, Dictionary keys, etc.)
    public override readonly int GetHashCode() => HashCode.Combine(y_, x_);

    public readonly int CompareTo(Point other)
    {
        int yComparison = y_.CompareTo(other.y_);
        return yComparison != 0 ? yComparison : x_.CompareTo(other.x_);
    }

    public override readonly bool Equals(object? obj) => obj is Point other && Equals(other);

    public readonly bool Equals(Point other) => x_ == other.x_ && y_ == other.y_;

    // A point is less than another point if its y-value is closer to the origin.
    // If the y-values are the same, then point with the x-value closer to the origin is considered less than the other.
    // This comparison is required to use Point in sets, or sorted vectors.
    public static bool operator < (in Point left, in Point right) => left.CompareTo(right) < 0;
    public static bool operator > (in Point left, in Point right) => left.CompareTo(right) > 0;
    public static bool operator <= (in Point left, in Point right) => left.CompareTo(right) <= 0;
    public static bool operator >= (in Point left, in Point right) => left.CompareTo(right) >= 0;

    public static bool operator == (in Point left, in Point right) => left.Equals(right);

    public static bool operator != (in Point left, in Point right) => !left.Equals(right);

    public void operator +=(in Vector2D vector)
    {
        x_ = ClampAdd(x_, vector.x);
        y_ = ClampAdd(y_, vector.y);
    }

    public void operator -=(in Vector2D vector)
    {
        x_ = ClampSub(x_, vector.x);
        y_ = ClampSub(y_, vector.y);
    }

    public static Point operator +(in Point lhs, in Vector2D rhs)
    {
        Point result = lhs;
        result += rhs;
        return result;
    }

    public static Point operator -(in Point lhs, in Vector2D rhs)
    {
        Point result = lhs;
        result -= rhs;
        return result;
    }

    public static Vector2D operator -(in Point lhs, in Point rhs)
    {
        return new Vector2D(ClampSub(lhs.x, rhs.x), ClampSub(lhs.y, rhs.y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point PointAtOffsetFromOrigin(in Vector2D offset_from_origin)
    {
        return new Point(offset_from_origin.x, offset_from_origin.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point TransposePoint(in Point p)
    {
        return new Point(p.y_, p.x_);
    }

    // Helper methods to scale a Point to a new Point.

    public static Point ScaleToCeiledPoint(in Point point, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return point;
        return PointConversions.ToCeiledPoint(PointF.ScalePoint(new PointF(point), x_scale, y_scale));
    }

    public static Point ScaleToCeiledPoint(in Point point, float scale)
    {
        if (scale == 1.0f)
            return point;
        return PointConversions.ToCeiledPoint(PointF.ScalePoint(new PointF(point), scale, scale));
    }

    public static Point ScaleToFlooredPoint(in Point point, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return point;
        return PointConversions.ToFlooredPoint(PointF.ScalePoint(new PointF(point), x_scale, y_scale));
    }

    public static Point ScaleToFlooredPoint(in Point point, float scale)
    {
        if (scale == 1.0f)
            return point;
        return PointConversions.ToFlooredPoint(PointF.ScalePoint(new PointF(point), scale, scale));
    }

    public static Point ScaleToRoundedPoint(in Point point, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return point;
        return PointConversions.ToRoundedPoint(PointF.ScalePoint(new PointF(point), x_scale, y_scale));
    }

    public static Point ScaleToRoundedPoint(in Point point, float scale)
    {
        if (scale == 1.0f)
            return point;
        return PointConversions.ToRoundedPoint(PointF.ScalePoint(new PointF(point), scale, scale));
    }
}
