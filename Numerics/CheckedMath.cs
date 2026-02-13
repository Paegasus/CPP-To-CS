using System.Numerics;
using System.Runtime.CompilerServices;

namespace UI.Numerics;

public static class CheckedMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckedAdd_Signed<T>(T x, T y, out T result)
    where T : IBinaryInteger<T>, ISignedNumber<T>
    {
        T sum = x + y;

        // Overflow if x and y have same sign, but sum has different sign. ((sum ^ x) & (sum ^ y)) < 0
        if (((sum ^ x) & (sum ^ y)) < T.Zero)
        {
            result = T.Zero;
            return false;
        }

        result = sum;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckedAdd_Unsigned<T>(T x, T y, out T result)
    where T : IBinaryInteger<T>, IUnsignedNumber<T>
    {
        T sum = x + y;

        // Unsigned overflow if wrapping happened.
        if (sum < x)
        {
            result = T.Zero;
            return false;
        }

        result = sum;
        return true;
    }
}
