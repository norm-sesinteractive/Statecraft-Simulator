using System;
using System.Collections.Generic;
using Statecraft.Sim.Core;
using Statecraft.Sim.Institutions;

namespace Statecraft.Sim.Commands
{
    /// <summary>Sets a nation's curriculum from one weight per <see cref="CurriculumFocus"/>, in order.</summary>
    public sealed class SetCurriculum : SimCommand
    {
        readonly double[] weights;

        public SetCurriculum(int nationIndex, IReadOnlyList<double> weights)
        {
            if (weights == null) throw new ArgumentNullException(nameof(weights));
            if (weights.Count != Curriculum.FocusCount)
                throw new ArgumentException($"Expected {Curriculum.FocusCount} weights, got {weights.Count}.", nameof(weights));

            NationIndex = nationIndex;
            this.weights = new double[weights.Count];
            for (int i = 0; i < weights.Count; i++) this.weights[i] = weights[i];
        }

        public int NationIndex { get; }

        public IReadOnlyList<double> Weights => weights;

        internal override void Apply(World world) => ResolveNation(world, NationIndex).Institutions.Curriculum.SetFromWeights(weights);
    }
}
