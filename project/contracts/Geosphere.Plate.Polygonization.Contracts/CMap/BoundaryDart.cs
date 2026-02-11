using System.Runtime.InteropServices;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;

namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.CMap;

/// <summary>
/// Represents a directed half-edge (dart) in the boundary combinatorial map.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly record struct BoundaryDart : IComparable<BoundaryDart>
{
    public required BoundaryId BoundaryId { get; init; }
    public required int SegmentIndex { get; init; }
    public required DartDirection Direction { get; init; }

    public int CompareTo(BoundaryDart other)
    {
        var boundaryCompare = BoundaryId.Value.CompareTo(other.BoundaryId.Value);
        if (boundaryCompare != 0) return boundaryCompare;

        var segmentCompare = SegmentIndex.CompareTo(other.SegmentIndex);
        if (segmentCompare != 0) return segmentCompare;

        return Direction.CompareTo(other.Direction);
    }

    public override string ToString()
        => $"Dart({BoundaryId.Value.ToString("N").Substring(0, 8)}[{SegmentIndex}]{(Direction == DartDirection.Forward ? "→" : "←")})";

    public static bool operator <(BoundaryDart left, BoundaryDart right) => left.CompareTo(right) < 0;
    public static bool operator >(BoundaryDart left, BoundaryDart right) => left.CompareTo(right) > 0;
    public static bool operator <=(BoundaryDart left, BoundaryDart right) => left.CompareTo(right) <= 0;
    public static bool operator >=(BoundaryDart left, BoundaryDart right) => left.CompareTo(right) >= 0;
}
