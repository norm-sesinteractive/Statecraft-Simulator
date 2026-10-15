using NUnit.Framework;
using Statecraft.Sim.Commands;
using Statecraft.Sim.Core;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Scenarios;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Tests
{
    /// <summary>
    /// Balance targets for suspicion and the hand's survival (MODEL.md, "Balance targets", 2026-10-15). They use
    /// default tuning on purpose: if a tuning change breaks one, the game's feel changed and Norm should hear of it.
    /// </summary>
    public class BalanceTests
    {
        [Test]
        public void AnUntouchedWorldStaysCalmForACentury()
        {
            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), 1);
            for (int year = 0; year < 100; year++)
            {
                simulation.Advance(12);
                foreach (var nation in simulation.World.Nations)
                {
                    Assert.That(nation.Government.Suspicion, Is.LessThan(0.3), $"{nation.NameKey} in year {year}");
                    Assert.That(nation.Government.ShadowSuspicion, Is.LessThan(0.05), $"{nation.NameKey} in year {year}");
                }
            }
            for (int i = 0; i < simulation.Events.Count; i++)
                Assert.That(simulation.Events.GetRecent(i).Kind.ToString(), Does.Not.StartWith("Revolt"));
        }

        [Test]
        public void AModerateTiltStaysBelowTheAlarm()
        {
            var simulation = Home(sim =>
            {
                sim.Enqueue(new SetMediaMix(0, new[] { 2.0, 1, 1, 1, 1, 1, 1, 1 }));
                sim.Enqueue(new SetCurriculum(0, new[] { 1.0, 2, 1, 1, 1, 1, 1 }));
            });
            simulation.Advance(TestWorlds.Years(20));
            Assert.That(Home(simulation).Suspicion, Is.LessThan(simulation.Tuning.Politics.SuspicionAlarm));
        }

        [TestCase("airtime")]
        [TestCase("curriculum")]
        public void OneExtremeLeverRaisesTheAlarmWithinADecade(string lever)
        {
            var simulation = Home(sim =>
            {
                if (lever == "airtime") sim.Enqueue(new SetMediaMix(0, TestWorlds.AllAirtimeOn(Vice.Envy)));
                else sim.Enqueue(new SetCurriculum(0, TestWorlds.OnlyFocus(CurriculumFocus.Commerce)));
            });
            simulation.Advance(TestWorlds.Years(10));
            Assert.That(Home(simulation).Suspicion, Is.GreaterThanOrEqualTo(simulation.Tuning.Politics.SuspicionAlarm * 0.95));
        }

        [Test]
        public void EveryLeverAtAnExtremeOverloadsSuspicionWithinFiveYears()
        {
            // Peak, not the value on one date: revolts may already be venting it by year five.
            var simulation = Home(Extremist);
            double peak = 0;
            for (int month = 0; month < TestWorlds.Years(5); month++)
            {
                simulation.Advance();
                peak = System.Math.Max(peak, Home(simulation).Suspicion);
            }
            Assert.That(peak, Is.GreaterThan(0.85));
        }

        [Test]
        public void SustainedSuspicionReachesTheBreakingPointAndUsuallyTopplesTheHand()
        {
            int toppled = 0;
            for (ulong seed = 1; seed <= 3; seed++)
            {
                var simulation = Home(Extremist, seed);
                bool brokeInTime = false;
                for (int month = 0; month < TestWorlds.Years(30) && !simulation.IsGameOver; month++)
                {
                    simulation.Advance();
                    if (month < TestWorlds.Years(15) &&
                        Home(simulation).ShadowSuspicion >= simulation.Tuning.Politics.ShadowBreakingPoint) brokeInTime = true;
                }
                Assert.That(brokeInTime || simulation.IsGameOver, Is.True, $"seed {seed}: shadow suspicion never broke within 15 years");
                if (simulation.IsGameOver) toppled++;
            }
            Assert.That(toppled, Is.GreaterThanOrEqualTo(2), "the hand should fall within 30 years in most games");
        }

        [Test]
        public void ModerateLeversAreFeltWithinTheirHorizon()
        {
            // Pass 2 (D-0034): a 3:1 media tilt is felt within a year; the courts within a year.
            var control = Home(sim => { });
            var tilted = Home(sim => sim.Enqueue(new SetMediaMix(0, new[] { 3.0, 1, 1, 1, 1, 1, 1, 1 })));
            var lenient = Home(sim => sim.Enqueue(new SetLawSeverity(0, 0)));
            var harsh = Home(sim => sim.Enqueue(new SetLawSeverity(0, 1)));
            foreach (var simulation in new[] { control, tilted, lenient, harsh }) simulation.Advance(TestWorlds.Years(1));

            Assert.That(Society(tilted).Aggression - Society(control).Aggression, Is.GreaterThan(0.03));
            Assert.That(Society(control).Contentment - Society(tilted).Contentment, Is.GreaterThan(0.02));
            Assert.That(Society(lenient).Crime - Society(harsh).Crime, Is.GreaterThan(0.05));
            Assert.That(Society(lenient).Contentment - Society(harsh).Contentment, Is.GreaterThan(0.1));
        }

        [Test]
        public void SchoolingThatFitsTheCultureEasesItWithinAGeneration()
        {
            Simulation Warriors(double[] curriculum) => new Simulation(new[]
            {
                new Nations.NationSetup
                {
                    NameKey = "[TEST]", PlayerControlled = true, CurriculumWeights = curriculum,
                    Disposition = new ViceVector(0.7, 0.35, 0.35, 0.35, 0.35, 0.35, 0.35),
                },
            }, SimTuning.CreateDefault(), 1);
            var even = Warriors(Curriculum.EvenWeights());
            var fitted = Warriors(Curriculum.EvenWeights());
            fitted.Enqueue(new SetCurriculum(0, new[] { 1.0, 3, 1, 1, 1, 1, 1 }));
            even.Advance(TestWorlds.Years(25));
            fitted.Advance(TestWorlds.Years(25));

            Assert.That(Society(even).Frustration.Wrath - Society(fitted).Frustration.Wrath, Is.GreaterThan(0.05));
            Assert.That(Society(fitted).Contentment - Society(even).Contentment, Is.GreaterThan(0.02));
        }

        [Test]
        public void MiseryRevoltsAgainstTheGovernmentBeforeAnyoneSuspectsTheHand()
        {
            // A people whose drives have nowhere to go suffers and revolts within a few years, at the shield.
            var simulation = new Simulation(new[]
            {
                new Nations.NationSetup
                {
                    NameKey = "[TEST]", PlayerControlled = true,
                    Disposition = new ViceVector(0.9, 0.35, 0.9, 0.35, 0.35, 0.9, 0.35),
                },
            }, SimTuning.CreateDefault(), 1);
            bool revolted = false;
            for (int month = 0; month < TestWorlds.Years(5) && !revolted; month++)
            {
                simulation.Advance();
                for (int i = 0; i < simulation.Events.Count; i++)
                {
                    var kind = simulation.Events.GetRecent(i).Kind;
                    if (kind == SimEventKind.RevoltSucceeded || kind == SimEventKind.RevoltCrushed) revolted = true;
                }
            }
            Assert.That(Society(simulation).Contentment, Is.LessThan(0));
            Assert.That(revolted, Is.True, "misery should bring a revolt within five years");
            Assert.That(simulation.IsGameOver, Is.False, "hardship alone topples the government, not the hand");
        }

        static SocietyState Society(Simulation simulation) => simulation.World.Nations[0].Society;

        static void Extremist(Simulation sim)
        {
            sim.Enqueue(new SetMediaMix(0, TestWorlds.AllAirtimeOn(Vice.Envy)));
            sim.Enqueue(new SetCurriculum(0, TestWorlds.OnlyFocus(CurriculumFocus.Science)));
            sim.Enqueue(new SetLawSeverity(0, 1));
        }

        // The three-nation scenario with default tuning (news noise on), levers set on the player's home nation.
        static Simulation Home(System.Action<Simulation> levers, ulong seed = 1)
        {
            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), seed);
            levers(simulation);
            return simulation;
        }

        static Politics.GovernmentState Home(Simulation simulation) => simulation.World.Nations[0].Government;
    }
}
