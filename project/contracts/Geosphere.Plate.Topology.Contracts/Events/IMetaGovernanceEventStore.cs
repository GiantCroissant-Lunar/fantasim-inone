using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Events;

/// <summary>
/// Append-only store contract for branch lineage/meta governance streams.
/// </summary>
public interface IMetaGovernanceEventStore
{
    /// <summary>
    /// Appends governance events to the specified meta stream.
    /// </summary>
    Task AppendMetaAsync(
        MetaStreamIdentity stream,
        IEnumerable<IMetaGovernanceEvent> events,
        CancellationToken cancellationToken);

    /// <summary>
    /// Reads governance events from a meta stream in ascending sequence order.
    /// </summary>
    IAsyncEnumerable<IMetaGovernanceEvent> ReadMetaAsync(
        MetaStreamIdentity stream,
        long fromSequenceInclusive,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the current head of a meta stream.
    /// </summary>
    Task<StreamHead> GetMetaHeadAsync(
        MetaStreamIdentity stream,
        CancellationToken cancellationToken);
}
