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

        /// <summary>Target of government suspicion.</summary>
        GovernmentSuspicion,

        /// <summary>This month's drift of shadow suspicion (not counting one-off jumps from events).</summary>
        ShadowSuspicion,

        /// <summary>Alignment with a government type; the index is a GovernmentType, not a vice.</summary>
        Alignment,
    }
}
