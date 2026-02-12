namespace FantaSim.Geosphere.Plate.Simulation.Des.Contracts;

public readonly record struct DriverHandle(int Id)
{
    public static readonly DriverHandle Empty = new(0);
}

public readonly record struct TriggerHandle(int Id)
{
    public static readonly TriggerHandle Empty = new(0);
}
