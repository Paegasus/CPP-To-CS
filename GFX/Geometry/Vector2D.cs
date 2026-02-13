namespace UI.GFX.Geometry;

public struct Vector2D
{
    private int x_, y_;

    public int x { readonly get => x_; set => x_ = value; }
    public int y { readonly get => y_; set => y_ = value; }

    public Vector2D()
    {
        x_ = 0;
        y_ = 0;
    }
    public Vector2D(int x, int y)
    {
        x_ = x;
        y_ = y;
    }
}
