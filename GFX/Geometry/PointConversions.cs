using static UI.Numerics.SafeConversions;

namespace UI.GFX.Geometry;

public static class PointConversions
{
    public static Point ToFlooredPoint(in PointF point)
    {
        return new Point(ClampFloor(point.x), ClampFloor(point.y));
    }

    public static Point ToCeiledPoint(in PointF point)
    {
        return new Point(ClampCeil(point.x), ClampCeil(point.y));
    }

    public static Point ToRoundedPoint(in PointF point)
    {
        return new Point(ClampRound(point.x), ClampRound(point.y));
    }
}
