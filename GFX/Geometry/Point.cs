namespace UI.GFX.Geometry;

public struct Point
{
    private int m_X, m_Y;

    public int X { readonly get => m_X; set => m_X = value; }
    public int Y { readonly get => m_Y; set => m_Y = value; }
}
