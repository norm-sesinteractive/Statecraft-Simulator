using System;
using System.Collections.Generic;
using Statecraft.Sim.Core;
using Statecraft.Sim.Institutions;

namespace Statecraft.Sim.Commands
{
    /// <summary>Sets a nation's airtime mix from eight weights: the seven vices in order, then neutral programming.</summary>
    public sealed class SetMediaMix : SimCommand
    {
        readonly double[] weights;

        public SetMediaMix(int nationIndex, IReadOnlyList<double> weights)
        {
            if (weights == null) throw new ArgumentNullException(nameof(weights));
            if (weights.Count != MediaMix.CategoryCount)
                throw new ArgumentException($"Expected {MediaMix.CategoryCount} weights, got {weights.Count}.", nameof(weights));

            NationIndex = nationIndex;
            this.weights = new double[weights.Count];
            for (int i = 0; i < weights.Count; i++) this.weights[i] = weights[i];
        }

        public int NationIndex { get; }

        public IReadOnlyList<double> Weights => weights;

        internal override void Apply(SimContext context)
        {
            var nation = ResolveNation(context.World, NationIndex);
            var before = nation.Institutions.Media.ToArray();
            nation.Institutions.Media.SetFromWeights(weights);
            RecordShock(nation, before, nation.Institutions.Media.ToArray());
        }
    }
}
