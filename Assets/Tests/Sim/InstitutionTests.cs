using NUnit.Framework;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Tests
{
    /// <summary>Lever data: shares normalize, media push is measured from an even mix, severity clamps.</summary>
    public class InstitutionTests
    {
        [Test]
        public void MediaSharesAlwaysSumToOne()
        {
            var media = new MediaMix(new[] { 3.0, 0.5, 1.0, 0.5, 2.0, 1.0, 0.5, 1.0 });
            double sum = media.NeutralShare;
            for (int v = 0; v < ViceVector.Count; v++) sum += media.Share((Vice)v);
            Assert.That(sum, Is.EqualTo(1.0).Within(1e-12));
            Assert.That(media.Share(Vice.Wrath), Is.EqualTo(3.0 / 9.5).Within(1e-12));
        }

        [Test]
        public void AllZeroWeightsMeanAnEvenMix()
        {
            var media = new MediaMix(new double[MediaMix.CategoryCount]);
            Assert.That(media.Share(Vice.Envy), Is.EqualTo(MediaMix.EvenShare).Within(1e-12));
        }

        [Test]
        public void NegativeAndNaNWeightsCountAsZero()
        {
            var media = new MediaMix(new[] { -5.0, double.NaN, 1.0, 0, 0, 0, 0, 0 });
            Assert.That(media.Share(Vice.Wrath), Is.EqualTo(0));
            Assert.That(media.Share(Vice.Sloth), Is.EqualTo(0));
            Assert.That(media.Share(Vice.Greed), Is.EqualTo(1));
        }

        [Test]
        public void PushIsZeroForAnEvenMixAndOneForAllAirtime()
        {
            Assert.That(new MediaMix(MediaMix.EvenWeights()).Push(Vice.Lust), Is.EqualTo(0).Within(1e-12));
            var allPride = new MediaMix(TestWorlds.AllAirtimeOn(Vice.Pride));
            Assert.That(allPride.Push(Vice.Pride), Is.EqualTo(1).Within(1e-12));
            Assert.That(allPride.Push(Vice.Wrath), Is.EqualTo(-1.0 / 7.0).Within(1e-12));
        }

        [Test]
        public void CurriculumSharesSumToOne()
        {
            var curriculum = new Curriculum(new[] { 2.0, 1.0, 1.0, 0, 0, 0, 0 });
            Assert.That(curriculum.Share(CurriculumFocus.Agriculture), Is.EqualTo(0.5).Within(1e-12));
            Assert.That(curriculum.Share(CurriculumFocus.Arts), Is.EqualTo(0));
        }

        [Test]
        public void LawSeverityIsClamped()
        {
            Assert.That(new LawPolicy(4).Severity, Is.EqualTo(1));
            Assert.That(new LawPolicy(-1).Severity, Is.EqualTo(0));
        }
    }
}
