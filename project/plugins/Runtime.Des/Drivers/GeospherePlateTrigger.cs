using System;
using System.Collections.Generic;
using FantaSim.World.Contracts.Time;
using FantaSim.Geosphere.Plate.Runtime.Des.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using Plate.TimeDete.Determinism.Abstractions;
using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using FantaSim.Geosphere.Plate.Runtime.Des.Contracts;

namespace FantaSim.Geosphere.Plate.Runtime.Des.Drivers;

public sealed class GeospherePlateTrigger : IExecutableTrigger
{
    private readonly TruthStreamIdentity _streamIdentity;

    public GeospherePlateTrigger(TruthStreamIdentity streamIdentity)
    {
        _streamIdentity = streamIdentity;
    }

    public TriggerId Id => new("GeospherePlateTrigger");
    public SphereId Sphere => SphereIds.Geosphere;

    public IReadOnlyList<ITruthEventDraft> EmitDrafts(DriverOutput output, CanonicalTick tick, ISeededRng rng)
    {
        if (output.Signal is string s && string.Equals(s, "Genesis", StringComparison.Ordinal))
        {
            var plateId = new PlateId(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            var eventId = new EventId(Guid.Parse("00000000-0000-0000-0000-000000000002"));

            var draft = new GenericTruthEventDraft(
                tick,
                _streamIdentity,
                (seq) => new PlateCreatedEvent(
                    eventId,
                    plateId,
                    tick,
                    seq,
                    _streamIdentity,
                    ReadOnlyMemory<byte>.Empty,
                    ReadOnlyMemory<byte>.Empty
                )
            );

            return new[] { draft };
        }

        return Array.Empty<ITruthEventDraft>();
    }
}
