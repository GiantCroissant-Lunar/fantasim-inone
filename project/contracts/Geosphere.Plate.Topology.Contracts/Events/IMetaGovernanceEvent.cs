using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Events;

/// <summary>
/// Base contract for governance events stored in meta streams.
/// </summary>
public interface IMetaGovernanceEvent
{
    string EventType { get; }
    long Sequence { get; }
    MetaStreamIdentity StreamIdentity { get; }
    ReadOnlyMemory<byte> PreviousHash { get; }
    ReadOnlyMemory<byte> Hash { get; }
}
