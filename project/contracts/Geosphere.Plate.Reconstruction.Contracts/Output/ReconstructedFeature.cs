using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using UnifySerialization.Abstractions;
using UnifyGeometry;

namespace FantaSim.Geosphere.Plate.Reconstruction.Contracts.Output;

[UnifyModel]
public readonly record struct ReconstructedFeature(
    [property: UnifyProperty(0)] FeatureId FeatureId,
    [property: UnifyProperty(1)] PlateId PlateIdProvenance,
    [property: UnifyProperty(2)] IGeometry Geometry);
