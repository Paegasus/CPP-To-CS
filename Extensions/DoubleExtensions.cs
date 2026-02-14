namespace UI.Extensions;

public static class DoubleExtensions
{
    extension(double source)
    {
        /// <summary>
        /// Returns the "machine epsilon" for IEEE-754 double.
        /// Equivalent to std::numeric_limits<double>::epsilon()
        /// </summary>
        public static double MachineEpsilon => Math.Pow(2, -52);
    }
}
