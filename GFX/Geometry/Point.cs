namespace UI.GFX.Geometry;

using System.Numerics;
using static Numerics.ClampedMath;

// A point has an x and y coordinate.
public struct Point
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
}
