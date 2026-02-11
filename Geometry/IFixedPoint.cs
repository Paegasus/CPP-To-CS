using System.Numerics;

namespace UI.Geometry;

// Avoid storing structs that implement the interface in interface-typed variables
// Example: IFixedPoint<int,uint> x = new LayoutUnit();
// LayoutUnit gets boxed -> causing heap allocation

public interface IFixedPoint<TSigned, TUnsigned>
 where TSigned : 
  unmanaged,
  IBinaryInteger<TSigned>,
  IMinMaxValue<TSigned>
 where TUnsigned :
  unmanaged,
  IBinaryInteger<TUnsigned>,
  IMinMaxValue<TUnsigned>
{
    public TSigned RawValue();
    public void SetRawValue(int value);
	public void SetRawValue(long value);
	public TSigned ToInteger();
    public TUnsigned ToUnsignedInteger();
    public float ToFloat();
	public double ToDouble();
}
