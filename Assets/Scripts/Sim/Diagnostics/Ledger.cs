using System;
using System.Collections.Generic;
using Statecraft.Sim.Economy;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Diagnostics
{
    /// <summary>
    /// Records every term that went into this tick's computed targets, so any outcome can answer
    /// "why?" (the in-game explanation panel, tests, and the generated causal graph all read it).
    /// Cleared at the start of each tick; allocation-free after warm-up.
    /// </summary>
    public sealed class Ledger
    {
        readonly List<Contribution> entries = new List<Contribution>(2048);

        /// <summary>Recording on or off. Off costs nothing; on costs one struct per term.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>All terms recorded during the most recent tick.</summary>
        public IReadOnlyList<Contribution> Entries => entries;

        /// <summary>Technical name of a quantity, e.g. "Contentment", "Frustration.Wrath" or "Alignment.Democracy".</summary>
        public static string NameOf(Quantity target, int index)
        {
            if (index < 0) return target.ToString();
            if (target == Quantity.Alignment) return target + "." + (GovernmentType)index;
            if (target == Quantity.Yield) return target + "." + (Resource)index;
            return target + "." + (Vice)index;
        }

        /// <summary>
        /// Copies the most recent tick's national terms for one target into <paramref name="results"/>, largest
        /// magnitude first. Pass -1 as <paramref name="index"/> for targets that are not per vice or per type.
        /// </summary>
        public void Collect(int nationIndex, Quantity target, int index, List<Contribution> results) =>
            Collect(nationIndex, target, index, -1, results);

        /// <summary>As above, for one generation's terms (cohort is a Cohort value) or national ones (-1).</summary>
        public void Collect(int nationIndex, Quantity target, int index, int cohort, List<Contribution> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            results.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                var c = entries[i];
                if (c.NationIndex == nationIndex && c.Target == target && c.ViceIndex == index && c.Cohort == cohort) results.Add(c);
            }
            results.Sort((a, b) => Math.Abs(b.Amount).CompareTo(Math.Abs(a.Amount)));
        }

        internal void Clear() => entries.Clear();

        internal double Add(int nationIndex, Quantity target, string source, double amount) =>
            Add(nationIndex, target, -1, -1, source, amount);

        internal double Add(int nationIndex, Quantity target, int index, string source, double amount) =>
            Add(nationIndex, target, index, -1, source, amount);

        internal double Add(int nationIndex, Quantity target, int index, int cohort, string source, double amount)
        {
            if (Enabled) entries.Add(new Contribution(nationIndex, target, index, cohort, source, amount));
            return amount;
        }
    }
}
