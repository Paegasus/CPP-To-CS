using System;
using System.Runtime.CompilerServices;

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

    public readonly bool IsZero() => x_ == 0.0f && y_ == 0.0f;

    public void Add(in Vector2DF other)
    {
        x_ += other.x_;
        y_ += other.y_;
    }

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

    public readonly double LengthSquared() => (double)x_ * x_ + (double)y_ * y_;

    public readonly float Length() => (float)Math.Sqrt(LengthSquared());

    public void Scale(float scale)
    {
        x_ *= scale;
        y_ *= scale;
    }

    public void Transpose()
    {
        (x_, y_) = (y_, x_);
    }

    public override readonly string ToString() => $"[{x_} {y_}]";

    public override readonly int GetHashCode() => HashCode.Combine(x_, y_);

    public override readonly bool Equals(object? obj) => obj is Vector2DF other && Equals(other);

    public readonly bool Equals(Vector2DF other) => x_ == other.x_ && y_ == other.y_;

    public static bool operator ==(in Vector2DF left, in Vector2DF right) => left.Equals(right);

    public static bool operator !=(in Vector2DF left, in Vector2DF right) => !left.Equals(right);

    public static Vector2DF operator +(in Vector2DF lhs, in Vector2DF rhs)
    {
        return new Vector2DF(lhs.x_ + rhs.x_, lhs.y_ + rhs.y_);
    }

    public static Vector2DF operator -(in Vector2DF lhs, in Vector2DF rhs)
    {
        return new Vector2DF(lhs.x_ - rhs.x_, lhs.y_ - rhs.y_);
    }

    public static Vector2DF operator -(in Vector2DF v)
    {
        return new Vector2DF(-v.x_, -v.y_);
    }
}
