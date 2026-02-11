using System.Collections.Generic;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Numerics;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Kinematics.Contracts.Derived;

/// <summary>
/// RFC-V2-0046 Section 5.1: Frame-aware kinematics view.
/// Provides plate rotations expressed in any reference frame.
/// </summary>
public interface IFrameAwareKinematicsView
{
    FiniteRotation? GetRotationInFrame(PlateId plateId, CanonicalTick tick, ReferenceFrameId frame);

    IReadOnlyDictionary<PlateId, FiniteRotation> GetAllRotationsInFrame(CanonicalTick tick, ReferenceFrameId frame);
}
