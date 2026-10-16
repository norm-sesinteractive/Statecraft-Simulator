using System;
using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Nations;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// Time passing through people (D-0036). Adults have children (more with Lust and open land, fewer with Pride,
    /// crowded land and hunger);
    /// youth graduate into adults and adults retire into elders, carrying disposition and channeling with them;
    /// every generation dies at its own rate (less with Medicine, more in famine, elders more where Pride and
    /// Wrath neglect them). Newborns inherit their parents' culture and arrive untrained. Every culture also drifts
    /// slowly back toward human nature.
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

        /// <summary>Recomputes the total and each cohort's share after populations change.</summary>
        internal static void Recount(SocietyState society)
        {
            double total = 0;
            for (int c = 0; c < society.cohorts.Length; c++) total += society.cohorts[c].population;
            society.population = total;
            if (total <= 0) return;
            for (int c = 0; c < society.cohorts.Length; c++) society.cohorts[c].share = society.cohorts[c].population / total;
        }

        public void Tick(SimContext context)
        {
            var tuning = context.Tuning.Generations;
            double graduate = 1.0 / (tuning.YouthYears * SimDate.MonthsPerYear);
            double retire = 1.0 / (tuning.AdultYears * SimDate.MonthsPerYear);
            double nature = SimMath.AlphaFromTau(tuning.NatureTauMonths);
            var nations = context.World.Nations;

            for (int n = 0; n < nations.Count; n++)
            {
                var nation = nations[n];
                var society = nation.Society;
                var youth = society.cohorts[(int)Cohort.Youth];
                var adult = society.cohorts[(int)Cohort.Adult];
                var elder = society.cohorts[(int)Cohort.Elder];

                double births = adult.population * BirthRate(context, nation) / SimDate.MonthsPerYear;
                double graduates = youth.population * graduate;
                double retirees = adult.population * retire;
                double deathsTotal = 0, famineDeaths = 0;
                Span<double> deaths = stackalloc double[3];
                for (int c = 0; c < society.cohorts.Length; c++)
                {
                    double famineShare;
                    double monthly = Math.Min(0.9, DeathRate(context, nation, c, out famineShare) / SimDate.MonthsPerYear);
                    deaths[c] = society.cohorts[c].population * monthly;
                    deathsTotal += deaths[c];
                    famineDeaths += society.cohorts[c].population * Math.Min(monthly, famineShare);
                }

                double newYouth = Math.Max(0, youth.population + births - graduates - deaths[0]);
                double newAdults = Math.Max(0, adult.population + graduates - retirees - deaths[1]);
                double newElders = Math.Max(0, elder.population + retirees - deaths[2]);

                // Culture moves with the people: each flow is a fraction of the receiving cohort.
                double intoYouth = Fraction(births, newYouth);
                double intoAdults = Fraction(graduates, newAdults);
                double intoElders = Fraction(retirees, newElders);

                // Snapshot first so every flow reads last month's culture, whatever the order below.
                ViceVector youthD = youth.disposition, adultD = adult.disposition, elderD = elder.disposition;
                ViceVector youthC = youth.channeling, adultC = adult.channeling, elderC = elder.channeling;

                youth.disposition = ViceVector.Lerp(youthD, adultD, intoYouth);
                youth.channeling = ViceVector.Lerp(youthC, ViceVector.Zero, intoYouth);
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

                youth.population = newYouth;
                adult.population = newAdults;
                elder.population = newElders;
                Recount(society);
                society.disposition = NationalDisposition(society);

                var economy = nation.Economy;
                economy.birthsPerYear = births * SimDate.MonthsPerYear;
                economy.deathsPerYear = deathsTotal * SimDate.MonthsPerYear;
                economy.famineDeathsThisMonth = famineDeaths;
                EconomySystem.TrackFamine(context, nation);
            }
        }

        static double Fraction(double inflow, double receiving) => receiving > 0 ? SimMath.Clamp01(inflow / receiving) : 0;

        /// <summary>Births per adult per year, recorded in the ledger: Lust and open land raise it; Pride, crowding and hunger lower it.</summary>
        internal static double BirthRate(SimContext context, Nation nation)
        {
            var tuning = context.Tuning.Generations;
            var ledger = context.Ledger;
            var expressed = nation.Society.cohorts[(int)Cohort.Adult].expressed;
            var nature = tuning.HumanNature;
            double baseRate = tuning.BirthsPerAdult;
            int n = nation.Index;

            double rate =
                ledger.Add(n, Quantity.Births, LedgerNames.Base, baseRate) +
                ledger.Add(n, Quantity.Births, LedgerNames.Expressed[(int)Vice.Lust], baseRate * tuning.LustBirths * (expressed.Lust - nature.Lust)) +
                ledger.Add(n, Quantity.Births, LedgerNames.Expressed[(int)Vice.Pride], -baseRate * tuning.PrideBirths * (expressed.Pride - nature.Pride));
            double crowding = Math.Min(1, nation.Economy.landPressure) - tuning.CrowdingStart;
            if (crowding > 0)
                rate += ledger.Add(n, Quantity.Births, LedgerNames.Crowding, -baseRate * tuning.CrowdingBirths * crowding);
            double openLand = tuning.OpenLandBelow - nation.Economy.landPressure;
            if (openLand > 0)
                rate += ledger.Add(n, Quantity.Births, LedgerNames.OpenLand, baseRate * tuning.OpenLandBirths * openLand);
            double hunger = nation.Economy.hunger;
            if (hunger > 0)
                rate += ledger.Add(n, Quantity.Births, LedgerNames.Hunger, -baseRate * tuning.HungerBirths * hunger);
            return Math.Max(0, rate);
        }

        /// <summary>
        /// Deaths per person per year in one generation, recorded per cohort: the base rate, Medicine, elder neglect
        /// and famine. <paramref name="famineShare"/> is the famine part, per month.
        /// </summary>
        internal static double DeathRate(SimContext context, Nation nation, int cohort, out double famineShare)
        {
            var tuning = context.Tuning.Generations;
            var ledger = context.Ledger;
            var adult = nation.Society.cohorts[(int)Cohort.Adult];
            var nature = tuning.HumanNature;
            double even = 1.0 / Curriculum.FocusCount;
            double baseRate = tuning.Mortality[cohort];
            int n = nation.Index;

            double rate = ledger.Add(n, Quantity.Deaths, -1, cohort, LedgerNames.Base, baseRate);
            double medicine = Math.Max(tuning.MinMortalityScale - 1,
                -tuning.MedicineLife * (adult.schooling[(int)CurriculumFocus.Medicine] - even));
            rate += ledger.Add(n, Quantity.Deaths, -1, cohort, LedgerNames.Schooling[(int)CurriculumFocus.Medicine], baseRate * medicine);

            if (cohort == (int)Cohort.Elder)
            {
                double pride = Math.Max(0, adult.expressed.Pride - nature.Pride);
                double wrath = Math.Max(0, adult.expressed.Wrath - nature.Wrath);
                if (pride > 0)
                    rate += ledger.Add(n, Quantity.Deaths, -1, cohort, LedgerNames.ElderNeglect,
                        baseRate * tuning.ElderNeglect * pride * (1 + tuning.NeglectWrath * wrath));
            }

            double hunger = nation.Economy.hunger;
            famineShare = tuning.FamineMortality[cohort] * hunger * hunger;
            if (famineShare > 0)
                rate += ledger.Add(n, Quantity.Deaths, -1, cohort, LedgerNames.Famine, famineShare * SimDate.MonthsPerYear);
            return Math.Max(0, rate);
        }
    }
}
