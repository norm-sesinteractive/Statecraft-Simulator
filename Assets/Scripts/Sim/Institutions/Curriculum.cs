using System.Collections.Generic;
using Statecraft.Sim.Core;

namespace Statecraft.Sim.Institutions
{
    /// <summary>
    /// How a nation's schools split their effort across <see cref="CurriculumFocus"/> areas.
    /// Shares sum to 1: a school that teaches everything teaches each thing thinly.
    /// </summary>
    public sealed class Curriculum
    {
        public const int FocusCount = 7;

        readonly double[] shares = new double[FocusCount];

        internal Curriculum(IReadOnlyList<double> weights) => SetFromWeights(weights);

        public double Share(CurriculumFocus focus) => shares[(int)focus];

        /// <summary>Copies all shares in <see cref="CurriculumFocus"/> order.</summary>
        public double[] ToArray() => (double[])shares.Clone();

        public static double[] EvenWeights()
        {
            var weights = new double[FocusCount];
            for (int i = 0; i < weights.Length; i++) weights[i] = 1.0;
            return weights;
        }

        internal void SetFromWeights(IReadOnlyList<double> weights) => SimMath.NormalizeWeights(weights, shares);
    }
}
