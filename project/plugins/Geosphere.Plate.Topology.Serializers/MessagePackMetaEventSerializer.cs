using System;
using System.Buffers;
using MessagePack;
using FantaSim.Geosphere.Plate.Topology.Contracts;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;

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

        var buffer = new System.Buffers.ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteArrayHeader(2);
        writer.Write(eventType);
        writer.Write(payloadBytes);
        writer.Flush();

        return buffer.WrittenMemory.ToArray();
    }

    public static IMetaGovernanceEvent Deserialize(byte[] data)
    {
        var reader = new MessagePackReader(data);
        var length = reader.ReadArrayHeader();
        if (length != 2)
            throw new InvalidOperationException($"Envelope must have 2 elements, got {length}");

        var eventType = reader.ReadString();
        if (eventType == null)
            throw new InvalidOperationException("Envelope eventType cannot be null");

        var payloadBytes = reader.ReadBytes();
        if (!payloadBytes.HasValue)
            throw new InvalidOperationException("Envelope payload cannot be null");

        var eventTypeType = MetaEventTypeRegistry.Resolve(eventType);
        var payloadArray = ReadOnlySequenceExtensions.ToByteArray(payloadBytes.Value);
        var eventObj = MessagePackSerializer.Deserialize(eventTypeType, payloadArray, Options);
        return (IMetaGovernanceEvent)eventObj!;
    }
}
