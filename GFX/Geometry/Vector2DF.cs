namespace UI.GFX.Geometry;

public struct Vector2DF
{
    private float x_, y_;

    public float x { readonly get => x_; set => x_ = value; }
    public float y { readonly get => y_; set => y_ = value; }

    public Vector2DF()
    {
        x_ = 0;
        y_ = 0;
    }
    public Vector2DF(float x, float y)
    {
        x_ = x;
        y_ = y;
    }
}
