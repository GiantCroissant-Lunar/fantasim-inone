using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using FantaSim.Geosphere.Plate.Partition.Contracts;

namespace FantaSim.Geosphere.Plate.Partition.Contracts;

public static class PartitionSerializationOptions
{
    public static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(CompositeResolver.Create(
            new IMessagePackFormatter[]
            {
                new PlatePolygonMessagePackFormatter(),
                new PlatePartitionResultMessagePackFormatter(),
                new PartitionRequestMessagePackFormatter(),
                new PartitionOptionsMessagePackFormatter(),

                new PartitionProvenanceMessagePackFormatter(),
                new PartitionQualityMetricsMessagePackFormatter(),
            },
            new IFormatterResolver[]
            {
                NativeGuidResolver.Instance,
                BuiltinResolver.Instance,
                StandardResolver.Instance
            }
        ));
}
