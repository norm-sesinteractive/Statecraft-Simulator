using System.Collections.Generic;
using NUnit.Framework;
using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Scenarios;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Tests
{
    /// <summary>The "why?" ledger must account for every computed value exactly, or explanations lie.</summary>
    public class LedgerTests
    {
        [Test]
        public void ContributionsAddUpToEveryRecordedValue()
        {
            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), 5);
            simulation.Advance(TestWorlds.Years(3));
            var terms = new List<Contribution>();

            for (int n = 0; n < simulation.World.Nations.Count; n++)
            {
                var nation = simulation.World.Nations[n];
                var s = nation.Society;
                var g = nation.Government;
                Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Crime, -1, -1, terms)), Is.EqualTo(s.CrimeTarget).Within(1e-12));
                Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Aggression, -1, -1, terms)), Is.EqualTo(s.Aggression).Within(1e-12));
                Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Fear, -1, -1, terms)), Is.EqualTo(s.Fear).Within(1e-12));
                Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.GovernmentSuspicion, -1, -1, terms)), Is.EqualTo(g.SuspicionTarget).Within(1e-12));
                Assert.That(Sum(simulation, n, Quantity.ShadowSuspicion, -1, -1, terms), Is.EqualTo(g.shadowDrift).Within(1e-12));
                for (int k = 0; k < GovernmentState.TypeCount; k++)
                    Assert.That(Sum(simulation, n, Quantity.Alignment, k, -1, terms), Is.EqualTo(g.Alignment((GovernmentType)k)).Within(1e-12));

                for (int v = 0; v < ViceVector.Count; v++)
                {
                    Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Outlet, v, -1, terms)), Is.EqualTo(s.Outlets[v]).Within(1e-12));
                    Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Expressed, v, -1, terms)), Is.EqualTo(s.Expressed[v]).Within(1e-12));
                    Assert.That(Sum(simulation, n, Quantity.Frustration, v, -1, terms), Is.EqualTo(s.Frustration[v]).Within(1e-12));
                }

                double weighted = 0;
                for (int c = 0; c < 3; c++)
                {
                    var cohort = s.GetCohort((Cohort)c);
                    weighted += cohort.Share * cohort.Contentment;
                    Assert.That(SimMath.Clamp(Sum(simulation, n, Quantity.Contentment, -1, c, terms), -1, 1), Is.EqualTo(cohort.ContentmentTarget).Within(1e-12));
                    Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Unrest, -1, c, terms)), Is.EqualTo(cohort.UnrestTarget).Within(1e-12));
                    for (int v = 0; v < ViceVector.Count; v++)
                    {
                        Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Expressed, v, c, terms)), Is.EqualTo(cohort.Expressed[v]).Within(1e-12));
                        Assert.That(Sum(simulation, n, Quantity.Frustration, v, c, terms), Is.EqualTo(cohort.Frustration[v]).Within(1e-12));
                    }
                }
                Assert.That(s.Contentment, Is.EqualTo(weighted).Within(1e-12), "national contentment is the generations' weighted average");
            }
        }

        [Test]
        public void CollectSortsByMagnitude()
        {
            var simulation = TestWorlds.Single();
            simulation.Advance();
            var terms = new List<Contribution>();
            simulation.Ledger.Collect(0, Quantity.Contentment, -1, (int)Cohort.Adult, terms);
            Assert.That(terms.Count, Is.GreaterThan(1));
            for (int i = 1; i < terms.Count; i++)
                Assert.That(System.Math.Abs(terms[i - 1].Amount), Is.GreaterThanOrEqualTo(System.Math.Abs(terms[i].Amount)));
        }

        [Test]
        public void DisabledLedgerRecordsNothing()
        {
            var simulation = TestWorlds.Single();
            simulation.Ledger.Enabled = false;
            simulation.Advance();
            Assert.That(simulation.Ledger.Entries.Count, Is.EqualTo(0));
        }

        static double Sum(Simulation simulation, int nation, Quantity quantity, int index, int cohort, List<Contribution> terms)
        {
            simulation.Ledger.Collect(nation, quantity, index, cohort, terms);
            double sum = 0;
            foreach (var term in terms) sum += term.Amount;
            return sum;
        }
    }
}
