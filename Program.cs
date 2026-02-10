using UI.Geometry;

namespace UI;

public static class Program
{
    public static void Main(string[] args)
    {
        LayoutUnit a = new();

        Console.WriteLine($"Value: {a} (Raw: {a.RawValue()})"); // Value: 0 (Raw: 0)
        a++;
        Console.WriteLine($"Value: {a} (Raw: {a.RawValue()})"); // Value: 1 (Raw: 64)
        a++;
        Console.WriteLine($"Value: {a} (Raw: {a.RawValue()})"); // Value: 2 (Raw: 128)
        a--;
        Console.WriteLine($"Value: {a} (Raw: {a.RawValue()})"); // Value: 1 (Raw: 64)

        /*
        LayoutUnit test1 = new LayoutUnit(1);
        LayoutUnit test2 = new LayoutUnit(2);
        LayoutUnit test3 = new LayoutUnit(3);


        Console.WriteLine(test1.RawValue());
        Console.WriteLine(test2.RawValue());
        Console.WriteLine(test3.RawValue());

        Console.WriteLine(LayoutUnit.Max.RawValue());
        */

        /*
        var unit_ceil_nan = Geometry.LayoutUnit.FromFloatCeil(float.NaN);
        var unit_floor_nan = Geometry.LayoutUnit.FromFloatFloor(float.NaN);
        var unit_round_nan = Geometry.LayoutUnit.FromFloatRound(float.NaN);

        Console.WriteLine($"unit_ceil_nan value: {unit_ceil_nan.RawValue()}");
        Console.WriteLine($"unit_floor_nan value: {unit_floor_nan.RawValue()}");
        Console.WriteLine($"unit_round_nan value: {unit_round_nan.RawValue()}");

        var unit_ceil_max = Geometry.LayoutUnit.FromFloatCeil(float.MaxValue + 100);
        var unit_floor_max = Geometry.LayoutUnit.FromFloatFloor(float.MaxValue + 100);
        var unit_round_max = Geometry.LayoutUnit.FromFloatRound(float.MaxValue + 100);

        Console.WriteLine($"unit_ceil_max value: {unit_ceil_max.RawValue()}");
        Console.WriteLine($"unit_floor_max value: {unit_floor_max.RawValue()}");
        Console.WriteLine($"unit_round_max value: {unit_round_max.RawValue()}");

        var unit_ceil_min = Geometry.LayoutUnit.FromFloatCeil(float.MinValue - 100);
        var unit_floor_min = Geometry.LayoutUnit.FromFloatFloor(float.MinValue - 100);
        var unit_round_min = Geometry.LayoutUnit.FromFloatRound(float.MinValue - 100);

        Console.WriteLine($"unit_ceil_min value: {unit_ceil_min.RawValue()}");
        Console.WriteLine($"unit_floor_min value: {unit_floor_min.RawValue()}");
        Console.WriteLine($"unit_round_min value: {unit_round_min.RawValue()}");

        var unit_ceil_val = Geometry.LayoutUnit.FromFloatCeil(LayoutUnit.Epsilon() * 1);
        var unit_floor_val = Geometry.LayoutUnit.FromFloatFloor(LayoutUnit.Epsilon() * 2);
        var unit_round_val = Geometry.LayoutUnit.FromFloatRound(LayoutUnit.Epsilon() * 3);

        Console.WriteLine($"unit_ceil_val value: {unit_ceil_val.RawValue()}");
        Console.WriteLine($"unit_floor_val value: {unit_floor_val.RawValue()}");
        Console.WriteLine($"unit_round_val value: {unit_round_val.RawValue()}");
        */
    }
}
