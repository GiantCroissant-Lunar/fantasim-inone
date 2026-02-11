using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Events;

/// <summary>
/// A draft event that has been proposed but not yet committed to the truth log.
/// </summary>
public interface ITruthEventDraft
{
    /// <summary>
    /// The stream identity this event belongs to.
    /// </summary>
    TruthStreamIdentity Stream { get; }

    /// <summary>
    /// Materializes the draft into a truth event with the assigned sequence number.
    /// </summary>
    IPlateTopologyEvent ToTruthEvent(long sequence);
}
