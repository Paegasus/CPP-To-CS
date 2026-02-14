namespace UI.Extensions;

public static class FloatExtensions
{
    extension(float source)
    {
        /// <summary>
        /// Returns the "machine epsilon" for IEEE-754 float.
        /// Equivalent to std::numeric_limits<float>::epsilon()
        /// </summary>
        public static float MachineEpsilon => MathF.Pow(2, -23);
    }
}
