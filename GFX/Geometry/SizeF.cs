using UI.Extensions;

namespace UI.GFX.Geometry;

public struct SizeF // : IEquatable<SizeF>
{
    private float width_, height_;

    public float width
    {
        readonly get => width_;
        set => width_ = MathF.Max(0.0f, value);
    }

    public float height
    {
        readonly get => height_;
        set => height_ = MathF.Max(0.0f, value);
    }

    public SizeF()
    {
        width_ = 0.0f;
        height_ = 0.0f;
    }
    
    public SizeF(float width, float height)
    {
        width_ = clamp(width);
        height_ = clamp(height);
    }

    public static explicit operator SizeF(Size s) => new((float)s.width, (float)s.height);

    private static readonly float Trivial = 8.0f * float.MachineEpsilon;

    private static float clamp(float f) => f > Trivial ? f : 0.0f;

    public void SetSize(float width, float height)
    {
        width_ = width;
        height_ = height;
    }

    public void Scale(float scale) => Scale(scale, scale);
    
    public void Scale(float x_scale, float y_scale)
    {
        width_ *= x_scale;
        height_ *= y_scale;
    }

    public static SizeF ScaleSize(in SizeF s, float x_scale, float y_scale)
    {
        return new SizeF(s.width_ * x_scale, s.height_ * y_scale);
    }
}
