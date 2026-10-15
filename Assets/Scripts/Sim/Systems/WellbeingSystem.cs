using System;
using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Nations;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// Where the river overflows or runs calm. Expressed vice meets its outlets: what fits becomes satisfaction,
    /// what does not becomes frustration. Nationally that yields fear, crime and aggression; per generation
    /// (D-0032) it yields contentment and unrest, whose population-weighted averages are the national values.
    /// </summary>
    internal sealed class WellbeingSystem : ISimSystem
    {
        public void Tick(SimContext context)
        {
            var nations = context.World.Nations;
            for (int n = 0; n < nations.Count; n++) Evaluate(context, nations[n], settle: false);
        }

        /// <summary>
        /// Computes this month's targets and moves the stocks toward them. When settling (game start) the
        /// stocks jump straight to their targets and no events fire.
        /// </summary>
        internal static void Evaluate(SimContext context, Nation nation, bool settle)
        {
            var tuning = context.Tuning.Wellbeing;
            var traits = context.Tuning.Politics.Types[(int)nation.Government.type];
            var ledger = context.Ledger;
            var society = nation.Society;
            int n = nation.Index;
            double severity = nation.Institutions.Law.Severity;

            // National expressed vice and frustration: what crime and aggression respond to.
            for (int v = 0; v < ViceVector.Count; v++)
            {
                double expressed = SimMath.Clamp01(
                    ledger.Add(n, Quantity.Expressed, v, LedgerNames.Disposition[v], society.disposition[v]) +
                    ledger.Add(n, Quantity.Expressed, v, LedgerNames.Mood[v], society.mood[v]));
                society.expressed[v] = expressed;
                double outlet = society.outlets[v];
                double frustration = 0;
                if (expressed > outlet)
                {
                    frustration =
                        ledger.Add(n, Quantity.Frustration, v, LedgerNames.Expressed[v], expressed) +
                        ledger.Add(n, Quantity.Frustration, v, LedgerNames.Outlet[v], -outlet);
                }
                society.frustration[v] = frustration;
            }

            society.fear = SimMath.Clamp01(
                ledger.Add(n, Quantity.Fear, LedgerNames.LawSeverity, tuning.FearPerSeverity * severity));

            // Crime: frustrated vice breaks the law; the courts deter it; a content people needs it less.
            double crime = ledger.Add(n, Quantity.Crime, LedgerNames.Base, tuning.CrimeBase);
            for (int v = 0; v < ViceVector.Count; v++)
            {
                if (society.frustration[v] > 0)
                    crime += ledger.Add(n, Quantity.Crime, LedgerNames.Frustration[v],
                        tuning.CrimeFromFrustration[v] * society.frustration[v]);
            }
            crime += ledger.Add(n, Quantity.Crime, LedgerNames.LawSeverity, -tuning.Deterrence * severity);
            if (society.contentment > 0)
                crime += ledger.Add(n, Quantity.Crime, LedgerNames.Contentment, -tuning.ContentmentCrimeRelief * society.contentment);
            society.crimeTarget = SimMath.Clamp01(crime);

            // Each generation's own contentment and unrest.
            double contentmentSum = 0, contentmentTargetSum = 0, unrestSum = 0, unrestTargetSum = 0;
            for (int c = 0; c < society.cohorts.Length; c++)
            {
                var cohort = society.cohorts[c];
                EvaluateCohort(context, nation, cohort, c, traits);
                if (settle)
                {
                    cohort.contentment = cohort.contentmentTarget;
                    cohort.unrest = cohort.unrestTarget;
                }
                else
                {
                    cohort.contentment = SimMath.Approach(cohort.contentment, cohort.contentmentTarget, tuning.ContentmentTauMonths);
                    cohort.unrest = SimMath.Approach(cohort.unrest, cohort.unrestTarget, tuning.UnrestTauMonths);
                }
                contentmentSum += cohort.share * cohort.contentment;
                contentmentTargetSum += cohort.share * cohort.contentmentTarget;
                unrestSum += cohort.share * cohort.unrest;
                unrestTargetSum += cohort.share * cohort.unrestTarget;
            }
            society.contentment = contentmentSum;
            society.contentmentTarget = contentmentTargetSum;
            society.unrest = unrestSum;
            society.unrestTarget = unrestTargetSum;

            // Aggression: the appetite for war the puppet government will one day act on.
            double aggression = 0;
            for (int v = 0; v < ViceVector.Count; v++)
            {
                double weight = tuning.AggressionWeights[v];
                if (weight != 0)
                    aggression += ledger.Add(n, Quantity.Aggression, LedgerNames.Expressed[v], weight * society.expressed[v]);
            }
            aggression += ledger.Add(n, Quantity.Aggression, LedgerNames.EnvyTimesWrath,
                tuning.EnvyWrathAggression * society.expressed.Envy * society.expressed.Wrath);
            if (traits.AggressionBonus != 0)
                aggression += ledger.Add(n, Quantity.Aggression, LedgerNames.Government, traits.AggressionBonus);
            society.aggression = SimMath.Clamp01(aggression);

            if (settle)
            {
                society.crime = society.crimeTarget;
                society.unrestAlarm = society.unrest >= tuning.UnrestAlarm;
                society.sufferingAlarm = society.contentment <= tuning.SufferingAlarm;
                society.crimeAlarm = society.crime >= tuning.CrimeAlarm;
                return;
            }

            society.crime = SimMath.Approach(society.crime, society.crimeTarget, tuning.CrimeTauMonths);

            Watch(context, n, ref society.unrestAlarm, society.unrest >= tuning.UnrestAlarm, society.unrest <= tuning.UnrestCalm,
                SimEventKind.UnrestRising, SimEventKind.UnrestEased, society.unrest);
            Watch(context, n, ref society.sufferingAlarm, society.contentment <= tuning.SufferingAlarm, society.contentment >= tuning.SufferingCalm,
                SimEventKind.SufferingSevere, SimEventKind.SufferingEased, society.contentment);
            Watch(context, n, ref society.crimeAlarm, society.crime >= tuning.CrimeAlarm, society.crime <= tuning.CrimeCalm,
                SimEventKind.CrimeWave, SimEventKind.CrimeEased, society.crime);
        }

        // One generation: its own expressed vices meet the nation's outlets (D-0032).
        static void EvaluateCohort(SimContext context, Nation nation, CohortState cohort, int c, Tuning.GovernmentTraits traits)
        {
            var tuning = context.Tuning.Wellbeing;
            var ledger = context.Ledger;
            var society = nation.Society;
            var government = nation.Government;
            int n = nation.Index;

            double satisfiedSum = 0, frustrationSum = 0;
            for (int v = 0; v < ViceVector.Count; v++)
            {
                double expressed = SimMath.Clamp01(
                    ledger.Add(n, Quantity.Expressed, v, c, LedgerNames.Disposition[v], cohort.disposition[v]) +
                    ledger.Add(n, Quantity.Expressed, v, c, LedgerNames.Mood[v], society.mood[v]));
                cohort.expressed[v] = expressed;
                double outlet = society.outlets[v];
                double frustration = 0;
                if (expressed > outlet)
                {
                    frustration =
                        ledger.Add(n, Quantity.Frustration, v, c, LedgerNames.Expressed[v], expressed) +
                        ledger.Add(n, Quantity.Frustration, v, c, LedgerNames.Outlet[v], -outlet);
                }
                cohort.frustration[v] = frustration;
                frustrationSum += frustration;
                satisfiedSum += Math.Min(expressed, outlet);
            }
            double meanSatisfied = satisfiedSum / ViceVector.Count;

            // Contentment: satisfaction lifts it; frustration (softened by Sloth's acceptance), fear and crime weigh on it.
            double contentment =
                ledger.Add(n, Quantity.Contentment, -1, c, LedgerNames.Base, tuning.ContentmentBase) +
                ledger.Add(n, Quantity.Contentment, -1, c, LedgerNames.Satisfaction, tuning.SatisfactionGain * meanSatisfied);
            for (int v = 0; v < ViceVector.Count; v++)
            {
                if (cohort.frustration[v] > 0)
                    contentment += ledger.Add(n, Quantity.Contentment, -1, c, LedgerNames.Frustration[v],
                        -tuning.FrustrationGain * cohort.frustration[v]);
            }
            if (frustrationSum > 0)
                contentment += ledger.Add(n, Quantity.Contentment, -1, c, LedgerNames.Expressed[(int)Vice.Sloth],
                    tuning.FrustrationGain * frustrationSum * tuning.SlothAcceptance * cohort.expressed.Sloth);
            contentment += ledger.Add(n, Quantity.Contentment, -1, c, LedgerNames.Fear,
                -tuning.FearSuffering * traits.FearSufferingScale * society.fear);
            contentment += ledger.Add(n, Quantity.Contentment, -1, c, LedgerNames.Crime, -tuning.CrimeSuffering * society.crime);
            cohort.contentmentTarget = SimMath.Clamp(contentment, -1, 1);

            // Unrest: suffering, amplified by Pride and Envy, plus Wrath frustration and suspicion of the rulers;
            // Sloth, fear and the government's own vents hold it down; a fomented revolution stokes it.
            double unrest = 0;
            double suffering = Math.Max(0, -cohort.contentment);
            if (suffering > 0)
            {
                double fromSuffering = tuning.UnrestFromSuffering * suffering;
                unrest += ledger.Add(n, Quantity.Unrest, -1, c, LedgerNames.Suffering, fromSuffering);
                unrest += ledger.Add(n, Quantity.Unrest, -1, c, LedgerNames.Expressed[(int)Vice.Pride],
                    fromSuffering * tuning.PrideAmplifier * cohort.expressed.Pride);
                unrest += ledger.Add(n, Quantity.Unrest, -1, c, LedgerNames.Expressed[(int)Vice.Envy],
                    fromSuffering * tuning.EnvyAmplifier * cohort.expressed.Envy);
            }
            if (cohort.frustration.Wrath > 0)
                unrest += ledger.Add(n, Quantity.Unrest, -1, c, LedgerNames.Frustration[(int)Vice.Wrath],
                    tuning.UnrestFromWrath * cohort.frustration.Wrath);
            if (government.suspicion > 0)
                unrest += ledger.Add(n, Quantity.Unrest, -1, c, LedgerNames.GovernmentSuspicion,
                    context.Tuning.Politics.UnrestFromSuspicion * government.suspicion);
            if (government.shadowSuspicion > 0)
                unrest += ledger.Add(n, Quantity.Unrest, -1, c, LedgerNames.ShadowSuspicion,
                    context.Tuning.Politics.UnrestFromShadowSuspicion * government.shadowSuspicion);
            if (government.pending == PendingChange.Revolution)
                unrest += ledger.Add(n, Quantity.Unrest, -1, c, LedgerNames.Revolution, context.Tuning.Politics.FomentUnrest);
            unrest += ledger.Add(n, Quantity.Unrest, -1, c, LedgerNames.Expressed[(int)Vice.Sloth], -tuning.SlothDampening * cohort.expressed.Sloth);
            unrest += ledger.Add(n, Quantity.Unrest, -1, c, LedgerNames.Fear, -tuning.FearDampening * traits.FearDampeningScale * society.fear);
            if (traits.UnrestVenting != 0)
                unrest += ledger.Add(n, Quantity.Unrest, -1, c, LedgerNames.Government, -traits.UnrestVenting);
            cohort.unrestTarget = SimMath.Clamp01(unrest);
        }

        // Hysteresis: an alarm fires once on crossing, and clears only after falling back past the calm line.
        internal static void Watch(SimContext context, int nation, ref bool alarm, bool pastAlarm, bool backToCalm,
            SimEventKind rising, SimEventKind eased, double value)
        {
            if (!alarm && pastAlarm)
            {
                alarm = true;
                context.Events.Raise(new SimEvent(context.Date, nation, rising, value));
            }
            else if (alarm && backToCalm)
            {
                alarm = false;
                context.Events.Raise(new SimEvent(context.Date, nation, eased, value));
            }
        }
    }
}
