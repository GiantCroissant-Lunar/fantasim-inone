namespace FantaSim.Geosphere.Plate.Simulation.Des.Contracts;

public sealed record SimObservation(
    long Tick,
    int EventCount,
    string Checksum
);
