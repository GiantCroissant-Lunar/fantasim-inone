using MessagePack;
using FantaSim.Geosphere.Plate.Topology.Contracts;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using UnifySerialization.MessagePack.Runtime;

namespace FantaSim.Geosphere.Plate.Topology.Serializers;

/// <summary>
/// MessagePack serializer for plate topology events with envelope-based polymorphic API.
/// </summary>
public static class MessagePackEventSerializer
{
    public static readonly MessagePackSerializerOptions Options = TopologySerializationOptions.Options;

    public static byte[] Serialize<T>(T value) where T : IPlateTopologyEvent
    {
        var payloadBytes = MessagePackSerializer.Serialize(value, Options);
        var eventType = EventTypeRegistry.GetId(typeof(T));
        return MessagePackEnvelopeCodec.Serialize(eventType, payloadBytes);
    }

    public static byte[] Serialize(IPlateTopologyEvent value)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value);
#else
        if (value is null) throw new ArgumentNullException(nameof(value));
#endif

        var payloadBytes = MessagePackSerializer.Serialize(value.GetType(), value, Options);
        var eventType = EventTypeRegistry.GetId(value.GetType());
        return MessagePackEnvelopeCodec.Serialize(eventType, payloadBytes);
    }

    public static IPlateTopologyEvent Deserialize(byte[] data)
    {
        var envelope = MessagePackEnvelopeCodec.Deserialize(data);
        var eventType = EventTypeRegistry.Resolve(envelope.TypeId);
        var eventObj = MessagePackSerializer.Deserialize(eventType, envelope.Payload, Options);
        return (IPlateTopologyEvent)eventObj!;
    }

    public static T Deserialize<T>(byte[] data) where T : IPlateTopologyEvent
    {
        var envelope = MessagePackEnvelopeCodec.Deserialize(data);

        var expectedTypeId = EventTypeRegistry.GetId(typeof(T));
        if (!string.Equals(envelope.TypeId, expectedTypeId, StringComparison.Ordinal))
            throw new InvalidOperationException($"Event type mismatch. Expected {expectedTypeId}, got {envelope.TypeId}");

        return MessagePackSerializer.Deserialize<T>(envelope.Payload, Options);
    }
}
