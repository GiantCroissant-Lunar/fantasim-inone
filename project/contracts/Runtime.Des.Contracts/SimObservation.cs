namespace FantaSim.Geosphere.Plate.Runtime.Des.Contracts;

public sealed record SimObservation(
    long Tick,
    int EventCount,
    string Checksum
);
