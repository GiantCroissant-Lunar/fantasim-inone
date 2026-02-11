using FantaSim.Geosphere.Plate.Reconstruction.Contracts.Provenance;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Reconstruction.Contracts.Output;

/// <summary>
/// Result of a plate assignment query per RFC-V2-0045 section 3.2.
/// Contains the assigned plate, confidence level, and alternatives.
/// </summary>
[UnifyModel]
public sealed record PlateAssignmentResult
{
    /// <summary>
    /// The plate assigned to the queried location/feature.
    /// </summary>
    [UnifyProperty(0)]
    public required PlateId AssignedPlateId { get; init; }

    /// <summary>
    /// Confidence level of the plate assignment.
    /// </summary>
    [UnifyProperty(1)]
    public required AssignmentConfidence Confidence { get; init; }

    /// <summary>
    /// Candidate plates considered during assignment.
    /// </summary>
    [UnifyProperty(2)]
    public required CandidatePlate[] CandidatePlates { get; init; }

    /// <summary>
    /// Distance to the nearest plate boundary (in simulation units).
    /// </summary>
    [UnifyProperty(3)]
    public required double DistanceToBoundary { get; init; }

    /// <summary>
    /// Complete provenance chain for this assignment.
    /// </summary>
    [UnifyProperty(4)]
    public required ProvenanceChain Provenance { get; init; }
}

/// <summary>
/// Confidence level for plate assignment.
/// </summary>
public enum AssignmentConfidence
{
    /// <summary>
    /// Point is clearly within plate interior.
    /// </summary>
    Certain = 0,

    /// <summary>
    /// Point is near boundary, assignment may be ambiguous.
    /// </summary>
    Uncertain = 1,

    /// <summary>
    /// Point is on or very close to a boundary.
    /// </summary>
    Boundary = 2
}

/// <summary>
/// A candidate plate considered during assignment.
/// </summary>
[UnifyModel]
public readonly record struct CandidatePlate
{
    /// <summary>
    /// The candidate plate identifier.
    /// </summary>
    [UnifyProperty(0)]
    public required PlateId PlateId { get; init; }

    /// <summary>
    /// Distance from the query point to this plate's boundary.
    /// </summary>
    [UnifyProperty(1)]
    public required double DistanceToBoundary { get; init; }

    /// <summary>
    /// Confidence score for this candidate (0.0 to 1.0).
    /// </summary>
    [UnifyProperty(2)]
    public required double Score { get; init; }
}
