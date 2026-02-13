namespace UI.GFX.Geometry;

// A point has an x and y coordinate.
public struct Point
{
    private int x_, y_; // Rename to m_X and m_Y

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

    void Offset(int delta_x, int delta_y)
    {
        //x_ = base::ClampAdd(x_, delta_x);
        //y_ = base::ClampAdd(y_, delta_y);
    }
}
