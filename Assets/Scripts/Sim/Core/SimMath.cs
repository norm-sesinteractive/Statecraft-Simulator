using System;
using System.Collections.Generic;

namespace Statecraft.Sim.Core
{
    /// <summary>Numeric helpers shared by every system. All simulation math is double precision.</summary>
    public static class SimMath
    {
        public static double Clamp01(double x) => x < 0 ? 0 : (x > 1 ? 1 : x);

        public static double Clamp(double x, double min, double max) => x < min ? min : (x > max ? max : x);

        /// <summary>
        /// Fraction of the remaining gap a first-order lag closes in one tick, for a time constant
        /// in months. A time constant of 12 closes about 63% of the gap in a year.
        /// </summary>
        public static double AlphaFromTau(double tauMonths) => tauMonths <= 0 ? 1.0 : 1.0 - Math.Exp(-1.0 / tauMonths);

        /// <summary>Moves <paramref name="current"/> one tick toward <paramref name="target"/> with time constant <paramref name="tauMonths"/>.</summary>
        public static double Approach(double current, double target, double tauMonths) =>
            current + (target - current) * AlphaFromTau(tauMonths);

        /// <summary>
        /// Turns arbitrary non-negative weights into shares that sum to 1, written into
        /// <paramref name="destination"/>. Negative or non-finite weights count as 0; if nothing
        /// positive remains, the shares are even.
        /// </summary>
        public static void NormalizeWeights(IReadOnlyList<double> weights, double[] destination)
        {
            if (weights == null) throw new ArgumentNullException(nameof(weights));
            if (weights.Count != destination.Length)
                throw new ArgumentException($"Expected {destination.Length} weights, got {weights.Count}.", nameof(weights));

            double sum = 0;
            for (int i = 0; i < destination.Length; i++)
            {
                double w = weights[i];
                if (double.IsNaN(w) || double.IsInfinity(w) || w < 0) w = 0;
                destination[i] = w;
                sum += w;
            }

            for (int i = 0; i < destination.Length; i++)
                destination[i] = sum > 0 ? destination[i] / sum : 1.0 / destination.Length;
        }
    }
}
