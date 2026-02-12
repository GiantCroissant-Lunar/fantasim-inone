using FantaSim.Geosphere.Plate.Topology.Contracts.Events;

namespace FantaSim.Geosphere.Plate.Kinematics.Contracts.Events;

/// <summary>
/// Base interface for all plate kinematics truth events (RFC-V2-0023).
/// </summary>
public interface IPlateKinematicsEvent : IPlateTruthEvent
{
    // Keep explicit interface implementations in existing event records source-compatible.
    string EventType { get; }
}
