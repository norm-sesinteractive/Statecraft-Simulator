using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// Media fills the river. Airtime stirs mood within months (fast, shallow) and, held for
    /// years, cultivates each cohort's disposition (slow, deep; youth most of all).
    /// </summary>
    internal sealed class MediaSystem : ISimSystem
    {
        /// <summary>The mood each vice settles at under a given airtime mix.</summary>
        public static ViceVector MoodTarget(MediaMix media, MediaTuning tuning)
        {
            var target = ViceVector.Zero;
            for (int v = 0; v < ViceVector.Count; v++) target[v] = tuning.MoodGain * media.Push((Vice)v);
            return target;
        }

        public void Tick(SimContext context)
        {
            var tuning = context.Tuning.Media;
            double alpha = SimMath.AlphaFromTau(tuning.MoodTauMonths);
            var nations = context.World.Nations;

            for (int n = 0; n < nations.Count; n++)
            {
                var society = nations[n].Society;
                var media = nations[n].Institutions.Media;

                for (int v = 0; v < ViceVector.Count; v++)
                {
                    double push = media.Push((Vice)v);
                    double target = context.Ledger.Add(n, Quantity.Mood, v, LedgerNames.Media[v], tuning.MoodGain * push);
                    double noise = tuning.MoodNoise > 0 ? tuning.MoodNoise * context.Random.NextGaussian() : 0;
                    society.mood[v] = SimMath.Clamp(society.mood[v] + (target - society.mood[v]) * alpha + noise, -1, 1);

                    Cultivate(society.cohorts[(int)Cohort.Youth], v, push * tuning.CultivationYouth);
                    Cultivate(society.cohorts[(int)Cohort.Adult], v, push * tuning.CultivationAdult);
                    Cultivate(society.cohorts[(int)Cohort.Elder], v, push * tuning.CultivationElder);
                }
            }
        }

        // Saturating drift: a positive push closes part of the gap to 1, a negative push part of the gap to 0.
        static void Cultivate(CohortState cohort, int vice, double rate)
        {
            double d = cohort.disposition[vice];
            d += rate > 0 ? rate * (1 - d) : rate * d;
            cohort.disposition[vice] = SimMath.Clamp01(d);
        }
    }
}
