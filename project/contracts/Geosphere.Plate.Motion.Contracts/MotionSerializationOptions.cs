using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using FantaSim.Geosphere.Plate.Motion.Contracts;

namespace FantaSim.Geosphere.Plate.Motion.Contracts;

public static class MotionSerializationOptions
{
    public static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(CompositeResolver.Create(
            new IMessagePackFormatter[]
            {
                new MotionPathMessagePackFormatter(),
                new MotionPathSampleMessagePackFormatter(),
                new ReconstructionProvenanceMessagePackFormatter(),
            },
            new IFormatterResolver[]
            {
                NativeGuidResolver.Instance,
                BuiltinResolver.Instance,
                StandardResolver.Instance
            }
        ));
}
