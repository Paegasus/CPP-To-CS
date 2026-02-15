
Insets ScaleToCeiledInsets(const Insets& insets, float x_scale, float y_scale) {
  if (x_scale == 1.f && y_scale == 1.f)
    return insets;
  return ToCeiledInsets(ScaleInsets(InsetsF(insets), x_scale, y_scale));
}

Insets ScaleToCeiledInsets(const Insets& insets, float scale) {
  if (scale == 1.f)
    return insets;
  return ToCeiledInsets(ScaleInsets(InsetsF(insets), scale));
}

Insets ScaleToFlooredInsets(const Insets& insets,
                            float x_scale,
                            float y_scale) {
  if (x_scale == 1.f && y_scale == 1.f)
    return insets;
  return ToFlooredInsets(ScaleInsets(InsetsF(insets), x_scale, y_scale));
}

Insets ScaleToFlooredInsets(const Insets& insets, float scale) {
  if (scale == 1.f)
    return insets;
  return ToFlooredInsets(ScaleInsets(InsetsF(insets), scale));
}

Insets ScaleToRoundedInsets(const Insets& insets,
                            float x_scale,
                            float y_scale) {
  if (x_scale == 1.f && y_scale == 1.f)
    return insets;
  return ToRoundedInsets(ScaleInsets(InsetsF(insets), x_scale, y_scale));
}

Insets ScaleToRoundedInsets(const Insets& insets, float scale) {
  if (scale == 1.f)
    return insets;
  return ToRoundedInsets(ScaleInsets(InsetsF(insets), scale));
}

}  // namespace gfx
