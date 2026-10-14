using System;
using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Nations;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// Where the river overflows or runs calm. Expressed vice meets its outlets: what fits becomes
    /// satisfaction, what does not becomes frustration. From these, plus the courts' fear and the
    /// crime it fails to deter, come contentment, crime, unrest and aggression.
    /// </summary>
    internal sealed class WellbeingSystem : ISimSystem
    {
        public void Tick(SimContext context)
        {
            var nations = context.World.Nations;
            for (int n = 0; n < nations.Count; n++) Evaluate(context, nations[n], settle: false);
        }

        /// <summary>
        /// Computes this month's targets and moves the stocks toward them. When settling (game
        /// start) the stocks jump straight to their targets and no events fire.
        /// </summary>
        internal static void Evaluate(SimContext context, Nation nation, bool settle)
        {
            var tuning = context.Tuning.Wellbeing;
            var ledger = context.Ledger;
            var society = nation.Society;
            int n = nation.Index;
            double severity = nation.Institutions.Law.Severity;

            // Expressed vice is deep disposition plus current mood; frustration is expressed beyond outlet.
            double satisfiedSum = 0;
            double frustrationSum = 0;
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
                frustrationSum += frustration;
                satisfiedSum += Math.Min(expressed, outlet);
            }
            double meanSatisfied = satisfiedSum / ViceVector.Count;
            double meanFrustration = frustrationSum / ViceVector.Count;

            society.fear = SimMath.Clamp01(
                ledger.Add(n, Quantity.Fear, LedgerNames.LawSeverity, tuning.FearPerSeverity * severity));

            // Contentment: satisfaction lifts it; frustration (softened by Sloth's acceptance), fear and crime weigh on it.
            double contentment =
                ledger.Add(n, Quantity.Contentment, LedgerNames.Base, tuning.ContentmentBase) +
                ledger.Add(n, Quantity.Contentment, LedgerNames.Satisfaction, tuning.SatisfactionGain * meanSatisfied);
            for (int v = 0; v < ViceVector.Count; v++)
            {
                if (society.frustration[v] > 0)
                    contentment += ledger.Add(n, Quantity.Contentment, LedgerNames.Frustration[v],
                        -tuning.FrustrationGain * society.frustration[v] / ViceVector.Count);
            }
            if (meanFrustration > 0)
                contentment += ledger.Add(n, Quantity.Contentment, LedgerNames.Expressed[(int)Vice.Sloth],
                    tuning.FrustrationGain * meanFrustration * tuning.SlothAcceptance * society.expressed.Sloth);
            contentment += ledger.Add(n, Quantity.Contentment, LedgerNames.Fear, -tuning.FearSuffering * society.fear);
            contentment += ledger.Add(n, Quantity.Contentment, LedgerNames.Crime, -tuning.CrimeSuffering * society.crime);
            society.contentmentTarget = SimMath.Clamp(contentment, -1, 1);

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

            // Unrest: suffering, amplified by Pride and Envy, plus raw Wrath frustration; Sloth and fear hold it down.
            double unrest = 0;
            double suffering = Math.Max(0, -society.contentment);
            if (suffering > 0)
            {
                double fromSuffering = tuning.UnrestFromSuffering * suffering;
                unrest += ledger.Add(n, Quantity.Unrest, LedgerNames.Suffering, fromSuffering);
                unrest += ledger.Add(n, Quantity.Unrest, LedgerNames.Expressed[(int)Vice.Pride],
                    fromSuffering * tuning.PrideAmplifier * society.expressed.Pride);
                unrest += ledger.Add(n, Quantity.Unrest, LedgerNames.Expressed[(int)Vice.Envy],
                    fromSuffering * tuning.EnvyAmplifier * society.expressed.Envy);
            }
            if (society.frustration.Wrath > 0)
                unrest += ledger.Add(n, Quantity.Unrest, LedgerNames.Frustration[(int)Vice.Wrath],
                    tuning.UnrestFromWrath * society.frustration.Wrath);
            unrest += ledger.Add(n, Quantity.Unrest, LedgerNames.Expressed[(int)Vice.Sloth], -tuning.SlothDampening * society.expressed.Sloth);
            unrest += ledger.Add(n, Quantity.Unrest, LedgerNames.Fear, -tuning.FearDampening * society.fear);
            society.unrestTarget = SimMath.Clamp01(unrest);

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
            society.aggression = SimMath.Clamp01(aggression);

            if (settle)
            {
                society.contentment = society.contentmentTarget;
                society.crime = society.crimeTarget;
                society.unrest = society.unrestTarget;
                society.unrestAlarm = society.unrest >= tuning.UnrestAlarm;
                society.sufferingAlarm = society.contentment <= tuning.SufferingAlarm;
                society.crimeAlarm = society.crime >= tuning.CrimeAlarm;
                return;
            }

            society.contentment = SimMath.Approach(society.contentment, society.contentmentTarget, tuning.ContentmentTauMonths);
            society.crime = SimMath.Approach(society.crime, society.crimeTarget, tuning.CrimeTauMonths);
            society.unrest = SimMath.Approach(society.unrest, society.unrestTarget, tuning.UnrestTauMonths);

            Watch(context, n, ref society.unrestAlarm, society.unrest >= tuning.UnrestAlarm, society.unrest <= tuning.UnrestCalm,
                SimEventKind.UnrestRising, SimEventKind.UnrestEased, society.unrest);
            Watch(context, n, ref society.sufferingAlarm, society.contentment <= tuning.SufferingAlarm, society.contentment >= tuning.SufferingCalm,
                SimEventKind.SufferingSevere, SimEventKind.SufferingEased, society.contentment);
            Watch(context, n, ref society.crimeAlarm, society.crime >= tuning.CrimeAlarm, society.crime <= tuning.CrimeCalm,
                SimEventKind.CrimeWave, SimEventKind.CrimeEased, society.crime);
        }

        // Hysteresis: an alarm fires once on crossing, and clears only after falling back past the calm line.
        static void Watch(SimContext context, int nation, ref bool alarm, bool pastAlarm, bool backToCalm,
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
