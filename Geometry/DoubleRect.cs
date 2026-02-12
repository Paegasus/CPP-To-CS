using SkiaSharp;

namespace UI.Geometry;

public struct DoubleRect
{
    private DoublePoint m_Location;
    private DoubleSize m_Size;

    public double X { readonly get => m_Location.X; set => m_Location.X = value; }
    public double Y { readonly get => m_Location.Y; set => m_Location.Y = value; }
    public readonly double MaxX => X + Width;
    public readonly double MaxY => Y + Height;
    public readonly double Width => m_Size.Width;
    public readonly double Height => m_Size.Height;

    public DoubleRect(in DoublePoint location, in DoubleSize size)
    {
        m_Location = location;
        m_Size = size;
    }

    public DoubleRect(double x, double y, double width, double height)
    {
        m_Location = new DoublePoint(x, y);
        m_Size = new DoubleSize(width, height);
    }
    
    public DoubleRect(in IntRect r)
    {
        m_Location = r.Location();
        m_Size = r.Size();
    }
    
    public DoubleRect(in FloatRect r)
    {
        m_Location = r.Location();
        m_Size = r.Size();
    }
    
    public DoubleRect(in LayoutRect r)
    {
        m_Location = r.Location();
        m_Size = r.Size();
    }
}
