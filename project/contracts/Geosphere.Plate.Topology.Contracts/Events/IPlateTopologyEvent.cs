namespace FantaSim.Geosphere.Plate.Topology.Contracts.Events;

/// <summary>
/// Base interface for all plate topology events per FR-006, FR-015.
///
/// Events represent immutable changes to plate topology truth. Each event contains
/// all information required to reconstruct topology state without external
/// dependencies or solver execution.
///
/// Event envelope structure:
/// - EventId: Unique identifier for the event (UUIDv7 for sortability)
/// - EventType: Discriminator for polymorphic deserialization
/// - Tick: Canonical simulation time (RFC-V2-0010)
/// - Sequence: Ordering within stream (deterministic replay)
/// - StreamIdentity: Which truth stream this belongs to
///
/// Concrete event types (per FR-008):
/// - Creation events: PlateCreated, BoundaryCreated, JunctionCreated
/// - Lifecycle events: BoundaryRetired, JunctionRetired, PlateRetired (optional)
/// - State change events: BoundaryTypeChanged, BoundaryGeometryUpdated, JunctionUpdated
/// - Topology evolution (future): PlateSplit, PlateMerge, BoundaryReSegmented
/// </summary>
public interface IPlateTopologyEvent : IPlateTruthEvent
{
    // Keep explicit interface implementations in existing event records source-compatible.
    string EventType { get; }
}
