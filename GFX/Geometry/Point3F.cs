using System;

namespace UI.GFX.Geometry;

// A point has an x, y and z coordinate.
public struct Point3F : IEquatable<Point3F>
{
    private float x_;
    private float y_;
    private float z_;

    public float x { readonly get => x_; set => x_ = value; }
    public float y { readonly get => y_; set => y_ = value; }
    public float z { readonly get => z_; set => z_ = value; }

    public Point3F() : this(0f, 0f, 0f) { }

    public Point3F(float x, float y, float z)
    {
        x_ = x;
        y_ = y;
        z_ = z;
    }

    public Point3F(in PointF point)
    {
        x_ = point.x;
        y_ = point.y;
        z_ = 0;
    }

    public void SetPoint(float x, float y, float z)
    {
        x_ = x;
        y_ = y;
        z_ = z;
    }

    public void Scale(float scale) => Scale(scale, scale, scale);

    public void Scale(float x_scale, float y_scale, float z_scale) => SetPoint(x * x_scale, y * y_scale, z * z_scale);

    public readonly bool IsOrigin() => x_ == 0 && y_ == 0 && z_ == 0;

    // Returns the squared euclidean distance between two points.
    public readonly float SquaredDistanceTo(in Point3F other)
    {
        float dx = x_ - other.x_;
        float dy = y_ - other.y_;
        float dz = z_ - other.z_;
        return dx * dx + dy * dy + dz * dz;
    }

    public readonly PointF AsPointF() => new PointF(x_, y_);

    public readonly Vector3DF OffsetFromOrigin() => new Vector3DF(x_, y_, z_);

    // Returns a string representation of 3d point.
    public override readonly string ToString() => $"{x_},{y_},{z_}";

    public override readonly int GetHashCode() => HashCode.Combine(x_, y_, z_);

    public override readonly bool Equals(object? obj) => obj is Point3F other && Equals(other);

    public readonly bool Equals(Point3F other) => x_ == other.x_ && y_ == other.y_ && z_ == other.z_;

    public static bool operator ==(in Point3F lhs, in Point3F rhs) => lhs.Equals(rhs);
    public static bool operator !=(in Point3F lhs, in Point3F rhs) => !lhs.Equals(rhs);

    // Add a vector to a point, producing a new point offset by the vector.
    public static Point3F operator +(in Point3F lhs, in Vector3DF rhs) => new Point3F(lhs.x_ + rhs.x, lhs.y_ + rhs.y, lhs.z_ + rhs.z);

    // Subtract a vector from a point, producing a new point offset by the vector's inverse.
    public static Point3F operator -(in Point3F lhs, in Vector3DF rhs) => new Point3F(lhs.x_ - rhs.x, lhs.y_ - rhs.y, lhs.z_ - rhs.z);

    // Subtract one point from another, producing a vector that represents the distances between the two points along each axis.
    public static Vector3DF operator -(in Point3F lhs, in Point3F rhs) => new Vector3DF(lhs.x_ - rhs.x, lhs.y_ - rhs.y, lhs.z_ - rhs.z);

    // Offset the point by the given vector.
    public void operator +=(in Vector3DF v)
    {
        x_ += v.x;
        y_ += v.y;
        z_ += v.z;
    }

    // Offset the point by the given vector's inverse.
    public void operator -=(in Vector3DF v)
    {
        x_ -= v.x;
        y_ -= v.y;
        z_ -= v.z;
    }

    public static Point3F PointAtOffsetFromOrigin(in Vector3DF offset) => new Point3F(offset.x, offset.y, offset.z);

    public static Point3F ScalePoint(in Point3F p, float x_scale, float y_scale, float z_scale) => new Point3F(p.x_ * x_scale, p.y_ * y_scale, p.z_ * z_scale);

    public static Point3F ScalePoint(in Point3F p, in Vector3DF v) => new Point3F(p.x_ * v.x, p.y_ * v.y, p.z_ * v.z);

    public static Point3F ScalePoint(in Point3F p, float scale) => ScalePoint(p, scale, scale, scale);
}
