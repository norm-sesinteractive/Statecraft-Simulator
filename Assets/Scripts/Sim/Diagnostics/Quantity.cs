namespace Statecraft.Sim.Diagnostics
{
    /// <summary>Computed quantities whose contributing terms the <see cref="Ledger"/> records each tick.</summary>
    public enum Quantity
    {
        Mood,
        Expressed,
        Outlet,
        Frustration,
        Fear,
        Crime,
        Contentment,
        Unrest,
        Aggression,
    }
}
