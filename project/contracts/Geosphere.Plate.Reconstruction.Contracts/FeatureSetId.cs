using MessagePack;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Reconstruction.Contracts;

/// <summary>
/// Feature set identifier for batch reconstruction operations.
/// </summary>
[UnifyModel]
public readonly record struct FeatureSetId
{
    [SerializationConstructor]
    public FeatureSetId(Guid value)
    {
        Value = value;
    }

    [UnifyProperty(0)]
    public Guid Value { get; init; }

    [UnifyIgnore]
    public bool IsEmpty => Value == Guid.Empty;

    public static FeatureSetId NewId() => new(Guid.NewGuid());

    public static FeatureSetId Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("FeatureSetId cannot be null or whitespace.", nameof(value));
        return new FeatureSetId(Guid.Parse(value));
    }

    public override string ToString() => Value.ToString("D");
}
