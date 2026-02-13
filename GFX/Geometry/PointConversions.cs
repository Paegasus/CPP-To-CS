namespace UI.GFX.Geometry;

public static class PointConversions
{
    public static Point ToFlooredPoint(in PointF point)
    {
        return Point(base::ClampFloor(point.x()), base::ClampFloor(point.y()));
    }

    public static Point ToCeiledPoint(in PointF point)
    {
        return Point(base::ClampCeil(point.x()), base::ClampCeil(point.y()));
    }

    public static Point ToRoundedPoint(in PointF point)
    {
        return Point(base::ClampRound(point.x()), base::ClampRound(point.y()));
    }
}
