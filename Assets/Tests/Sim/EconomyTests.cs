using System;
using System.Collections.Generic;
using NUnit.Framework;
using Statecraft.Sim.Commands;
using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Economy;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Nations;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Tests
{
    /// <summary>
    /// The economy and population (D-0036): land, labor, food, goods, wealth, births, deaths and starvation. Each test
    /// states a design behavior; if one breaks, the design changed and Docs/simulation/MODEL.md must say so.
    /// </summary>
    public sealed class EconomyTests
    {
        static EconomyState Economy(Simulation simulation, int nation = 0) => simulation.World.Nations[nation].Economy;

        [Test]
        public void AnUntouchedNationNeitherGrowsNorShrinks()
        {
            var simulation = TestWorlds.Single();
            simulation.Advance(TestWorlds.Years(100));
            Assert.That(TestWorlds.Society(simulation).Population, Is.EqualTo(1000000).Within(3).Percent);
            Assert.That(Economy(simulation).Hunger, Is.EqualTo(0));
            Assert.That(Economy(simulation).MaterialsShortage, Is.EqualTo(0));
        }

        [Test]
        public void ALustfulPeopleHasMoreChildren()
        {
            var plain = TestWorlds.Single();
            var lustful = TestWorlds.Single(s => s.Disposition = With(Vice.Lust, 0.65));
            plain.Advance(TestWorlds.Years(30));
            lustful.Advance(TestWorlds.Years(30));
            Assert.That(Economy(lustful).BirthsPerYear, Is.GreaterThan(Economy(plain).BirthsPerYear));
            Assert.That(TestWorlds.Society(lustful).Population, Is.GreaterThan(TestWorlds.Society(plain).Population * 1.05));
        }

        [Test]
        public void MedicineSchoolingLengthensLives()
        {
            var plain = TestWorlds.Single();
            var healers = TestWorlds.Single(s => s.CurriculumWeights = TestWorlds.OnlyFocus(CurriculumFocus.Medicine));
            plain.Advance(TestWorlds.Years(10));
            healers.Advance(TestWorlds.Years(10));
            double plainRate = Economy(plain).DeathsPerYear / TestWorlds.Society(plain).Population;
            double healerRate = Economy(healers).DeathsPerYear / TestWorlds.Society(healers).Population;
            Assert.That(healerRate, Is.LessThan(plainRate * 0.8));
            Assert.That(TestWorlds.Society(healers).Population, Is.GreaterThan(TestWorlds.Society(plain).Population));
        }

        [Test]
        public void FarmersFeedMoreFromTheSameLandAndFreeHandsForComforts()
        {
            var plain = TestWorlds.Single();
            var farmers = TestWorlds.Single(s => s.CurriculumWeights = TestWorlds.OnlyFocus(CurriculumFocus.Agriculture));
            plain.Advance(1);
            farmers.Advance(1);
            Assert.That(Economy(farmers).Potential(Resource.Food), Is.GreaterThan(Economy(plain).Potential(Resource.Food) * 1.5));
            Assert.That(Economy(farmers).Workers(Resource.Food), Is.LessThan(Economy(plain).Workers(Resource.Food)));
            Assert.That(Economy(farmers).LandPressure, Is.LessThan(Economy(plain).LandPressure));
        }

        [Test]
        public void GluttonyRaisesWhatThePeopleEat()
        {
            var plain = TestWorlds.Single();
            var gluttons = TestWorlds.Single(s => s.Disposition = With(Vice.Gluttony, 0.8));
            plain.Advance(1);
            gluttons.Advance(1);
            double plainPerHead = Economy(plain).Need(Resource.Food) / TestWorlds.Society(plain).Population;
            double gluttonPerHead = Economy(gluttons).Need(Resource.Food) / TestWorlds.Society(gluttons).Population;
            Assert.That(gluttonPerHead, Is.GreaterThan(plainPerHead * 1.2));
        }

        [Test]
        public void ASlothfulPeopleAchievesLess()
        {
            var plain = TestWorlds.Single();
            var idle = TestWorlds.Single(s => s.Disposition = With(Vice.Sloth, 0.8));
            plain.Advance(1);
            idle.Advance(1);
            Assert.That(Economy(idle).Productivity, Is.LessThan(Economy(plain).Productivity - 0.2));
            Assert.That(Economy(idle).LuxuriesPerHead, Is.LessThan(Economy(plain).LuxuriesPerHead));
        }

        [Test]
        public void AProudAndWrathfulPeopleNeglectsItsElders()
        {
            var plain = TestWorlds.Single();
            var proud = TestWorlds.Single(s =>
            {
                var d = With(Vice.Pride, 0.8);
                d.Wrath = 0.8;
                s.Disposition = d;
            }, t => t.Politics.RevoltThreshold = 2);
            plain.Advance(TestWorlds.Years(20));
            proud.Advance(TestWorlds.Years(20));
            Assert.That(TestWorlds.Society(proud).GetCohort(Cohort.Elder).Share,
                Is.LessThan(TestWorlds.Society(plain).GetCohort(Cohort.Elder).Share * 0.9));
        }

        [Test]
        public void AFillingLandSlowsItsBirthsBeforeItStarves()
        {
            // Twice the people on the same land: crowded, but still fed.
            var crowded = TestWorlds.Single(s => s.Population = 1650000);
            crowded.Advance(1);
            Assert.That(Economy(crowded).LandPressure, Is.GreaterThan(0.8));
            Assert.That(Economy(crowded).Hunger, Is.EqualTo(0));
            Assert.That(Term(crowded, Quantity.Births, -1, LedgerNames.Crowding), Is.LessThan(0));
        }

        [Test]
        public void SpareHandsWorkMoreFieldsInAPoorYear()
        {
            var normal = TestWorlds.Single();
            var poor = TestWorlds.Single(tune: t => PinHarvest(t, 0.6));
            normal.Advance(TestWorlds.Years(2));
            poor.Advance(TestWorlds.Years(2));
            Assert.That(Economy(poor).Workers(Resource.Food), Is.GreaterThan(Economy(normal).Workers(Resource.Food) * 1.2));
            Assert.That(Economy(poor).Hunger, Is.EqualTo(0));
            Assert.That(Economy(poor).LuxuriesPerHead, Is.LessThan(Economy(normal).LuxuriesPerHead));
        }

        [Test]
        public void StoresCarryANationThroughBadHarvestsForAWhile()
        {
            // Harvests at 40%: more than the land can make up, whoever works it.
            var simulation = TestWorlds.Single(tune: t => PinHarvest(t, 0.4));
            simulation.Advance(TestWorlds.Years(1));
            Assert.That(Economy(simulation).Hunger, Is.EqualTo(0), "the stores cover the first year");
            Assert.That(Economy(simulation).StoreMonths, Is.LessThan(4));
            simulation.Advance(TestWorlds.Years(3));
            Assert.That(Economy(simulation).Hunger, Is.GreaterThan(0), "years of failed harvests empty them");
        }

        [Test]
        public void HungerBreedsSufferingAndUnrest()
        {
            // No stores, so the shortfall bites at once; no revolts, so politics does not blur the comparison.
            Action<SimTuning> noStores = t =>
            {
                t.Politics.RevoltThreshold = 2;
                t.Economy.StoreMonths = 0;
            };
            var fed = TestWorlds.Single(tune: noStores);
            var hungry = TestWorlds.Single(s => s.Population = 2200000, noStores);
            fed.Advance(TestWorlds.Years(1));
            hungry.Advance(TestWorlds.Years(1));
            Assert.That(Economy(hungry).Hunger, Is.GreaterThan(0));
            Assert.That(TestWorlds.Society(hungry).Contentment, Is.LessThan(TestWorlds.Society(fed).Contentment - 0.1));
            Assert.That(TestWorlds.Society(hungry).Unrest, Is.GreaterThan(TestWorlds.Society(fed).Unrest));
        }

        [Test]
        public void ALandThatCannotFeedItsPeopleStarves()
        {
            // Three times the people the land can feed: famine kills until the nation is lost (D-0019).
            var simulation = TestWorlds.Single(s => s.Population = 5000000, t => t.Politics.RevoltThreshold = 2);
            simulation.Advance(TestWorlds.Years(10));
            Assert.That(HasEvent(simulation, SimEventKind.FamineBegan), Is.True);
            Assert.That(Economy(simulation).Starved, Is.True);
            Assert.That(simulation.World.Nations[0].PlayerControlled, Is.False);
            Assert.That(simulation.IsGameOver, Is.True);
            Assert.That(simulation.Defeat, Is.EqualTo(Defeat.Starvation));
        }

        [Test]
        public void AHungryYearIsNotAStarvedNation()
        {
            var simulation = TestWorlds.Single(tune: t => PinHarvest(t, 0.6));
            simulation.Advance(TestWorlds.Years(3));
            Assert.That(Economy(simulation).Starved, Is.False);
            Assert.That(simulation.IsGameOver, Is.False);
        }

        [Test]
        public void ComfortRisesWithLuxuriesThenFadesAsPeopleGetUsedToThem()
        {
            var simulation = TestWorlds.Single();
            simulation.Enqueue(new SetMediaMix(0, TestWorlds.AllAirtimeOn(Vice.Greed)));
            simulation.Advance(TestWorlds.Years(2));
            double early = Economy(simulation).Comfort;
            Assert.That(early, Is.GreaterThan(0.05));
            simulation.Advance(TestWorlds.Years(30));
            Assert.That(Economy(simulation).Comfort, Is.LessThan(early));
        }

        [Test]
        public void CommerceMakesANationRicher()
        {
            var plain = TestWorlds.Single();
            //                                                       Agri Mil Med Infra Sci Comm Arts
            var merchants = TestWorlds.Single(s => s.CurriculumWeights = new double[] { 1, 1, 1, 1, 1, 3, 1 });
            plain.Advance(TestWorlds.Years(20));
            merchants.Advance(TestWorlds.Years(20));
            Assert.That(Economy(merchants).Wealth, Is.GreaterThan(Economy(plain).Wealth * 1.15));
        }

        [Test]
        public void RevolutionsCostLives()
        {
            var calm = TestWorlds.Single();
            var plotting = TestWorlds.Single();
            plotting.Enqueue(new FomentRevolution(0, GovernmentType.Monarchy));
            calm.Advance(TestWorlds.Years(2));
            plotting.Advance(TestWorlds.Years(2));
            Assert.That(TestWorlds.Society(plotting).Population, Is.LessThan(TestWorlds.Society(calm).Population));
        }

        [Test]
        public void BirthTermsSumToTheBirthRate()
        {
            var simulation = TestWorlds.Single(s => s.Disposition = With(Vice.Lust, 0.6));
            var society = TestWorlds.Society(simulation);
            double adults = society.GetCohort(Cohort.Adult).Population;
            simulation.Advance(1);
            var terms = new List<Contribution>();
            simulation.Ledger.Collect(0, Quantity.Births, -1, terms);
            double sum = 0;
            foreach (var term in terms) sum += term.Amount;
            Assert.That(Economy(simulation).BirthsPerYear, Is.EqualTo(adults * sum).Within(1e-6).Percent);
        }

        // Every year's harvest at one multiplier; revolts off so politics does not blur the economy.
        static void PinHarvest(SimTuning tuning, double harvest)
        {
            tuning.Economy.HarvestNoise = 0.1;
            tuning.Economy.HarvestFloor = harvest;
            tuning.Economy.HarvestCeiling = harvest;
            tuning.Politics.RevoltThreshold = 2;
        }

        static ViceVector With(Vice vice, double value)
        {
            var d = new NationSetup().Disposition;
            d[vice] = value;
            return d;
        }

        static double Term(Simulation simulation, Quantity quantity, int index, string source)
        {
            var terms = new List<Contribution>();
            simulation.Ledger.Collect(0, quantity, index, terms);
            double sum = 0;
            foreach (var term in terms)
                if (term.Source == source) sum += term.Amount;
            return sum;
        }

        static bool HasEvent(Simulation simulation, SimEventKind kind)
        {
            for (int i = 0; i < simulation.Events.Count; i++)
                if (simulation.Events.GetRecent(i).Kind == kind) return true;
            return false;
        }
    }
}
