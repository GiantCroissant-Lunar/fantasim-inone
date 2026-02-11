using System;
using MessagePack;
using MessagePack.Formatters;
using UnifyGeometry;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Serialization;

/// <summary>
/// Custom MessagePack formatter for Segment2.
/// Format: [startX, startY, endX, endY]
/// </summary>
public sealed class Segment2Formatter : IMessagePackFormatter<Segment2>
{
    public void Serialize(ref MessagePackWriter writer, Segment2 value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(4);
        writer.Write(value.Start.X);
        writer.Write(value.Start.Y);
        writer.Write(value.End.X);
        writer.Write(value.End.Y);
    }

    public Segment2 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new InvalidOperationException("Segment2 cannot be nil");
        }

        var count = reader.ReadArrayHeader();
        if (count != 4)
        {
            throw new InvalidOperationException($"Segment2 expected 4 elements, got {count}");
        }

        var startX = reader.ReadDouble();
        var startY = reader.ReadDouble();
        var endX = reader.ReadDouble();
        var endY = reader.ReadDouble();

        return new Segment2(new Point2(startX, startY), new Point2(endX, endY));
    }
}
