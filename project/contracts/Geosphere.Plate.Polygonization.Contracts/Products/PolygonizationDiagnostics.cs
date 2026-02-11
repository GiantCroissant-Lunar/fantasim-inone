using System.Collections.Immutable;
using System.Runtime.InteropServices;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using UnifyGeometry;

namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.Products;

[StructLayout(LayoutKind.Auto)]
public readonly record struct PolygonizationDiagnostics(
    bool IsValid,
    ImmutableArray<OpenBoundaryDiagnostic> OpenBoundaries,
    ImmutableArray<NonManifoldJunctionDiagnostic> NonManifoldJunctions,
    ImmutableArray<DisconnectedComponentDiagnostic> DisconnectedComponents
)
{
    public static PolygonizationDiagnostics Valid()
        => new(true,
            ImmutableArray<OpenBoundaryDiagnostic>.Empty,
            ImmutableArray<NonManifoldJunctionDiagnostic>.Empty,
            ImmutableArray<DisconnectedComponentDiagnostic>.Empty);
}

[StructLayout(LayoutKind.Auto)]
public readonly record struct OpenBoundaryDiagnostic(
    BoundaryId BoundaryId,
    Point3 OpenEndpoint,
    string Message
);

public readonly record struct NonManifoldJunctionDiagnostic(
    JunctionId JunctionId,
    Point3 Position,
    int IncidentCount,
    string Message
);

public readonly record struct DisconnectedComponentDiagnostic(
    int ComponentIndex,
    ImmutableArray<BoundaryId> Boundaries,
    string Message
);
