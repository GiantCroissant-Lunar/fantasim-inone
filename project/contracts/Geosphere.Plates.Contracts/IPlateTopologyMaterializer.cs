namespace FantaSim.Geosphere.Plates.Contracts;

/// <summary>
/// Materializes plate topology events into a read model.
/// </summary>
public interface IPlateTopologyMaterializer
{
    IPlateTopologyStateView Materialize();
    IPlateTopologyStateView MaterializeAtSequence(long targetSequence);
    IPlateTopologyStateView MaterializeAtTick(long targetTick);
}
