using System;
using System.Runtime.InteropServices;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Events;

/// <summary>
/// Records creation of a branch inside a variant lineage stream.
/// </summary>
[StructLayout(LayoutKind.Auto)]
[UnifyModel]
public readonly record struct BranchCreatedEvent(
    [property: UnifyProperty(0)] BranchId BranchId,
    [property: UnifyProperty(1)] string? Label,
    [property: UnifyProperty(2)] DateTimeOffset CreatedAt,
    [property: UnifyProperty(3)] long? Seed,
    [property: UnifyProperty(4)] long Sequence,
    [property: UnifyProperty(5)] MetaStreamIdentity StreamIdentity,
    [property: UnifyProperty(6)] ReadOnlyMemory<byte> PreviousHash,
    [property: UnifyProperty(7)] ReadOnlyMemory<byte> Hash) : IMetaGovernanceEvent
{
    string IMetaGovernanceEvent.EventType => nameof(BranchCreatedEvent);
}
