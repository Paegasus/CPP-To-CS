using System;
using System.Diagnostics;

namespace UI.GFX.Geometry;

public struct BoxF : IEquatable<BoxF>
{
    private Point3F origin_;
    private float width_;
    private float height_;
    private float depth_;

    public BoxF(float width, float height, float depth)
        : this(0, 0, 0, width, height, depth)
    {
    }

    public BoxF(float x, float y, float z, float width, float height, float depth)
        : this(new Point3F(x, y, z), width, height, depth)
    {
    }

    public BoxF(Point3F origin, float width, float height, float depth)
    {
        origin_ = origin;
        width_ = width >= 0 ? width : 0;
        height_ = height >= 0 ? height : 0;
        depth_ = depth >= 0 ? depth : 0;
    }

    public void Scale(float scale) => Scale(scale, scale, scale);

    public void Scale(float x_scale, float y_scale, float z_scale)
    {
        origin_.Scale(x_scale, y_scale, z_scale);
        set_size(width_ * x_scale, height_ * y_scale, depth_ * z_scale);
    }

    public readonly bool IsEmpty() => (width_ == 0 && height_ == 0) || (width_ == 0 && depth_ == 0) || (height_ == 0 && depth_ == 0);

    public void Union(in BoxF box)
    {
        if (IsEmpty())
        {
            this = box;
            return;
        }
        if (box.IsEmpty())
            return;
        ExpandTo(box);
    }

    public override string ToString() => $"{origin_} {width_}x{height_}x{depth_}";

    public float x { readonly get => origin_.x; set => origin_.x = value; }
    public float y { readonly get => origin_.y; set => origin_.y = value; }
    public float z { readonly get => origin_.z; set => origin_.z = value; }

    public float width { readonly get => width_; set => width_ = value < 0 ? 0 : value; }
    public float height { readonly get => height_; set => height_ = value < 0 ? 0 : value; }
    public float depth { readonly get => depth_; set => depth_ = value < 0 ? 0 : value; }

    public readonly float right() => x + width;
    public readonly float bottom() => y + height;
    public readonly float front() => z + depth;

    public void set_size(float width, float height, float depth)
    {
        width_ = width < 0 ? 0 : width;
        height_ = height < 0 ? 0 : height;
        depth_ = depth < 0 ? 0 : depth;
    }

    public readonly Point3F origin() => origin_;
    public void set_origin(Point3F origin) { origin_ = origin; }

    public void ExpandTo(in Point3F point) => ExpandTo(point, point);

    public void ExpandTo(in BoxF box) => ExpandTo(box.origin_, new Point3F(box.right(), box.bottom(), box.front()));

    private void ExpandTo(in Point3F min, in Point3F max)
    {
        Debug.Assert(min.x <= max.x);
        Debug.Assert(min.y <= max.y);
        Debug.Assert(min.z <= max.z);

        float min_x = Math.Min(x, min.x);
        float min_y = Math.Min(y, min.y);
        float min_z = Math.Min(z, min.z);
        float max_x = Math.Max(right(), max.x);
        float max_y = Math.Max(bottom(), max.y);
        float max_z = Math.Max(front(), max.z);

        origin_.SetPoint(min_x, min_y, min_z);
        width_ = max_x - min_x;
        height_ = max_y - min_y;
        depth_ = max_z - min_z;
    }

    public override int GetHashCode() => HashCode.Combine(origin_, width_, height_, depth_);

    public override bool Equals(object obj) => obj is BoxF other && Equals(other);

    public bool Equals(BoxF other) =>
        origin_.Equals(other.origin_) && width_ == other.width_ && height_ == other.height_ && depth_ == other.depth_;

    public static bool operator ==(in BoxF lhs, in BoxF rhs) => lhs.Equals(rhs);
    public static bool operator !=(in BoxF lhs, in BoxF rhs) => !lhs.Equals(rhs);

    public static BoxF operator +(in BoxF b, in Vector3DF v) =>
        new BoxF(b.x + v.x, b.y + v.y, b.z + v.z, b.width, b.height, b.depth);
        
    public static BoxF UnionBoxes(in BoxF a, in BoxF b)
    {
        BoxF result = a;
        result.Union(b);
        return result;
    }

    public static BoxF ScaleBox(in BoxF b, float x_scale, float y_scale, float z_scale) =>
        new BoxF(b.x * x_scale, b.y * y_scale, b.z * z_scale, b.width * x_scale, b.height * y_scale, b.depth * z_scale);

    public static BoxF ScaleBox(in BoxF b, float scale) => ScaleBox(b, scale, scale, scale);
}
