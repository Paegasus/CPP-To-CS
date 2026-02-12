using System.Runtime.CompilerServices;
using SkiaSharp;

using UI.Numerics;

namespace UI.Geometry;

public struct IntRect
{
    private IntPoint m_Location;
    private IntSize m_Size;
    
    public IntRect(int x, int y, int width, int height)
    {
        m_Location = new IntPoint(x, y);
        m_Size = new IntSize(width, height);
    }

    public IntRect(in IntPoint location, in IntSize size)
    {
        m_Location = location;
        m_Size = size;
    }
    
    public IntRect(in FloatRect rect)
    {
        m_Location = new IntPoint(MathExtras.ClampTo(rect.x()), MathExtras.ClampTo(rect.y()));
        m_Size = new IntSize(MathExtras.ClampTo(rect.width()), MathExtras.ClampTo(rect.height()));
    }

    // Note: verify if we're doing this correctly
    public IntRect(in LayoutRect rect)
    {
        m_Location = new IntPoint(rect.x().ToInteger(), rect.y().ToInteger());
        m_Size = new IntSize(rect.width().ToInteger(), rect.height().ToInteger());
    }

    public readonly IntPoint Location() { return m_Location; }
    public readonly IntSize Size() { return m_Size; }

    public void SetLocation(in IntPoint location) { m_Location = location; }
    public void SetSize(in IntSize size) { m_Size = size; }
    
    public int X { readonly get => m_Location.X; set => m_Location.X = value; }
    public int Y { readonly get => m_Location.Y; set => m_Location.Y = value; }
    public readonly int MaxX => X + Width;
    public readonly int MaxY => Y + Height;
    public readonly int Width => m_Size.Width;
    public readonly int Height => m_Size.Height;

    public void SetWidth(int width) { m_Size.Width = width; }
    public void SetHeight(int height) { m_Size.Height = height; }

    public readonly bool IsEmpty() { return m_Size.IsEmpty(); }

    // NOTE: The result is rounded to integer values, and thus may be not the exact center point.
    public readonly IntPoint Center() { return new IntPoint(X + Width / 2, Y + Height / 2); }

    public void Move(in IntSize size) { m_Location += size; }
    public void MoveBy(in IntPoint offset) { m_Location.Move(offset.X, offset.Y); }
    public void Move(int dx, int dy) { m_Location.Move(dx, dy); }

    public void Expand(in IntSize size) { m_Size += size; }
    public void Expand(int dw, int dh) { m_Size.Expand(dw, dh); }
    public void Contract(in IntSize size) { m_Size -= size; }
    public void Contract(int dw, int dh) { m_Size.Expand(-dw, -dh); }

    public void ShiftXEdgeTo(int edge)
    {
        int delta = edge - X;
        X = edge;
        SetWidth(Math.Max(0, Width - delta));
    }

    public void ShiftMaxXEdgeTo(int edge)
    {
        int delta = edge - MaxX;
        SetWidth(Math.Max(0, Width + delta));
    }

    public void ShiftYEdgeTo(int edge)
    {
        int delta = edge - Y;
        Y = edge;
        SetHeight(Math.Max(0, Height - delta));
    }

    public void ShiftMaxYEdgeTo(int edge)
    {
        int delta = edge - MaxY;
        SetHeight(Math.Max(0, Height + delta));
    }

    public readonly IntPoint MinXMinYCorner() { return m_Location; } // typically topLeft
    public readonly IntPoint MaxXMinYCorner() { return new IntPoint(m_Location.X + m_Size.Width, m_Location.Y); } // typically topRight
    public readonly IntPoint MinXMaxYCorner() { return new IntPoint(m_Location.X, m_Location.Y + m_Size.Height); } // typically bottomLeft
    public readonly IntPoint MaxXMaxYCorner() { return new IntPoint(m_Location.X + m_Size.Width, m_Location.Y + m_Size.Height); } // typically bottomRight

    public readonly bool Intersects(in IntRect other)
    {
        // Checking emptiness handles negative widths as well as zero.
        return !IsEmpty() && !other.IsEmpty()
            && X < other.MaxX && other.X < MaxX
            && Y < other.MaxY && other.Y < MaxY;
    }

    public readonly bool Contains(in IntRect other)
    {
        return X <= other.X && MaxX >= other.MaxX
            && Y <= other.Y && MaxY >= other.MaxY;
    }

    // This checks to see if the rect contains x,y in the traditional sense.
    // Equivalent to checking if the rect contains a 1x1 rect below and to the right of (px,py).
    public readonly bool Contains(int px, int py)
    {
        return px >= X && px < MaxX && py >= Y && py < MaxY;
    }

    public readonly bool Contains(in IntPoint point)
    {
        return Contains(point.X, point.Y);
    }

    public void Intersect(in IntRect other)
    {
        int left = Math.Max(X, other.X);
        int top = Math.Max(Y, other.Y);
        int right = Math.Min(MaxX, other.MaxX);
        int bottom = Math.Min(MaxY, other.MaxY);

        // Return a clean empty rectangle for non-intersecting cases.
        if (left >= right || top >= bottom)
        {
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;
        }

        m_Location.X = left;
        m_Location.Y = top;
        m_Size.Width = right - left;
        m_Size.Height = bottom - top;
    }

    public void Unite(in IntRect other)
    {
        // Handle empty special cases first.
        if (other.IsEmpty()) return;
        
        if (IsEmpty())
        {
            m_Location = other.m_Location;
            m_Size = other.m_Size;

            return;
        }

        UniteEvenIfEmpty(other);
    }

    public void UniteIfNonZero(in IntRect other)
    {
        // Handle empty special cases first.
        if (other.Width == 0 && other.Height == 0) return;
        
        if (Width == 0 && Height == 0)
        {
            m_Location = other.m_Location;
            m_Size = other.m_Size;

            return;
        }
        
        UniteEvenIfEmpty(other);
    }

    // Besides non-empty rects, this method also unites empty rects (as points or line segments).
    // For example, union of (100, 100, 0x0) and (200, 200, 50x0) is (100, 100, 150x100).
    public void UniteEvenIfEmpty(in IntRect other)
    {
        int left = Math.Min(X, other.X);
        int top = Math.Min(Y, other.Y);
        int right = Math.Max(MaxX, other.MaxX);
        int bottom = Math.Max(MaxY, other.MaxY);

        m_Location.X = left;
        m_Location.Y = top;
        m_Size.Width = right - left;
        m_Size.Height = bottom - top;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntRect Intersection(in IntRect a, in IntRect b)
    {
        IntRect c = a;
        c.Intersect(b);
        return c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntRect UnionRect(in IntRect a, in IntRect b)
    {
        IntRect c = a;
        c.Unite(b);
        return c;
    }

    public static IntRect UnionRect(ReadOnlySpan<IntRect> rects)
    {
        IntRect result = new();

        for (var i = 0; i < rects.Length; ++i)
        
            result.Unite(rects[i]);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntRect UnionRectEvenIfEmpty(in IntRect a, in IntRect b)
    {
        IntRect c = a;
        c.UniteEvenIfEmpty(b);
        return c;
    }

    public static IntRect UnionRectEvenIfEmpty(ReadOnlySpan<IntRect> rects)
    {
        if (rects.Length == 0) return new IntRect();

        IntRect result = rects[0];

        for (var i = 1; i < rects.Length; ++i)

            result.UniteEvenIfEmpty(rects[i]);

        return result;
    }

    public void InflateX(int dx)
    {
        m_Location.X -= dx;
        m_Size.Width = m_Size.Width + dx + dx;
    }

    public void InflateY(int dy)
    {
        m_Location.Y -= dy;
        m_Size.Height = m_Size.Height + dy + dy;
    }

    public void Inflate(int d)
    {
        InflateX(d); InflateY(d);
    }

    public void Scale(float s)
    {
        m_Location.X = (int)(X * s);
        m_Location.Y = (int)(Y * s);
        m_Size.Width = (int)(Width * s);
        m_Size.Height = (int)(Height * s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int DistanceToInterval(int pos, int start, int end)
    {
        if (pos < start)
            return start - pos;

        if (pos > end)
            return end - pos;
        
        return 0;
    }

    public readonly IntSize DifferenceToPoint(in IntPoint point)
    {
        int xDistance = DistanceToInterval(point.X, X, MaxX);
        int yDistance = DistanceToInterval(point.Y, Y, MaxY);

        return new IntSize(xDistance, yDistance);
    }

    public readonly int DistanceSquaredToPoint(in IntPoint p)
    {
        return DifferenceToPoint(p).DiagonalLengthSquared();
    }

    public readonly IntRect TransposedRect()
    {
        return new IntRect(m_Location.TransposedPoint(), m_Size.TransposedSize());
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
        return new SKRect(rect.X, rect.Y, rect.MaxX, rect.MaxY);
    }

    public static implicit operator SKRectI(in IntRect rect)
    {
        return new SKRectI(rect.X, rect.Y, rect.MaxX, rect.MaxY);
    }

    public static bool operator ==(in IntRect a, in IntRect b)
    {
        return a.m_Location == b.m_Location && a.m_Size == b.m_Size;
    }

    public static bool operator !=(in IntRect a, in IntRect b)
    {
        return !(a == b);
    }

    public override readonly bool Equals(object? obj) => obj is IntRect other && this == other;

    public readonly bool Equals(in IntRect other) => this == other;

    public override readonly int GetHashCode() => HashCode.Combine(m_Location, m_Size);

#if DEBUG
    // Prints the rect to the screen.
    public readonly void Show()
    {
        //new LayoutRect(this).show();
        throw new NotImplementedException();
    }
#endif
}
