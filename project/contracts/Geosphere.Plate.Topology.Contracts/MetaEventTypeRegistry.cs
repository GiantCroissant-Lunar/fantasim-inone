using System;
using System.Collections.Generic;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;

namespace FantaSim.Geosphere.Plate.Topology.Contracts;

/// <summary>
/// Registry for stable bidirectional mapping between meta/governance event type IDs and CLR types.
/// </summary>
public static class MetaEventTypeRegistry
{
    private static readonly Dictionary<string, Type> IdToType = new(StringComparer.Ordinal);
    private static readonly Dictionary<Type, string> TypeToId = new();

    static MetaEventTypeRegistry()
    {
        var mappings = new (string id, Type type)[]
        {
            ("BranchCreatedEvent", typeof(BranchCreatedEvent)),
            ("BranchForkedEvent", typeof(BranchForkedEvent))
        };

        foreach (var (id, type) in mappings)
        {
            if (IdToType.ContainsKey(id))
                throw new InvalidOperationException($"Duplicate meta event type ID: {id}");
            if (TypeToId.ContainsKey(type))
                throw new InvalidOperationException($"Duplicate meta event type: {type}");

            IdToType[id] = type;
            TypeToId[type] = id;
        }
    }

    public static Type Resolve(string id)
    {
        if (IdToType.TryGetValue(id, out var type))
            return type;

        throw new UnknownEventTypeException(id);
    }

    public static string GetId(Type type)
    {
        if (TypeToId.TryGetValue(type, out var id))
            return id;

        throw new UnregisteredEventTypeException(type);
    }
}
