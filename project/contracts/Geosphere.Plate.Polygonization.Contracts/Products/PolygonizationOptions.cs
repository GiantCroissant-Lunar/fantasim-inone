using System.Runtime.InteropServices;

namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.Products;

public enum WindingConvention
{
    CounterClockwise,
    Clockwise
}

[StructLayout(LayoutKind.Auto)]
public readonly record struct PolygonizationOptions(
    WindingConvention Winding = WindingConvention.CounterClockwise,
    double SnapTolerance = 1e-9,
    bool AllowPartialPolygonization = false
);
