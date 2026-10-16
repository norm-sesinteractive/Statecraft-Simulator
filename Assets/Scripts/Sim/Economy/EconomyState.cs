using System;
using Statecraft.Sim.Nations;

namespace Statecraft.Sim.Economy
{
    /// <summary>
    /// A nation's land and livelihood (D-0036): the area of each terrain, what the land and the workforce produce,
    /// what the people need, food stores and hunger, comfort, wealth and famine. Population numbers live on the
    /// cohorts. Systems write the internal fields; everything outside the sim reads the public properties.
    /// </summary>
    public sealed class EconomyState
    {
        public const int TerrainCount = 8;
        public const int ResourceCount = 3;

        internal readonly double[] area = new double[TerrainCount];
        internal readonly double[] potential = new double[ResourceCount];
        internal readonly double[] output = new double[ResourceCount];
        internal readonly double[] need = new double[ResourceCount];
        internal readonly double[] workers = new double[ResourceCount];

        internal double workforce;
        internal double productivity = 1;
        internal double harvest = 1;
        internal double foodStore;
        internal double hunger;
        internal double landPressure;
        internal double materialsShortage;
        internal double luxuriesPerHead;
        internal double luxuryExpectation;
        internal double wealth;
        internal double wealthIncome;
        internal double birthsPerYear;
        internal double deathsPerYear;
        internal double famineDeathsThisMonth;

        internal bool famine;
        internal double famineStartPopulation;
        internal double famineDeaths;
        internal bool starved;

        internal EconomyState(NationSetup setup)
        {
            var areas = setup.LandAreas ?? Array.Empty<double>();
            for (int t = 0; t < TerrainCount && t < areas.Length; t++) area[t] = Math.Max(0, areas[t]);
        }

        /// <summary>Area of one terrain the nation holds (abstract area units; the map will supply them).</summary>
        public double Area(Terrain terrain) => area[(int)terrain];

        /// <summary>
        /// The most the land could yield of a resource this month with unlimited labor, after skill and drive.
        /// </summary>
        public double Potential(Resource resource) => potential[(int)resource];

        /// <summary>What was produced this month (food after the year's harvest).</summary>
        public double Output(Resource resource) => output[(int)resource];

        /// <summary>What the people need this month (luxuries are wanted, never needed: 0).</summary>
        public double Need(Resource resource) => need[(int)resource];

        /// <summary>Workers on each resource this month, in effective (productivity-weighted) workers.</summary>
        public double Workers(Resource resource) => workers[(int)resource];

        /// <summary>Adults plus working elders, before productivity.</summary>
        public double Workforce => workforce;

        /// <summary>Labor multiplier: Sloth, unrest and hunger drag it down.</summary>
        public double Productivity => productivity;

        /// <summary>This year's harvest multiplier on food (weather), about 1.</summary>
        public double Harvest => harvest;

        /// <summary>Food kept in stores, in units of one person's monthly need.</summary>
        public double FoodStore => foodStore;

        /// <summary>Months of food need the stores would cover.</summary>
        public double StoreMonths => need[(int)Resource.Food] > 0 ? foodStore / need[(int)Resource.Food] : 0;

        /// <summary>Share of food need that went uneaten this month, 0 (fed) to 1 (nothing to eat).</summary>
        public double Hunger => hunger;

        /// <summary>Food need over what the land could yield with unlimited labor: above 1 the land cannot feed its people.</summary>
        public double LandPressure => landPressure;

        /// <summary>Share of materials need unmet this month, 0 to 1.</summary>
        public double MaterialsShortage => materialsShortage;

        /// <summary>Luxuries per person this month.</summary>
        public double LuxuriesPerHead => luxuriesPerHead;

        /// <summary>The luxuries per person the people have come to expect.</summary>
        public double LuxuryExpectation => luxuryExpectation;

        /// <summary>Luxuries against expectation, -1 to +1: 0 when people have what they are used to.</summary>
        public double Comfort => luxuryExpectation > 0 ? Math.Max(-1, Math.Min(1, luxuriesPerHead / luxuryExpectation - 1)) : 0;

        /// <summary>The nation's accumulated riches, in food units. Trade and war will spend it (M3).</summary>
        public double Wealth => wealth;

        /// <summary>Wealth earned this month before upkeep.</summary>
        public double WealthIncome => wealthIncome;

        /// <summary>Births this month, as a yearly rate.</summary>
        public double BirthsPerYear => birthsPerYear;

        /// <summary>Deaths this month, as a yearly rate.</summary>
        public double DeathsPerYear => deathsPerYear;

        /// <summary>Whether a famine is underway.</summary>
        public bool Famine => famine;

        /// <summary>Deaths the current famine has caused, as a share of the people alive when it began.</summary>
        public double FamineToll => famine && famineStartPopulation > 0 ? famineDeaths / famineStartPopulation : 0;

        /// <summary>True once a famine has killed enough of the people that the nation is lost (D-0019).</summary>
        public bool Starved => starved;
    }
}
