using System;
using UI.Numerics;
using static UI.Numerics.ClampedMath;

namespace UI.GFX.Geometry;

/// <summary>
/// This can be used to represent a space surrounding a rectangle, by
/// "expanding" the rectangle by the outset amount on all four sides.
/// </summary>
public struct Outsets : IEquatable<Outsets>
{
    private int top_;
    private int left_;
    private int bottom_;
    private int right_;

    public int top() => top_;
    public int left() => left_;
    public int bottom() => bottom_;
    public int right() => right_;

    public Outsets(int all)
    {
        top_ = all;
        left_ = all;
        bottom_ = ClampBottomOrRight(all, all);
        right_ = ClampBottomOrRight(all, all);
    }

    public Outsets(int vertical, int horizontal)
    {
        top_ = vertical;
        left_ = horizontal;
        bottom_ = ClampBottomOrRight(vertical, vertical);
        right_ = ClampBottomOrRight(horizontal, horizontal);
    }
    
    public Outsets(int top, int left, int bottom, int right)
    {
        top_ = top;
        left_ = left;
        bottom_ = ClampBottomOrRight(top, bottom);
        right_ = ClampBottomOrRight(left, right);
    }

    /// <summary>
    /// Returns the total width taken up by the outsets.
    /// </summary>
    public readonly int width() => left_ + right_;

    /// <summary>
    /// Returns the total height taken up by the outsets.
    /// </summary>
    public readonly int height() => top_ + bottom_;

    /// <summary>
    /// Returns the size of the outsets.
    /// </summary>
    public readonly Size size() => new Size(width(), height());

    /// <summary>
    /// Returns true if the outsets are empty.
    /// </summary>
    public readonly bool IsEmpty() => width() == 0 && height() == 0;
    
    /// <summary>
    /// Flips x- and y-axes.
    /// </summary>
    public void Transpose()
    {
        (top_, left_) = (left_, top_);
        (bottom_, right_) = (right_, bottom_);
    }

    public Outsets set_top(int top)
    {
        top_ = top;
        bottom_ = ClampBottomOrRight(top_, bottom_);
        return this;
    }

    public Outsets set_left(int left)
    {
        left_ = left;
        right_ = ClampBottomOrRight(left_, right_);
        return this;
    }

    public Outsets set_bottom(int bottom)
    {
        bottom_ = ClampBottomOrRight(top_, bottom);
        return this;
    }

    public Outsets set_right(int right)
    {
        right_ = ClampBottomOrRight(left_, right);
        return this;
    }
    
    public Outsets set_left_right(int left, int right) {
        left_ = left;
        right_ = ClampBottomOrRight(left_, right);
        return this;
    }

    public Outsets set_top_bottom(int top, int bottom) {
        top_ = top;
        bottom_ = ClampBottomOrRight(top_, bottom);
        return this;
    }

    public static Outsets TLBR(int top, int left, int bottom, int right)
    {
        return new Outsets(top, left, bottom, right);
    }

    public static Outsets VH(int vertical, int horizontal)
    {
        return new Outsets(vertical, horizontal);
    }
    
    public void SetToMax(in Outsets other) {
        top_ = Math.Max(top_, other.top_);
        left_ = Math.Max(left_, other.left_);
        bottom_ = Math.Max(bottom_, other.bottom_);
        right_ = Math.Max(right_, other.right_);
    }

    // Conversion from Outsets to Insets negates all components.
    public Insets ToInsets() => new Insets(SaturatingNegate(top()), SaturatingNegate(left()), SaturatingNegate(bottom()), SaturatingNegate(right()));

    public override readonly string ToString() => $"x:{left_},{right_} y:{top_},{bottom_}";

    public override readonly int GetHashCode() => HashCode.Combine(top_, left_, bottom_, right_);

    public override readonly bool Equals(object? obj) => obj is Outsets other && Equals(other);

    public readonly bool Equals(Outsets other) =>
        top_ == other.top_ && left_ == other.left_ && bottom_ == other.bottom_ && right_ == other.right_;
    
    public static bool operator ==(in Outsets left, in Outsets right) => left.Equals(right);
    public static bool operator !=(in Outsets left, in Outsets right) => !left.Equals(right);

    public void operator +=(in Outsets other)
    {
        top_ = ClampAdd(top_, other.top_);
        left_ = ClampAdd(left_, other.left_);
        bottom_ = ClampBottomOrRight(top_, ClampAdd(bottom_, other.bottom_));
        right_ = ClampBottomOrRight(left_, ClampAdd(right_, other.right_));
    }

    public void operator -=(in Outsets other)
    {
        top_ = ClampSub(top_, other.top_);
        left_ = ClampSub(left_, other.left_);
        bottom_ = ClampBottomOrRight(top_, ClampSub(bottom_, other.bottom_));
        right_ = ClampBottomOrRight(left_, ClampSub(right_, other.right_));
    }

    public static Outsets operator -(in Outsets v)
    {
        return new Outsets(
            SaturatingNegate(v.top()),
            SaturatingNegate(v.left()),
            SaturatingNegate(v.bottom()),
            SaturatingNegate(v.right())
        );
    }
    
    public static Outsets operator +(Outsets lhs, in Outsets rhs)
    {
        lhs += rhs;
        return lhs;
    }
    
    public static Outsets operator -(Outsets lhs, in Outsets rhs)
    { 
        lhs -= rhs;
        return lhs;
    }
    
    private static int ClampBottomOrRight(int top_or_left, int bottom_or_right)
    {
        return ClampAdd(top_or_left, bottom_or_right) - top_or_left;
    }

    private static int SaturatingNegate(int v) => v == int.MinValue ? int.MaxValue : -v;
}
