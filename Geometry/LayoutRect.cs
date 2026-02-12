using System.Runtime.CompilerServices;

namespace UI.Geometry;

public struct LayoutRect
{
    private LayoutPoint m_location;
    private LayoutSize m_size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly LayoutUnit x() { return m_location.X; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly LayoutUnit y() { return m_location.Y; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly LayoutUnit maxX() { return x() + width(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly LayoutUnit maxY() { return y() + height(); }
    public readonly LayoutUnit width() { return m_size.Width; }
    public readonly LayoutUnit height() { return m_size.Height; }

    public LayoutRect()
    {
        
    }
}
