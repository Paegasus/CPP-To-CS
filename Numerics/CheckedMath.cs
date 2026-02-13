using System.Numerics;

namespace UI.Numerics;

public static class CheckedMath
{
    public static bool CheckedAdd<T>(T x, T y, out T result)
    where T : IBinaryInteger<T>, IMinMaxValue<T>
    {
        T sum = x + y;

        if (T.IsNegative(T.MinValue)) // signed types have negative MinValue
        {
            // Signed overflow: ((sum ^ x) & (sum ^ y)) < 0
            if (((sum ^ x) & (sum ^ y)) < T.Zero)
            {
                result = T.Zero;
                return false;
            }
        }
        else
        {
            // Unsigned overflow: wraparound makes sum smaller than an operand.
            if (sum < x)
            {
                result = T.Zero;
                return false;
            }
        }

        result = sum;
        return true;
    }

    public static bool CheckedAdd_Signed<T>(T x, T y, out T result)
    where T : IBinaryInteger<T>, ISignedNumber<T>
    {
        T sum = x + y;

        // Overflow if x and y have same sign, but sum has different sign.
        // ((sum ^ x) & (sum ^ y)) < 0
        if (((sum ^ x) & (sum ^ y)) < T.Zero)
        {
            result = T.Zero;
            return false;
        }

        result = sum;
        return true;
    }

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

    /// <summary>
    /// Adds two integers and returns a value indicating whether an overflow occurred.
    /// </summary>
    /// <param name="x">The first integer to add.</param>
    /// <param name="y">The second integer to add.</param>
    /// <param name="result">
    /// When this method returns, contains the sum of the two integers if the operation
    /// succeeded, or a default value (0) if an overflow occurred.
    /// </param>
    /// <returns>
    /// <c>true</c> if the sum is within the range of the Int32 type;
    /// otherwise, <c>false</c>.
    /// </returns>
    /*
    public static bool CheckedAdd(int x, int y, out int result)
    {
        // Use 'long' to perform the addition, as it can hold any result
        // from adding two 'int's without overflowing.
        long tempResult = (long)x + y;

        // Check if the long result fits back into an int.
        if (tempResult > int.MaxValue || tempResult < int.MinValue)
        {
            // Overflow occurred.
            result = 0;
            return false; // Signal failure.
        }

        // The result is within the valid range for an int.
        result = (int)tempResult;
        return true; // Signal success.
    }
    */
}
