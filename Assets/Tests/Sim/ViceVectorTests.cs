using System;
using NUnit.Framework;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Tests
{
    /// <summary>Indexing and component-wise math on the seven-vice vector.</summary>
    public class ViceVectorTests
    {
        [Test]
        public void IndexerRoundTripsEveryVice()
        {
            var vector = new ViceVector();
            for (int i = 0; i < ViceVector.Count; i++) vector[(Vice)i] = i + 1;

            Assert.That(vector.Wrath, Is.EqualTo(1));
            Assert.That(vector.Sloth, Is.EqualTo(2));
            Assert.That(vector.Greed, Is.EqualTo(3));
            Assert.That(vector.Lust, Is.EqualTo(4));
            Assert.That(vector.Pride, Is.EqualTo(5));
            Assert.That(vector.Envy, Is.EqualTo(6));
            Assert.That(vector.Gluttony, Is.EqualTo(7));
            for (int i = 0; i < ViceVector.Count; i++) Assert.That(vector[i], Is.EqualTo(i + 1));
        }

        [Test]
        public void OutOfRangeIndexThrows()
        {
            var vector = new ViceVector();
            Assert.Throws<ArgumentOutOfRangeException>(() => { _ = vector[ViceVector.Count]; });
        }

        [Test]
        public void ArithmeticIsComponentWise()
        {
            var a = new ViceVector(1, 2, 3, 4, 5, 6, 7);
            var b = ViceVector.Uniform(1);
            Assert.That(a + b, Is.EqualTo(new ViceVector(2, 3, 4, 5, 6, 7, 8)));
            Assert.That(a - b, Is.EqualTo(new ViceVector(0, 1, 2, 3, 4, 5, 6)));
            Assert.That(a * 2, Is.EqualTo(new ViceVector(2, 4, 6, 8, 10, 12, 14)));
            Assert.That(ViceVector.Lerp(ViceVector.Zero, a, 0.5), Is.EqualTo(a * 0.5));
            Assert.That(a.Sum(), Is.EqualTo(28));
        }

        [Test]
        public void ClampAndArgMax()
        {
            var vector = new ViceVector(-1, 0.5, 2, 0, 0, 0.9, 0);
            Assert.That(vector.Clamp01(), Is.EqualTo(new ViceVector(0, 0.5, 1, 0, 0, 0.9, 0)));
            Assert.That(vector.ArgMax(), Is.EqualTo(Vice.Greed));
        }
    }
}
