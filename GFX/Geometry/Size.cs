using UI.Numerics;

using static UI.Numerics.ClampedMath;
using static UI.GFX.Geometry.SizeConversions;

namespace UI.GFX.Geometry;

public struct Size : IEquatable<Size>
{
    private int width_, height_;

    public int width
    {
        readonly get => width_;
        set => width_ = Math.Max(0, value);
    }

    public int height
    {
        readonly get => height_;
        set => height_ = Math.Max(0, value);
    }

    public Size()
    {
        width_ = 0;
        height_ = 0;
    }

    public Size(int width, int height)
    {
        width_ = Math.Max(0, width);
        height_ = Math.Max(0, height);
    }

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public readonly bool IsEmpty() => width_ == 0 || height_ == 0;

    public readonly bool IsZero() => width_ == 0 && height_ == 0;

    public void Enlarge(int growWidth, int growHeight)
    {
        width = ClampAdd(width_, growWidth);
        height = ClampAdd(height_, growHeight);
    }

    public void SetToMin(in Size other)
    {
        width_ = Math.Min(width_, other.width_);
        height_ = Math.Min(height_, other.height_);
    }

    public void SetToMax(in Size other)
    {
        width_ = Math.Max(width_, other.width_);
        height_ = Math.Max(height_, other.height_);
    }

    public void Transpose()
    {
        (width_, height_) = (height_, width_);
    }

    /// <summary>
    /// Returns the area. This method will throw an OverflowException
    /// if the area exceeds the bounds of a 32-bit integer.
    /// </summary>
    public readonly int GetArea()
    {
        checked
        {
            return width_ * height_;
        }
    }

    /// <summary>
    /// Returns the area as a 64-bit integer, avoiding overflow issues.
    /// </summary>
    public readonly long Area64() => (long)width_ * height_;

    public override readonly string ToString() => $"{width_}x{height_}";

    public override readonly int GetHashCode() => HashCode.Combine(width_, height_);

    public readonly bool Equals(Size other) => width_ == other.width_ && height_ == other.height_;

    public override readonly bool Equals(object? obj) => obj is Size other && Equals(other);

    public static bool operator ==(in Size left, in Size right) => left.Equals(right);
    public static bool operator !=(in Size left, in Size right) => !left.Equals(right);

    public static Size operator +(Size lhs, in Size rhs)
    {
        lhs.Enlarge(rhs.width_, rhs.height_);
        return lhs;
    }

    public static Size operator -(Size lhs, in Size rhs)
    {
        lhs.Enlarge(-rhs.width_, -rhs.height_);
        return lhs;
    }

    public static Size TransposeSize(in Size s)
    {
        return new Size(s.height_, s.width_);
    }

    public static Size ScaleToCeiledSize(in Size size, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return size;

        return ToCeiledSize(SizeF.ScaleSize((SizeF)size, x_scale, y_scale));
    }
    
    public static Size ScaleToCeiledSize(in Size size, float scale)
    {
        if (scale == 1.0f)
            return size;

        return ToCeiledSize(SizeF.ScaleSize((SizeF)size, scale, scale));
    }

    public static Size ScaleToFlooredSize(in Size size, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return size;

        return ToFlooredSize(SizeF.ScaleSize((SizeF)size, x_scale, y_scale));
    }

    public static Size ScaleToFlooredSize(in Size size, float scale)
    {
        if (scale == 1.0f)
            return size;

        return ToFlooredSize(SizeF.ScaleSize((SizeF)size, scale, scale));
    }

    public static Size ScaleToRoundedSize(in Size size, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return size;

        return ToRoundedSize(SizeF.ScaleSize((SizeF)size, x_scale, y_scale));
    }

    public static Size ScaleToRoundedSize(in Size size, float scale)
    {
        if (scale == 1.0f)
            return size;

        return ToRoundedSize(SizeF.ScaleSize((SizeF)size, scale, scale));
    }
}
