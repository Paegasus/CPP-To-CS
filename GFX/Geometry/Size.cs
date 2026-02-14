using System;

namespace UI.GFX.Geometry;

public struct Size : IEquatable<Size>
{
    private int width_, height_;

    public int width { readonly get => width_; set => width_ = value; }
    public int height { readonly get => height_; set => height_ = value; }

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

    public readonly bool IsEmpty() => width_ == 0 || height_ == 0;

    public void Enlarge(int width, int height)
    {
        width_ += width;
        height_ += height;
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

    public readonly long Area() => (long)width_ * height_;

    public override readonly string ToString() => $"{width_}x{height_}";

    public override readonly int GetHashCode() => HashCode.Combine(width_, height_);

    public readonly bool Equals(Size other) => width_ == other.width_ && height_ == other.height_;

    public override readonly bool Equals(object? obj) => obj is Size other && Equals(other);

    public static bool operator ==(in Size left, in Size right) => left.Equals(right);
    public static bool operator !=(in Size left, in Size right) => !left.Equals(right);
}
