using UI.Extensions;

namespace UI.GFX.Geometry;

public struct SizeF : IEquatable<SizeF>
{
    private float width_, height_;

    public float width { readonly get => width_; set => width_ = Clamp(value); }
    public float height { readonly get => height_; set => height_ = Clamp(value); }

    public SizeF()
    {
        width_ = 0.0f;
        height_ = 0.0f;
    }

    public SizeF(float width, float height)
    {
        width_ = Clamp(width);
        height_ = Clamp(height);
    }

    public SizeF(in Size size)
    {
        width_ = Clamp(size.width);
        height_ = Clamp(size.height);
    }

    private static readonly float Trivial = 8.0f * float.MachineEpsilon;

    private static float Clamp(float f) => f > Trivial ? f : 0.0f;

    public void SetSize(float width, float height)
    {
        this.width = width;
        this.height = height;
    }

    public readonly float GetArea() => width_ * height_;

    public readonly float AspectRatio() => width_ / height_;

    public void Enlarge(float growWidth, float growHeight)
    {
        width_ += growWidth;
        height_ += growHeight;
    }

    public void SetToMin(in SizeF other)
    {
        width_ = MathF.Min(width_, other.width_);
        height_ = MathF.Min(height_, other.height_);
    }

    public void SetToMax(in SizeF other)
    {
        width_ = MathF.Max(width_, other.width_);
        height_ = MathF.Max(height_, other.height_);
    }
    
    // Expands width/height to the next representable value.
    public void SetToNextWidth() => width_ = next(width_);
    public void SetToNextHeight() => height_ = next(height_);

    public readonly bool IsEmpty() => width_ == 0.0f || height_ == 0.0f;
    public readonly bool IsZero() => width_ == 0.0f && height_ == 0.0f;

    public void Transpose()
    {
        (width_, height_) = (height_, width_);
    }

    public void Scale(float scale) => Scale(scale, scale);

    public void Scale(float x_scale, float y_scale)
    {
        width *= x_scale;
        height *= y_scale;
    }

    public static SizeF ScaleSize(in SizeF s, float x_scale, float y_scale)
    {
        SizeF scaled_s = s;
        scaled_s.Scale(x_scale, y_scale);
        return scaled_s;
    }

    public static SizeF ScaleSize(in SizeF s, float scale)
    {
        return ScaleSize(s, scale, scale);
    }

    public static SizeF TransposeSize(in SizeF s)
    {
        return new SizeF(s.height_, s.width_);
    }

    public override readonly string ToString() => $"{width_}x{height_}";

    static float next(float f)
    {
        float x = MathF.Max(Trivial, f);
        return MathF.BitIncrement(x);
    }

    public override readonly int GetHashCode() => HashCode.Combine(width_, height_);

    public readonly bool Equals(SizeF other) => width_ == other.width_ && height_ == other.height_;

    public override readonly bool Equals(object? obj) => obj is SizeF other && Equals(other);

    public static bool operator ==(in SizeF left, in SizeF right) => left.Equals(right);
    public static bool operator !=(in SizeF left, in SizeF right) => !left.Equals(right);

    public static SizeF operator +(in SizeF lhs, in SizeF rhs)
    {
        return new SizeF(lhs.width_ + rhs.width_, lhs.height_ + rhs.height_);
    }

    public static SizeF operator -(in SizeF lhs, in SizeF rhs)
    {
        return new SizeF(lhs.width_ - rhs.width_, lhs.height_ - rhs.height_);
    }

    public void operator +=(SizeF size)
    {
        SetSize(width_ + size.width_, height_ + size.height_);
    }

    public void operator -=(SizeF size)
    {
        SetSize(width_ - size.width_, height_ - size.height_);
    }

    public static explicit operator SizeF(in Size size) => new(size);
}
