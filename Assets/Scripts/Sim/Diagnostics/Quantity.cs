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

        /// <summary>Births per adult per year.</summary>
        Births,

        /// <summary>Deaths per person per year in one generation (recorded per cohort).</summary>
        Deaths,

        /// <summary>Food each person needs per month.</summary>
        FoodNeed,

        /// <summary>Labor productivity multiplier.</summary>
        Productivity,

        /// <summary>Output multiplier for a resource; the index is a Resource, not a vice.</summary>
        Yield,

        /// <summary>This month's change in wealth: income from what is produced beyond need, less upkeep.</summary>
        Wealth,
    }
}
