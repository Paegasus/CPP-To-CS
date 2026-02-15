using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace UI.GFX.Geometry;

public struct Vector2DF : IEquatable<Vector2DF>
{
    private float x_, y_;

    public float x { readonly get => x_; set => x_ = value; }
    public float y { readonly get => y_; set => y_ = value; }

    public Vector2DF()
    {
        x_ = 0.0f;
        y_ = 0.0f;
    }

    public Vector2DF(float x, float y)
    {
        x_ = x;
        y_ = y;
    }

    // True if both components of the vector are 0.
    public readonly bool IsZero() => x_ == 0.0f && y_ == 0.0f;

    // Add the components of the |other| vector to the current vector.
    public void Add(in Vector2DF other)
    {
        x_ += other.x_;
        y_ += other.y_;
    }

    // Subtract the components of the |other| vector from the current vector.
    public void Subtract(in Vector2DF other)
    {
        x_ -= other.x_;
        y_ -= other.y_;
    }

    public void SetToMin(in Vector2DF other)
    {
        x_ = Math.Min(x_, other.x_);
        y_ = Math.Min(y_, other.y_);
    }

    public void SetToMax(in Vector2DF other)
    {
        x_ = Math.Max(x_, other.x_);
        y_ = Math.Max(y_, other.y_);
    }

    // Gives the square of the diagonal length, i.e. the square of magnitude, of the vector.
    public readonly double LengthSquared() => (double)x_ * x_ + (double)y_ * y_;

    // Gives the diagonal length (i.e. the magnitude) of the vector.
    public readonly float Length() => float.Hypot(x_, y_); // (float)Math.Sqrt(LengthSquared());

    public readonly float AspectRatio() => x_ / y_;

    // Gives the slope angle in radians of the vector from the positive x axis,
    // in the range of (-pi, pi]. The sign of the result is the same as the sign
    // of y(), except that the result is pi for Vector2dF(negative-x, zero-y).
    public readonly float SlopeAngleRadians()
    {
        return MathF.Atan2(y_, x_);
    }

    // Scale the x and y components of the vector by |scale|.
    public void Scale(float scale)
    {
        x_ *= scale;
        y_ *= scale;
    }

    // Scale the x and y components of the vector by |x_scale| and |y_scale|
    // respectively.
    public void Scale(float x_scale, float y_scale)
    {
        x_ *= x_scale;
        y_ *= y_scale;
    }

    // Divides all components of the vector by |scale|.
    public void InvScale(float inv_scale) => InvScale(inv_scale, inv_scale);

    // Divides each component of the vector by the given scale factors.
    public void InvScale(float inv_x_scale, float inv_y_scale)
    {
        x_ /= inv_x_scale;
        y_ /= inv_y_scale;
    }

    public void Normalize() => InvScale(Length());

    public void Transpose()
    {
        (x_, y_) = (y_, x_);
    }

    // Return the cross product of two vectors, i.e. the determinant.
    public static double CrossProduct(in Vector2DF lhs, in Vector2DF rhs) => (double)lhs.x_ * rhs.y_ - (double)lhs.y_ * rhs.x_;
    // Return the dot product of two vectors.
    public static double DotProduct(in Vector2DF lhs, in Vector2DF rhs) => (double)lhs.x_ * rhs.x_ + (double)lhs.y_ * rhs.y_;

    // Return a vector that is |v| scaled by the given scale factors along each axis.
    public static Vector2DF ScaleVector2d(in Vector2DF v, float x_scale, float y_scale)
    {
        return new Vector2DF(v.x_ * x_scale, v.y_ * y_scale);
    }

    // Return a vector that is |v| scaled by the given scale factor.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static  Vector2DF ScaleVector2d(in Vector2DF v, float scale)
    {
        return ScaleVector2d(v, scale, scale);
    }

    public static Vector2DF TransposeVector2d(in Vector2DF v)
    {
        return new Vector2DF(v.y_, v.x_);
    }

    // Return a unit vector with the same direction as v.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2DF NormalizeVector2d(in Vector2DF v)
    {
        Vector2DF normal = v;
        normal.Normalize();
        return normal;
    }

    public override readonly string ToString() => $"[{x_} {y_}]";

    public override readonly int GetHashCode() => HashCode.Combine(x_, y_);

    public readonly bool Equals(Vector2DF other) => x_ == other.x_ && y_ == other.y_;

    public override readonly bool Equals(object? obj) => obj is Vector2DF other && Equals(other);

    public static bool operator ==(in Vector2DF left, in Vector2DF right) => left.Equals(right);

    public static bool operator !=(in Vector2DF left, in Vector2DF right) => !left.Equals(right);

    public void operator +=(in Vector2DF other)
    {
        Add(other);
    }

    public void operator -=(in Vector2DF other)
    {
        Subtract(other);
    }

    public static Vector2DF operator -(in Vector2DF v)
    {
        return new Vector2DF(-v.x_, -v.y_);
    }

    public static Vector2DF operator +(in Vector2DF lhs, in Vector2DF rhs)
    {
        Vector2DF result = lhs;
        result.Add(rhs);
        return result;
    }

    public static Vector2DF operator -(in Vector2DF lhs, in Vector2DF rhs)
    {
        Vector2DF result = lhs;
        result.Add(-rhs);
        return result;
    }
}
