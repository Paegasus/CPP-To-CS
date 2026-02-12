using SkiaSharp;

namespace UI.Geometry;

enum ContainsMode
{
    InsideOrOnStroke,
    InsideButNotOnStroke
}

public struct FloatRect
{
    private FloatPoint m_Location;
    private FloatSize m_Size;

    public float X { readonly get => m_Location.X; set => m_Location.X = value; }
    public float Y { readonly get => m_Location.Y; set => m_Location.Y = value; }
    public readonly float MaxX => X + Width;
    public readonly float MaxY => Y + Height;
    public readonly float Width => m_Size.Width;
    public readonly float Height => m_Size.Height;

    public readonly FloatPoint Location => m_Location;
    public readonly FloatSize Size => m_Size;
}
