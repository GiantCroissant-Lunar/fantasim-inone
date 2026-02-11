using Plate.SCG.General.AutoToString.Attributes;

namespace FantaSim.World.Contracts.Time;

[AutoToString]
public readonly partial record struct SphereId(string Value) : IComparable<SphereId>
{
    public int CompareTo(SphereId other) =>
        string.Compare(Value, other.Value, StringComparison.Ordinal);
}
