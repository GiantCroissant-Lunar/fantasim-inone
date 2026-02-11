namespace FantaSim.App.Bundles.Contracts.Interaction;

/// <summary>
/// Marker interface for all HUD commands flowing into the system.
/// </summary>
public interface IHudCommand
{
    Guid CommandId { get; }
}
