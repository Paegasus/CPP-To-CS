namespace UI.Numerics
{
    public static class MathExtras
    {
        public static int ClampTo(float value)
        {
            if (value >= int.MaxValue)
                return int.MaxValue;
            if (value <= int.MinValue)
                return int.MinValue;
            return (int)value;
        }

        public static int ClampTo(double value)
        {
            if (value >= int.MaxValue)
                return int.MaxValue;
            if (value <= int.MinValue)
                return int.MinValue;
            return (int)value;
        }
    }
}
