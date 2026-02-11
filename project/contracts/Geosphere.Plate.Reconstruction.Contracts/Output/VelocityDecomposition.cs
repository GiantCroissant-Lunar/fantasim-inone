using FantaSim.Geosphere.Plate.Kinematics.Contracts;
using FantaSim.Geosphere.Plate.Velocity.Contracts;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Reconstruction.Contracts.Output;

/// <summary>
/// Decomposition of velocity into components per RFC-V2-0045 section 3.3.
/// Breaks down total velocity into rigid rotation and optional deformation components.
/// </summary>
[UnifyModel]
public sealed record VelocityDecomposition
{
    /// <summary>
    /// Velocity component due to rigid plate rotation.
    /// </summary>
    [UnifyProperty(0)]
    public required Velocity3d RigidRotationComponent { get; init; }

    /// <summary>
    /// Velocity component due to deformation (null if no deformation model applied).
    /// </summary>
    [UnifyProperty(1)]
    public Velocity3d? DeformationComponent { get; init; }

    /// <summary>
    /// Reference frame for the velocity components.
    /// </summary>
    [UnifyProperty(2)]
    public required ReferenceFrameId RelativeToFrame { get; init; }

    /// <summary>
    /// Total velocity magnitude (speed) in simulation units per tick.
    /// </summary>
    [UnifyProperty(3)]
    public required double Magnitude { get; init; }

    /// <summary>
    /// Azimuth angle of velocity direction (radians, clockwise from north).
    /// </summary>
    [UnifyProperty(4)]
    public required double Azimuth { get; init; }
}
