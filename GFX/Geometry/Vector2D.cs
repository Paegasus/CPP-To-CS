using System.Runtime.CompilerServices;
using UI.Numerics;

using static UI.Numerics.ClampedMath;

namespace UI.GFX.Geometry;

public struct Vector2D
{
    private int x_, y_;

    public int x { readonly get => x_; set => x_ = value; }
    public int y { readonly get => y_; set => y_ = value; }

    public Vector2D()
    {
        x_ = 0;
        y_ = 0;
    }
    public Vector2D(int x, int y)
    {
        x_ = x;
        y_ = y;
    }

    // True if both components of the vector are 0.
    public readonly bool IsZero() => x_ == 0 && y_ == 0;

    // Add the components of the |other| vector to the current vector.
    public void Add(in Vector2D other)
    {
        x_ = ClampAdd(other.x_, x_);
        y_ = ClampAdd(other.y_, y_);
    }

    // Subtract the components of the |other| vector from the current vector.
    public void Subtract(in Vector2D other)
    {
        x_ = ClampSub(x_, other.x_);
        y_ = ClampSub(y_, other.y_);
    }

    public void SetToMin(in Vector2D other)
    {
        x_ = Math.Min(x_, other.x_);
        y_ = Math.Min(y_, other.y_);
    }
  
    public void SetToMax(in Vector2D other)
    {
        x_ = Math.Max(x_, other.x_);
        y_ = Math.Max(y_, other.y_);
    }

    // Gives the square of the diagonal length of the vector. Since this is
    // cheaper to compute than Length(), it is useful when you want to compare
    // relative lengths of different vectors without needing the actual lengths.
    public readonly long LengthSquared()
    {
        return (long)x_ * x_ + (long)y_ * y_;
    }

    // Gives the diagonal length of the vector.
    public readonly float Length()
    {
        return (float)Math.Sqrt((double)LengthSquared());
    }

    public void Transpose()
    {
        (x_, y_) = (y_, x_);
    }

    public override readonly string ToString() => $"[{x_} {y_}]";

    public readonly bool Equals(Vector2D other) => x_ == other.x_ && y_ == other.y_;

    public override readonly bool Equals(object? obj) => obj is Vector2D other && Equals(other);

    public override readonly int GetHashCode() => HashCode.Combine(y_, x_);

    public static bool operator == (in Vector2D left, in Vector2D right) => left.Equals(right);
    public static bool operator != (in Vector2D left, in Vector2D right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D operator +(in Vector2D lhs, in Vector2D rhs)
    {
        Vector2D result = new(lhs.x_, lhs.y_);
        result.Add(rhs);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D operator -(in Vector2D lhs, in Vector2D rhs)
    {
        Vector2D result = new(lhs.x_, lhs.y_);
        result.Subtract(rhs);
        return result;
    }

    /*
    public static Vector2D operator - (in Vector2D v)
    {
        return new Vector2D(-(int)new ClampedNumeric<int>(v.x_), -(int)new ClampedNumeric<int>(v.y_));
    }
    */

    public static Vector2D operator - (in Vector2D v)
    {
        // Negation can overflow for int.MinValue, so we use ClampSub from zero.
        return new Vector2D(ClampSub(0, v.x_), ClampSub(0, v.y_));
    }

    public static explicit operator Vector2DF(in Vector2D source)
    {
        return new Vector2DF((float)source.x_, (float)source.y_);
    }
}
