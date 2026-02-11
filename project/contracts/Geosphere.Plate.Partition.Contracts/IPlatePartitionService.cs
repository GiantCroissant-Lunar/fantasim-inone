namespace FantaSim.Geosphere.Plate.Partition.Contracts;

public interface IPlatePartitionService
{
    PlatePartitionResult Partition(PartitionRequest request);

    Task<PlatePartitionResult> PartitionAsync(PartitionRequest request, CancellationToken cancellationToken = default);
}
