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
}
