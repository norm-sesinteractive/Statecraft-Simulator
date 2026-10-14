using System.Collections.Generic;
using Statecraft.Sim.Core;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Institutions
{
    /// <summary>
    /// What a nation's media airs: a share of airtime for each vice plus neutral programming,
    /// always summing to 1 ("a balance of the 7 aired to the population", Norm's pitch).
    /// </summary>
    public sealed class MediaMix
    {
        /// <summary>Seven vices plus neutral programming.</summary>
        public const int CategoryCount = ViceVector.Count + 1;

        /// <summary>Index of the neutral share in weight arrays (after the seven vices).</summary>
        public const int NeutralIndex = ViceVector.Count;

        /// <summary>The share each category gets in an even mix (1/8).</summary>
        public const double EvenShare = 1.0 / CategoryCount;

        readonly double[] shares = new double[CategoryCount];

        internal MediaMix(IReadOnlyList<double> weights) => SetFromWeights(weights);

        /// <summary>Airtime share of a vice, 0 to 1.</summary>
        public double Share(Vice vice) => shares[(int)vice];

        /// <summary>Airtime share of neutral programming that pushes no vice.</summary>
        public double NeutralShare => shares[NeutralIndex];

        /// <summary>
        /// How hard the media pushes a vice relative to an even mix: 0 at an even 1/8 share, +1
        /// when the vice has all the airtime, -1/7 when it has none. Inflaming is easier than calming.
        /// </summary>
        public double Push(Vice vice) => (shares[(int)vice] - EvenShare) / (1.0 - EvenShare);

        /// <summary>Copies all eight shares (seven vices in order, then neutral).</summary>
        public double[] ToArray() => (double[])shares.Clone();

        public static double[] EvenWeights()
        {
            var weights = new double[CategoryCount];
            for (int i = 0; i < weights.Length; i++) weights[i] = 1.0;
            return weights;
        }

        internal void SetFromWeights(IReadOnlyList<double> weights) => SimMath.NormalizeWeights(weights, shares);
    }
}
