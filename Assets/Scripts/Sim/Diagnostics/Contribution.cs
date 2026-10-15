namespace Statecraft.Sim.Diagnostics
{
    /// <summary>
    /// One recorded term of a computed value: this tick, <see cref="Target"/> (optionally for one vice,
    /// government type or generation) received <see cref="Amount"/> from <see cref="Source"/>. Sources that
    /// are themselves quantities use the same naming as <see cref="Ledger.NameOf"/>, so terms chain into a graph.
    /// </summary>
    public readonly struct Contribution
    {
        public Contribution(int nationIndex, Quantity target, int index, int cohort, string source, double amount)
        {
            NationIndex = nationIndex;
            Target = target;
            ViceIndex = index;
            Cohort = cohort;
            Source = source;
            Amount = amount;
        }

        public int NationIndex { get; }

        public Quantity Target { get; }

        /// <summary>Vice of a per-vice target, government type for Alignment, or -1.</summary>
        public int ViceIndex { get; }

        /// <summary>Generation (a Cohort) for per-generation targets, or -1 for national ones.</summary>
        public int Cohort { get; }

        /// <summary>Technical source name such as "Frustration.Wrath", "Law.Severity" or "Base".</summary>
        public string Source { get; }

        public double Amount { get; }

        public string TargetName => Ledger.NameOf(Target, ViceIndex);
    }
}
