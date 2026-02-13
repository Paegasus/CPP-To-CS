namespace UI.GFX.Geometry;

using static Numerics.ClampedMath;

public struct PointF
{
    private float x_, y_;

    public float x { readonly get => x_; set => x_ = value; }
    public float y { readonly get => y_; set => y_ = value; }

    public PointF()
    {
        x_ = 0.0f;
        y_ = 0.0f;
    }
    
    public PointF(float x, float y)
    {
        x_ = x;
        y_ = y;
    }

    public PointF(in Point p)
    {
        x_ = (float)p.x;
        y_ = (float)p.y;
    }

    public void SetPoint(float x, float y)
    {
        x_ = x;
        y_ = y;
    }

    public void Offset(float delta_x, float delta_y)
    {
        x_ += delta_x;
        y_ += delta_y;
    }

    public static PointF operator + (PointF point, in Vector2DF vector)
    {
        return new PointF
        {
            x_ = point.x_ += vector.x,
            y_ = point.y_ += vector.y,
        };
    }
    
    public static PointF operator - (PointF point, in Vector2DF vector)
    {
        return new PointF
        {
            x_ = point.x_ -= vector.x,
            y_ = point.y_ -= vector.y,
        };
    }

    public void SetToMin(in PointF other)
    {
        x_ = Math.Min(x_, other.x_);
        y_ = Math.Min(y_, other.y_);
    }

    public void SetToMax(in PointF other)
    {
        x_ = Math.Max(x_, other.x_);
        y_ = Math.Max(y_, other.y_);
    }
}
