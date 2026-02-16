using static UI.Numerics.ClampedMath;
using static UI.Numerics.SafeConversions;
using static UI.GFX.Geometry.RectConversions;

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
        size_ = new Size(ClampWidthOrHeight(origin.x, size.width), ClampWidthOrHeight(origin.y, size.height));
    }

    public readonly int x() => origin_.x;

    // Sets the X position while preserving the width.
    public void set_x(int x)
    {
        origin_.x = x;
        size_.width = ClampWidthOrHeight(x, width());
    }

    public readonly int y() => origin_.y;

    // Sets the Y position while preserving the height.
    public void set_y(int y)
    {
        origin_.y = y;
        size_.height = ClampWidthOrHeight(y, height());
    }

    public readonly int width() => size_.width;
    public void set_width(int width) => size_.width = ClampWidthOrHeight(x(), width);

    public readonly int height() => size_.height;
    public void set_height(int height) => size_.height = ClampWidthOrHeight(y(), height);

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
        set_width(size.width);
        set_height(size.height);
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

    // Use in place of SetRect() when you know the edges of the rectangle instead
    // of the dimensions, rather than trying to determine the width/height
    // yourself. This safely handles cases where the width/height would overflow.
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

    // Shrink the rectangle by |inset| on all sides.
    public void Inset(int inset) => Inset(new Insets(inset));

    // Shrink the rectangle by the given |insets|
    public void Inset(in Insets insets)
    {
        origin_ += new Vector2D(insets.left(), insets.top());
        set_width(ClampSub(width(), insets.width()));
        set_height(ClampSub(height(), insets.height()));
    }

    // Expand the rectangle by |outset| on all sides.
    public void Outset(int outset) => Inset(-outset);

    // Expand the rectangle by the given |outsets|.
    public void Outset(in Outsets outsets) => Inset(outsets.ToInsets());

    // Move the rectangle by a horizontal and vertical distance.
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

    // Returns true if the area of the rectangle is zero.
    public readonly bool IsEmpty() => size_.IsEmpty();

    // Returns true if the point identified by point_x and point_y falls inside
    // this rectangle.  The point (x, y) is inside the rectangle, but the
    // point (x + width, y + height) is not.
    public readonly bool Contains(int point_x, int point_y) => (point_x >= x()) && (point_x < right()) && (point_y >= y()) && (point_y < bottom());

    // Returns true if the specified point is contained by this rectangle.
    public readonly bool Contains(in Point point) => Contains(point.x, point.y);

    // Returns true if this rectangle contains the specified rectangle.
    public readonly bool Contains(in Rect rect) => rect.x() >= x() && rect.right() <= right() && rect.y() >= y() && rect.bottom() <= bottom();

    // Returns true if this rectangle intersects the specified rectangle.
    // An empty rectangle doesn't intersect any rectangle.
    public readonly bool Intersects(in Rect rect) => !(IsEmpty() || rect.IsEmpty() || rect.x() >= right() || rect.right() <= x() || rect.y() >= bottom() || rect.bottom() <= y());

    // Sets this rect to be the intersection of this rectangle with the given rectangle.
    public void Intersect(in Rect rect)
    {
        if (IsEmpty() || rect.IsEmpty())
        {
            SetRect(0, 0, 0, 0); // Throws away empty position.
            return;
        }

        int left = Math.Max(x(), rect.x());
        int top = Math.Max(y(), rect.y());
        int new_right = Math.Min(right(), rect.right());
        int new_bottom = Math.Min(bottom(), rect.bottom());

        if (left >= new_right || top >= new_bottom)
        {
            SetRect(0, 0, 0, 0); // Throws away empty position.
            return;
        }

        SetByBounds(left, top, new_right, new_bottom);
    }

    // Sets this rect to be the intersection of itself and |rect| using
    // edge-inclusive geometry.  If the two rectangles overlap but the overlap
    // region is zero-area (either because one of the two rectangles is zero-area,
    // or because the rectangles overlap at an edge or a corner), the result is
    // the zero-area intersection.  The return value indicates whether the two
    // rectangle actually have an intersection, since checking the result for
    // isEmpty() is not conclusive.
    public bool InclusiveIntersect(in Rect rect)
    {
        int left = Math.Max(x(), rect.x());
        int top = Math.Max(y(), rect.y());
        int new_right = Math.Min(right(), rect.right());
        int new_bottom = Math.Min(bottom(), rect.bottom());

        // Return a clean empty rectangle for non-intersecting cases.
        if (left > new_right || top > new_bottom)
        {
            SetRect(0, 0, 0, 0);
            return false;
        }

        SetByBounds(left, top, new_right, new_bottom);
        return true;
    }

    // Sets this rect to be the union of this rectangle with the given rectangle.
    // The union is the smallest rectangle containing both rectangles if not
    // empty. If both rects are empty, this rect will become |rect|.
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

    // Similar to Union(), but the result will contain both rectangles even if
    // either of them is empty. For example, union of (100, 100, 0x0) and
    // (200, 200, 50x0) is (100, 100, 150x100).
    public void UnionEvenIfEmpty(in Rect rect)
    {
        SetByBounds(Math.Min(x(), rect.x()), Math.Min(y(), rect.y()),
                    Math.Max(right(), rect.right()),
                    Math.Max(bottom(), rect.bottom()));
    }

    // Sets this rect to be the rectangle resulting from subtracting |rect| from
    // |*this|, i.e. the bounding rect of |Region(*this) - Region(rect)|.
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
            // complete intersection in the y-direction
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
            // complete intersection in the x-direction
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

    // Fits as much of the receiving rectangle into the supplied rectangle as
    // possible, becoming the result. For example, if the receiver had
    // a x-location of 2 and a width of 4, and the supplied rectangle had
    // an x-location of 0 with a width of 5, the returned rectangle would have
    // an x-location of 1 with a width of 4.
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

    // Returns the center of this rectangle.
    public readonly Point CenterPoint() => new Point(x() + width() / 2, y() + height() / 2);

    // Becomes a rectangle that has the same center point but with a |size|.
    public void ToCenteredSize(in Size size)
    {
        int new_x = x() + (width() - size.width) / 2;
        int new_y = y() + (height() - size.height) / 2;
        SetRect(new_x, new_y, size.width, size.height);
    }

    // Becomes a rectangle that has the same center point but with a size capped at given |size|.
    public void ClampToCenteredSize(in Size to_size)
    {
        Size new_size = size();
        new_size.SetToMin(to_size);
        ToCenteredSize(new_size);
    }

    // Transpose x and y axis.
    public void Transpose() => SetRect(y(), x(), height(), width());

    // Splits `this` in two halves, `left_half` and `right_half`.
    public void SplitVertically(out Rect left_half, out Rect right_half)
    {
        left_half = new Rect(x(), y(), width() / 2, height());
        right_half = new Rect(left_half.right(), y(), width() - left_half.width(), height());
    }

    // Splits `this` in two halves, `top_half` and `bottom_half`.
    public void SplitHorizontally(out Rect top_half, out Rect bottom_half)
    {
        top_half = new Rect(x(), y(), width(), height() / 2);
        bottom_half = new Rect(x(), top_half.bottom(), width(), height() - top_half.height());
    }

    // Returns true if this rectangle shares an entire edge (i.e., same width or same height)
    // with the given rectangle, and the rectangles do not overlap.
    public readonly bool SharesEdgeWith(in Rect rect) => (y() == rect.y() && height() == rect.height() && (x() == rect.right() || right() == rect.x())) || (x() == rect.x() && width() == rect.width() && (y() == rect.bottom() || bottom() == rect.y()));

    // Returns the manhattan distance from the rect to the point. If the point is inside the rect, returns 0.
    public readonly int ManhattanDistanceToPoint(in Point point)
    {
        int x_distance = Math.Max(0, Math.Max(x() - point.x, point.x - right()));
        int y_distance = Math.Max(0, Math.Max(y() - point.y, point.y - bottom()));

        return x_distance + y_distance;
    }

    // Returns the manhattan distance between the contents of this rect and the
    // contents of the given rect. That is, if the intersection of the two rects
    // is non-empty then the function returns 0. If the rects share a side, it
    // returns the smallest non-zero value appropriate for int.
    public readonly int ManhattanInternalDistance(in Rect rect)
    {
        Rect c = this;
        c.Union(rect);

        int x = Math.Max(0, c.width() - width() - rect.width() + 1);
        int y = Math.Max(0, c.height() - height() - rect.height() + 1);
        
        return x + y;
    }

    public override readonly string ToString() => $"{origin_} {size_}";

    public readonly bool ApproximatelyEqual(in Rect rect, int tolerance) => Math.Abs(x() - rect.x()) <= tolerance && Math.Abs(y() - rect.y()) <= tolerance && Math.Abs(right() - rect.right()) <= tolerance && Math.Abs(bottom() - rect.bottom()) <= tolerance;

    public readonly bool Equals(Rect other) => origin_.Equals(other.origin_) && size_.Equals(other.size_);
    public override readonly bool Equals(object? obj) => obj is Rect other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(origin_, size_);
    public static bool operator ==(in Rect lhs, in Rect rhs) => lhs.Equals(rhs);
    public static bool operator !=(in Rect lhs, in Rect rhs) => !lhs.Equals(rhs);

    // Clamp the width/height to avoid integer overflow in bottom() and right().
    // This returns the clamped width/height given an |x_or_y| and a
    // |width_or_height|.
    private static int ClampWidthOrHeight(int x_or_y, int width_or_height) => ClampAdd(x_or_y, width_or_height) - x_or_y;

    private void AdjustForSaturatedRight(int right)
    {
        int new_x, width;
        SaturatedClampRange(x(), right, out new_x, out width);
        set_x(new_x);
        size_.width = width;
    }

    private void AdjustForSaturatedBottom(int bottom)
    {
        int new_y, height;
        SaturatedClampRange(y(), bottom, out new_y, out height);
        set_y(new_y);
        size_.height = height;
    }
    
    private static void AdjustAlongAxis(int dst_origin, int dst_size, ref int origin, ref int size)
    {
      size = Math.Min(dst_size, size);

      if (origin < dst_origin)
        origin = dst_origin;
      else
        origin = Math.Min(dst_origin + dst_size, origin + size) - size;
    }

    // This is the per-axis heuristic for picking the most useful origin and width/height to represent the input range.
    private static void SaturatedClampRange(int min, int max, out int origin, out int span)
    {
        if (max < min)
        {
            span = 0;
            origin = min;
            return;
        }

        int effective_span = ClampSub(max, min);
        int span_loss = ClampSub(max, min + effective_span);

        // If the desired width is within the limits of ints,
        // we can just use the simple computations to represent the range precisely.
        if (span_loss == 0)
        {
            span = effective_span;
            origin = min;
            return;
        }

        // Now we have to approximate. If one of min or max is close enough
        // to zero we choose to represent that one precisely.
        // The other side is probably practically "infinite", so we move it.
        const uint MaxDimension = int.MaxValue / 2;
        if (SafeUnsignedAbs(max) < MaxDimension)
        {
            // Maintain origin + span == max.
            span = effective_span;
            origin = max - effective_span;
        }
        else if (SafeUnsignedAbs(min) < MaxDimension)
        {
            // Maintain origin == min.
            span = effective_span;
            origin = min;
        }
        else
        {
            // Both are big, so keep the center.
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

    // Note: This originally uses base::span<const Rect> in C++, consider using a C# Span type
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

    // Constructs a rectangle with |p1| and |p2| as opposite corners.
    //
    // This could also be thought of as "the smallest rect that contains both
    // points", except that we consider points on the right/bottom edges of the
    // rect to be outside the rect.  So technically one or both points will not be
    // contained within the rect, because they will appear on one of these edges.
    public static Rect BoundingRect(in Point p1, in Point p2)
    {
        Rect result = new();
        result.SetByBounds(Math.Min(p1.x, p2.x), Math.Min(p1.y, p2.y),
                         Math.Max(p1.x, p2.x), Math.Max(p1.y, p2.y));
        return result;
    }

    // Scales the rect and returns the enclosing rect. The components are clamped if they would overflow.
    public static Rect ScaleToEnclosingRect(in Rect rect, float x_scale, float y_scale)
    {
        if (x_scale == 1f && y_scale == 1f)
            return rect;
        int x = ClampFloor(rect.x() * x_scale);
        int y = ClampFloor(rect.y() * y_scale);
        int r = rect.width() == 0 ? x : ClampCeil(rect.right() * x_scale);
        int b = rect.height() == 0 ? y : ClampCeil(rect.bottom() * y_scale);
        Rect result = new();
        result.SetByBounds(x, y, r, b);
        return result;
    }
    
    public static Rect ScaleToEnclosingRect(in Rect rect, float scale) => ScaleToEnclosingRect(rect, scale, scale);
    
    public static Rect ScaleToEnclosedRect(in Rect rect, float x_scale, float y_scale)
    {
        if (x_scale == 1f && y_scale == 1f)
            return rect;
        int x = ClampCeil(rect.x() * x_scale);
        int y = ClampCeil(rect.y() * y_scale);
        int r = rect.width() == 0 ? x : ClampFloor(rect.right() * x_scale);
        int b = rect.height() == 0 ? y : ClampFloor(rect.bottom() * y_scale);
        Rect result = new();
        result.SetByBounds(x, y, r, b);
        return result;
    }
    
    public static Rect ScaleToEnclosedRect(in Rect rect, float scale) => ScaleToEnclosedRect(rect, scale, scale);

    // Scales |rect| by scaling its four corner points. If the corner points lie on
    // non-integral coordinate after scaling, their values are rounded to the
    // nearest integer. The components are clamped if they would overflow.
    // This is helpful during layout when relative positions of multiple gfx::Rect
    // in a given coordinate space needs to be same after scaling as it was before
    // scaling. ie. this gives a lossless relative positioning of rects.
    public static Rect ScaleToRoundedRect(in Rect rect, float x_scale, float y_scale)
    {
        if (x_scale == 1f && y_scale == 1f)
            return rect;
        int x = ClampRound(rect.x() * x_scale);
        int y = ClampRound(rect.y() * y_scale);
        int r = rect.width() == 0 ? x : ClampRound(rect.right() * x_scale);
        int b = rect.height() == 0 ? y : ClampRound(rect.bottom() * y_scale);
        Rect result = new Rect();
        result.SetByBounds(x, y, r, b);
        return result;
    }
    
    public static Rect ScaleToRoundedRect(in Rect rect, float scale) => ScaleToRoundedRect(rect, scale, scale);

    // Scales `rect` by `scale` and rounds to enclosing rect, but for each edge, if
    // the distance between the edge and the nearest integer grid is smaller than
    // `error`, the edge is snapped to the integer grid.  The default error is 0.001
    // , which is used by cc/viz. Use this when scaling the window/layer size.
    public static Rect ScaleToEnclosingRectIgnoringError(in Rect rect, float scale, float error = 0.001f)
    {
        RectF rect_f = rect;
        rect_f.Scale(scale);
        return ToEnclosingRectIgnoringError(rect_f, epsilon);
    }

    // Return a maximum rectangle that is covered by the a or b.
    public static Rect MaximumCoveredRect(in Rect a, in Rect b)
    {
        // Check a or b by itself.
        Rect maximum = a;
        uint64_t maximum_area = a.size().Area64();
        if (b.size().Area64() > maximum_area)
        {
            maximum = b;
            maximum_area = b.size().Area64();
        }
        // Check the regions that include the intersection of a and b. This can be
        // done by taking the intersection and expanding it vertically and
        // horizontally. These expanded intersections will both still be covered by
        // a or b.
        Rect intersection = a;
        intersection.InclusiveIntersect(b);
        if (!intersection.size().IsZero())
        {
            Rect vert_expanded_intersection = intersection;
            vert_expanded_intersection.SetVerticalBounds(
                std::min(a.y(), b.y()), std::max(a.bottom(), b.bottom()));
            if (vert_expanded_intersection.size().Area64() > maximum_area)
            {
                maximum = vert_expanded_intersection;
                maximum_area = vert_expanded_intersection.size().Area64();
            }
            Rect horiz_expanded_intersection = intersection;
            horiz_expanded_intersection.SetHorizontalBounds(
                std::min(a.x(), b.x()), std::max(a.right(), b.right()));
            if (horiz_expanded_intersection.size().Area64() > maximum_area)
            {
                maximum = horiz_expanded_intersection;
                maximum_area = horiz_expanded_intersection.size().Area64();
            }
        }
        return maximum;
    }
}
