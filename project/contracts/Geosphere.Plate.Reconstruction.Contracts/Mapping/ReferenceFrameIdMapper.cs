using FantaSim.Geosphere.Plate.Kinematics.Contracts;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Numerics;
using FantaSim.Geosphere.Plate.Reconstruction.Contracts.Generated;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Reconstruction.Contracts.Mapping;

public sealed class ReferenceFrameIdMapper
{
    /// <summary>
    /// Maps DTO to domain model.
    /// </summary>
    public ReferenceFrameId ToDomain(ReferenceFrameIdDto dto)
        => MapReferenceFrameIdDtoToDomain(dto);

    /// <summary>
    /// Maps domain model back to DTO.
    /// </summary>
    public ReferenceFrameIdDto ToDto(ReferenceFrameId domain)
        => MapReferenceFrameIdDomainToDto(domain);

    /// <summary>
    /// Maps FrameDefinition DTO to domain model.
    /// </summary>
    public FrameDefinition ToDomain(FrameDefinitionDto dto)
        => MapFrameDefinitionDtoToDomain(dto);

    /// <summary>
    /// Maps FrameDefinition domain model back to DTO.
    /// </summary>
    public FrameDefinitionDto ToDto(FrameDefinition domain)
        => MapFrameDefinitionDomainToDto(domain);

    /// <summary>
    /// Maps FrameChainLink DTO to domain model.
    /// </summary>
    public FrameChainLink ToDomain(FrameChainLinkDto dto)
        => MapFrameChainLinkDtoToDomain(dto);

    /// <summary>
    /// Maps FrameChainLink domain model back to DTO.
    /// </summary>
    public FrameChainLinkDto ToDto(FrameChainLink domain)
        => MapFrameChainLinkDomainToDto(domain);

    #region ReferenceFrameId Discriminated Union Mapping

    private static ReferenceFrameId MapReferenceFrameIdDtoToDomain(ReferenceFrameIdDto dto)
    {
        return dto.Type switch
        {
            "mantle" => MantleFrame.Instance,
            "plateAnchor" => new PlateAnchor { PlateId = PlateId.Parse(dto.PlateId ?? string.Empty) },
            "absolute" => AbsoluteFrame.Instance,
            "custom" => new CustomFrame { Definition = MapFrameDefinitionDtoToDomain(dto.Definition!) },
            _ => throw new ArgumentException($"Unknown reference frame type: {dto.Type}")
        };
    }

    private static ReferenceFrameIdDto MapReferenceFrameIdDomainToDto(ReferenceFrameId domain)
    {
        return domain switch
        {
            MantleFrame => new ReferenceFrameIdDto { Type = "mantle" },
            PlateAnchor anchor => new ReferenceFrameIdDto
            {
                Type = "plateAnchor",
                PlateId = anchor.PlateId.Value.ToString()
            },
            AbsoluteFrame => new ReferenceFrameIdDto { Type = "absolute" },
            CustomFrame custom => new ReferenceFrameIdDto
            {
                Type = "custom",
                Definition = MapFrameDefinitionDomainToDto(custom.Definition)
            },
            _ => throw new ArgumentException($"Unknown reference frame type: {domain.GetType().Name}")
        };
    }

    #endregion

    #region FrameDefinition Mapping

    private static FrameDefinition MapFrameDefinitionDtoToDomain(FrameDefinitionDto dto)
        => new()
        {
            Name = dto.Name,
            Chain = dto.Chain.Select(MapFrameChainLinkDtoToDomain).ToList(),
            Metadata = dto.Metadata is null ? null : MapFrameDefinitionMetadataDtoToDomain(dto.Metadata)
        };

    private static FrameDefinitionDto MapFrameDefinitionDomainToDto(FrameDefinition domain)
        => new()
        {
            Name = domain.Name,
            Chain = domain.Chain.Select(MapFrameChainLinkDomainToDto).ToList(),
            Metadata = domain.Metadata is null ? null : MapFrameDefinitionMetadataDomainToDto(domain.Metadata)
        };

    #endregion

    #region FrameChainLink Mapping

    private static FrameChainLink MapFrameChainLinkDtoToDomain(FrameChainLinkDto dto)
        => new()
        {
            BaseFrame = MapReferenceFrameIdDtoToDomain(dto.BaseFrame),
            Transform = MapFiniteRotationDtoToDomain(dto.Transform),
            ValidityRange = dto.ValidityRange is null ? null : MapCanonicalTickRangeDtoToDomain(dto.ValidityRange),
            SequenceHint = dto.SequenceHint
        };

    private static FrameChainLinkDto MapFrameChainLinkDomainToDto(FrameChainLink domain)
        => new()
        {
            BaseFrame = MapReferenceFrameIdDomainToDto(domain.BaseFrame),
            Transform = MapFiniteRotationDomainToDto(domain.Transform),
            ValidityRange = domain.ValidityRange is null ? null : MapCanonicalTickRangeDomainToDto(domain.ValidityRange.Value),
            SequenceHint = domain.SequenceHint
        };

    #endregion

    #region FiniteRotation Mapping

    private static FiniteRotation MapFiniteRotationDtoToDomain(FiniteRotationDto dto)
    {
        var orientation = new Quaterniond(dto.W, dto.X, dto.Y, dto.Z);
        return new FiniteRotation(orientation);
    }

    private static FiniteRotationDto MapFiniteRotationDomainToDto(FiniteRotation domain)
        => new()
        {
            W = domain.Orientation.W,
            X = domain.Orientation.X,
            Y = domain.Orientation.Y,
            Z = domain.Orientation.Z
        };

    #endregion

    #region CanonicalTickRange Mapping

    private static CanonicalTickRange? MapCanonicalTickRangeDtoToDomain(CanonicalTickRangeDto dto)
        => new()
        {
            StartTick = new CanonicalTick(dto.StartTick),
            EndTick = new CanonicalTick(dto.EndTick)
        };

    private static CanonicalTickRangeDto MapCanonicalTickRangeDomainToDto(CanonicalTickRange domain)
        => new()
        {
            StartTick = domain.StartTick.Value,
            EndTick = domain.EndTick.Value
        };

    #endregion

    #region FrameDefinitionMetadata Mapping

    private static FrameDefinitionMetadata MapFrameDefinitionMetadataDtoToDomain(FrameDefinitionMetadataDto dto)
        => new()
        {
            Description = dto.Description,
            Author = dto.Author
        };

    private static FrameDefinitionMetadataDto MapFrameDefinitionMetadataDomainToDto(FrameDefinitionMetadata domain)
        => new()
        {
            Description = domain.Description,
            Author = domain.Author
        };

    #endregion
}
