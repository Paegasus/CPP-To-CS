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

    public readonly LayoutPoint Location => m_Location;
    public readonly LayoutSize Size => m_Size;

    public LayoutRect(in IntRect rect)
    {
        m_Location = rect.Location;
        m_Size = rect.Size;
    }

#if DEBUG
    public readonly void Show(bool showRawValue = false)
    {
        if (showRawValue)
            Console.WriteLine($"Rect (in raw layout units): [x={X.RawValue()} y={Y.RawValue()} maxX={MaxX.RawValue()} maxY={MaxY.RawValue()}]");
        else
            Console.WriteLine($"Rect (in pixels): [x={X.ToDouble()} y={Y.ToDouble()} maxX={MaxX.ToDouble()} maxY={MaxY.ToDouble()}]");
    }
#endif
}
