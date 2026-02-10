namespace Fantasim.World.Plates.Contracts.Events;

/// <summary>
/// Event envelope for plate topology events per RFC-V2-0002.
/// </summary>
public interface IPlateTopologyEvent
{
    Guid EventId { get; }
    string EventType { get; }
    long Sequence { get; }
    long Tick { get; }
}
