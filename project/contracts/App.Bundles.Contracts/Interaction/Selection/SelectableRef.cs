namespace FantaSim.App.Bundles.Contracts.Interaction.Selection;

/// <summary>
/// Lightweight, immutable reference to any selectable entity in the HUD.
/// </summary>
public sealed record SelectableRef(string Kind, string Id)
{
    public const string KindPlate = "plate";
    public const string KindBoundary = "boundary";
    public const string KindBundle = "bundle";
}
