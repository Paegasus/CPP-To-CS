using System.Runtime.CompilerServices;
using SkiaSharp;

using UI.Numerics;

namespace UI.Geometry;

public struct IntRect
{
    private IntPoint m_location;
    private IntSize m_size;
    
    public IntRect(int x, int y, int width, int height)
    {
        m_location = new IntPoint(x, y);
        m_size = new IntSize(width, height);
    }

    public IntRect(in IntPoint location, in IntSize size)
    {
        m_location = location;
        m_size = size;
    }
    
    public IntRect(in FloatRect r)
    {
        m_location = new IntPoint(MathExtras.ClampTo(r.x()), MathExtras.ClampTo(r.y()));
        m_size = new IntSize(MathExtras.ClampTo(r.width()), MathExtras.ClampTo(r.height()));
    }

    // Note: verify if we're doing this correctly
    public IntRect(in LayoutRect r)
    {
        m_location = new IntPoint(r.x().ToInteger(), r.y().ToInteger());
        m_size = new IntSize(r.width().ToInteger(), r.height().ToInteger());
    }

    public readonly IntPoint location() { return m_location; }
    public readonly IntSize size() { return m_size; }

    public void setLocation(in IntPoint location) { m_location = location; }
    public void setSize(in IntSize size) { m_size = size; }

    public readonly int x() { return m_location.X; }
    public readonly int y() { return m_location.Y; }
    public readonly int maxX() { return x() + width(); }
    public readonly int maxY() { return y() + height(); }
    public readonly int width() { return m_size.Width; }
    public readonly int height() { return m_size.Height; }

    public void setX(int x) { m_location.X = x; }
    public void setY(int y) { m_location.Y = y; }
    public void setWidth(int width) { m_size.Width = width; }
    public void setHeight(int height) { m_size.Height = height; }

    public readonly bool isEmpty() { return m_size.IsEmpty(); }

    // NOTE: The result is rounded to integer values, and thus may be not the exact center point.
    public readonly IntPoint center() { return new IntPoint(x() + width() / 2, y() + height() / 2); }

    public void move(in IntSize size) { m_location += size; }
    public void moveBy(in IntPoint offset) { m_location.Move(offset.X, offset.Y); }
    public void move(int dx, int dy) { m_location.Move(dx, dy); }

    public void expand(in IntSize size) { m_size += size; }
    public void expand(int dw, int dh) { m_size.Expand(dw, dh); }
    public void contract(in IntSize size) { m_size -= size; }
    public void contract(int dw, int dh) { m_size.Expand(-dw, -dh); }

    public void shiftXEdgeTo(int edge)
    {
        int delta = edge - x();
        setX(edge);
        setWidth(Math.Max(0, width() - delta));
    }

    public void shiftMaxXEdgeTo(int edge)
    {
        int delta = edge - maxX();
        setWidth(Math.Max(0, width() + delta));
    }

    public void shiftYEdgeTo(int edge)
    {
        int delta = edge - y();
        setY(edge);
        setHeight(Math.Max(0, height() - delta));
    }

    public void shiftMaxYEdgeTo(int edge)
    {
        int delta = edge - maxY();
        setHeight(Math.Max(0, height() + delta));
    }

    public readonly IntPoint minXMinYCorner() { return m_location; } // typically topLeft
    public readonly IntPoint maxXMinYCorner() { return new IntPoint(m_location.X + m_size.Width, m_location.Y); } // typically topRight
    public readonly IntPoint minXMaxYCorner() { return new IntPoint(m_location.X, m_location.Y + m_size.Height); } // typically bottomLeft
    public readonly IntPoint maxXMaxYCorner() { return new IntPoint(m_location.X + m_size.Width, m_location.Y + m_size.Height); } // typically bottomRight

    public readonly bool intersects(in IntRect other)
    {
        // Checking emptiness handles negative widths as well as zero.
        return !isEmpty() && !other.isEmpty()
            && x() < other.maxX() && other.x() < maxX()
            && y() < other.maxY() && other.y() < maxY();
    }

    public readonly bool contains(in IntRect other)
    {
        return x() <= other.x() && maxX() >= other.maxX()
        && y() <= other.y() && maxY() >= other.maxY();
    }

    // This checks to see if the rect contains x,y in the traditional sense.
    // Equivalent to checking if the rect contains a 1x1 rect below and to the right of (px,py).
    public readonly bool contains(int px, int py)
    {
        return px >= x() && px < maxX() && py >= y() && py < maxY();
    }

    public readonly bool contains(in IntPoint point)
    {
        return contains(point.X, point.Y);
    }

    public void intersect(in IntRect other)
    {
        int left = Math.Max(x(), other.x());
        int top = Math.Max(y(), other.y());
        int right = Math.Min(maxX(), other.maxX());
        int bottom = Math.Min(maxY(), other.maxY());

        // Return a clean empty rectangle for non-intersecting cases.
        if (left >= right || top >= bottom)
        {
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;
        }

        m_location.X = left;
        m_location.Y = top;
        m_size.Width = right - left;
        m_size.Height = bottom - top;
    }

    public void unite(in IntRect other)
    {
        // Handle empty special cases first.
        if (other.isEmpty()) return;
        
        if (isEmpty())
        {
            m_location = other.m_location;
            m_size = other.m_size;

            return;
        }

        uniteEvenIfEmpty(other);
    }

    public void uniteIfNonZero(in IntRect other)
    {
        // Handle empty special cases first.
        if (other.width() == 0 && other.height() == 0)
            return;
        
        if (width() == 0 && height() == 0)
        {
            m_location = other.m_location;
            m_size = other.m_size;

            return;
        }
        
        uniteEvenIfEmpty(other);
    }

    // Besides non-empty rects, this method also unites empty rects (as points or line segments).
    // For example, union of (100, 100, 0x0) and (200, 200, 50x0) is (100, 100, 150x100).
    public void uniteEvenIfEmpty(in IntRect other)
    {
        int left = Math.Min(x(), other.x());
        int top = Math.Min(y(), other.y());
        int right = Math.Max(maxX(), other.maxX());
        int bottom = Math.Max(maxY(), other.maxY());

        m_location.X = left;
        m_location.Y = top;
        m_size.Width = right - left;
        m_size.Height = bottom - top;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntRect intersection(in IntRect a, in IntRect b)
    {
        IntRect c = a;
        c.intersect(b);
        return c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntRect unionRect(in IntRect a, in IntRect b)
    {
        IntRect c = a;
        c.unite(b);
        return c;
    }

    public static IntRect unionRect(List<IntRect> rects) // (const Vector<IntRect>& rects)
    {
        IntRect result = new();

        var count = rects.Count;

        for (var i = 0; i < count; ++i)
        
            result.unite(rects[i]);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntRect unionRectEvenIfEmpty(in IntRect a, in IntRect b)
    {
        IntRect c = a;
        c.uniteEvenIfEmpty(b);
        return c;
    }

    IntRect unionRectEvenIfEmpty(List<IntRect> rects) // (const Vector<IntRect>& rects)
    {
        var count = rects.Count;

        if (count == 0) return new IntRect();

        IntRect result = rects[0];

        for (var i = 1; i < count; ++i)

            result.uniteEvenIfEmpty(rects[i]);

        return result;
    }

    public void inflateX(int dx)
    {
        m_location.X = m_location.X - dx;
        m_size.Width = m_size.Width + dx + dx;
    }

    public void inflateY(int dy)
    {
        m_location.Y = m_location.Y - dy;
        m_size.Height = m_size.Height + dy + dy;
    }

    public void inflate(int d)
    {
        inflateX(d); inflateY(d);
    }

    public void scale(float s)
    {
        m_location.X = (int)(x() * s);
        m_location.Y = (int)(y() * s);
        m_size.Width = (int)(width() * s);
        m_size.Height = (int)(height() * s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int distanceToInterval(int pos, int start, int end)
    {
        if (pos < start)
            return start - pos;
        if (pos > end)
            return end - pos;
        return 0;
    }

    public readonly IntSize differenceToPoint(in IntPoint point)
    {
        int xdistance = distanceToInterval(point.X, x(), maxX());
        int ydistance = distanceToInterval(point.Y, y(), maxY());

        return new IntSize(xdistance, ydistance);
    }

    public readonly int distanceSquaredToPoint(in IntPoint p)
    {
        return differenceToPoint(p).DiagonalLengthSquared();
    }

    public readonly IntRect transposedRect()
    {
        return new IntRect(m_location.TransposedPoint(), m_size.TransposedSize());
    }

    // don't do this implicitly since it's lossy
    public static explicit operator IntRect(in FloatRect rect)
    {
        return new IntRect(rect);
    }

    // don't do this implicitly since it's lossy
    public static explicit operator IntRect(in LayoutRect rect)
    {
        return new IntRect(rect);
    }

    public static implicit operator SKRect(in IntRect rect)
    {
        return new SKRect(rect.x(), rect.y(), rect.maxX(), rect.maxY());
    }

    public static implicit operator SKRectI(in IntRect rect)
    {
        return new SKRectI(rect.x(), rect.y(), rect.maxX(), rect.maxY());
    }

    public static bool operator ==(in IntRect a, in IntRect b)
    {
        return a.m_location == b.m_location && a.m_size == b.m_size;
    }

    public static bool operator !=(in IntRect a, in IntRect b)
    {
        return !(a == b);
    }

    public override readonly bool Equals(object? obj) => obj is IntRect other && this == other;

    public readonly bool Equals(in IntRect other) => this == other;

    public override readonly int GetHashCode() => HashCode.Combine(m_location, m_size);

#if DEBUG
    // Prints the rect to the screen.
    public readonly void Show()
    {
        //new LayoutRect(this).show();
        throw new NotImplementedException();
    }
#endif
}
