using MessagePack;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Reconstruction.Contracts.Policies;

/// <summary>
/// Uniquely identifies a kinematics model for rotation calculations.
/// </summary>
[UnifyModel]
public readonly record struct ModelId
{
    [SerializationConstructor]
    public ModelId(Guid value)
    {
        Value = value;
    }

    [UnifyProperty(0)]
    public Guid Value { get; init; }

    [UnifyIgnore]
    public bool IsEmpty => Value == Guid.Empty;

    public static readonly ModelId Default = new(Guid.Empty);

    public static ModelId NewId() => new(Guid.NewGuid());

    public static ModelId Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("ModelId cannot be null or whitespace.", nameof(value));
        return new ModelId(Guid.Parse(value));
    }

    public override string ToString() => Value.ToString("D");
}
