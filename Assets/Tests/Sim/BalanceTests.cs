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
            var simulation = Home(Extremist);
            simulation.Advance(TestWorlds.Years(5));
            Assert.That(Home(simulation).Suspicion, Is.GreaterThan(0.85));
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
