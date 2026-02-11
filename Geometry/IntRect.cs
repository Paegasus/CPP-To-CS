using System.Runtime.CompilerServices;
using SkiaSharp;

namespace UI.Geometry;

public struct IntRect
{
    private IntPoint m_location;
    private IntSize m_size;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IntRect intersection(ref IntRect a, ref IntRect b) // C++: const IntRect& a, const IntRect& b
    {
        IntRect c = a;
        c.Intersect(ref b);
        return c;
    }

    private void Intersect(ref IntRect other)
    {
        throw new NotImplementedException();
    }
    
    private void Unite(ref IntRect other)
    {
        throw new NotImplementedException();
    }

    private void UniteIfNonZero(ref IntRect other)
    {
        throw new NotImplementedException();
    }
}
