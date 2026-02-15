namespace UI.GFX.Geometry;

using System.Runtime.CompilerServices;

public struct PointF : IComparable<PointF>, IEquatable<PointF>
{
    private float x_, y_;

    public float x { readonly get => x_; set => x_ = value; }
    public float y { readonly get => y_; set => y_ = value; }

    public PointF() : this(0f, 0f) { }
    
    public PointF(float x, float y)
    {
        x_ = x;
        y_ = y;
    }

    public PointF(in Point p)
    {
        x_ = (float)p.x;
        y_ = (float)p.y;
    }

    public void SetPoint(float x, float y)
    {
        x_ = x;
        y_ = y;
    }

    public void Offset(float delta_x, float delta_y)
    {
        x_ += delta_x;
        y_ += delta_y;
    }

    public void SetToMin(in PointF other)
    {
        x_ = Math.Min(x_, other.x_);
        y_ = Math.Min(y_, other.y_);
    }

    public void SetToMax(in PointF other)
    {
        x_ = Math.Max(x_, other.x_);
        y_ = Math.Max(y_, other.y_);
    }

    public readonly bool IsOrigin()
    {
        return x_ == 0 && y_ == 0;
    }
    
    public readonly Vector2DF OffsetFromOrigin()
    {
        return new Vector2DF(x_, y_);
    }

    public void Scale(float scale)
    {
        Scale(scale, scale);
    }

    public void Scale(float x_scale, float y_scale)
    {
        SetPoint(x_ * x_scale, y_ * y_scale);
    }

    // Scales each component by the inverse of the given scales.
    public void InvScale(float inv_x_scale, float inv_y_scale)
    {
        x_ /= inv_x_scale;
        y_ /= inv_y_scale;
    }

    // Scales the point by the inverse of the given scale.
    public void InvScale(float inv_scale)
    {
        InvScale(inv_scale, inv_scale);
    }
    
    public void Transpose()
    {
        // Swap x_ and y_ (using tuple deconstruction swap)
        (x_, y_) = (y_, x_);
    }

    // Uses the Pythagorean theorem to determine the straight line distance
    // between the two points, and returns true if it is less than
    // |allowed_distance|.
    public readonly bool IsWithinDistance(in PointF rhs, float allowed_distance)
    {
#if DEBUG
        //DCHECK(allowed_distance > 0);
#endif
        float diff_x = x_ - rhs.x_;
        float diff_y = y_ - rhs.y_;
        float distance = MathF.Sqrt(diff_x * diff_x + diff_y * diff_y);

        return distance < allowed_distance;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF PointAtOffsetFromOrigin(in Vector2DF offset_from_origin)
    {
        return new PointF(offset_from_origin.x, offset_from_origin.y);
    }

    public static PointF ScalePoint(in PointF p, float x_scale, float y_scale)
    {
        PointF scaled_p = p;

        scaled_p.Scale(x_scale, y_scale);

        return scaled_p;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ScalePoint(in PointF p, float scale)
    {
        return ScalePoint(p, scale, scale);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF TransposePoint(in PointF p)
    {
        return new PointF(p.y_, p.x_);
    }

    public override readonly string ToString()
    {
        return $"{x_},{y_}";
    }

    // For use in collections (SortedSet, Dictionary keys, etc.)
    public override readonly int GetHashCode() => HashCode.Combine(y, x);

    public readonly int CompareTo(PointF other)
    {
        int yComparison = y.CompareTo(other.y);
        return yComparison != 0 ? yComparison : x.CompareTo(other.x);
    }

    public override readonly bool Equals(object? obj) => obj is PointF other && Equals(other);
    
    public readonly bool Equals(PointF other) => x_ == other.x_ && y_ == other.y_;

    // A point is less than another point if its y-value is closer to the origin.
    // If the y-values are the same, then point with the x-value closer to the origin is considered less than the other.
    // This comparison is required to use PointF in sets, or sorted vectors.
    public static bool operator < (in PointF left, in PointF right) => left.CompareTo(right) < 0;
    public static bool operator > (in PointF left, in PointF right) => left.CompareTo(right) > 0;
    public static bool operator <= (in PointF left, in PointF right) => left.CompareTo(right) <= 0;
    public static bool operator >= (in PointF left, in PointF right) => left.CompareTo(right) >= 0;

    public static bool operator == (in PointF left, in PointF right) => left.Equals(right);
    public static bool operator != (in PointF left, in PointF right) => !left.Equals(right);

    public void operator +=(in Vector2DF vector)
    {
        x_ += vector.x;
        y_ += vector.y;
    }

    public void operator -=(in Vector2DF vector)
    {
        x_ -= vector.x;
        y_ -= vector.y;
    }

    public static PointF operator +(in PointF lhs, in Vector2DF rhs)
    {
        PointF result = lhs;
        result += rhs;
        return result;
    }

    public static PointF operator -(in PointF lhs, in Vector2DF rhs)
    {
        PointF result = lhs;
        result -= rhs;
        return result;
    }

    public static Vector2DF operator -(in PointF lhs, in PointF rhs)
    {
        return new Vector2DF(lhs.x_ - rhs.x_, lhs.y_ - rhs.y_);
    }
}
