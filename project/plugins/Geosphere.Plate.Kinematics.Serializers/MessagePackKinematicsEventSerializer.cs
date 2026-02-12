using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Events;
using FantaSim.Geosphere.Plate.Kinematics.Serializers.Formatters;
using FantaSim.Geosphere.Plate.Topology.Contracts.Serialization;
using UnifySerialization.MessagePack.Runtime;

namespace FantaSim.Geosphere.Plate.Kinematics.Serializers;

public static class MessagePackKinematicsEventSerializer
{
    public static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(CompositeResolver.Create(
            new IMessagePackFormatter[]
            {
                new DomainFormatter(),
                new CanonicalTickFormatter(),
                new PlateIdFormatter(),
                new MotionSegmentIdFormatter(),

                // Generated formatters
                new FantaSim.Geosphere.Plate.Topology.Contracts.Identity.TruthStreamIdentityMessagePackFormatter(),
                new FantaSim.Geosphere.Plate.Kinematics.Contracts.Events.PlateMotionModelAssignedEventMessagePackFormatter(),
                new FantaSim.Geosphere.Plate.Kinematics.Contracts.Events.MotionSegmentUpsertedEventMessagePackFormatter(),
                new FantaSim.Geosphere.Plate.Kinematics.Contracts.Events.MotionSegmentRetiredEventMessagePackFormatter()
            },
            new IFormatterResolver[]
            {
                NativeGuidResolver.Instance,
                BuiltinResolver.Instance,
                StandardResolver.Instance
            }
        ));

    private static readonly Dictionary<string, Type> EventTypeMap = new(StringComparer.Ordinal)
    {
        { nameof(PlateMotionModelAssignedEvent), typeof(PlateMotionModelAssignedEvent) },
        { nameof(MotionSegmentUpsertedEvent), typeof(MotionSegmentUpsertedEvent) },
        { nameof(MotionSegmentRetiredEvent), typeof(MotionSegmentRetiredEvent) }
    };

    private static readonly Dictionary<Type, string> TypeToEventTypeMap = new()
    {
        { typeof(PlateMotionModelAssignedEvent), nameof(PlateMotionModelAssignedEvent) },
        { typeof(MotionSegmentUpsertedEvent), nameof(MotionSegmentUpsertedEvent) },
        { typeof(MotionSegmentRetiredEvent), nameof(MotionSegmentRetiredEvent) }
    };

    public static byte[] Serialize(IPlateKinematicsEvent value)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value);
#else
        if (value is null) throw new ArgumentNullException(nameof(value));
#endif

        var payloadBytes = MessagePackSerializer.Serialize(value.GetType(), value, Options);
        return MessagePackEnvelopeCodec.Serialize(value.EventType, payloadBytes);
    }

    public static IPlateKinematicsEvent Deserialize(byte[] data)
    {
        var envelope = MessagePackEnvelopeCodec.Deserialize(data);
        if (!EventTypeMap.TryGetValue(envelope.TypeId, out var eventTypeType))
            throw new InvalidOperationException($"Unknown event type: {envelope.TypeId}");

        var eventObj = MessagePackSerializer.Deserialize(eventTypeType, envelope.Payload, Options);
        return (IPlateKinematicsEvent)eventObj!;
    }

    public static T Deserialize<T>(byte[] data) where T : IPlateKinematicsEvent
    {
        var envelope = MessagePackEnvelopeCodec.Deserialize(data);

        if (!TypeToEventTypeMap.TryGetValue(typeof(T), out var expectedEventType))
            throw new InvalidOperationException($"Type {typeof(T).Name} is not a registered kinematics event type.");

        if (!string.Equals(envelope.TypeId, expectedEventType, StringComparison.Ordinal))
            throw new InvalidOperationException($"Event type mismatch. Expected {expectedEventType}, got {envelope.TypeId}");

        return MessagePackSerializer.Deserialize<T>(envelope.Payload, Options);
    }
}
