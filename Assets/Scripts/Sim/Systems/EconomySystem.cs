using System;
using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Economy;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Nations;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// Land and labor (D-0036). The land sets a ceiling on each resource; skill (adult schooling), drive (Greed) and
    /// science raise it; the workforce, dragged by Sloth, unrest and hunger, approaches it with diminishing returns.
    /// Labor goes to needs first: food (and refilling the stores), then materials, and everything left makes
    /// luxuries, which also always take a small reserved share. The people eat, the stores buffer the harvest, and what is produced beyond need becomes wealth.
    /// </summary>
    internal sealed class EconomySystem : ISimSystem
    {
        public void Tick(SimContext context)
        {
            var nations = context.World.Nations;
            for (int n = 0; n < nations.Count; n++) Evaluate(context, nations[n], settle: false);
        }

        /// <summary>
        /// One month of work and eating. When settling (game start) the stores start full, comfort at expectation,
        /// wealth at its equilibrium, the harvest average and nothing random is drawn.
        /// </summary>
        internal static void Evaluate(SimContext context, Nation nation, bool settle)
        {
            var tuning = context.Tuning.Economy;
            var ledger = context.Ledger;
            var society = nation.Society;
            var economy = nation.Economy;
            var adult = society.cohorts[(int)Cohort.Adult];
            var elder = society.cohorts[(int)Cohort.Elder];
            var nature = context.Tuning.Generations.HumanNature;
            double even = 1.0 / Curriculum.FocusCount;
            int n = nation.Index;
            double people = society.population;

            if (!settle && context.Date.Month == 1 && tuning.HarvestNoise > 0)
                economy.harvest = SimMath.Clamp(1 + tuning.HarvestNoise * context.Random.NextGaussian(), tuning.HarvestFloor, tuning.HarvestCeiling);
            if (settle) economy.harvest = 1;

            // Needs.
            double foodPerHead =
                ledger.Add(n, Quantity.FoodNeed, LedgerNames.Base, tuning.FoodNeed) +
                ledger.Add(n, Quantity.FoodNeed, LedgerNames.Expressed[(int)Vice.Gluttony],
                    tuning.GluttonyAppetite * (society.expressed.Gluttony - nature.Gluttony));
            economy.need[(int)Resource.Food] = people * Math.Max(0.1, foodPerHead);
            economy.need[(int)Resource.Materials] = people * tuning.MaterialsNeed;
            economy.need[(int)Resource.Luxuries] = 0;

            // Labor.
            double productivity =
                ledger.Add(n, Quantity.Productivity, LedgerNames.Base, 1) +
                ledger.Add(n, Quantity.Productivity, LedgerNames.Expressed[(int)Vice.Sloth],
                    -tuning.SlothIdleness * (society.expressed.Sloth - nature.Sloth));
            if (society.unrest > 0)
                productivity += ledger.Add(n, Quantity.Productivity, LedgerNames.Unrest, -tuning.UnrestIdleness * society.unrest);
            if (economy.hunger > 0)
                productivity += ledger.Add(n, Quantity.Productivity, LedgerNames.Hunger, -tuning.HungerWeakness * economy.hunger);
            economy.productivity = Math.Max(tuning.MinProductivity, productivity);
            economy.workforce = adult.population + tuning.ElderLabor * elder.population;
            double labor = economy.workforce * economy.productivity;

            // What the land could yield, and how much labor each resource needs.
            Span<double> land = stackalloc double[EconomyState.ResourceCount];
            for (int r = 0; r < EconomyState.ResourceCount; r++)
            {
                double raw = 0;
                for (int t = 0; t < EconomyState.TerrainCount; t++) raw += economy.area[t] * tuning.TerrainYields[t][r];
                land[r] = raw;
                economy.potential[r] = raw * YieldMultiplier(context, nation, (Resource)r, even, nature);
            }

            economy.landPressure = economy.potential[0] > 0 ? economy.need[0] / economy.potential[0] : double.PositiveInfinity;

            double store = economy.foodStore;
            double storeCap = tuning.StoreMonths * economy.need[(int)Resource.Food];
            if (settle) store = storeCap;

            // Farmers see the year's weather and work more fields in a poor one.
            double foodTarget = (economy.need[(int)Resource.Food] * (1 + tuning.PlanningMargin) +
                Math.Max(0, storeCap - store) / Math.Max(1, tuning.StoreRefillMonths)) / Math.Max(0.1, economy.harvest);
            double materialsTarget = economy.need[(int)Resource.Materials] * (1 + tuning.PlanningMargin);

            double reserved = labor * tuning.LuxuryLaborShare;
            double remaining = labor - reserved;
            double onFood = Math.Min(remaining, LaborFor(foodTarget, economy.potential[0], land[0], tuning.WorkerOutput[0]));
            remaining -= onFood;
            double onMaterials = Math.Min(remaining, LaborFor(materialsTarget, economy.potential[1], land[1], tuning.WorkerOutput[1]));
            remaining -= onMaterials;
            double onLuxuries = remaining + reserved;

            economy.workers[0] = onFood;
            economy.workers[1] = onMaterials;
            economy.workers[2] = onLuxuries;
            for (int r = 0; r < EconomyState.ResourceCount; r++)
                economy.output[r] = Produce(economy.workers[r], economy.potential[r], land[r], tuning.WorkerOutput[r]);
            economy.output[(int)Resource.Food] *= economy.harvest;

            // Eating, and the stores.
            double foodNeed = economy.need[(int)Resource.Food];
            double available = store + economy.output[(int)Resource.Food];
            double eaten = Math.Min(foodNeed, available);
            store = available - eaten;
            double foodSurplus = Math.Max(0, store - storeCap);
            economy.foodStore = store - foodSurplus;
            economy.hunger = foodNeed > 0 ? SimMath.Clamp01(1 - eaten / foodNeed) : 0;

            double materials = economy.output[(int)Resource.Materials];
            double materialsNeed = economy.need[(int)Resource.Materials];
            economy.materialsShortage = materialsNeed > 0 ? SimMath.Clamp01(1 - materials / materialsNeed) : 0;
            double materialsSurplus = Math.Max(0, materials - materialsNeed);

            double luxuries = economy.output[(int)Resource.Luxuries];
            economy.luxuriesPerHead = people > 0 ? luxuries / people : 0;
            if (settle || economy.luxuryExpectation <= 0) economy.luxuryExpectation = economy.luxuriesPerHead;
            else economy.luxuryExpectation = SimMath.Approach(economy.luxuryExpectation, economy.luxuriesPerHead, tuning.ExpectationTauMonths);

            // Wealth: what is produced beyond need, valued and traded at home; Commerce makes more of it.
            double income =
                ledger.Add(n, Quantity.Wealth, LedgerNames.Surplus[0], tuning.Prices[0] * foodSurplus) +
                ledger.Add(n, Quantity.Wealth, LedgerNames.Surplus[1], tuning.Prices[1] * materialsSurplus) +
                ledger.Add(n, Quantity.Wealth, LedgerNames.Surplus[2], tuning.Prices[2] * luxuries);
            income += ledger.Add(n, Quantity.Wealth, LedgerNames.Schooling[(int)CurriculumFocus.Commerce],
                income * tuning.CommerceSkill * (adult.schooling[(int)CurriculumFocus.Commerce] - even));
            economy.wealthIncome = Math.Max(0, income);
            double upkeepAlpha = SimMath.AlphaFromTau(tuning.WealthTauMonths);
            if (settle)
            {
                economy.wealth = upkeepAlpha > 0 ? economy.wealthIncome / upkeepAlpha : 0;
                return;
            }
            double upkeep = ledger.Add(n, Quantity.Wealth, LedgerNames.Upkeep, -economy.wealth * upkeepAlpha);
            economy.wealth = Math.Max(0, economy.wealth + economy.wealthIncome + upkeep);
        }

        // Skill, drive and science multiply what the land and each worker can yield.
        static double YieldMultiplier(SimContext context, Nation nation, Resource resource, double even, ViceVector nature)
        {
            var tuning = context.Tuning.Economy;
            var ledger = context.Ledger;
            var adult = nation.Society.cohorts[(int)Cohort.Adult];
            int n = nation.Index;
            int r = (int)resource;

            double y = ledger.Add(n, Quantity.Yield, r, LedgerNames.Base, 1);
            switch (resource)
            {
                case Resource.Food:
                    y += Skill(ledger, n, r, adult, CurriculumFocus.Agriculture, tuning.AgricultureSkill, even);
                    break;
                case Resource.Materials:
                    y += Skill(ledger, n, r, adult, CurriculumFocus.Infrastructure, tuning.InfrastructureSkill, even);
                    break;
                case Resource.Luxuries:
                    y += Skill(ledger, n, r, adult, CurriculumFocus.Arts, tuning.ArtsSkill, even);
                    break;
            }
            y += Skill(ledger, n, r, adult, CurriculumFocus.Science, tuning.ScienceSkill, even);
            if (resource != Resource.Food)
                y += ledger.Add(n, Quantity.Yield, r, LedgerNames.Expressed[(int)Vice.Greed],
                    tuning.GreedDrive * (nation.Society.expressed.Greed - nature.Greed));
            return Math.Max(0.1, y);
        }

        static double Skill(Ledger ledger, int n, int r, CohortState adult, CurriculumFocus focus, double gain, double even) =>
            ledger.Add(n, Quantity.Yield, r, LedgerNames.Schooling[(int)focus], gain * (adult.schooling[(int)focus] - even));

        // Output from labor on land: potential x (1 - exp(-labor x perWorker / land)). Near zero labor each worker
        // makes about perWorker x multiplier; crowded land approaches the potential and no further.
        static double Produce(double labor, double potential, double land, double perWorker)
        {
            if (labor <= 0 || land <= 0 || potential <= 0) return 0;
            return potential * (1 - Math.Exp(-labor * perWorker / land));
        }

        // Labor needed to reach a target, or infinity when the land cannot yield it.
        static double LaborFor(double target, double potential, double land, double perWorker)
        {
            if (target <= 0) return 0;
            if (land <= 0 || potential <= target || perWorker <= 0) return double.PositiveInfinity;
            return -land / perWorker * Math.Log(1 - target / potential);
        }

        /// <summary>
        /// Famine bookkeeping after this month's deaths (called by the generation system): famines begin and end
        /// with hunger, count their dead, and a famine that kills too many loses the nation (D-0019).
        /// </summary>
        internal static void TrackFamine(SimContext context, Nation nation)
        {
            var tuning = context.Tuning.Economy;
            var economy = nation.Economy;
            double deaths = economy.famineDeathsThisMonth;

            if (!economy.famine)
            {
                if (economy.hunger < tuning.FamineHunger) return;
                economy.famine = true;
                economy.famineStartPopulation = nation.Society.population + deaths;
                economy.famineDeaths = deaths;
                context.Events.Raise(new SimEvent(context.Date, nation.Index, SimEventKind.FamineBegan, economy.hunger));
            }
            else
            {
                economy.famineDeaths += deaths;
            }

            if (!economy.starved && economy.famineDeaths >= tuning.StarvationCollapse * economy.famineStartPopulation)
            {
                economy.starved = true;
                nation.PlayerControlled = false;
                context.Events.Raise(new SimEvent(context.Date, nation.Index, SimEventKind.NationStarved, economy.FamineToll));
            }

            if (economy.hunger <= tuning.FamineEnds)
            {
                double toll = economy.FamineToll;
                economy.famine = false;
                economy.famineDeaths = 0;
                context.Events.Raise(new SimEvent(context.Date, nation.Index, SimEventKind.FamineEnded, toll));
            }
        }
    }
}
