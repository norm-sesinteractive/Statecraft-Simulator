using NUnit.Framework;
using Statecraft.Sim.Core;
using Statecraft.Sim.Scenarios;

namespace Statecraft.Sim.Tests
{
    /// <summary>Built-in scenarios must keep the personalities they were written to have.</summary>
    public class ScenarioTests
    {
        [Test]
        public void ThreeNationsHasOnePlayerHome()
        {
            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), TestWorlds.QuietTuning(), 1);
            Assert.That(simulation.World.Nations.Count, Is.EqualTo(3));
            Assert.That(simulation.World.PlayerHomeIndex, Is.EqualTo(0));
        }

        [Test]
        public void TheWarriorCultureIsTheMostAggressiveAndTheIndulgentOneTheLeast()
        {
            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), TestWorlds.QuietTuning(), 1);
            simulation.Advance(TestWorlds.Years(5));
            double home = TestWorlds.Society(simulation, 0).Aggression;
            double warriors = TestWorlds.Society(simulation, 1).Aggression;
            double indulgent = TestWorlds.Society(simulation, 2).Aggression;
            Assert.That(warriors, Is.GreaterThan(home));
            Assert.That(home, Is.GreaterThan(indulgent));
        }
    }
}
