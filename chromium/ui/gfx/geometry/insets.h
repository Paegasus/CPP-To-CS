

  // Adjusts the vertical and horizontal dimensions by the values described in
  // |vector|. Offsetting insets before applying to a rectangle would be
  // equivalent to offsetting the rectangle then applying the insets.
  void Offset(const gfx::Vector2d& vector);

  explicit operator InsetsF() const {
    return InsetsF()
        .set_top(static_cast<float>(top()))
        .set_left(static_cast<float>(left()))
        .set_bottom(static_cast<float>(bottom()))
        .set_right(static_cast<float>(right()));
  }
};

inline Insets operator+(Insets lhs, const Insets& rhs) {
  lhs += rhs;
  return lhs;
}

inline Insets operator-(Insets lhs, const Insets& rhs) {
  lhs -= rhs;
  return lhs;
}

inline Insets operator+(Insets insets, const gfx::Vector2d& offset) {
  insets.Offset(offset);
  return insets;
}

// Helper methods to scale a gfx::Insets to a new gfx::Insets.
COMPONENT_EXPORT(GEOMETRY)
Insets ScaleToCeiledInsets(const Insets& insets, float x_scale, float y_scale);
COMPONENT_EXPORT(GEOMETRY)
Insets ScaleToCeiledInsets(const Insets& insets, float scale);
COMPONENT_EXPORT(GEOMETRY)
Insets ScaleToFlooredInsets(const Insets& insets, float x_scale, float y_scale);
COMPONENT_EXPORT(GEOMETRY)
Insets ScaleToFlooredInsets(const Insets& insets, float scale);
COMPONENT_EXPORT(GEOMETRY)
Insets ScaleToRoundedInsets(const Insets& insets, float x_scale, float y_scale);
COMPONENT_EXPORT(GEOMETRY)
Insets ScaleToRoundedInsets(const Insets& insets, float scale);

// This is declared here for use in gtest-based unit tests but is defined in
// the //ui/gfx:test_support target. Depend on that to use this in your unit
// test. This should not be used in production code - call ToString() instead.
void PrintTo(const Insets&, ::std::ostream* os);

}  // namespace gfx

#endif  // UI_GFX_GEOMETRY_INSETS_H_
