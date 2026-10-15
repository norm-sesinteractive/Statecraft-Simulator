using System;
using Statecraft.Sim.Core;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// Time passing through people. Newborns inherit their parents' culture and arrive untrained;
    /// youth graduate into adults and adults retire into elders, carrying disposition and
    /// channeling with them. Every culture also drifts slowly back toward human nature.
    /// </summary>
    internal sealed class GenerationSystem : ISimSystem
    {
        /// <summary>Population-weighted disposition across cohorts.</summary>
        public static ViceVector NationalDisposition(SocietyState society)
        {
            var sum = ViceVector.Zero;
            for (int c = 0; c < society.cohorts.Length; c++)
                sum = sum + society.cohorts[c].disposition * society.cohorts[c].share;
            return sum.Clamp01();
        }

        public void Tick(SimContext context)
        {
            var tuning = context.Tuning.Generations;
            double birth = 1.0 / (tuning.YouthYears * SimDate.MonthsPerYear);
            double retire = 1.0 / (tuning.AdultYears * SimDate.MonthsPerYear);
            double nature = SimMath.AlphaFromTau(tuning.NatureTauMonths);
            var nations = context.World.Nations;

            for (int n = 0; n < nations.Count; n++)
            {
                var society = nations[n].Society;
                var youth = society.cohorts[(int)Cohort.Youth];
                var adult = society.cohorts[(int)Cohort.Adult];
                var elder = society.cohorts[(int)Cohort.Elder];

                // Snapshot first so every flow reads last month's culture, whatever the order below.
                ViceVector youthD = youth.disposition, adultD = adult.disposition, elderD = elder.disposition;
                ViceVector youthC = youth.channeling, adultC = adult.channeling, elderC = elder.channeling;

                // Flows expressed as a fraction of the receiving cohort (shares are fixed in model v0).
                double intoAdults = adult.share > 0 ? Math.Min(1.0, birth * youth.share / adult.share) : 0;
                double intoElders = elder.share > 0 ? Math.Min(1.0, retire * adult.share / elder.share) : 0;

                youth.disposition = ViceVector.Lerp(youthD, adultD, birth);
                youth.channeling = ViceVector.Lerp(youthC, ViceVector.Zero, birth);
                adult.disposition = ViceVector.Lerp(adultD, youthD, intoAdults);
                adult.channeling = ViceVector.Lerp(adultC, youthC, intoAdults);
                elder.disposition = ViceVector.Lerp(elderD, adultD, intoElders);
                elder.channeling = ViceVector.Lerp(elderC, adultC, intoElders);

                // Schooling profiles graduate the same way. Elders first, so each flow reads last month's
                // values. Newborns do not dilute it: it describes what the schooled were taught.
                for (int f = 0; f < elder.schooling.Length; f++)
                {
                    elder.schooling[f] += (adult.schooling[f] - elder.schooling[f]) * intoElders;
                    adult.schooling[f] += (youth.schooling[f] - adult.schooling[f]) * intoAdults;
                }

                for (int c = 0; c < society.cohorts.Length; c++)
                {
                    var cohort = society.cohorts[c];
                    cohort.disposition = ViceVector.Lerp(cohort.disposition, tuning.HumanNature, nature).Clamp01();
                }

                society.disposition = NationalDisposition(society);
            }
        }
    }
}
