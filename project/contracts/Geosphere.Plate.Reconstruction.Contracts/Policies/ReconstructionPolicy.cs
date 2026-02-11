using FantaSim.Geosphere.Plate.Kinematics.Contracts;
using FrameId = FantaSim.Geosphere.Plate.Kinematics.Contracts.ReferenceFrameId;
using FantaSim.Geosphere.Plate.Motion.Contracts;
using FantaSim.Geosphere.Plate.Velocity.Contracts;
using FantaSim.Geosphere.Plate.Partition.Contracts;
using FantaSim.Geosphere.Plate.Reconstruction.Contracts.Cache;

using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Reconstruction.Contracts.Policies;

/// <summary>
/// Unified policy object that captures how reconstruction is performed.
/// This bundles configuration options scattered across RFC-V2-0045 through RFC-V2-0049.
/// </summary>
[UnifyModel]
public sealed record ReconstructionPolicy
{
    /// <summary>
    /// Reference frame for reconstruction output (from RFC-V2-0046).
    /// </summary>
    [UnifyProperty(0)]
    public required FrameId Frame { get; init; }

    /// <summary>
    /// Kinematics model selection (M-axis).
    /// </summary>
    [UnifyProperty(1)]
    public required ModelId KinematicsModel { get; init; }

    /// <summary>
    /// Tolerance policy for partition quality (from RFC-V2-0047).
    /// </summary>
    [UnifyProperty(2)]
    public required TolerancePolicy PartitionTolerance { get; init; }

    /// <summary>
    /// Optional: Boundary sampling specification (from RFC-V2-0048).
    /// </summary>
    [UnifyProperty(3)]
    public BoundarySampleSpec? BoundarySampling { get; init; }

    /// <summary>
    /// Optional: Integration step policy (from RFC-V2-0049).
    /// </summary>
    [UnifyProperty(4)]
    public StepPolicy? IntegrationPolicy { get; init; }

    /// <summary>
    /// Provenance strictness level for output validation.
    /// Default is Strict.
    /// </summary>
    [UnifyProperty(5)]
    public ProvenanceStrictness Strictness { get; init; } = ProvenanceStrictness.Strict;

    public string ComputeHash() => PolicyCacheKey.ComputeHash(this);
}
