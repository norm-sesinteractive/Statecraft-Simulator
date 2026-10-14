using System;
using System.Collections.Generic;
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
        readonly List<Contribution> entries = new List<Contribution>(1024);

        /// <summary>Recording on or off. Off costs nothing; on costs one struct per term.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>All terms recorded during the most recent tick.</summary>
        public IReadOnlyList<Contribution> Entries => entries;

        /// <summary>Technical name of a quantity, e.g. "Contentment" or "Frustration.Wrath".</summary>
        public static string NameOf(Quantity target, int viceIndex) =>
            viceIndex < 0 ? target.ToString() : target + "." + (Vice)viceIndex;

        /// <summary>
        /// Copies the most recent tick's terms for one target into <paramref name="results"/>,
        /// largest magnitude first. Pass -1 as <paramref name="viceIndex"/> for non-vice targets.
        /// </summary>
        public void Collect(int nationIndex, Quantity target, int viceIndex, List<Contribution> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            results.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                var c = entries[i];
                if (c.NationIndex == nationIndex && c.Target == target && c.ViceIndex == viceIndex) results.Add(c);
            }
            results.Sort((a, b) => Math.Abs(b.Amount).CompareTo(Math.Abs(a.Amount)));
        }

        internal void Clear() => entries.Clear();

        internal double Add(int nationIndex, Quantity target, string source, double amount) =>
            Add(nationIndex, target, -1, source, amount);

        internal double Add(int nationIndex, Quantity target, int viceIndex, string source, double amount)
        {
            if (Enabled) entries.Add(new Contribution(nationIndex, target, viceIndex, source, amount));
            return amount;
        }
    }
}
