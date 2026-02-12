using System;
using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Events;

/// <summary>
/// Common truth-event envelope shared across plate domains.
/// </summary>
public interface IPlateTruthEvent
{
    Guid EventId { get; }

    CanonicalTick Tick { get; }

    long Sequence { get; }

    TruthStreamIdentity StreamIdentity { get; }

    ReadOnlyMemory<byte> PreviousHash { get; }

    ReadOnlyMemory<byte> Hash { get; }
}
