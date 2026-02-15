using System;
using UI.Numerics;
using static UI.Numerics.ClampedMath;

namespace UI.GFX.Geometry;

/// <summary>
/// The common base template class for Insets and Outsets.
/// Represents the widths of the four borders or margins of an unspecified
/// rectangle. It stores the thickness of the top, left, bottom and right
/// edges, without storing the actual size and position of the rectangle itself.
/// </summary>
public abstract class InsetsOutsetsBase<T> : IEquatable<T> where T : InsetsOutsetsBase<T>, new()
{
    private int top_ = 0;
    private int left_ = 0;
    private int bottom_ = 0;
    private int right_ = 0;

    public int top => top_;
    public int left => left_;
    public int bottom => bottom_;
    public int right => right_;

    protected InsetsOutsetsBase() { }

    protected InsetsOutsetsBase(int all)
    {
        top_ = all;
        left_ = all;
        bottom_ = ClampBottomOrRight(all, all);
        right_ = ClampBottomOrRight(all, all);
    }

    /// <summary>
    /// Returns the total width taken up by the insets/outsets.
    /// </summary>
    public int width() => left_ + right_;

    /// <summary>
    /// Returns the total height taken up by the insets/outsets.
    /// </summary>
    public int height() => top_ + bottom_;

    /// <summary>
    /// Returns the size of the insets/outsets.
    /// </summary>
    public Size size() => new Size(width(), height());

    /// <summary>
    /// Returns true if the insets/outsets are empty.
    /// </summary>
    public bool IsEmpty() => width() == 0 && height() == 0;

    /// <summary>
    /// Flips x- and y-axes.
    /// </summary>
    public void Transpose()
    {
        (top_, left_) = (left_, top_);
        (bottom_, right_) = (right_, bottom_);
    }

    public T set_top(int top)
    {
        top_ = top;
        bottom_ = ClampBottomOrRight(top_, bottom_);
        return (T)this;
    }

    public T set_left(int left)
    {
        left_ = left;
        right_ = ClampBottomOrRight(left_, right_);
        return (T)this;
    }

    public T set_bottom(int bottom)
    {
        bottom_ = ClampBottomOrRight(top_, bottom);
        return (T)this;
    }

    public T set_right(int right)
    {
        right_ = ClampBottomOrRight(left_, right);
        return (T)this;
    }

    public T set_left_right(int left, int right)
    {
        left_ = left;
        right_ = ClampBottomOrRight(left_, right);
        return (T)this;
    }

    public T set_top_bottom(int top, int bottom)
    {
        top_ = top;
        bottom_ = ClampBottomOrRight(top_, bottom);
        return (T)this;
    }

    public static T TLBR(int top, int left, int bottom, int right)
    {
        return new T().set_top_bottom(top, bottom).set_left_right(left, right);
    }

    public static T VH(int vertical, int horizontal)
    {
        return TLBR(vertical, horizontal, vertical, horizontal);
    }

    public void SetToMax(T other)
    {
        top_ = Math.Max(top_, other.top_);
        left_ = Math.Max(left_, other.left_);
        bottom_ = Math.Max(bottom_, other.bottom_);
        right_ = Math.Max(right_, other.right_);
    }

    public override string ToString() => $"x:{left_},{right_} y:{top_},{bottom_}";

    public bool Equals(T? other)
    {
        if (other is null) return false;
        return top_ == other.top_ && left_ == other.left_ && bottom_ == other.bottom_ && right_ == other.right_;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as T);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(top_, left_, bottom_, right_);
    }

    public static bool operator ==(InsetsOutsetsBase<T> lhs, InsetsOutsetsBase<T> rhs)
    {
        if (lhs is null) return rhs is null;
        return lhs.Equals(rhs);
    }

    public static bool operator !=(InsetsOutsetsBase<T> lhs, InsetsOutsetsBase<T> rhs)
    {
        return !(lhs == rhs);
    }

    public static T operator +(InsetsOutsetsBase<T> lhs, T rhs)
    {
        T result = new T();
        result.top_ = ClampAdd(lhs.top_, rhs.top_);
        result.left_ = ClampAdd(lhs.left_, rhs.left_);
        result.bottom_ = ClampBottomOrRight(result.top_, ClampAdd(lhs.bottom_, rhs.bottom_));
        result.right_ = ClampBottomOrRight(result.left_, ClampAdd(lhs.right_, rhs.right_));
        return result;
    }

    public static T operator -(InsetsOutsetsBase<T> lhs, T rhs)
    {
        T result = new T();
        result.top_ = ClampSub(lhs.top_, rhs.top_);
        result.left_ = ClampSub(lhs.left_, rhs.left_);
        result.bottom_ = ClampBottomOrRight(result.top_, ClampSub(lhs.bottom_, rhs.bottom_));
        result.right_ = ClampBottomOrRight(result.left_, ClampSub(lhs.right_, rhs.right_));
        return result;
    }

    public static T operator -(InsetsOutsetsBase<T> v)
    {
        var left = ClampSub(0, v.left_);
        var right = ClampSub(0, v.right_);
        var top = ClampSub(0, v.top_);
        var bottom = ClampSub(0, v.bottom_);
        return TLBR(top, left, bottom, right);
    }

    private static int ClampBottomOrRight(int top_or_left, int bottom_or_right)
    {
        return ClampAdd(top_or_left, bottom_or_right) - top_or_left;
    }
}