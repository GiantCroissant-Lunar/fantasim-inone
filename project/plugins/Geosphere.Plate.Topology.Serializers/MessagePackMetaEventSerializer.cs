using System;
using MessagePack;
using FantaSim.Geosphere.Plate.Topology.Contracts;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using UnifySerialization.MessagePack.Runtime;

namespace FantaSim.Geosphere.Plate.Topology.Serializers;

/// <summary>
/// MessagePack serializer for meta/governance events.
/// Envelope format: [eventType:string, payload:binary]
/// </summary>
public static class MessagePackMetaEventSerializer
{
    public static readonly MessagePackSerializerOptions Options = TopologySerializationOptions.Options;

    public static byte[] Serialize(IMetaGovernanceEvent value)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value);
#else
        if (value is null) throw new ArgumentNullException(nameof(value));
#endif

        var payloadBytes = MessagePackSerializer.Serialize(value.GetType(), value, Options);
        var eventType = MetaEventTypeRegistry.GetId(value.GetType());
        return MessagePackEnvelopeCodec.Serialize(eventType, payloadBytes);
    }

    public static IMetaGovernanceEvent Deserialize(byte[] data)
    {
        var envelope = MessagePackEnvelopeCodec.Deserialize(data);
        var eventTypeType = MetaEventTypeRegistry.Resolve(envelope.TypeId);
        var eventObj = MessagePackSerializer.Deserialize(eventTypeType, envelope.Payload, Options);
        return (IMetaGovernanceEvent)eventObj!;
    }
}
