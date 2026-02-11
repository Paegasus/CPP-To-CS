using System.Runtime.CompilerServices;
using SkiaSharp;

namespace UI.Geometry;

public struct IntRect
{
    private IntPoint m_location;
    private IntSize m_size;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly IntRect Intersection(in IntRect a, in IntRect b)
    {
        IntRect c = a;
        c.Intersect(in b);
        return c;
    }

    private void Intersect(in IntRect other)
    {
        throw new NotImplementedException();
    }
    
    private void Unite(in IntRect other)
    {
        throw new NotImplementedException();
    }

    private void UniteIfNonZero(in IntRect other)
    {
        throw new NotImplementedException();
    }
}
