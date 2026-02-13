namespace UI.Numerics;

public static class ClampedMath
{
    public static int ClampAdd(int x, int y)
    {
        // Use 'long' to perform the addition, as it can hold any result
        // from adding two 'int's without overflowing.
        long result = (long)x + y;

        // Now, check if the long result fits back into an int.
        if (result > int.MaxValue)
        {
            return int.MaxValue;
        }
        if (result < int.MinValue)
        {
            return int.MinValue;
        }

        // The result is within the valid range for an int.
        return (int)result;
    }
}
