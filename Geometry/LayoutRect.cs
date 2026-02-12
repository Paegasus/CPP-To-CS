using System.Runtime.CompilerServices;

namespace UI.Geometry;

public struct LayoutRect
{
    private LayoutPoint m_Location;
    private LayoutSize m_Size;

    public LayoutUnit X { readonly get => m_Location.X; set => m_Location.X = value; }
    public LayoutUnit Y { readonly get => m_Location.Y; set => m_Location.Y = value; }
    public readonly LayoutUnit MaxX => X + Width;
    public readonly LayoutUnit MaxY => Y + Height;
    public readonly LayoutUnit Width => m_Size.Width;
    public readonly LayoutUnit Height => m_Size.Height;

    public readonly LayoutPoint Location() { return m_Location; }
    public readonly LayoutSize Size() { return m_Size; }
}
