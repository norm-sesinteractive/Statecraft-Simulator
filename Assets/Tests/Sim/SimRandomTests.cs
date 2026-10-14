using NUnit.Framework;
using Statecraft.Sim.Core;

namespace Statecraft.Sim.Tests
{
    /// <summary>The PCG32 generator matches the reference output, repeats from a seed, and restores from saved state.</summary>
    public class SimRandomTests
    {
        [Test]
        public void MatchesPcg32ReferenceOutput()
        {
            // First six outputs of the reference pcg32 demo seeded with pcg32_srandom_r(42u, 54u).
            var random = new SimRandom(42, 54);
            uint[] expected = { 0xa15c02b7, 0x7b47f409, 0xba1d3330, 0x83d2f293, 0xbfa4784b, 0xcbed606e };
            foreach (uint value in expected) Assert.That(random.NextUInt(), Is.EqualTo(value));
        }

        [Test]
        public void SameSeedGivesSameSequence()
        {
            var a = new SimRandom(2026);
            var b = new SimRandom(2026);
            for (int i = 0; i < 1000; i++) Assert.That(a.NextUInt(), Is.EqualTo(b.NextUInt()));
        }

        [Test]
        public void DifferentSeedsGiveDifferentSequences()
        {
            var a = new SimRandom(1);
            var b = new SimRandom(2);
            int same = 0;
            for (int i = 0; i < 100; i++)
                if (a.NextUInt() == b.NextUInt()) same++;
            Assert.That(same, Is.LessThan(3));
        }

        [Test]
        public void NextDoubleStaysInTheUnitInterval()
        {
            var random = new SimRandom(5);
            for (int i = 0; i < 10000; i++) Assert.That(random.NextDouble(), Is.InRange(0.0, 0.9999999999));
        }

        [Test]
        public void StateRoundTripsForSaveFiles()
        {
            var original = new SimRandom(77);
            original.NextUInt();
            var restored = SimRandom.FromState(original.State, original.Increment);
            for (int i = 0; i < 100; i++) Assert.That(restored.NextUInt(), Is.EqualTo(original.NextUInt()));
        }

        [Test]
        public void GaussianIsCenteredWithUnitSpread()
        {
            var random = new SimRandom(11);
            const int samples = 20000;
            double sum = 0, sumSquares = 0;
            for (int i = 0; i < samples; i++)
            {
                double x = random.NextGaussian();
                sum += x;
                sumSquares += x * x;
            }
            double mean = sum / samples;
            double variance = sumSquares / samples - mean * mean;
            Assert.That(mean, Is.EqualTo(0.0).Within(0.05));
            Assert.That(variance, Is.EqualTo(1.0).Within(0.1));
        }
    }
}
