using SkiaSharp;

namespace UI.Geometry;

public struct FloatRect
{
    private FloatPoint m_location;
    private FloatSize m_size;

    public readonly float x() { return m_location.X; }
    public readonly float y() { return m_location.Y; }
    public readonly float maxX() { return x() + width(); }
    public readonly float maxY() { return y() + height(); }
    public readonly float width() { return m_size.Width; }
    public readonly float height() { return m_size.Height; }
}
