using System;
using NUnit.Framework;
using Statecraft.Sim.Commands;
using Statecraft.Sim.Core;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Tests
{
    /// <summary>
    /// Executable statements of the design (Docs/design, Docs/simulation/MODEL.md). Each test
    /// names the behavior it protects; if a balance change breaks one, the design changed and
    /// the docs must say so.
    /// </summary>
    public class RiverBehaviorTests
    {
        [Test]
        public void MediaStirsMoodWithinAYear()
        {
            var simulation = TestWorlds.Single();
            double before = TestWorlds.Society(simulation).Expressed.Wrath;

            simulation.Enqueue(new SetMediaMix(0, TestWorlds.AllAirtimeOn(Vice.Wrath)));
            simulation.Advance(TestWorlds.Years(1));

            Assert.That(TestWorlds.Society(simulation).Expressed.Wrath - before, Is.GreaterThan(0.15));
        }

        [Test]
        public void YearsOfAirtimeLeaveACulturalResidueAfterTheMoodFades()
        {
            var exposed = TestWorlds.Single();
            var control = TestWorlds.Single();

            exposed.Enqueue(new SetMediaMix(0, TestWorlds.AllAirtimeOn(Vice.Wrath)));
            exposed.Advance(TestWorlds.Years(20));
            control.Advance(TestWorlds.Years(20));
            exposed.Enqueue(new SetMediaMix(0, MediaMix.EvenWeights()));
            exposed.Advance(TestWorlds.Years(2));
            control.Advance(TestWorlds.Years(2));

            var e = TestWorlds.Society(exposed);
            var c = TestWorlds.Society(control);
            Assert.That(Math.Abs(e.Mood.Wrath), Is.LessThan(0.02), "the mood should have faded");
            Assert.That(e.Expressed.Wrath - c.Expressed.Wrath, Is.GreaterThan(0.05), "the culture should remember");
        }

        [Test]
        public void SchoolingReachesTheWorkforceAGenerationLate()
        {
            double full = WrathOutletGainAfterMilitarySchooling(100);
            double early = WrathOutletGainAfterMilitarySchooling(2);
            double oneGeneration = WrathOutletGainAfterMilitarySchooling(40);

            Assert.That(full, Is.GreaterThan(0.2));
            Assert.That(early, Is.LessThan(0.1 * full), "two years of school barely touch the workforce");
            Assert.That(oneGeneration, Is.GreaterThan(0.45 * full), "a generation later the change has arrived");
        }

        [Test]
        public void OutletsTurnWrathIntoContentmentInsteadOfCrime()
        {
            // Isolate Wrath: only the Military focus trains anything, and only Wrath.
            Action<SimTuning> onlyMilitaryTrainsWrath = t =>
            {
                for (int i = 0; i < t.Education.FocusChanneling.Length; i++) t.Education.FocusChanneling[i] = ViceVector.Zero;
                t.Education.FocusChanneling[(int)CurriculumFocus.Military] = new ViceVector(1, 0, 0, 0, 0, 0, 0);
            };
            var wrathful = new ViceVector(0.7, 0.35, 0.35, 0.35, 0.35, 0.35, 0.35);
            var soldiers = TestWorlds.Single(s => { s.Disposition = wrathful; s.CurriculumWeights = TestWorlds.OnlyFocus(CurriculumFocus.Military); }, onlyMilitaryTrainsWrath);
            var idlers = TestWorlds.Single(s => { s.Disposition = wrathful; s.CurriculumWeights = TestWorlds.OnlyFocus(CurriculumFocus.Arts); }, onlyMilitaryTrainsWrath);

            soldiers.Advance(TestWorlds.Years(10));
            idlers.Advance(TestWorlds.Years(10));

            var s1 = TestWorlds.Society(soldiers);
            var s2 = TestWorlds.Society(idlers);
            Assert.That(s1.Frustration.Wrath, Is.LessThan(s2.Frustration.Wrath));
            Assert.That(s1.Crime, Is.LessThan(s2.Crime));
            Assert.That(s1.Contentment, Is.GreaterThan(s2.Contentment));
        }

        [Test]
        public void AnAggressiveCultureWithOutletsCanBeHappy()
        {
            // Norm's pitch: "Aggressive cultures, that have outlets for that aggression, can still be happy cultures."
            Action<SimTuning> onlyMilitaryTrainsWrath = t =>
            {
                for (int i = 0; i < t.Education.FocusChanneling.Length; i++) t.Education.FocusChanneling[i] = ViceVector.Zero;
                t.Education.FocusChanneling[(int)CurriculumFocus.Military] = new ViceVector(1, 0, 0, 0, 0, 0, 0);
            };
            var warriors = TestWorlds.Single(s => { s.Disposition = new ViceVector(0.7, 0.35, 0.35, 0.35, 0.35, 0.35, 0.35); s.CurriculumWeights = TestWorlds.OnlyFocus(CurriculumFocus.Military); }, onlyMilitaryTrainsWrath);
            var peaceful = TestWorlds.Single(s => { s.CurriculumWeights = TestWorlds.OnlyFocus(CurriculumFocus.Military); }, onlyMilitaryTrainsWrath);

            warriors.Advance(TestWorlds.Years(1));
            peaceful.Advance(TestWorlds.Years(1));

            Assert.That(TestWorlds.Society(warriors).Contentment, Is.GreaterThanOrEqualTo(TestWorlds.Society(peaceful).Contentment));
            Assert.That(TestWorlds.Society(warriors).Aggression, Is.GreaterThan(TestWorlds.Society(peaceful).Aggression));
        }

        [Test]
        public void HarshCourtsCutCrimeButBreedFear()
        {
            // Norm's pitch: pardoning eases suffering, but less fear of punishment means more crime.
            var harsh = TestWorlds.Single(s => s.LawSeverity = 1);
            var lenient = TestWorlds.Single(s => s.LawSeverity = 0);
            harsh.Advance(TestWorlds.Years(10));
            lenient.Advance(TestWorlds.Years(10));

            var h = TestWorlds.Society(harsh);
            var l = TestWorlds.Society(lenient);
            Assert.That(h.Crime, Is.LessThan(l.Crime));
            Assert.That(h.Fear, Is.GreaterThan(l.Fear));
            Assert.That(h.Contentment, Is.LessThan(l.Contentment));
        }

        [Test]
        public void ASufficientlyContentPeopleNeedsNoCourts()
        {
            // Norm's pitch: "Perhaps a society can be so happy and so well supplied that crime no longer takes place."
            var utopia = TestWorlds.Single(s => s.LawSeverity = 0, t => t.Wellbeing.ContentmentBase = 0.9);
            utopia.Advance(TestWorlds.Years(5));
            Assert.That(TestWorlds.Society(utopia).Crime, Is.LessThan(0.01));
        }

        [Test]
        public void SlothSoftensSufferingIntoAcceptance()
        {
            var frustrated = new ViceVector(0.8, 0.2, 0.8, 0.35, 0.35, 0.8, 0.35);
            // No revolts: they would reset unrest at random and blur the comparison this test makes.
            Action<SimTuning> noRevolts = t => t.Politics.RevoltThreshold = 2;
            var restless = TestWorlds.Single(s => s.Disposition = frustrated, noRevolts);
            var accepting = TestWorlds.Single(s => { var d = frustrated; d.Sloth = 0.9; s.Disposition = d; }, noRevolts);
            restless.Advance(TestWorlds.Years(2));
            accepting.Advance(TestWorlds.Years(2));
            Assert.That(TestWorlds.Society(accepting).Unrest, Is.LessThanOrEqualTo(TestWorlds.Society(restless).Unrest));
            Assert.That(TestWorlds.Society(accepting).ContentmentTarget, Is.GreaterThan(TestWorlds.Society(restless).ContentmentTarget));
        }

        static double WrathOutletGainAfterMilitarySchooling(int years)
        {
            var simulation = TestWorlds.Single();
            double start = TestWorlds.Society(simulation).Outlets.Wrath;
            simulation.Enqueue(new SetCurriculum(0, TestWorlds.OnlyFocus(CurriculumFocus.Military)));
            simulation.Advance(TestWorlds.Years(years));
            return TestWorlds.Society(simulation).Outlets.Wrath - start;
        }
    }
}
