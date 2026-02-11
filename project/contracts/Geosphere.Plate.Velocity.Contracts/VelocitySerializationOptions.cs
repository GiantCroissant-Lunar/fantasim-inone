using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using FantaSim.Geosphere.Plate.Velocity.Contracts;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

public static class VelocitySerializationOptions
{
    public static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(CompositeResolver.Create(
            new IMessagePackFormatter[]
            {
                // Core velocity types
                new Velocity3dMessagePackFormatter(),
                new AngularVelocity3dMessagePackFormatter(),

                // Boundary samples and profiles
                new BoundaryVelocitySampleMessagePackFormatter(),
                new BoundaryVelocityProfileMessagePackFormatter(),
                new BoundaryVelocityCollectionMessagePackFormatter(),
                new BoundaryRateSampleMessagePackFormatter(),
                new BoundaryRateProfileMessagePackFormatter(),

                // Sampling and statistics
                new BoundarySampleSpecMessagePackFormatter(),
                new SampleProvenanceMessagePackFormatter(),
                new RateStatisticsMessagePackFormatter(),
                new RateUncertaintyMessagePackFormatter(),

                // Summary types
                new ConvergenceRateSummaryMessagePackFormatter(),
                new SpreadingRateMetricsMessagePackFormatter(),
                new StrikeSlipSummaryMessagePackFormatter(),
            },
            new IFormatterResolver[]
            {
                NativeGuidResolver.Instance,
                BuiltinResolver.Instance,
                StandardResolver.Instance
            }
        ));
}
