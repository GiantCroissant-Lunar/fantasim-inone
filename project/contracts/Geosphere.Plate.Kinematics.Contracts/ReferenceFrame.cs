using System.Collections.Generic;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Numerics;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using MessagePack;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Kinematics.Contracts;

#region Reference Frames

[MessagePackObject]
[Union(0, typeof(MantleFrame))]
[Union(1, typeof(PlateAnchor))]
[Union(2, typeof(AbsoluteFrame))]
[Union(3, typeof(CustomFrame))]
public abstract record ReferenceFrameId
{
    public abstract override string ToString();
}

[MessagePackObject]
public sealed record MantleFrame : ReferenceFrameId
{
    public static readonly MantleFrame Instance = new();
    public override string ToString() => "Mantle";
}

[MessagePackObject]
public sealed record PlateAnchor : ReferenceFrameId
{
    [Key(0)]
    public required PlateId PlateId { get; init; }
    public override string ToString() => $"Anchor({PlateId.Value})";
}

[MessagePackObject]
public sealed record AbsoluteFrame : ReferenceFrameId
{
    public static readonly AbsoluteFrame Instance = new();
    public override string ToString() => "Absolute";
}

[MessagePackObject]
public sealed record CustomFrame : ReferenceFrameId
{
    [Key(0)]
    public required FrameDefinition Definition { get; init; }
    public override string ToString() => $"Custom({Definition.Name})";
}

#endregion

public sealed class CyclicFrameReferenceException : System.InvalidOperationException
{
    public CyclicFrameReferenceException(string message) : base(message) { }
}

#region Frame Definition

[MessagePackObject]
public sealed record FrameDefinition
{
    [Key(0)]
    public required string Name { get; init; }

    [Key(1)]
    public required IReadOnlyList<FrameChainLink> Chain { get; init; }

    [Key(2)]
    public FrameDefinitionMetadata? Metadata { get; init; }
}

[MessagePackObject]
public sealed record FrameChainLink
{
    [Key(0)]
    public required ReferenceFrameId BaseFrame { get; init; }

    [Key(1)]
    public required FiniteRotation Transform { get; init; }

    [Key(2)]
    public CanonicalTickRange? ValidityRange { get; init; }

    [Key(3)]
    public int? SequenceHint { get; init; }
}

[MessagePackObject]
public readonly record struct CanonicalTickRange
{
    [Key(0)]
    public required CanonicalTick StartTick { get; init; }

    [Key(1)]
    public required CanonicalTick EndTick { get; init; }

    public bool Contains(CanonicalTick tick) => tick >= StartTick && tick <= EndTick;
}

[MessagePackObject]
public sealed record FrameDefinitionMetadata
{
    [Key(0)]
    public string? Description { get; init; }

    [Key(1)]
    public string? Author { get; init; }
}

#endregion
