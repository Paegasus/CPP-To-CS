namespace UI.GFX.Geometry;

using System.Runtime.CompilerServices;
using static Numerics.ClampedMath;

public struct PointF : IComparable<PointF>, IEquatable<PointF>
{
    private float x_, y_;

    public float x { readonly get => x_; set => x_ = value; }
    public float y { readonly get => y_; set => y_ = value; }

    public PointF()
    {
        x_ = 0.0f;
        y_ = 0.0f;
    }
    
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
        SetPoint(x * x_scale, y * y_scale);
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
        //DCHECK(allowed_distance > 0);

        float diff_x = x_ - rhs.x;
        float diff_y = y_ - rhs.y;
        float distance = MathF.Sqrt(diff_x * diff_x + diff_y * diff_y);

        return distance < allowed_distance;
    }

    public override readonly string ToString()
    {
        return $"{x},{y}";
    }

    // For use in collections (SortedSet, Dictionary keys, etc.)
    public override readonly int GetHashCode() => HashCode.Combine(y, x);

    public readonly int CompareTo(PointF other)
    {
        int yComparison = y.CompareTo(other.y);
        return yComparison != 0 ? yComparison : x.CompareTo(other.x);
    }

    public readonly bool Equals(PointF other) => x == other.x && y == other.y;

    public override readonly bool Equals(object? obj) => obj is PointF other && Equals(other);

    // A point is less than another point if its y-value is closer to the origin.
    // If the y-values are the same, then point with the x-value closer to the origin is considered less than the other.
    // This comparison is required to use PointF in sets, or sorted vectors.
    public static bool operator < (PointF left, PointF right) => left.CompareTo(right) < 0;
    public static bool operator > (PointF left, PointF right) => left.CompareTo(right) > 0;
    public static bool operator <= (PointF left, PointF right) => left.CompareTo(right) <= 0;
    public static bool operator >= (PointF left, PointF right) => left.CompareTo(right) >= 0;

    public static bool operator == (PointF left, PointF right) => left.Equals(right);
    public static bool operator != (PointF left, PointF right) => !left.Equals(right);

    public static PointF operator + (PointF lhs, in Vector2DF rhs)
    {
        PointF result = new(lhs.x, lhs.y);

        result.x += rhs.x;
        result.y += rhs.y;
        
        return result;
    }

    public static PointF operator - (in PointF lhs, in Vector2DF rhs)
    {
        PointF result = new(lhs.x, lhs.y);
        
        result.x -= rhs.x;
        result.y -= rhs.y;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2DF operator - (in PointF lhs, in PointF rhs)
    {
        return new Vector2DF(lhs.x - rhs.x, lhs.y - rhs.y);
    }
}
