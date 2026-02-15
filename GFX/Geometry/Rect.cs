using System;
using System.Collections.Generic;
using UI.Numerics;
using static UI.Numerics.ClampedMath;

namespace UI.GFX.Geometry;

public struct Rect : IEquatable<Rect>
{
    private Point origin_;
    private Size size_;

    public Rect(int width, int height)
    {
        origin_ = new Point();
        size_ = new Size(width, height);
    }

    public Rect(int x, int y, int width, int height)
    {
        origin_ = new Point(x, y);
        size_ = new Size(ClampWidthOrHeight(x, width), ClampWidthOrHeight(y, height));
    }

    public Rect(Size size)
    {
        origin_ = new Point();
        size_ = size;
    }

    public Rect(Point origin, Size size)
    {
        origin_ = origin;
        size_ = new Size(ClampWidthOrHeight(origin.x(), size.width()), ClampWidthOrHeight(origin.y(), size.height()));
    }

    public readonly int x() => origin_.x();
    public void set_x(int x)
    {
        origin_.set_x(x);
        size_.set_width(ClampWidthOrHeight(x, width()));
    }

    public readonly int y() => origin_.y();
    public void set_y(int y)
    {
        origin_.set_y(y);
        size_.set_height(ClampWidthOrHeight(y, height()));
    }

    public readonly int width() => size_.width();
    public void set_width(int width) => size_.set_width(ClampWidthOrHeight(x(), width));

    public readonly int height() => size_.height();
    public void set_height(int height) => size_.set_height(ClampWidthOrHeight(y(), height));

    public readonly Point origin() => origin_;
    public void set_origin(Point origin)
    {
        origin_ = origin;
        // Ensure that width and height remain valid.
        set_width(width());
        set_height(height());
    }

    public readonly Size size() => size_;
    public void set_size(Size size)
    {
        set_width(size.width());
        set_height(size.height());
    }

    public readonly int right() => x() + width();
    public readonly int bottom() => y() + height();

    public readonly Point top_right() => new Point(right(), y());
    public readonly Point bottom_left() => new Point(x(), bottom());
    public readonly Point bottom_right() => new Point(right(), bottom());

    public readonly Point left_center() => new Point(x(), y() + height() / 2);
    public readonly Point top_center() => new Point(x() + width() / 2, y());
    public readonly Point right_center() => new Point(right(), y() + height() / 2);
    public readonly Point bottom_center() => new Point(x() + width() / 2, bottom());

    public readonly Vector2D OffsetFromOrigin() => new Vector2D(x(), y());

    public void SetRect(int x, int y, int width, int height)
    {
        origin_.SetPoint(x, y);
        // Ensure that width and height remain valid.
        set_width(width);
        set_height(height);
    }

    public void SetByBounds(int left, int top, int right, int bottom)
    {
        SetHorizontalBounds(left, right);
        SetVerticalBounds(top, bottom);
    }

    public void SetHorizontalBounds(int left, int right)
    {
        set_x(left);
        set_width(ClampSub(right, left));
        if (this.right() != right)
        {
            AdjustForSaturatedRight(right);
        }
    }

    public void SetVerticalBounds(int top, int bottom)
    {
        set_y(top);
        set_height(ClampSub(bottom, top));
        if (this.bottom() != bottom)
        {
            AdjustForSaturatedBottom(bottom);
        }
    }

    public void Inset(int inset) => Inset(new Insets(inset));
    public void Inset(in Insets insets)
    {
        origin_ += new Vector2D(insets.left(), insets.top());
        set_width(ClampSub(width(), insets.width()));
        set_height(ClampSub(height(), insets.height()));
    }

    public void Outset(int outset) => Inset(-outset);
    public void Outset(in Outsets outsets) => Inset(outsets.ToInsets());

    public void Offset(int horizontal, int vertical) => Offset(new Vector2D(horizontal, vertical));
    public void Offset(in Vector2D distance)
    {
        origin_ += distance;
        // Ensure that width and height remain valid.
        set_width(width());
        set_height(height());
    }

    public static Rect operator +(Rect lhs, in Vector2D rhs)
    {
        lhs.Offset(rhs);
        return lhs;
    }

    public static Rect operator -(Rect lhs, in Vector2D rhs)
    {
        lhs.Offset(-rhs);
        return lhs;
    }

    public readonly Insets InsetsFrom(in Rect inner) => Insets.TLBR(inner.y() - y(), inner.x() - x(), bottom() - inner.bottom(), right() - inner.right());

    public readonly bool IsEmpty() => size_.IsEmpty();

    public readonly bool Contains(int point_x, int point_y) => (point_x >= x()) && (point_x < right()) && (point_y >= y()) && (point_y < bottom());

    public readonly bool Contains(in Point point) => Contains(point.x(), point.y());

    public readonly bool Contains(in Rect rect) => (rect.x() >= x() && rect.right() <= right() && rect.y() >= y() && rect.bottom() <= bottom());

    public readonly bool Intersects(in Rect rect) => !(IsEmpty() || rect.IsEmpty() || rect.x() >= right() || rect.right() <= x() || rect.y() >= bottom() || rect.bottom() <= y());

    public void Intersect(in Rect rect)
    {
        if (IsEmpty() || rect.IsEmpty())
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        int left = Math.Max(x(), rect.x());
        int top = Math.Max(y(), rect.y());
        int new_right = Math.Min(right(), rect.right());
        int new_bottom = Math.Min(bottom(), rect.bottom());

        if (left >= new_right || top >= new_bottom)
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        SetByBounds(left, top, new_right, new_bottom);
    }

    public bool InclusiveIntersect(in Rect rect)
    {
        int left = Math.Max(x(), rect.x());
        int top = Math.Max(y(), rect.y());
        int new_right = Math.Min(right(), rect.right());
        int new_bottom = Math.Min(bottom(), rect.bottom());

        if (left > new_right || top > new_bottom)
        {
            SetRect(0, 0, 0, 0);
            return false;
        }

        SetByBounds(left, top, new_right, new_bottom);
        return true;
    }

    public void Union(in Rect rect)
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

    public void UnionEvenIfEmpty(in Rect rect)
    {
        SetByBounds(Math.Min(x(), rect.x()), Math.Min(y(), rect.y()),
                    Math.Max(right(), rect.right()),
                    Math.Max(bottom(), rect.bottom()));
    }

    public void Subtract(in Rect rect)
    {
        if (!Intersects(rect))
            return;
        if (rect.Contains(this))
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        int rx = x();
        int ry = y();
        int rr = right();
        int rb = bottom();

        if (rect.y() <= y() && rect.bottom() >= bottom())
        {
            if (rect.x() <= x())
            {
                rx = rect.right();
            }
            else if (rect.right() >= right())
            {
                rr = rect.x();
            }
        }
        else if (rect.x() <= x() && rect.right() >= right())
        {
            if (rect.y() <= y())
            {
                ry = rect.bottom();
            }
            else if (rect.bottom() >= bottom())
            {
                rb = rect.y();
            }
        }
        SetByBounds(rx, ry, rr, rb);
    }

    public void AdjustToFit(in Rect rect)
    {
        int new_x = x();
        int new_y = y();
        int new_width = width();
        int new_height = height();
        AdjustAlongAxis(rect.x(), rect.width(), ref new_x, ref new_width);
        AdjustAlongAxis(rect.y(), rect.height(), ref new_y, ref new_height);
        SetRect(new_x, new_y, new_width, new_height);
    }

    public readonly Point CenterPoint() => new Point(x() + width() / 2, y() + height() / 2);

    public void ToCenteredSize(in Size size)
    {
        int new_x = x() + (width() - size.width()) / 2;
        int new_y = y() + (height() - size.height()) / 2;
        SetRect(new_x, new_y, size.width(), size.height());
    }

    public void ClampToCenteredSize(in Size to_size)
    {
        Size new_size = size();
        new_size.SetToMin(to_size);
        ToCenteredSize(new_size);
    }

    public void Transpose() => SetRect(y(), x(), height(), width());

    public void SplitVertically(out Rect left_half, out Rect right_half)
    {
        left_half = new Rect(x(), y(), width() / 2, height());
        right_half = new Rect(left_half.right(), y(), width() - left_half.width(), height());
    }

    public void SplitHorizontally(out Rect top_half, out Rect bottom_half)
    {
        top_half = new Rect(x(), y(), width(), height() / 2);
        bottom_half = new Rect(x(), top_half.bottom(), width(), height() - top_half.height());
    }

    public readonly bool SharesEdgeWith(in Rect rect) => (y() == rect.y() && height() == rect.height() && (x() == rect.right() || right() == rect.x())) || (x() == rect.x() && width() == rect.width() && (y() == rect.bottom() || bottom() == rect.y()));

    public readonly int ManhattanDistanceToPoint(in Point point)
    {
        int x_distance = Math.Max(0, Math.Max(x() - point.x(), point.x() - right()));
        int y_distance = Math.Max(0, Math.Max(y() - point.y(), point.y() - bottom()));

        return x_distance + y_distance;
    }

    public int ManhattanInternalDistance(in Rect rect)
    {
        Rect c = this;
        c.Union(rect);

        int x = Math.Max(0, c.width() - width() - rect.width() + 1);
        int y = Math.Max(0, c.height() - height() - rect.height() + 1);
        return x + y;
    }
    public override readonly string ToString() => $"{origin_.ToString()} {size_.ToString()}";

    public readonly bool ApproximatelyEqual(in Rect rect, int tolerance) => Math.Abs(x() - rect.x()) <= tolerance && Math.Abs(y() - rect.y()) <= tolerance && Math.Abs(right() - rect.right()) <= tolerance && Math.Abs(bottom() - rect.bottom()) <= tolerance;

    public readonly bool Equals(Rect other) => origin_.Equals(other.origin_) && size_.Equals(other.size_);
    public override readonly bool Equals(object? obj) => obj is Rect other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(origin_, size_);
    public static bool operator ==(in Rect lhs, in Rect rhs) => lhs.Equals(rhs);
    public static bool operator !=(in Rect lhs, in Rect rhs) => !lhs.Equals(rhs);

    private static int ClampWidthOrHeight(int x_or_y, int width_or_height) => ClampAdd(x_or_y, width_or_height) - x_or_y;

    private void AdjustForSaturatedRight(int right)
    {
        int new_x, width;
        SaturatedClampRange(x(), right, out new_x, out width);
        set_x(new_x);
        size_.set_width(width);
    }

    private void AdjustForSaturatedBottom(int bottom)
    {
        int new_y, height;
        SaturatedClampRange(y(), bottom, out new_y, out height);
        set_y(new_y);
        size_.set_height(height);
    }
    
    private static void AdjustAlongAxis(int dst_origin, int dst_size, ref int origin, ref int size) {
      size = Math.Min(dst_size, size);
      if (origin < dst_origin)
        origin = dst_origin;
      else
        origin = Math.Min(dst_origin + dst_size, origin + size) - size;
    }
    
    private static void SaturatedClampRange(int min, int max, out int origin, out int span) {
      if (max < min) {
        span = 0;
        origin = min;
        return;
      }

      int effective_span = ClampSub(max, min);
      int span_loss = ClampSub(max, min + effective_span);
      
      if (span_loss == 0) {
        span = effective_span;
        origin = min;
        return;
      }
      
      const uint kMaxDimension = int.MaxValue / 2;
      if (Math.Abs((uint)max) < kMaxDimension) {
        span = effective_span;
        origin = max - effective_span;
      } else if (Math.Abs((uint)min) < kMaxDimension) {
        span = effective_span;
        origin = min;
      } else {
        span = effective_span;
        origin = min + span_loss / 2;
      }
    }

    public static Rect IntersectRects(in Rect a, in Rect b)
    {
        Rect result = a;
        result.Intersect(b);
        return result;
    }

    public static Rect UnionRects(in Rect a, in Rect b)
    {
        Rect result = a;
        result.Union(b);
        return result;
    }

    public static Rect UnionRects(IEnumerable<Rect> rects)
    {
        Rect result = new Rect();
        foreach (var rect in rects)
        {
            result.Union(rect);
        }
        return result;
    }

    public static Rect UnionRectsEvenIfEmpty(in Rect a, in Rect b)
    {
        Rect result = a;
        result.UnionEvenIfEmpty(b);
        return result;
    }

    public static Rect SubtractRects(in Rect a, in Rect b)
    {
        Rect result = a;
        result.Subtract(b);
        return result;
    }

    public static Rect BoundingRect(in Point p1, in Point p2)
    {
        Rect result = new Rect();
        result.SetByBounds(Math.Min(p1.x(), p2.x()), Math.Min(p1.y(), p2.y()),
                         Math.Max(p1.x(), p2.x()), Math.Max(p1.y(), p2.y()));
        return result;
    }

    public static Rect ScaleToEnclosingRect(in Rect rect, float x_scale, float y_scale)
    {
        if (x_scale == 1f && y_scale == 1f)
            return rect;
        int x = (int)Math.Floor(rect.x() * x_scale);
        int y = (int)Math.Floor(rect.y() * y_scale);
        int r = rect.width() == 0 ? x : (int)Math.Ceiling(rect.right() * x_scale);
        int b = rect.height() == 0 ? y : (int)Math.Ceiling(rect.bottom() * y_scale);
        Rect result = new Rect();
        result.SetByBounds(x, y, r, b);
        return result;
    }
    
    public static Rect ScaleToEnclosingRect(in Rect rect, float scale) => ScaleToEnclosingRect(rect, scale, scale);
    
    public static Rect ScaleToEnclosedRect(in Rect rect, float x_scale, float y_scale)
    {
        if (x_scale == 1f && y_scale == 1f)
            return rect;
        int x = (int)Math.Ceiling(rect.x() * x_scale);
        int y = (int)Math.Ceiling(rect.y() * y_scale);
        int r = rect.width() == 0 ? x : (int)Math.Floor(rect.right() * x_scale);
        int b = rect.height() == 0 ? y : (int)Math.Floor(rect.bottom() * y_scale);
        Rect result = new Rect();
        result.SetByBounds(x, y, r, b);
        return result;
    }
    
    public static Rect ScaleToEnclosedRect(in Rect rect, float scale) => ScaleToEnclosedRect(rect, scale, scale);
    
    public static Rect ScaleToRoundedRect(in Rect rect, float x_scale, float y_scale)
    {
        if (x_scale == 1f && y_scale == 1f)
            return rect;
        int x = (int)Math.Round(rect.x() * x_scale);
        int y = (int)Math.Round(rect.y() * y_scale);
        int r = rect.width() == 0 ? x : (int)Math.Round(rect.right() * x_scale);
        int b = rect.height() == 0 ? y : (int)Math.Round(rect.bottom() * y_scale);
        Rect result = new Rect();
        result.SetByBounds(x, y, r, b);
        return result;
    }
    
    public static Rect ScaleToRoundedRect(in Rect rect, float scale) => ScaleToRoundedRect(rect, scale, scale);
}
