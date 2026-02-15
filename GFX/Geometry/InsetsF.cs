using System;

namespace UI.GFX.Geometry;

/// <summary>
/// A floating point version of gfx::Insets.
/// </summary>
public struct InsetsF : IEquatable<InsetsF>
{
    private float top_;
    private float left_;
    private float bottom_;
    private float right_;

    public float top() => top_;
    public float left() => left_;
    public float bottom() => bottom_;
    public float right() => right_;

    public InsetsF(float all) 
    {
        top_ = all;
        left_ = all;
        bottom_ = all;
        right_ = all;
    }

    public InsetsF(float vertical, float horizontal)
    {
        top_ = vertical;
        left_ = horizontal;
        bottom_ = vertical;
        right_ = horizontal;
    }

    public InsetsF(float top, float left, float bottom, float right)
    {
        top_ = top;
        left_ = left;
        bottom_ = bottom;
        right_ = right;
    }

    public readonly float width() => left_ + right_;
    public readonly float height() => top_ + bottom_;
    public readonly bool IsEmpty() => width() == 0f && height() == 0f;

    public void Transpose()
    {
        (top_, left_) = (left_, top_);
        (bottom_, right_) = (right_, bottom_);
    }

    public InsetsF set_top(float top)
    {
        top_ = top;
        return this;
    }

    public InsetsF set_left(float left)
    {
        left_ = left;
        return this;
    }

    public InsetsF set_bottom(float bottom)
    {
        bottom_ = bottom;
        return this;
    }

    public InsetsF set_right(float right)
    {
        right_ = right;
        return this;
    }

    public InsetsF set_left_right(float left, float right) {
        left_ = left;
        right_ = right;
        return this;
    }

    public InsetsF set_top_bottom(float top, float bottom) {
        top_ = top;
        bottom_ = bottom;
        return this;
    }

    public static InsetsF TLBR(float top, float left, float bottom, float right)
    {
        return new InsetsF(top, left, bottom, right);
    }

    public static InsetsF VH(float vertical, float horizontal)
    {
        return new InsetsF(vertical, horizontal);
    }

    public void SetToMax(in InsetsF other)
    {
        top_ = Math.Max(top_, other.top_);
        left_ = Math.Max(left_, other.left_);
        bottom_ = Math.Max(bottom_, other.bottom_);
        right_ = Math.Max(right_, other.right_);
    }

    public void Scale(float scale) => Scale(scale, scale);

    public void Scale(float x_scale, float y_scale)
    {
        top_ *= y_scale;
        left_ *= x_scale;
        bottom_ *= y_scale;
        right_ *= x_scale;
    }
    
    // public OutsetsF ToOutsets() => new OutsetsF(-top(), -left(), -bottom(), -right());

    public override readonly string ToString() => $"x:{left_},{right_} y:{top_},{bottom_}";
    public override readonly int GetHashCode() => HashCode.Combine(top_, left_, bottom_, right_);
    public override readonly bool Equals(object? obj) => obj is InsetsF other && Equals(other);
    public readonly bool Equals(InsetsF other) =>
        top_ == other.top_ && left_ == other.left_ && bottom_ == other.bottom_ && right_ == other.right_;

    public static bool operator ==(in InsetsF left, in InsetsF right) => left.Equals(right);
    public static bool operator !=(in InsetsF left, in InsetsF right) => !left.Equals(right);
    
    public void operator +=(in InsetsF other)
    {
        top_ += other.top_;
        left_ += other.left_;
        bottom_ += other.bottom_;
        right_ += other.right_;
    }

    public void operator -=(in InsetsF other)
    {
        top_ -= other.top_;
        left_ -= other.left_;
        bottom_ -= other.bottom_;
        right_ -= other.right_;
    }

    public static InsetsF operator -(in InsetsF v) => new InsetsF(-v.top_, -v.left_, -v.bottom_, -v.right_);
    public static InsetsF operator +(InsetsF lhs, in InsetsF rhs)
    {
        lhs += rhs;
        return lhs;
    }
    
    public static InsetsF operator -(InsetsF lhs, in InsetsF rhs)
    {
        lhs -= rhs;
        return lhs;
    }
}
