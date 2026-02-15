using System;
using System.Collections.Generic;

namespace UI.GFX.Geometry;

public struct RectF : IEquatable<RectF>
{
    private PointF origin_;
    private SizeF size_;

    public RectF(float width, float height) : this(0, 0, width, height) { }
    public RectF(float x, float y, float width, float height) 
    {
        origin_ = new PointF(x, y);
        size_ = new SizeF(width, height);
    }
    public RectF(in SizeF size) : this(0, 0, size.width, size.height) { }
    public RectF(in PointF origin, in SizeF size) : this(origin.x, origin.y, size.width, size.height) { }
    public RectF(in Rect r) : this(r.x(), r.y(), r.width(), r.height()) { }

    public float x { readonly get => origin_.x; set => origin_.x = value; }
    public float y { readonly get => origin_.y; set => origin_.y = value; }
    public float width { readonly get => size_.width; set => size_.width = value; }
    public float height { readonly get => size_.height; set => size_.height = value; }

    public PointF origin { readonly get => origin_; set => origin_ = value; }
    public SizeF size { readonly get => size_; set => size_ = value; }

    public readonly float right() => x + width;
    public readonly float bottom() => y + height;

    public readonly PointF top_right() => new(right(), y);
    public readonly PointF bottom_left() => new(x, bottom());
    public readonly PointF bottom_right() => new(right(), bottom());

    public readonly PointF left_center() => new(x, y + height / 2);
    public readonly PointF top_center() => new(x + width / 2, y);
    public readonly PointF right_center() => new(right(), y + height / 2);
    public readonly PointF bottom_center() => new(x + width / 2, bottom());

    public readonly Vector2DF OffsetFromOrigin() => new(x, y);

    public void SetRect(float x, float y, float width, float height)
    {
        origin_.SetPoint(x, y);
        size_.SetSize(width, height);
    }

    public void Inset(float inset) => Inset(new InsetsF(inset));
    public void Inset(in InsetsF insets)
    {
        origin_.Offset(insets.left(), insets.top());
        width -= insets.width();
        height -= insets.height();
    }

    public void Outset(float outset) => Inset(-outset);
    public void Outset(in OutsetsF outsets) => Inset(outsets.ToInsets());

    public void Offset(float horizontal, float vertical) => origin_.Offset(horizontal, vertical);
    public void Offset(in Vector2DF distance) => origin_.Offset(distance.x, distance.y);

    public readonly InsetsF InsetsFrom(in RectF inner) =>
        InsetsF.TLBR(inner.y - y, inner.x - x, bottom() - inner.bottom(), right() - inner.right());

    public readonly bool IsEmpty() => size_.IsEmpty();

    public readonly bool Contains(float point_x, float point_y) =>
        point_x >= x && point_x < right() && point_y >= y && point_y < bottom();

    public readonly bool Contains(in PointF point) => Contains(point.x, point.y);
    public readonly bool Contains(in RectF rect) =>
        rect.x >= x && rect.right() <= right() && rect.y >= y && rect.bottom() <= bottom();
    
    public readonly bool InclusiveContains(float point_x, float point_y) =>
        point_x >= x && point_x <= right() && point_y >= y && point_y <= bottom();

    public readonly bool InclusiveContains(in PointF point) => InclusiveContains(point.x, point.y);

    public readonly bool Intersects(in RectF rect) =>
        !IsEmpty() && !rect.IsEmpty() && rect.x < right() && rect.right() > x && rect.y < bottom() &&
        rect.bottom() > y;

    public void Intersect(in RectF rect)
    {
        if (IsEmpty() || rect.IsEmpty())
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        float rx = Math.Max(x, rect.x);
        float ry = Math.Max(y, rect.y);
        float rr = Math.Min(right(), rect.right());
        float rb = Math.Min(bottom(), rect.bottom());

        if (rx >= rr || ry >= rb)
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        SetRect(rx, ry, rr - rx, rb - ry);
    }
    
    public bool InclusiveIntersect(in RectF rect)
    {
        float rx = Math.Max(x, rect.x);
        float ry = Math.Max(y, rect.y);
        float rr = Math.Min(right(), rect.right());
        float rb = Math.Min(bottom(), rect.bottom());

        if (rx > rr || ry > rb)
        {
            SetRect(0, 0, 0, 0);
            return false;
        }

        SetRect(rx, ry, rr - rx, rb - ry);
        return true;
    }

    public void Union(in RectF rect)
    {
        if (IsEmpty())
        {
            this = rect;
            return;
        }
        if (rect.IsEmpty())
            return;

        UnionEvenIfEmpty(rect);
    }

    public void UnionEvenIfEmpty(in RectF rect)
    {
        float rx = Math.Min(x, rect.x);
        float ry = Math.Min(y, rect.y);
        float rr = Math.Max(right(), rect.right());
        float rb = Math.Max(bottom(), rect.bottom());
        
        SetRect(rx, ry, rr - rx, rb - ry);
        
        if (right() < rr) size_.SetToNextWidth();
        if (bottom() < rb) size_.SetToNextHeight();
    }

    public void Subtract(in RectF rect)
    {
        if (!Intersects(rect))
            return;
        if (rect.Contains(this))
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        float rx = x;
        float ry = y;
        float rr = right();
        float rb = bottom();

        if (rect.y <= y && rect.bottom() >= bottom())
        {
            if (rect.x <= x) rx = rect.right();
            else if (rect.right() >= right()) rr = rect.x;
        }
        else if (rect.x <= x && rect.right() >= right())
        {
            if (rect.y <= y) ry = rect.bottom();
            else if (rect.bottom() >= bottom()) rb = rect.y;
        }
        SetRect(rx, ry, rr - rx, rb - ry);
    }

    public void AdjustToFit(in RectF rect)
    {
        float new_x = x;
        float new_y = y;
        float new_width = width;
        float new_height = height;
        AdjustAlongAxis(rect.x, rect.width, ref new_x, ref new_width);
        AdjustAlongAxis(rect.y, rect.height, ref new_y, ref new_height);
        SetRect(new_x, new_y, new_width, new_height);
    }

    public readonly PointF CenterPoint() => new(x + width / 2, y + height / 2);

    public void ClampToCenteredSize(in SizeF size)
    {
        float new_width = Math.Min(width, size.width);
        float new_height = Math.Min(height, size.height);
        float new_x = x + (width - new_width) / 2;
        float new_y = y + (height - new_height) / 2;
        SetRect(new_x, new_y, new_width, new_height);
    }

    public void Transpose() => SetRect(y, x, height, width);

    public void SplitVertically(out RectF left_half, out RectF right_half)
    {
        left_half = new RectF(x, y, width / 2, height);
        right_half = new RectF(left_half.right(), y, width - left_half.width, height);
    }

    public void SplitHorizontally(out RectF top_half, out RectF bottom_half)
    {
        top_half = new RectF(x, y, width, height / 2);
        bottom_half = new RectF(x, top_half.bottom(), width, height - top_half.height);
    }

    public readonly bool SharesEdgeWith(in RectF rect) =>
        (y == rect.y && height == rect.height && (x == rect.right() || right() == rect.x)) ||
        (x == rect.x && width == rect.width && (y == rect.bottom() || bottom() == rect.y));

    public readonly float ManhattanDistanceToPoint(in PointF point) =>
        Math.Max(0, Math.Max(x - point.x, point.x - right())) +
        Math.Max(0, Math.Max(y - point.y, point.y - bottom()));

    public float ManhattanInternalDistance(in RectF rect)
    {
        RectF c = this;
        c.Union(rect);
        
        float x = Math.Max(0, c.width - width - rect.width + float.Epsilon);
        float y = Math.Max(0, c.height - height - rect.height + float.Epsilon);
        return x + y;
    }

    public readonly PointF ClosestPoint(in PointF point) =>
        new(Math.Clamp(point.x, x, right()), Math.Clamp(point.y, y, bottom()));

    public void Scale(float scale) => Scale(scale, scale);
    public void Scale(float x_scale, float y_scale)
    {
        origin = PointF.ScalePoint(origin, x_scale, y_scale);
        size = SizeF.ScaleSize(size, x_scale, y_scale);
    }

    public void InvScale(float inv_scale) => InvScale(inv_scale, inv_scale);
    public void InvScale(float inv_x_scale, float inv_y_scale)
    {
        origin.InvScale(inv_x_scale, inv_y_scale);
        size.InvScale(inv_x_scale, inv_y_scale);
    }

    public readonly bool IsExpressibleAsRect() =>
        x >= int.MinValue && x <= int.MaxValue &&
        y >= int.MinValue && y <= int.MaxValue &&
        width >= int.MinValue && width <= int.MaxValue &&
        height >= int.MinValue && height <= int.MaxValue &&
        right() >= int.MinValue && right() <= int.MaxValue &&
        bottom() >= int.MinValue && bottom() <= int.MaxValue;

    public readonly bool ApproximatelyEqual(in RectF rect, float tolerance_x, float tolerance_y) =>
        Math.Abs(x - rect.x) <= tolerance_x && Math.Abs(y - rect.y) <= tolerance_y &&
        Math.Abs(right() - rect.right()) <= tolerance_x && Math.Abs(bottom() - rect.bottom()) <= tolerance_y;

    public override string ToString() => $"{origin_} {size_}";
    public override int GetHashCode() => HashCode.Combine(origin_, size_);
    public override bool Equals(object obj) => obj is RectF other && Equals(other);
    public bool Equals(RectF other) => origin_.Equals(other.origin_) && size_.Equals(other.size_);

    private static void AdjustAlongAxis(float dst_origin, float dst_size, ref float origin, ref float size)
    {
        size = Math.Min(dst_size, size);
        if (origin < dst_origin)
            origin = dst_origin;
        else
            origin = Math.Min(dst_origin + dst_size, origin + size) - size;
    }

    public static bool operator ==(in RectF lhs, in RectF rhs) => lhs.Equals(rhs);
    public static bool operator !=(in RectF lhs, in RectF rhs) => !lhs.Equals(rhs);

    public static RectF operator +(in RectF lhs, in Vector2DF rhs) =>
        new RectF(lhs.x + rhs.x, lhs.y + rhs.y, lhs.width, lhs.height);

    public static RectF operator -(in RectF lhs, in Vector2DF rhs) =>
        new RectF(lhs.x - rhs.x, lhs.y - rhs.y, lhs.width, lhs.height);

    public static RectF IntersectRects(in RectF a, in RectF b)
    {
        RectF result = a;
        result.Intersect(b);
        return result;
    }

    public static RectF UnionRects(in RectF a, in RectF b)
    {
        RectF result = a;
        result.Union(b);
        return result;
    }
    
    public static RectF UnionRects(IEnumerable<RectF> rects)
    {
        RectF result = new RectF();
        foreach (var rect in rects)
            result.Union(rect);
        return result;
    }
    
    public static RectF UnionRectsEvenIfEmpty(in RectF a, in RectF b)
    {
        RectF result = a;
        result.UnionEvenIfEmpty(b);
        return result;
    }

    public static RectF SubtractRects(in RectF a, in RectF b)
    {
        RectF result = a;
        result.Subtract(b);
        return result;
    }

    public static RectF BoundingRect(in PointF p1, in PointF p2)
    {
        float left = Math.Min(p1.x, p2.x);
        float top = Math.Min(p1.y, p2.y);
        float right = Math.Max(p1.x, p2.x);
        float bottom = Math.Max(p1.y, p2.y);
        float width = right - left;
        float height = bottom - top;

        if (left + width != right)
        {
            width = MathF.BitIncrement(width);
        }
        if (top + height != bottom)
        {
            height = MathF.BitIncrement(height);
        }

        return new RectF(left, top, width, height);
    }
    
    public static RectF MaximumCoveredRect(in RectF a, in RectF b)
    {
        RectF maximum = a;
        float maximum_area = a.size.GetArea();
        if (b.size.GetArea() > maximum_area)
        {
            maximum = b;
            maximum_area = b.size.GetArea();
        }
        
        RectF intersection = a;
        intersection.InclusiveIntersect(b);
        if (!intersection.size.IsZero())
        {
            RectF vert_expanded_intersection = intersection;
            vert_expanded_intersection.y = Math.Min(a.y, b.y);
            vert_expanded_intersection.height = Math.Max(a.bottom(), b.bottom()) - vert_expanded_intersection.y;
            if (vert_expanded_intersection.size.GetArea() > maximum_area)
            {
                maximum = vert_expanded_intersection;
                maximum_area = vert_expanded_intersection.size.GetArea();
            }
            RectF horiz_expanded_intersection = intersection;
            horiz_expanded_intersection.x = Math.Min(a.x, b.x);
            horiz_expanded_intersection.width = Math.Max(a.right(), b.right()) - horiz_expanded_intersection.x;
            if (horiz_expanded_intersection.size.GetArea() > maximum_area)
            {
                maximum = horiz_expanded_intersection;
            }
        }
        return maximum;
    }
    
    public static RectF MapRect(in RectF r, in RectF src_rect, in RectF dest_rect)
    {
        if (src_rect.IsEmpty())
            return new RectF();

        float width_scale = dest_rect.width / src_rect.width;
        float height_scale = dest_rect.height / src_rect.height;
        return new RectF(dest_rect.x + (r.x - src_rect.x) * width_scale,
            dest_rect.y + (r.y - src_rect.y) * height_scale,
            r.width * width_scale, r.height * height_scale);
    }
}
