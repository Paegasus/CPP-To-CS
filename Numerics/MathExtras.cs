using System.Numerics;

namespace UI.Numerics;

public static class MathExtras
{
    public static T ClampTo<T>(float value)
        where T : unmanaged, INumber<T>, IMinMaxValue<T>
    {
        var max = double.CreateChecked(T.MaxValue);
        var min = double.CreateChecked(T.MinValue);

        if (value >= max) return T.MaxValue;
        if (value <= min) return T.MinValue;

        return T.CreateChecked(value);
    }

    public static T ClampTo<T>(double value)
        where T : unmanaged, INumber<T>, IMinMaxValue<T>
    {
        var max = double.CreateChecked(T.MaxValue);
        var min = double.CreateChecked(T.MinValue);

        if (value >= max) return T.MaxValue;
        if (value <= min) return T.MinValue;

        return T.CreateChecked(value);
    }
}
