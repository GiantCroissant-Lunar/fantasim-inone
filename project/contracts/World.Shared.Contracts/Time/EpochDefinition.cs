namespace FantaSim.World.Contracts.Time;

public sealed record EpochDefinition(
    EpochId Id,
    SphereId Sphere,
    TickRange Range);
