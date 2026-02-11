using Plate.SCG.General.AutoToString.Attributes;

namespace FantaSim.World.Contracts.Time;

[AutoToString]
public readonly partial record struct EpochId(string Value);
