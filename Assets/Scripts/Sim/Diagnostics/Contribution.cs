namespace Statecraft.Sim.Diagnostics
{
    /// <summary>
    /// One recorded term of a computed value: this tick, <see cref="Target"/> (optionally for one
    /// vice) received <see cref="Amount"/> from <see cref="Source"/>. Sources that are themselves
    /// quantities use the same naming as <see cref="Ledger.NameOf"/>, so terms chain into a graph.
    /// </summary>
    public readonly struct Contribution
    {
        public Contribution(int nationIndex, Quantity target, int viceIndex, string source, double amount)
        {
            NationIndex = nationIndex;
            Target = target;
            ViceIndex = viceIndex;
            Source = source;
            Amount = amount;
        }

        public int NationIndex { get; }

        public Quantity Target { get; }

        /// <summary>Vice of a per-vice target, or -1 when the target is not per vice.</summary>
        public int ViceIndex { get; }

        /// <summary>Technical source name such as "Frustration.Wrath", "Law.Severity" or "Base".</summary>
        public string Source { get; }

        public double Amount { get; }

        public string TargetName => Ledger.NameOf(Target, ViceIndex);
    }
}
