template <unsigned fractional_bits, typename Storage>
class FixedPoint {
	
	// Remove the rest of the class to keep it bried and focus on ClampRawValue()

 public:

    template <typename T>
    static constexpr Storage ClampRawValue(T raw_value)
    {
	    return base::saturated_cast<Storage>(raw_value);
    }
}

// saturated_cast<> is analogous to static_cast<> for numeric types, except
// that the specified numeric conversion will saturate by default rather than
// overflow or underflow, and NaN assignment to an integral will return 0.
// All boundary condition behaviors can be overridden with a custom handler.
template <typename Dst, template <typename> class SaturationHandler = SaturationDefaultLimits, typename Src>
constexpr Dst saturated_cast(Src value)
{
  using SrcType = UnderlyingType<Src>;
  
  const auto underlying_value = static_cast<SrcType>(value);
  
  return !std::is_constant_evaluated() && SaturateFastOp<Dst, SrcType>::is_supported && std::is_same_v<SaturationHandler<Dst>, SaturationDefaultLimits<Dst>>
             ? SaturateFastOp<Dst, SrcType>::Do(underlying_value)
             : saturated_cast_impl<Dst, SaturationHandler, SrcType>(underlying_value, DstRangeRelationToSrcRange<Dst, SaturationHandler, SrcType>(underlying_value));
}

template <typename Dst, template <typename> class S, typename Src>
constexpr Dst saturated_cast_impl(Src value, RangeCheck constraint) {
  // For some reason clang generates much better code when the branch is
  // structured exactly this way, rather than a sequence of checks.
  return !constraint.IsOverflowFlagSet() ? (!constraint.IsUnderflowFlagSet() ? static_cast<Dst>(value) : S<Dst>::Underflow())
             // Skip this check for integral Src, which cannot be NaN.
             : (std::is_integral_v<Src> || !constraint.IsUnderflowFlagSet() ? S<Dst>::Overflow() : S<Dst>::NaN());
}

// We can reduce the number of conditions and get slightly better performance
// for normal signed and unsigned integer ranges. And in the specific case of
// Arm, we can use the optimized saturation instructions.
template <typename Dst, typename Src>
struct SaturateFastOp {
  static constexpr bool is_supported = false;
  static constexpr Dst Do(Src) {
    // Force a compile failure if instantiated.
    return CheckOnFailure::template HandleFailure<Dst>();
  }
};

template <typename Dst, typename Src>
  requires(std::integral<Src> && std::integral<Dst> && SaturateFastAsmOp<Dst, Src>::is_supported)
struct SaturateFastOp<Dst, Src> {
  static constexpr bool is_supported = true;
  static constexpr Dst Do(Src value) {
    return SaturateFastAsmOp<Dst, Src>::Do(value);
  }
};

template <typename Dst, typename Src>
  requires(std::integral<Src> && std::integral<Dst> && !SaturateFastAsmOp<Dst, Src>::is_supported)
struct SaturateFastOp<Dst, Src> {
  static constexpr bool is_supported = true;
  static constexpr Dst Do(Src value) {
    // The exact order of the following is structured to hit the correct
    // optimization heuristics across compilers. Do not change without
    // checking the emitted code.
    const Dst saturated = CommonMaxOrMin<Dst, Src>(
        kIsMaxInRangeForNumericType<Dst, Src> ||
        (!kIsMinInRangeForNumericType<Dst, Src> && IsValueNegative(value)));
    if (IsValueInRangeForNumericType<Dst>(value)) [[likely]] {
      return static_cast<Dst>(value);
    }
    return saturated;
  }
};

// Default boundaries for integral/float: max/infinity, lowest/-infinity, 0/NaN.
// You may provide your own limits (e.g. to saturated_cast) so long as you
// implement all of the static constexpr member functions in the class below.
template <typename T>
struct SaturationDefaultLimits : public std::numeric_limits<T>
{
  static constexpr T NaN()
  {
    if constexpr (std::numeric_limits<T>::has_quiet_NaN)
	{
      return std::numeric_limits<T>::quiet_NaN();
    }
	else
	{
      return T();
    }
  }
  
  using std::numeric_limits<T>::max;
  
  static constexpr T Overflow()
  {
    if constexpr (std::numeric_limits<T>::has_infinity)
	{
      return std::numeric_limits<T>::infinity();
    }
	else
	{
      return std::numeric_limits<T>::max();
    }
  }
  
  using std::numeric_limits<T>::lowest;
  
  static constexpr T Underflow()
  {
    if constexpr (std::numeric_limits<T>::has_infinity)
	{
      return std::numeric_limits<T>::infinity() * -1;
    }
	else
	{
      return std::numeric_limits<T>::lowest();
    }
  }
};

// Extracts the underlying type from an enum.
template <typename T, bool is_enum = std::is_enum<T>::value>
struct ArithmeticOrUnderlyingEnum;

template <typename T>
struct ArithmeticOrUnderlyingEnum<T, true>
{
    using type              = typename std::underlying_type<T>::type;
    static const bool value = std::is_arithmetic<type>::value;
};

template <typename T>
struct ArithmeticOrUnderlyingEnum<T, false>
{
    using type              = T;
    static const bool value = std::is_arithmetic<type>::value;
};

// Used to treat CheckedNumeric and arithmetic underlying types the same.
template <typename T>
struct UnderlyingType
{
    using type                   = typename ArithmeticOrUnderlyingEnum<T>::type;
    static const bool is_numeric = std::is_arithmetic<type>::value;
    static const bool is_checked = false;
    static const bool is_clamped = false;
    static const bool is_strict  = false;
};

template <typename T>
struct UnderlyingType<CheckedNumeric<T>>
{
    using type                   = T;
    static const bool is_numeric = true;
    static const bool is_checked = true;
    static const bool is_clamped = false;
    static const bool is_strict  = false;
};

template <typename T>
struct UnderlyingType<ClampedNumeric<T>>
{
    using type                   = T;
    static const bool is_numeric = true;
    static const bool is_checked = false;
    static const bool is_clamped = true;
    static const bool is_strict  = false;
};

template <typename T>
struct UnderlyingType<StrictNumeric<T>>
{
    using type                   = T;
    static const bool is_numeric = true;
    static const bool is_checked = false;
    static const bool is_clamped = false;
    static const bool is_strict  = true;
};

template <typename Dst, typename Src>
inline constexpr bool kIsMaxInRangeForNumericType =
    IsGreaterOrEqual<Dst, Src>::Test(std::numeric_limits<Dst>::max(),
                                     std::numeric_limits<Src>::max());

template <typename Dst, typename Src>
inline constexpr bool kIsMinInRangeForNumericType =
    IsLessOrEqual<Dst, Src>::Test(std::numeric_limits<Dst>::lowest(),
                                  std::numeric_limits<Src>::lowest());

template <typename Dst, typename Src>
inline constexpr Dst kCommonMax =
    kIsMaxInRangeForNumericType<Dst, Src>
        ? static_cast<Dst>(std::numeric_limits<Src>::max())
        : std::numeric_limits<Dst>::max();

template <typename Dst, typename Src>
inline constexpr Dst kCommonMin =
    kIsMinInRangeForNumericType<Dst, Src>
        ? static_cast<Dst>(std::numeric_limits<Src>::lowest())
        : std::numeric_limits<Dst>::lowest();

// This is a wrapper to generate return the max or min for a supplied type.
// If the argument is false, the returned value is the maximum. If true the
// returned value is the minimum.
template <typename Dst, typename Src = Dst>
constexpr Dst CommonMaxOrMin(bool is_min) {
  return is_min ? kCommonMin<Dst, Src> : kCommonMax<Dst, Src>;
}

// Determines if a numeric value is negative without throwing compiler
// warnings on: unsigned(value) < 0.
template <typename T>
  requires(std::is_arithmetic_v<T>)
constexpr bool IsValueNegative(T value) {
  if constexpr (std::is_signed_v<T>) {
    return value < 0;
  } else {
    return false;
  }
}

// Convenience function that returns true if the supplied value is in range
// for the destination type.
template <typename Dst, typename Src>
  requires(IsNumeric<Src> && std::is_arithmetic_v<Dst> &&
           std::numeric_limits<Dst>::lowest() < std::numeric_limits<Dst>::max())
constexpr bool IsValueInRangeForNumericType(Src value) {
  using SrcType = UnderlyingType<Src>;
  const auto underlying_value = static_cast<SrcType>(value);
  return numerics_internal::IsValueInRangeFastOp<Dst, SrcType>::is_supported
             ? numerics_internal::IsValueInRangeFastOp<Dst, SrcType>::Do(
                   underlying_value)
             : numerics_internal::DstRangeRelationToSrcRange<Dst>(
                   underlying_value)
                   .IsValid();
}

// The following special case a few specific integer conversions where we can
// eke out better performance than range checking.
template <typename Dst, typename Src>
struct IsValueInRangeFastOp {
  static constexpr bool is_supported = false;
  static constexpr bool Do(Src) {
    // Force a compile failure if instantiated.
    return CheckOnFailure::template HandleFailure<bool>();
  }
};

// Signed to signed range comparison.
template <typename Dst, typename Src>
  requires(std::signed_integral<Dst> && std::signed_integral<Src> &&
           !kIsTypeInRangeForNumericType<Dst, Src>)
struct IsValueInRangeFastOp<Dst, Src> {
  static constexpr bool is_supported = true;

  static constexpr bool Do(Src value) {
    // Just downcast to the smaller type, sign extend it back to the original
    // type, and then see if it matches the original value.
    return value == static_cast<Dst>(value);
  }
};

// Signed to unsigned range comparison.
template <typename Dst, typename Src>
  requires(std::unsigned_integral<Dst> && std::signed_integral<Src> && !kIsTypeInRangeForNumericType<Dst, Src>)
struct IsValueInRangeFastOp<Dst, Src> {
  static constexpr bool is_supported = true;

  static constexpr bool Do(Src value) {
    // We cast a signed as unsigned to overflow negative values to the top,
    // then compare against whichever maximum is smaller, as our upper bound.
    return as_unsigned(value) <= as_unsigned(kCommonMax<Src, Dst>);
  }
};

// This performs a fast negation, returning a signed value. It works on unsigned
// arguments, but probably doesn't do what you want for any unsigned value
// larger than max / 2 + 1 (i.e. signed min cast to unsigned).
template <typename T>
  requires std::is_integral_v<T>
constexpr auto ConditionalNegate(T x, bool is_negative) {
  using SignedT = std::make_signed_t<T>;
  using UnsignedT = std::make_unsigned_t<T>;
  return static_cast<SignedT>((static_cast<UnsignedT>(x) ^
                               static_cast<UnsignedT>(-SignedT(is_negative))) +
                              is_negative);
}

// This performs a safe, absolute value via unsigned overflow.
template <typename T>
  requires std::is_integral_v<T>
constexpr auto SafeUnsignedAbs(T value) {
  using UnsignedT = std::make_unsigned_t<T>;
  return IsValueNegative(value)
             ? static_cast<UnsignedT>(0u - static_cast<UnsignedT>(value))
             : static_cast<UnsignedT>(value);
}


// The following helper template addresses a corner case in range checks for
// conversion from a floating-point type to an integral type of smaller range
// but larger precision (e.g. float -> unsigned). The problem is as follows:
//   1. Integral maximum is always one less than a power of two, so it must be
//      truncated to fit the mantissa of the floating point. The direction of
//      rounding is implementation defined, but by default it's always IEEE
//      floats, which round to nearest and thus result in a value of larger
//      magnitude than the integral value.
//      Example: float f = UINT_MAX; // f is 4294967296f but UINT_MAX
//                                   // is 4294967295u.
//   2. If the floating point value is equal to the promoted integral maximum
//      value, a range check will erroneously pass.
//      Example: (4294967296f <= 4294967295u) // This is true due to a precision
//                                            // loss in rounding up to float.
//   3. When the floating point value is then converted to an integral, the
//      resulting value is out of range for the target integral type and
//      thus is implementation defined.
//      Example: unsigned u = (float)INT_MAX; // u will typically overflow to 0.
// To fix this bug we manually truncate the maximum value when the destination
// type is an integral of larger precision than the source floating-point type,
// such that the resulting maximum is represented exactly as a floating point.
template <typename Dst, typename Src, template <typename> class Bounds>
struct NarrowingRange {
  using SrcLimits = std::numeric_limits<Src>;
  using DstLimits = std::numeric_limits<Dst>;

  // Computes the mask required to make an accurate comparison between types.
  static constexpr int kShift = (kMaxExponent<Src> > kMaxExponent<Dst> &&
                                 SrcLimits::digits < DstLimits::digits)
                                    ? (DstLimits::digits - SrcLimits::digits)
                                    : 0;

  template <typename T>
    requires(std::same_as<T, Dst> &&
             ((std::integral<T> && kShift < DstLimits::digits) ||
              (std::floating_point<T> && kShift == 0)))
  // Masks out the integer bits that are beyond the precision of the
  // intermediate type used for comparison.
  static constexpr T Adjust(T value) {
    if constexpr (std::integral<T>) {
      using UnsignedDst = typename std::make_unsigned_t<T>;
      return static_cast<T>(
          ConditionalNegate(SafeUnsignedAbs(value) &
                                ~((UnsignedDst{1} << kShift) - UnsignedDst{1}),
                            IsValueNegative(value)));
    } else {
      return value;
    }
  }

  static constexpr Dst max() { return Adjust(Bounds<Dst>::max()); }
  static constexpr Dst lowest() { return Adjust(Bounds<Dst>::lowest()); }
};

enum class IntegerRepresentation { kUnsigned, kSigned };

// A range for a given nunmeric Src type is contained for a given numeric Dst
// type if both numeric_limits<Src>::max() <= numeric_limits<Dst>::max() and
// numeric_limits<Src>::lowest() >= numeric_limits<Dst>::lowest() are true.
// We implement this as template specializations rather than simple static
// comparisons to ensure type correctness in our comparisons.
enum class NumericRangeRepresentation { kNotContained, kContained };

// The std library doesn't provide a binary max_exponent for integers, however
// we can compute an analog using std::numeric_limits<>::digits.
template <typename NumericType>
inline constexpr int kMaxExponent = std::is_floating_point_v<NumericType> ? std::numeric_limits<NumericType>::max_exponent : std::numeric_limits<NumericType>::digits + 1;

inline constexpr auto kStaticDstRangeRelationToSrcRange = kMaxExponent<Dst> >= kMaxExponent<Src> ? NumericRangeRepresentation::kContained : NumericRangeRepresentation::kNotContained;

// Unsigned to signed: Dst is guaranteed to contain source only if its range is
// larger.
template <typename Dst, typename Src>
inline constexpr auto
    kStaticDstRangeRelationToSrcRange<Dst,
                                      Src,
                                      IntegerRepresentation::kSigned,
                                      IntegerRepresentation::kUnsigned> =
        kMaxExponent<Dst> > kMaxExponent<Src>
            ? NumericRangeRepresentation::kContained
            : NumericRangeRepresentation::kNotContained;

// Signed to unsigned: Dst cannot be statically determined to contain Src.
template <typename Dst, typename Src>
inline constexpr auto
    kStaticDstRangeRelationToSrcRange<Dst,
                                      Src,
                                      IntegerRepresentation::kUnsigned,
                                      IntegerRepresentation::kSigned> =
        NumericRangeRepresentation::kNotContained;

// This class wraps the range constraints as separate booleans so the compiler
// can identify constants and eliminate unused code paths.
class RangeCheck {
 public:
  constexpr RangeCheck() = default;
  constexpr RangeCheck(bool is_in_lower_bound, bool is_in_upper_bound)
      : is_underflow_(!is_in_lower_bound), is_overflow_(!is_in_upper_bound) {}

  constexpr bool operator==(const RangeCheck& rhs) const = default;

  constexpr bool IsValid() const { return !is_overflow_ && !is_underflow_; }
  constexpr bool IsInvalid() const { return is_overflow_ && is_underflow_; }
  constexpr bool IsOverflow() const { return is_overflow_ && !is_underflow_; }
  constexpr bool IsUnderflow() const { return !is_overflow_ && is_underflow_; }
  constexpr bool IsOverflowFlagSet() const { return is_overflow_; }
  constexpr bool IsUnderflowFlagSet() const { return is_underflow_; }

 private:
  // Do not change the order of these member variables. The integral conversion
  // optimization depends on this exact order.
  const bool is_underflow_ = false;
  const bool is_overflow_ = false;
};

template <typename Dst,
          template <typename> class Bounds = std::numeric_limits,
          typename Src>
  requires(std::is_arithmetic_v<Src> && std::is_arithmetic_v<Dst> &&
           Bounds<Dst>::lowest() < Bounds<Dst>::max())
constexpr RangeCheck DstRangeRelationToSrcRange(Src value) {
  return DstRangeRelationToSrcRangeImpl<Dst, Src, Bounds>::Check(value);
}

// Default case, used for same sign narrowing: The range is contained for normal
// limits.
template <typename Dst,
          typename Src,
          template <typename>
          class Bounds,
          IntegerRepresentation DstSign =
              std::is_signed_v<Dst> ? IntegerRepresentation::kSigned
                                    : IntegerRepresentation::kUnsigned,
          IntegerRepresentation SrcSign =
              std::is_signed_v<Src> ? IntegerRepresentation::kSigned
                                    : IntegerRepresentation::kUnsigned,
          NumericRangeRepresentation DstRange =
              kStaticDstRangeRelationToSrcRange<Dst, Src>>
struct DstRangeRelationToSrcRangeImpl {
  static constexpr RangeCheck Check(Src value) {
    using SrcLimits = std::numeric_limits<Src>;
    using DstLimits = NarrowingRange<Dst, Src, Bounds>;
    return RangeCheck(
        static_cast<Dst>(SrcLimits::lowest()) >= DstLimits::lowest() ||
            static_cast<Dst>(value) >= DstLimits::lowest(),
        static_cast<Dst>(SrcLimits::max()) <= DstLimits::max() ||
            static_cast<Dst>(value) <= DstLimits::max());
  }
};

// Signed to signed narrowing: Both the upper and lower boundaries may be
// exceeded for standard limits.
template <typename Dst, typename Src, template <typename> class Bounds>
struct DstRangeRelationToSrcRangeImpl<
    Dst,
    Src,
    Bounds,
    IntegerRepresentation::kSigned,
    IntegerRepresentation::kSigned,
    NumericRangeRepresentation::kNotContained> {
  static constexpr RangeCheck Check(Src value) {
    using DstLimits = NarrowingRange<Dst, Src, Bounds>;
    return RangeCheck(value >= DstLimits::lowest(), value <= DstLimits::max());
  }
};

// Unsigned to unsigned narrowing: Only the upper bound can be exceeded for
// standard limits.
template <typename Dst, typename Src, template <typename> class Bounds>
struct DstRangeRelationToSrcRangeImpl<
    Dst,
    Src,
    Bounds,
    IntegerRepresentation::kUnsigned,
    IntegerRepresentation::kUnsigned,
    NumericRangeRepresentation::kNotContained> {
  static constexpr RangeCheck Check(Src value) {
    using DstLimits = NarrowingRange<Dst, Src, Bounds>;
    return RangeCheck(
        DstLimits::lowest() == Dst{0} || value >= DstLimits::lowest(),
        value <= DstLimits::max());
  }
};

// Unsigned to signed: Only the upper bound can be exceeded for standard limits.
template <typename Dst, typename Src, template <typename> class Bounds>
struct DstRangeRelationToSrcRangeImpl<
    Dst,
    Src,
    Bounds,
    IntegerRepresentation::kSigned,
    IntegerRepresentation::kUnsigned,
    NumericRangeRepresentation::kNotContained> {
  static constexpr RangeCheck Check(Src value) {
    using DstLimits = NarrowingRange<Dst, Src, Bounds>;
    using Promotion = decltype(Src() + Dst());
    return RangeCheck(DstLimits::lowest() <= Dst{0} ||
                          static_cast<Promotion>(value) >=
                              static_cast<Promotion>(DstLimits::lowest()),
                      static_cast<Promotion>(value) <=
                          static_cast<Promotion>(DstLimits::max()));
  }
};

// Signed to unsigned: The upper boundary may be exceeded for a narrower Dst,
// and any negative value exceeds the lower boundary for standard limits.
template <typename Dst, typename Src, template <typename> class Bounds>
struct DstRangeRelationToSrcRangeImpl<
    Dst,
    Src,
    Bounds,
    IntegerRepresentation::kUnsigned,
    IntegerRepresentation::kSigned,
    NumericRangeRepresentation::kNotContained> {
  static constexpr RangeCheck Check(Src value) {
    using SrcLimits = std::numeric_limits<Src>;
    using DstLimits = NarrowingRange<Dst, Src, Bounds>;
    using Promotion = decltype(Src() + Dst());
    bool ge_zero;
    // Converting floating-point to integer will discard fractional part, so
    // values in (-1.0, -0.0) will truncate to 0 and fit in Dst.
    if constexpr (std::is_floating_point_v<Src>) {
      ge_zero = value > Src{-1};
    } else {
      ge_zero = value >= Src{0};
    }
    return RangeCheck(
        ge_zero && (DstLimits::lowest() == 0 ||
                    static_cast<Dst>(value) >= DstLimits::lowest()),
        static_cast<Promotion>(SrcLimits::max()) <=
                static_cast<Promotion>(DstLimits::max()) ||
            static_cast<Promotion>(value) <=
                static_cast<Promotion>(DstLimits::max()));
  }
};
