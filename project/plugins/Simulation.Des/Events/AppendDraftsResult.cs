namespace FantaSim.Geosphere.Plate.Simulation.Des.Events;

public record AppendDraftsResult(
    long LastSequence,
    ReadOnlyMemory<byte> LastHash
);
