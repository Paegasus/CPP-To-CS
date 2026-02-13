using System.Runtime.CompilerServices;

namespace UI.Geometry;

public struct LayoutRect
{
    private LayoutPoint m_Location;
    private LayoutSize m_Size;

    public readonly LayoutPoint Location => m_Location;
    public readonly LayoutSize Size => m_Size;

    public LayoutUnit X { readonly get => m_Location.X; set => m_Location.X = value; }
    public LayoutUnit Y { readonly get => m_Location.Y; set => m_Location.Y = value; }

    public LayoutUnit Width { readonly get => m_Size.Width; set => m_Size.Width = value; }
    public LayoutUnit Height { readonly get => m_Size.Height; set => m_Size.Height = value; }

    public readonly LayoutUnit MaxX => X + Width;
    public readonly LayoutUnit MaxY => Y + Height;
    
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
