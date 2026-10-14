using System.Collections.Generic;
using NUnit.Framework;
using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
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
                var s = simulation.World.Nations[n].Society;
                Assert.That(SimMath.Clamp(Sum(simulation, n, Quantity.Contentment, -1, terms), -1, 1), Is.EqualTo(s.ContentmentTarget).Within(1e-12));
                Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Crime, -1, terms)), Is.EqualTo(s.CrimeTarget).Within(1e-12));
                Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Unrest, -1, terms)), Is.EqualTo(s.UnrestTarget).Within(1e-12));
                Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Aggression, -1, terms)), Is.EqualTo(s.Aggression).Within(1e-12));
                Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Fear, -1, terms)), Is.EqualTo(s.Fear).Within(1e-12));
                for (int v = 0; v < ViceVector.Count; v++)
                {
                    Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Outlet, v, terms)), Is.EqualTo(s.Outlets[v]).Within(1e-12));
                    Assert.That(SimMath.Clamp01(Sum(simulation, n, Quantity.Expressed, v, terms)), Is.EqualTo(s.Expressed[v]).Within(1e-12));
                    Assert.That(Sum(simulation, n, Quantity.Frustration, v, terms), Is.EqualTo(s.Frustration[v]).Within(1e-12));
                }
            }
        }

        [Test]
        public void CollectSortsByMagnitude()
        {
            var simulation = TestWorlds.Single();
            simulation.Advance();
            var terms = new List<Contribution>();
            simulation.Ledger.Collect(0, Quantity.Contentment, -1, terms);
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

        static double Sum(Simulation simulation, int nation, Quantity quantity, int vice, List<Contribution> terms)
        {
            simulation.Ledger.Collect(nation, quantity, vice, terms);
            double sum = 0;
            foreach (var term in terms) sum += term.Amount;
            return sum;
        }
    }
}
