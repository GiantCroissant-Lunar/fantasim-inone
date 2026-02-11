using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using UnifySerialization.Abstractions;
using System.Runtime.InteropServices;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Events;

/// <summary>
/// Captures parent-domain head state at fork time.
/// </summary>
[UnifyModel]
[StructLayout(LayoutKind.Auto)]
public readonly record struct DomainForkHead(
    [property: UnifyProperty(0)] Domain Domain,
    [property: UnifyProperty(1)] CanonicalTick? HeadTick,
    [property: UnifyProperty(2)] ReadOnlyMemory<byte>? HeadHash);
