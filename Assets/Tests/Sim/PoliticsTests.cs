using System;
using NUnit.Framework;
using Statecraft.Sim.Commands;
using Statecraft.Sim.Core;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Scenarios;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Tests
{
    /// <summary>
    /// Executable statements of the politics design (D-0019 to D-0024, D-0032): alignment, two-tier
    /// suspicion, the government as a shield, revolutions, votes, generational revolts and defeat.
    /// </summary>
    public class PoliticsTests
    {
        [Test]
        public void EachScenarioNationIsMostAlignedWithItsOwnGovernment()
        {
            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), TestWorlds.QuietTuning(), 1);
            foreach (var nation in simulation.World.Nations)
            {
                var government = nation.Government;
                for (int k = 0; k < GovernmentState.TypeCount; k++)
                    Assert.That(government.Alignment(government.Type), Is.GreaterThanOrEqualTo(government.Alignment((GovernmentType)k)),
                        $"{nation.NameKey} prefers {(GovernmentType)k} over its own {government.Type}");
            }
        }

        [Test]
        public void AnEducatedPeopleGrowsSuspicious()
        {
            // Norm: "extremely high science ... would increase the likelihood of a country figuring out something is wrong."
            var scholars = TestWorlds.Single(s => s.CurriculumWeights = TestWorlds.OnlyFocus(CurriculumFocus.Science));
            var farmers = TestWorlds.Single(s => s.CurriculumWeights = TestWorlds.OnlyFocus(CurriculumFocus.Agriculture));
            scholars.Advance(TestWorlds.Years(20));
            farmers.Advance(TestWorlds.Years(20));
            Assert.That(Government(scholars).Suspicion, Is.GreaterThan(Government(farmers).Suspicion + 0.05));
        }

        [Test]
        public void AbruptLeverSwingsRaiseSuspicion()
        {
            var swinging = TestWorlds.Single();
            var steady = TestWorlds.Single();
            var wrathTilt = new[] { 4.0, 1, 1, 1, 1, 1, 1, 1 };
            var slothTilt = new[] { 1.0, 4, 1, 1, 1, 1, 1, 1 };
            for (int year = 0; year < 10; year++)
            {
                swinging.Enqueue(new SetMediaMix(0, year % 2 == 0 ? wrathTilt : slothTilt));
                swinging.Advance(12);
                steady.Advance(12);
            }
            Assert.That(Government(swinging).Suspicion, Is.GreaterThan(Government(steady).Suspicion + 0.05));
        }

        [Test]
        public void LastingGovernmentSuspicionLeaksIntoShadowSuspicion()
        {
            var simulation = TestWorlds.Single(
                s => s.CurriculumWeights = TestWorlds.OnlyFocus(CurriculumFocus.Science),
                t =>
                {
                    t.Politics.ScienceSuspicion = 3;
                    t.Politics.RevoltThreshold = 2; // no revolts: they would vent the suspicion this test watches leak
                });
            Assert.That(Government(simulation).ShadowSuspicion, Is.EqualTo(0));
            simulation.Advance(TestWorlds.Years(20));
            Assert.That(Government(simulation).Suspicion, Is.GreaterThan(0.8));
            Assert.That(Government(simulation).ShadowSuspicion, Is.GreaterThan(0.1));
        }

        [Test]
        public void AFalseRevolutionReplacesTheShieldAndVentsSuspicion()
        {
            var simulation = Suspicious(t => t.Politics.FomentBaseSuccess = 1);
            Assert.That(simulation.CanFoment(0, GovernmentType.Monarchy), Is.True);
            simulation.Enqueue(new FomentRevolution(0, GovernmentType.Monarchy));
            simulation.Advance(simulation.Tuning.Politics.FomentMonths - 1);
            double before = Government(simulation).Suspicion;
            Assert.That(Government(simulation).Pending, Is.EqualTo(PendingChange.Revolution));

            simulation.Advance();

            Assert.That(Government(simulation).Type, Is.EqualTo(GovernmentType.Monarchy));
            Assert.That(Government(simulation).Suspicion, Is.LessThan(before * 0.5));
            Assert.That(HasEvent(simulation, SimEventKind.RevolutionSucceeded), Is.True);
        }

        [Test]
        public void EachFalseRevolutionVentsLessAndLeaksMore()
        {
            var simulation = Suspicious(t => t.Politics.FomentBaseSuccess = 1);
            double firstVent = Revolt(simulation, GovernmentType.Monarchy, out double firstLeak);
            while (!simulation.CanFoment(0, GovernmentType.Democracy)) simulation.Advance();
            double secondVent = Revolt(simulation, GovernmentType.Democracy, out double secondLeak);
            Assert.That(secondVent, Is.LessThan(firstVent));
            Assert.That(secondLeak, Is.GreaterThan(firstLeak));
        }

        [Test]
        public void AFailedPlotRaisesSuspicion()
        {
            var simulation = Suspicious(t =>
            {
                t.Politics.FomentBaseSuccess = 0;
                t.Politics.FomentUnrestSuccess = 0;
                t.Politics.FomentAlignmentSuccess = 0;
            });
            simulation.Enqueue(new FomentRevolution(0, GovernmentType.Monarchy));
            simulation.Advance(simulation.Tuning.Politics.FomentMonths - 1);
            double suspicion = Government(simulation).Suspicion;
            double shadow = Government(simulation).ShadowSuspicion;

            simulation.Advance();

            Assert.That(Government(simulation).Type, Is.EqualTo(GovernmentType.Democracy));
            Assert.That(Government(simulation).Suspicion, Is.GreaterThan(suspicion));
            Assert.That(Government(simulation).ShadowSuspicion, Is.GreaterThan(shadow));
            Assert.That(HasEvent(simulation, SimEventKind.RevolutionFailed), Is.True);
        }

        [Test]
        public void OnlyDemocraciesVote()
        {
            var democracy = TestWorlds.Single();
            var monarchy = TestWorlds.Single(s => s.Government = GovernmentType.Monarchy);
            Assert.That(democracy.CanCallVote(0, GovernmentType.Monarchy), Is.True);
            Assert.That(monarchy.CanCallVote(0, GovernmentType.Democracy), Is.False);
            Assert.That(monarchy.CanFoment(0, GovernmentType.Democracy), Is.True);
            Assert.That(democracy.CanFoment(0, GovernmentType.Democracy), Is.False, "no revolution toward the type already in power");

            monarchy.Enqueue(new CallVote(0, GovernmentType.Democracy));
            monarchy.Advance();
            Assert.That(Government(monarchy).Pending, Is.EqualTo(PendingChange.None), "an ignored command must not start anything");
        }

        [Test]
        public void AVotePassesWhenThePeopleAlignWithTheTarget()
        {
            var simulation = TestWorlds.Single(
                s => s.Disposition = new ViceVector(0.35, 0.7, 0.35, 0.35, 0.7, 0.35, 0.35),
                t => { Certain(t); t.Politics.VoteSensitivity = 100; });
            Assert.That(Government(simulation).Alignment(GovernmentType.Monarchy),
                Is.GreaterThan(Government(simulation).Alignment(GovernmentType.Democracy)));
            simulation.Enqueue(new CallVote(0, GovernmentType.Monarchy));
            simulation.Advance(simulation.Tuning.Politics.VoteMonths);
            Assert.That(Government(simulation).Type, Is.EqualTo(GovernmentType.Monarchy));
            Assert.That(HasEvent(simulation, SimEventKind.VotePassed), Is.True);
        }

        [Test]
        public void ARevoltInstallsTheTypeThePeopleAlignWith()
        {
            // D-0024: "Whatever government type the current population would be most aligned with."
            var simulation = TestWorlds.Single(
                s => s.Disposition = new ViceVector(0.8, 0.35, 0.35, 0.35, 0.7, 0.35, 0.35),
                t => ForceRevolts(t, youth: 1, adults: 0, elders: 0));
            simulation.Advance();
            Assert.That(Government(simulation).Type, Is.EqualTo(GovernmentType.MilitaryJunta));
            Assert.That(LastEvent(simulation, SimEventKind.RevoltSucceeded).Detail, Is.EqualTo((int)Cohort.Youth));
        }

        [Test]
        public void ElderRevoltsRestoreWhatYouthRevoltsOverthrew()
        {
            // D-0032: elder revolts lean to restoration; the youth's do not.
            Simulation Run(bool elders)
            {
                var simulation = TestWorlds.Single(
                    s =>
                    {
                        s.Government = GovernmentType.Monarchy;
                        s.Disposition = new ViceVector(0.35, 0.35, 0.35, 0.35, 0.6, 0.6, 0.35);
                        s.CurriculumWeights = new[] { 0.0, 0, 0, 0, 1, 1, 0 };
                    },
                    t => { ForceRevolts(t, youth: 0, adults: 1, elders: 0); t.Politics.ElderRestorationBonus = 0.5; });
                simulation.Advance();
                Assert.That(Government(simulation).Type, Is.EqualTo(GovernmentType.Democracy), "first the people install the type they align with");
                ForceRevolts(simulation.Tuning, youth: elders ? 0 : 1, adults: 0, elders: elders ? 1 : 0);
                while (Government(simulation).MonthsInPower > 0 || Government(simulation).Type == GovernmentType.Democracy)
                {
                    simulation.Advance();
                    if (simulation.Date.Tick > 60) break;
                }
                return simulation;
            }

            Assert.That(Government(Run(elders: true)).Type, Is.EqualTo(GovernmentType.Monarchy));
            Assert.That(Government(Run(elders: false)).Type, Is.EqualTo(GovernmentType.Democracy));
        }

        [Test]
        public void ARevoltPastTheBreakingPointTopplesTheHiddenHand()
        {
            var simulation = TestWorlds.Single(configure: null, tune: t => ForceRevolts(t, youth: 1, adults: 0, elders: 0));
            Government(simulation).shadowSuspicion = 0.9;
            simulation.Advance();
            Assert.That(simulation.IsGameOver, Is.True);
            Assert.That(simulation.World.Nations[0].PlayerControlled, Is.False);
            Assert.That(simulation.CanFoment(0, GovernmentType.Monarchy), Is.False);
            Assert.That(HasEvent(simulation, SimEventKind.HiddenHandOverthrown), Is.True);
        }

        [Test]
        public void BelowTheBreakingPointTheGovernmentTakesTheFall()
        {
            var simulation = TestWorlds.Single(configure: null, tune: t => ForceRevolts(t, youth: 1, adults: 0, elders: 0));
            Government(simulation).shadowSuspicion = 0.3;
            simulation.Advance();
            Assert.That(simulation.IsGameOver, Is.False);
            Assert.That(simulation.World.Nations[0].PlayerControlled, Is.True);
            Assert.That(HasEvent(simulation, SimEventKind.RevoltSucceeded), Is.True);
        }

        [Test]
        public void EachGenerationHasItsOwnContentment()
        {
            var simulation = TestWorlds.Single();
            simulation.World.Nations[0].Society.cohorts[(int)Cohort.Youth].disposition.Wrath = 0.95;
            simulation.Advance();
            var society = TestWorlds.Society(simulation);
            Assert.That(society.GetCohort(Cohort.Youth).Contentment, Is.LessThan(society.GetCohort(Cohort.Adult).Contentment));
            Assert.That(society.GetCohort(Cohort.Youth).UnrestTarget, Is.GreaterThan(society.GetCohort(Cohort.Adult).UnrestTarget));
        }

        // A Democracy whose Science schooling keeps government suspicion high.
        static Simulation Suspicious(Action<SimTuning> extra) => TestWorlds.Single(
            s => s.CurriculumWeights = new[] { 1.0, 1, 1, 1, 3, 1, 1 },
            t => { t.Politics.ScienceSuspicion = 2; Certain(t); extra(t); });

        static double Revolt(Simulation simulation, GovernmentType target, out double leak)
        {
            simulation.Enqueue(new FomentRevolution(0, target));
            simulation.Advance(simulation.Tuning.Politics.FomentMonths - 1);
            double before = Government(simulation).Suspicion;
            double shadowBefore = Government(simulation).ShadowSuspicion;
            simulation.Advance();
            Assert.That(Government(simulation).Type, Is.EqualTo(target));
            leak = Government(simulation).ShadowSuspicion - shadowBefore;
            return 1 - Government(simulation).Suspicion / before;
        }

        static void Certain(SimTuning tuning)
        {
            tuning.Politics.ChanceFloor = 0;
            tuning.Politics.ChanceCeiling = 1;
        }

        // Makes the chosen generations revolt on the next tick, with certain success.
        static void ForceRevolts(SimTuning tuning, double youth, double adults, double elders)
        {
            Certain(tuning);
            tuning.Politics.RevoltThreshold = -1;
            tuning.Politics.RevoltChancePerUnrest = 100;
            tuning.Politics.RevoltFrequency = new[] { youth, adults, elders };
            tuning.Politics.RevoltSuccess = new[] { 1.0, 1.0, 1.0 };
        }

        static GovernmentState Government(Simulation simulation) => simulation.World.Nations[0].Government;

        static bool HasEvent(Simulation simulation, SimEventKind kind)
        {
            for (int i = 0; i < simulation.Events.Count; i++)
                if (simulation.Events.GetRecent(i).Kind == kind) return true;
            return false;
        }

        static SimEvent LastEvent(Simulation simulation, SimEventKind kind)
        {
            for (int i = 0; i < simulation.Events.Count; i++)
                if (simulation.Events.GetRecent(i).Kind == kind) return simulation.Events.GetRecent(i);
            throw new AssertionException($"No {kind} event.");
        }
    }
}
