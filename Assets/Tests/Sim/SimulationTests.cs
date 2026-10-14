using System;
using NUnit.Framework;
using Statecraft.Sim.Commands;
using Statecraft.Sim.Core;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Scenarios;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Tests
{
    /// <summary>Engine guarantees: determinism, bounded values, a quiet start, and command timing.</summary>
    public class SimulationTests
    {
        [Test]
        public void SameSeedAndCommandsGiveIdenticalWorlds()
        {
            var a = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), 123);
            var b = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), 123);
            DriveWithScriptedLevers(a);
            DriveWithScriptedLevers(b);

            for (int n = 0; n < a.World.Nations.Count; n++)
            {
                var sa = a.World.Nations[n].Society;
                var sb = b.World.Nations[n].Society;
                Assert.That(sa.Mood, Is.EqualTo(sb.Mood));
                Assert.That(sa.Expressed, Is.EqualTo(sb.Expressed));
                Assert.That(sa.Outlets, Is.EqualTo(sb.Outlets));
                Assert.That(sa.Contentment, Is.EqualTo(sb.Contentment));
                Assert.That(sa.Crime, Is.EqualTo(sb.Crime));
                Assert.That(sa.Unrest, Is.EqualTo(sb.Unrest));
                for (int c = 0; c < 3; c++)
                {
                    Assert.That(sa.GetCohort((Cohort)c).Disposition, Is.EqualTo(sb.GetCohort((Cohort)c).Disposition));
                    Assert.That(sa.GetCohort((Cohort)c).Channeling, Is.EqualTo(sb.GetCohort((Cohort)c).Channeling));
                }
            }
            Assert.That(a.Events.TotalRaised, Is.EqualTo(b.Events.TotalRaised));
        }

        [Test]
        public void DifferentSeedsGiveDifferentNewsCycles()
        {
            var a = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), 1);
            var b = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), 2);
            a.Advance(12);
            b.Advance(12);
            Assert.That(a.World.Nations[0].Society.Mood, Is.Not.EqualTo(b.World.Nations[0].Society.Mood));
        }

        [Test]
        public void ThreeHundredYearsOfRandomLeversStayInBounds()
        {
            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), 99);
            var chaos = new SimRandom(2024); // drives the test's lever changes; separate from the sim's own generator

            for (int month = 0; month < TestWorlds.Years(300); month++)
            {
                if (month % 12 == 0)
                {
                    for (int n = 0; n < simulation.World.Nations.Count; n++)
                    {
                        simulation.Enqueue(new SetMediaMix(n, RandomWeights(chaos, 8)));
                        simulation.Enqueue(new SetCurriculum(n, RandomWeights(chaos, 7)));
                        simulation.Enqueue(new SetLawSeverity(n, chaos.NextDouble()));
                    }
                }
                simulation.Advance();
                string violation = FindViolation(simulation);
                Assert.That(violation, Is.Null, $"At {simulation.Date}");
            }
        }

        [Test]
        public void ANewGameStartsAtRest()
        {
            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), TestWorlds.QuietTuning(), 1);
            var before = new double[simulation.World.Nations.Count * 3];
            for (int n = 0; n < simulation.World.Nations.Count; n++)
            {
                var s = simulation.World.Nations[n].Society;
                before[n * 3] = s.Contentment;
                before[n * 3 + 1] = s.Crime;
                before[n * 3 + 2] = s.Unrest;
            }

            simulation.Advance();

            for (int n = 0; n < simulation.World.Nations.Count; n++)
            {
                var s = simulation.World.Nations[n].Society;
                Assert.That(s.Contentment, Is.EqualTo(before[n * 3]).Within(0.005));
                Assert.That(s.Crime, Is.EqualTo(before[n * 3 + 1]).Within(0.005));
                Assert.That(s.Unrest, Is.EqualTo(before[n * 3 + 2]).Within(0.005));
            }
        }

        [Test]
        public void CommandsWaitForTheNextTick()
        {
            var simulation = TestWorlds.Single();
            simulation.Enqueue(new SetLawSeverity(0, 0.9));
            Assert.That(simulation.World.Nations[0].Institutions.Law.Severity, Is.EqualTo(0.5));
            Assert.That(simulation.PendingCommandCount, Is.EqualTo(1));

            simulation.Advance();

            Assert.That(simulation.World.Nations[0].Institutions.Law.Severity, Is.EqualTo(0.9));
            Assert.That(simulation.PendingCommandCount, Is.EqualTo(0));
        }

        [Test]
        public void CommandsForMissingNationsFailLoudly()
        {
            var simulation = TestWorlds.Single();
            simulation.Enqueue(new SetLawSeverity(5, 1));
            Assert.Throws<InvalidOperationException>(() => simulation.Advance());
        }

        static void DriveWithScriptedLevers(Simulation simulation)
        {
            for (int year = 0; year < 50; year++)
            {
                if (year == 5) simulation.Enqueue(new SetMediaMix(0, TestWorlds.AllAirtimeOn(Vice.Envy)));
                if (year == 12) simulation.Enqueue(new SetCurriculum(0, TestWorlds.OnlyFocus(CurriculumFocus.Science)));
                if (year == 20) simulation.Enqueue(new SetLawSeverity(1, 0.1));
                simulation.Advance(12);
            }
        }

        static double[] RandomWeights(SimRandom random, int count)
        {
            var weights = new double[count];
            for (int i = 0; i < count; i++) weights[i] = random.NextDouble();
            return weights;
        }

        static string FindViolation(Simulation simulation)
        {
            foreach (var nation in simulation.World.Nations)
            {
                var s = nation.Society;
                string where = nation.NameKey;
                if (!InRange(s.Mood, -1, 1)) return $"{where} mood {s.Mood}";
                if (!InRange(s.Disposition, 0, 1)) return $"{where} disposition {s.Disposition}";
                if (!InRange(s.Expressed, 0, 1)) return $"{where} expressed {s.Expressed}";
                if (!InRange(s.Outlets, 0, 1)) return $"{where} outlets {s.Outlets}";
                if (!InRange(s.Frustration, 0, 1)) return $"{where} frustration {s.Frustration}";
                for (int c = 0; c < 3; c++)
                {
                    var cohort = s.GetCohort((Cohort)c);
                    if (!InRange(cohort.Disposition, 0, 1)) return $"{where} {cohort.Cohort} disposition {cohort.Disposition}";
                    if (!InRange(cohort.Channeling, 0, 1)) return $"{where} {cohort.Cohort} channeling {cohort.Channeling}";
                }
                if (!InRange(s.Contentment, -1, 1)) return $"{where} contentment {s.Contentment}";
                if (!InRange(s.Crime, 0, 1)) return $"{where} crime {s.Crime}";
                if (!InRange(s.Unrest, 0, 1)) return $"{where} unrest {s.Unrest}";
                if (!InRange(s.Fear, 0, 1)) return $"{where} fear {s.Fear}";
                if (!InRange(s.Aggression, 0, 1)) return $"{where} aggression {s.Aggression}";
            }
            return null;
        }

        static bool InRange(double value, double min, double max) => !double.IsNaN(value) && value >= min && value <= max;

        static bool InRange(ViceVector vector, double min, double max)
        {
            if (!vector.IsFinite()) return false;
            for (int i = 0; i < ViceVector.Count; i++)
                if (vector[i] < min || vector[i] > max) return false;
            return true;
        }
    }
}
