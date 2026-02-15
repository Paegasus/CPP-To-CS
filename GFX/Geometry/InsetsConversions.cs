using static UI.Numerics.SafeConversions;

namespace UI.GFX.Geometry;

public static class InsetsConversions
{
    public static Insets ToFlooredInsets(in InsetsF insets)
    {
        return Insets.TLBR(
            ClampFloor(insets.top()), ClampFloor(insets.left()),
            ClampFloor(insets.bottom()), ClampFloor(insets.right()));
    }

    public static Insets ToCeiledInsets(in InsetsF insets)
    {
        return Insets.TLBR(
            ClampCeil(insets.top()), ClampCeil(insets.left()),
            ClampCeil(insets.bottom()), ClampCeil(insets.right()));
    }

    public static Insets ToRoundedInsets(in InsetsF insets)
    {
        return Insets.TLBR(
            ClampRound(insets.top()), ClampRound(insets.left()),
            ClampRound(insets.bottom()), ClampRound(insets.right()));
    }
}
