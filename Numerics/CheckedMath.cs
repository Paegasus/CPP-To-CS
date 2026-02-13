using System.Numerics;
using System.Runtime.CompilerServices;

namespace UI.Numerics;

public static class CheckedMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckedAdd(int x, int y, out T result)
    {
        int sum = x + y;

        // Overflow if x and y have same sign, but sum has different sign.
        if (((sum ^ x) & (sum ^ y)) < 0)
        {
            result = 0;
            return false;
        }

        result = sum;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckedAdd(uint x, uint y, out T result)
    {
        uint sum = x + y;

        // Unsigned overflow if wrapping happened.
        if (sum < x)
        {
            result = 0;
            return false;
        }

        result = sum;
        return true;
    }
}
