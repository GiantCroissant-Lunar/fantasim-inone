using System.Collections.Generic;
using System.Runtime.InteropServices;
using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Events;

/// <summary>
/// Records a branch fork lineage edge and causal cut in the meta lineage stream.
/// </summary>
[StructLayout(LayoutKind.Auto)]
[UnifyModel]
public readonly record struct BranchForkedEvent(
    [property: UnifyProperty(0)] BranchId BranchId,
    [property: UnifyProperty(1)] BranchId ParentBranchId,
    [property: UnifyProperty(2)] CanonicalTick ForkTick,
    [property: UnifyProperty(3)] IReadOnlyList<DomainForkHead> BaseHeads,
    [property: UnifyProperty(4)] long Sequence,
    [property: UnifyProperty(5)] MetaStreamIdentity StreamIdentity,
    [property: UnifyProperty(6)] ReadOnlyMemory<byte> PreviousHash,
    [property: UnifyProperty(7)] ReadOnlyMemory<byte> Hash) : IMetaGovernanceEvent
{
    string IMetaGovernanceEvent.EventType => nameof(BranchForkedEvent);
}
