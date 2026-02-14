using static UI.Numerics.SafeConversions;

namespace UI.GFX.Geometry;

public static class SizeConversions
{
    // Returns a Size with each component from the input SizeF floored.
    public static Size ToFlooredSize(in SizeF size)
    {
        return new Size(ClampFloor(size.width), ClampFloor(size.height));
    }

    // Returns a Size with each component from the input SizeF ceiled.
    public static Size ToCeiledSize(in SizeF size)
    {
        return new Size(ClampCeil(size.width), ClampCeil(size.height));
    }

    // Returns a Size with each component from the input SizeF rounded.
    public static Size ToRoundedSize(in SizeF size)
    {
        return new Size(ClampRound(size.width), ClampRound(size.height));
    }
}
