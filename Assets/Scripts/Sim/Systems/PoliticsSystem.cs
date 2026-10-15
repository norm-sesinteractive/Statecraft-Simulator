using System;
using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Nations;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// The shield and what threatens it (D-0020, D-0021, D-0024, D-0032): how the people align with each
    /// government type, government and shadow suspicion, fomented revolutions, votes, generational revolts,
    /// and the hidden hand's overthrow when shadow suspicion breaks (D-0019).
    /// </summary>
    internal sealed class PoliticsSystem : ISimSystem
    {
        public void Tick(SimContext context)
        {
            var nations = context.World.Nations;
            for (int n = 0; n < nations.Count; n++) Step(context, nations[n]);
        }

        static void Step(SimContext context, Nation nation)
        {
            var tuning = context.Tuning.Politics;
            var government = nation.Government;
            government.monthsInPower++;
            if (government.cooldown > 0) government.cooldown--;
            government.recentChanges -= government.recentChanges * SimMath.AlphaFromTau(tuning.RecentChangeTauMonths);
            government.leverShock -= government.leverShock * SimMath.AlphaFromTau(tuning.LeverShockTauMonths);
            var society = nation.Society;
            society.tradition = ViceVector.Lerp(society.tradition, society.disposition, SimMath.AlphaFromTau(tuning.TraditionTauMonths));

            Evaluate(context, nation, settle: false);
            Announce(context, nation);
            if (ResolvePending(context, nation)) return;
            if (MaybeCallPeoplesVote(context, nation)) return;
            MaybeRevolt(context, nation);
        }

        /// <summary>Alignment, the suspicion target and stocks, shadow drift and alarms. Settling jumps to targets.</summary>
        internal static void Evaluate(SimContext context, Nation nation, bool settle)
        {
            var tuning = context.Tuning.Politics;
            var government = nation.Government;
            var society = nation.Society;
            var ledger = context.Ledger;
            var traits = tuning.Types[(int)government.type];
            int n = nation.Index;

            ComputeAlignment(context, nation);

            // Government suspicion target: an out-of-balance society starts asking questions (D-0020).
            var adult = society.cohorts[(int)Cohort.Adult];
            double s = ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.Schooling[(int)CurriculumFocus.Science],
                tuning.ScienceSuspicion * adult.schooling[(int)CurriculumFocus.Science] * traits.ScienceSuspicionScale);
            s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.Expressed[(int)Vice.Pride], tuning.PrideSuspicion * society.expressed.Pride);
            if (society.frustration.Envy > 0)
                s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.Frustration[(int)Vice.Envy], tuning.BlockedAmbitionSuspicion * society.frustration.Envy);
            if (society.frustration.Greed > 0)
                s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.Frustration[(int)Vice.Greed], tuning.BlockedAmbitionSuspicion * society.frustration.Greed);

            // Vices "out of whack": culture pushed away from the nation's own tradition, not away from other nations.
            double culturalDrift = 0;
            for (int v = 0; v < ViceVector.Count; v++)
                culturalDrift += Math.Max(0, Math.Abs(society.disposition[v] - society.tradition[v]) - tuning.DriftTolerance);
            if (culturalDrift > 0)
                s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.CulturalDrift, tuning.DriftSuspicion * culturalDrift);

            double maxFocus = 0;
            var curriculum = nation.Institutions.Curriculum;
            for (int f = 0; f < Curriculum.FocusCount; f++) maxFocus = Math.Max(maxFocus, curriculum.Share((CurriculumFocus)f));
            if (maxFocus > tuning.CurriculumConcentrationStart)
                s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.CurriculumConcentration,
                    tuning.ConcentrationSuspicion * (maxFocus - tuning.CurriculumConcentrationStart));

            double maxAirtime = 0;
            var media = nation.Institutions.Media;
            for (int v = 0; v < ViceVector.Count; v++) maxAirtime = Math.Max(maxAirtime, media.Share((Vice)v));
            if (maxAirtime > tuning.AirtimeConcentrationStart)
                s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.AirtimeConcentration,
                    tuning.ConcentrationSuspicion * (maxAirtime - tuning.AirtimeConcentrationStart));

            if (government.leverShock > 0)
                s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.LeverShocks,
                    tuning.LeverShockSuspicion * government.leverShock * traits.LeverSwingSuspicionScale);
            if (society.contentment < 0)
                s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.Suffering, tuning.SufferingSuspicion * -society.contentment);
            if (traits.SeveritySuspicion != 0)
                s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.LawSeverity,
                    traits.SeveritySuspicion * nation.Institutions.Law.Severity);
            s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.Expressed[(int)Vice.Sloth], -tuning.SlothDamping * society.expressed.Sloth);
            s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.SatisfiedGluttony,
                -tuning.GluttonyDamping * Math.Min(society.expressed.Gluttony, society.outlets.Gluttony));
            s += ledger.Add(n, Quantity.GovernmentSuspicion, LedgerNames.MediaNeutral, -tuning.NeutralAirtimeDamping * media.NeutralShare);
            government.suspicionTarget = SimMath.Clamp01(s);

            if (settle)
            {
                government.suspicion = government.suspicionTarget;
                government.suspicionAlarm = government.suspicion >= tuning.SuspicionAlarm;
                government.shadowAlarm = government.shadowSuspicion >= tuning.ShadowAlarm;
                return;
            }

            government.suspicion = SimMath.Approach(government.suspicion, government.suspicionTarget, tuning.SuspicionTauMonths);

            // Government suspicion that stays high leaks into suspicion of the hidden hand; people remember for decades.
            double drift = 0;
            double leak = government.suspicion - tuning.LeakThreshold;
            if (leak > 0) drift += ledger.Add(n, Quantity.ShadowSuspicion, LedgerNames.GovernmentSuspicion, tuning.LeakRate * leak);
            if (government.shadowSuspicion > 0)
                drift += ledger.Add(n, Quantity.ShadowSuspicion, LedgerNames.Decay,
                    -government.shadowSuspicion * SimMath.AlphaFromTau(tuning.ShadowDecayTauMonths));
            government.shadowDrift = drift;
            government.shadowSuspicion = SimMath.Clamp01(government.shadowSuspicion + drift);

            WellbeingSystem.Watch(context, n, ref government.suspicionAlarm, government.suspicion >= tuning.SuspicionAlarm,
                government.suspicion <= tuning.SuspicionCalm, SimEventKind.SuspicionRising, SimEventKind.SuspicionEased, government.suspicion);
            WellbeingSystem.Watch(context, n, ref government.shadowAlarm, government.shadowSuspicion >= tuning.ShadowAlarm,
                government.shadowSuspicion <= tuning.ShadowCalm, SimEventKind.ShadowRumors, SimEventKind.ShadowRumorsFaded, government.shadowSuspicion);
        }

        // National alignment with every type, recorded in the ledger: base appeal, expressed vices and schooling
        // away from the norm, and habit for the type in power.
        static void ComputeAlignment(SimContext context, Nation nation)
        {
            var tuning = context.Tuning.Politics;
            var government = nation.Government;
            var society = nation.Society;
            var ledger = context.Ledger;
            var nature = context.Tuning.Generations.HumanNature;
            int n = nation.Index;
            double even = 1.0 / Curriculum.FocusCount;

            for (int k = 0; k < GovernmentState.TypeCount; k++)
            {
                var traits = tuning.Types[k];
                double a = ledger.Add(n, Quantity.Alignment, k, LedgerNames.Base, traits.AlignmentBase);
                for (int v = 0; v < ViceVector.Count; v++)
                {
                    double w = traits.AlignmentFromVices[v];
                    if (w != 0) a += ledger.Add(n, Quantity.Alignment, k, LedgerNames.Expressed[v], w * (society.expressed[v] - nature[v]));
                }
                int focuses = Math.Min(Curriculum.FocusCount, traits.AlignmentFromSchooling.Length);
                for (int f = 0; f < focuses; f++)
                {
                    double w = traits.AlignmentFromSchooling[f];
                    if (w == 0) continue;
                    double schooling = 0;
                    for (int c = 0; c < society.cohorts.Length; c++) schooling += society.cohorts[c].share * society.cohorts[c].schooling[f];
                    a += ledger.Add(n, Quantity.Alignment, k, LedgerNames.Schooling[f], w * (schooling - even));
                }
                if (k == (int)government.type) a += ledger.Add(n, Quantity.Alignment, k, LedgerNames.Inertia, tuning.InertiaBonus);
                government.alignment[k] = a;
            }
        }

        /// <summary>One generation's alignment with a type (no inertia), used when a revolt chooses the new government.</summary>
        internal static double CohortAlignment(SimContext context, CohortState cohort, int type)
        {
            var traits = context.Tuning.Politics.Types[type];
            var nature = context.Tuning.Generations.HumanNature;
            double even = 1.0 / Curriculum.FocusCount;
            double a = traits.AlignmentBase;
            for (int v = 0; v < ViceVector.Count; v++) a += traits.AlignmentFromVices[v] * (cohort.expressed[v] - nature[v]);
            int focuses = Math.Min(Curriculum.FocusCount, traits.AlignmentFromSchooling.Length);
            for (int f = 0; f < focuses; f++) a += traits.AlignmentFromSchooling[f] * (cohort.schooling[f] - even);
            return a;
        }

        static void Announce(SimContext context, Nation nation)
        {
            var government = nation.Government;
            if (government.pending == PendingChange.None || government.pendingAnnounced) return;
            government.pendingAnnounced = true;
            var kind = government.pending == PendingChange.Revolution ? SimEventKind.RevolutionStarted : SimEventKind.VoteCalled;
            context.Events.Raise(new SimEvent(context.Date, nation.Index, kind, government.pendingByPlayer ? 1 : 0, (int)government.pendingTarget));
        }

        static bool ResolvePending(SimContext context, Nation nation)
        {
            var tuning = context.Tuning.Politics;
            var government = nation.Government;
            if (government.pending == PendingChange.None) return false;
            if (--government.pendingMonths > 0) return false;

            var kind = government.pending;
            var target = government.pendingTarget;
            government.pending = PendingChange.None;
            government.cooldown = tuning.ChangeCooldownMonths;
            double lead = government.alignment[(int)target] - government.alignment[(int)government.type];

            if (kind == PendingChange.Revolution)
            {
                double chance = SimMath.Clamp(tuning.FomentBaseSuccess + tuning.FomentUnrestSuccess * nation.Society.unrest +
                    tuning.FomentAlignmentSuccess * lead - tuning.FomentFearPenalty * nation.Society.fear, tuning.ChanceFloor, tuning.ChanceCeiling);
                ApplyViolence(context, nation, tuning.FomentViolence);
                if (context.Random.NextDouble() < chance)
                {
                    ChangeGovernment(context, nation, target, tuning.RevolutionVent);
                    Raise(context, nation, SimEventKind.RevolutionSucceeded, chance, (int)target);
                }
                else
                {
                    government.suspicion = SimMath.Clamp01(government.suspicion + tuning.FailedPlotSuspicion);
                    government.shadowSuspicion = SimMath.Clamp01(government.shadowSuspicion + tuning.FailedPlotShadow);
                    Raise(context, nation, SimEventKind.RevolutionFailed, chance, (int)target);
                }
            }
            else
            {
                double chance = SimMath.Clamp(0.5 + tuning.VoteSensitivity * lead, tuning.ChanceFloor, tuning.ChanceCeiling);
                if (context.Random.NextDouble() < chance)
                {
                    ChangeGovernment(context, nation, target, tuning.VoteVent);
                    Raise(context, nation, SimEventKind.VotePassed, chance, (int)target);
                }
                else
                {
                    Raise(context, nation, SimEventKind.VoteFailed, chance, (int)target);
                }
            }
            return true;
        }

        // The people of a democracy call a vote on their own when they are restless and another type appeals more.
        static bool MaybeCallPeoplesVote(SimContext context, Nation nation)
        {
            var tuning = context.Tuning.Politics;
            var government = nation.Government;
            if (!tuning.Types[(int)government.type].CanVote || government.pending != PendingChange.None || government.cooldown > 0) return false;
            if (nation.Society.unrest < tuning.PeopleVoteUnrest) return false;

            int best = -1;
            for (int k = 0; k < GovernmentState.TypeCount; k++)
            {
                if (k == (int)government.type) continue;
                if (best < 0 || government.alignment[k] > government.alignment[best]) best = k;
            }
            if (government.alignment[best] - government.alignment[(int)government.type] < tuning.PeopleVoteMargin) return false;
            if (context.Random.NextDouble() >= tuning.PeopleVoteChance) return false;

            Start(government, PendingChange.Vote, (GovernmentType)best, tuning.VoteMonths, byPlayer: false);
            Announce(context, nation);
            return true;
        }

        // Any generation past the threshold may revolt (D-0032); at most one revolt per nation per month.
        static void MaybeRevolt(SimContext context, Nation nation)
        {
            var tuning = context.Tuning.Politics;
            var government = nation.Government;
            if (government.pending != PendingChange.None || government.cooldown > 0) return;
            var traits = tuning.Types[(int)government.type];
            var society = nation.Society;

            for (int c = 0; c < society.cohorts.Length; c++)
            {
                var cohort = society.cohorts[c];
                double excess = cohort.unrest - tuning.RevoltThreshold;
                if (excess < 0) continue;
                double chance = tuning.RevoltChancePerUnrest * excess * tuning.RevoltFrequency[c] * traits.RevoltChanceScale;
                if (context.Random.NextDouble() >= chance) continue;

                double success = SimMath.Clamp(tuning.RevoltSuccess[c] + tuning.RevoltUnrestSuccess * excess -
                    tuning.RevoltFearPenalty * society.fear, tuning.ChanceFloor, tuning.ChanceCeiling);
                ApplyViolence(context, nation, tuning.RevoltViolence[c]);
                government.cooldown = tuning.ChangeCooldownMonths;

                if (context.Random.NextDouble() < success)
                {
                    cohort.unrest *= 1 - tuning.RevoltUnrestRelief;
                    bool againstTheHand = nation.PlayerControlled && !government.hiddenHandOverthrown &&
                        government.shadowSuspicion >= tuning.ShadowBreakingPoint;
                    var newType = ChooseByAlignment(context, nation, c);
                    ChangeGovernment(context, nation, newType, tuning.RevolutionVent);
                    Raise(context, nation, SimEventKind.RevoltSucceeded, (int)newType, c);
                    if (againstTheHand)
                    {
                        government.hiddenHandOverthrown = true;
                        nation.PlayerControlled = false;
                        Raise(context, nation, SimEventKind.HiddenHandOverthrown, government.shadowSuspicion, c);
                        // The hand is exposed and gone: what the people suspected has been found and removed.
                        government.shadowSuspicion = 0;
                        government.suspicion *= 1 - tuning.RevolutionVent;
                    }
                }
                else
                {
                    cohort.contentment = SimMath.Clamp(cohort.contentment - tuning.CrushedContentmentShock, -1, 1);
                    government.suspicion = SimMath.Clamp01(government.suspicion + tuning.CrushedSuspicion);
                    Raise(context, nation, SimEventKind.RevoltCrushed, success, c);
                }
                return;
            }
        }

        // The people pick the type they are most aligned with, possibly the same one (D-0024). The revolting
        // generation counts double; elder revolts lean toward restoration (D-0032).
        static GovernmentType ChooseByAlignment(SimContext context, Nation nation, int revolting)
        {
            var tuning = context.Tuning.Politics;
            var government = nation.Government;
            var cohorts = nation.Society.cohorts;
            int best = (int)government.type;
            double bestScore = double.NegativeInfinity;
            for (int k = 0; k < GovernmentState.TypeCount; k++)
            {
                double score = 0, weights = 0;
                for (int c = 0; c < cohorts.Length; c++)
                {
                    double w = cohorts[c].share * (c == revolting ? tuning.RevoltingCohortWeight : 1);
                    score += w * CohortAlignment(context, cohorts[c], k);
                    weights += w;
                }
                score = weights > 0 ? score / weights : 0;
                if (k == (int)government.type) score += tuning.InertiaBonus;
                if (revolting == (int)Cohort.Elder &&
                    (k == (int)government.previousType || k == (int)GovernmentType.Monarchy || k == (int)GovernmentType.Theocracy))
                    score += tuning.ElderRestorationBonus;
                if (score > bestScore)
                {
                    bestScore = score;
                    best = k;
                }
            }
            return (GovernmentType)best;
        }

        // The shield takes the fall: replacing the government vents government suspicion, less each time,
        // and every repeat makes the hidden hand a little easier to see ("new masters, same strings").
        static void ChangeGovernment(SimContext context, Nation nation, GovernmentType newType, double vent)
        {
            var tuning = context.Tuning.Politics;
            var government = nation.Government;
            government.previousType = government.type;
            government.type = newType;
            government.monthsInPower = 0;
            double effective = vent * Math.Pow(tuning.VentDiminishing, government.recentChanges);
            government.suspicion *= 1 - effective;
            government.shadowSuspicion = SimMath.Clamp01(government.shadowSuspicion + tuning.ChangeLeak * government.recentChanges);
            government.recentChanges += 1;
            government.cooldown = tuning.ChangeCooldownMonths;
        }

        static void ApplyViolence(SimContext context, Nation nation, double violence)
        {
            var tuning = context.Tuning.Politics;
            var society = nation.Society;
            for (int c = 0; c < society.cohorts.Length; c++)
                society.cohorts[c].contentment = SimMath.Clamp(society.cohorts[c].contentment - tuning.ViolenceContentmentShock * violence, -1, 1);
            society.crime = SimMath.Clamp01(society.crime + tuning.ViolenceCrimeShock * violence);
        }

        internal static void Start(GovernmentState government, PendingChange kind, GovernmentType target, int months, bool byPlayer)
        {
            government.pending = kind;
            government.pendingTarget = target;
            government.pendingMonths = Math.Max(1, months);
            government.pendingByPlayer = byPlayer;
            government.pendingAnnounced = false;
        }

        static void Raise(SimContext context, Nation nation, SimEventKind kind, double value, int detail) =>
            context.Events.Raise(new SimEvent(context.Date, nation.Index, kind, value, detail));
    }
}
