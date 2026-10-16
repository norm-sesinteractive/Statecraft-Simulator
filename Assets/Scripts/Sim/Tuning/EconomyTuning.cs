namespace Statecraft.Sim.Tuning
{
    /// <summary>
    /// Coefficients for land, work and goods (D-0036): what each terrain yields, how labor turns land into output,
    /// what people need, how stores, wealth and living standards move, and when hunger becomes famine.
    /// Amounts are in units of one person's monthly food need.
    /// </summary>
    public sealed class EconomyTuning
    {
        /// <summary>
        /// Output each area of a terrain can reach with unlimited labor, per month: [terrain][resource] in Terrain and
        /// Resource order (Food, Materials, Luxuries). Land is the ceiling; labor approaches it with diminishing returns.
        /// </summary>
        public double[][] TerrainYields =
        {
            //           Food   Materials Luxuries
            new double[] { 30000, 1000, 500 },   // Plains
            new double[] { 4000, 6000, 1000 },   // Forest
            new double[] { 8000, 6000, 1000 },   // Hills
            new double[] { 1000, 8000, 2000 },   // Mountains
            new double[] { 500, 1000, 3000 },    // Desert
            new double[] { 15000, 1000, 4000 },  // Coast
            new double[] { 45000, 1000, 2000 },  // RiverValley
            new double[] { 2000, 3000, 1000 },   // Tundra
        };

        /// <summary>Output per effective worker per month on uncrowded land, per resource (Food, Materials, Luxuries).</summary>
        public double[] WorkerOutput = { 6.0, 2.0, 1.0 };

        /// <summary>
        /// Share of labor that always makes luxuries, whatever the needs: the court, the rich and the festivals take
        /// theirs first, even in hard years.
        /// </summary>
        public double LuxuryLaborShare = 0.05;

        /// <summary>Workers per elder: elders still work, at this fraction of an adult.</summary>
        public double ElderLabor = 0.3;

        /// <summary>Food each person needs per month at human-nature Gluttony (1 by definition of the unit).</summary>
        public double FoodNeed = 1.0;

        /// <summary>Extra food needed per person per unit of expressed Gluttony above human nature.</summary>
        public double GluttonyAppetite = 0.6;

        /// <summary>Materials each person needs per month (shelter, tools, fuel).</summary>
        public double MaterialsNeed = 0.1;

        /// <summary>Workers aim this fraction above a need, so ordinary bad luck does not bite at once.</summary>
        public double PlanningMargin = 0.05;

        /// <summary>Food stores hold at most this many months of need; surplus beyond it is sold.</summary>
        public double StoreMonths = 6;

        /// <summary>Months over which farmers aim to refill empty stores.</summary>
        public double StoreRefillMonths = 24;

        /// <summary>Spread of the yearly harvest (standard deviation of its multiplier), drawn each January.</summary>
        public double HarvestNoise = 0.08;

        /// <summary>Lowest harvest multiplier a year can bring.</summary>
        public double HarvestFloor = 0.5;

        /// <summary>Highest harvest multiplier a year can bring.</summary>
        public double HarvestCeiling = 1.5;

        /// <summary>Output multiplier per unit of adult Agriculture schooling above an even curriculum (Food).</summary>
        public double AgricultureSkill = 1.0;

        /// <summary>Output multiplier per unit of adult Infrastructure schooling above an even curriculum (Materials).</summary>
        public double InfrastructureSkill = 0.8;

        /// <summary>Output multiplier per unit of adult Arts schooling above an even curriculum (Luxuries).</summary>
        public double ArtsSkill = 1.0;

        /// <summary>Output multiplier for every resource per unit of adult Science schooling above an even curriculum.</summary>
        public double ScienceSkill = 0.3;

        /// <summary>Materials and luxuries multiplier per unit of expressed Greed above human nature (working for gain).</summary>
        public double GreedDrive = 0.5;

        /// <summary>Labor lost per unit of expressed Sloth above human nature ("achieves less").</summary>
        public double SlothIdleness = 0.6;

        /// <summary>Labor lost per unit of unrest (strikes, riots, disorder).</summary>
        public double UnrestIdleness = 0.3;

        /// <summary>Labor lost per unit of hunger (weak, hungry workers).</summary>
        public double HungerWeakness = 0.5;

        /// <summary>Lowest labor productivity, whatever the drags.</summary>
        public double MinProductivity = 0.2;

        /// <summary>Wealth per unit sold or kept beyond need, per resource (Food, Materials, Luxuries).</summary>
        public double[] Prices = { 1.0, 2.0, 6.0 };

        /// <summary>Wealth multiplier per unit of adult Commerce schooling above an even curriculum.</summary>
        public double CommerceSkill = 1.5;

        /// <summary>Time constant in months of wealth wearing away (upkeep, spending): wealth settles near income x tau.</summary>
        public double WealthTauMonths = 120;

        /// <summary>
        /// Time constant in months of the people's expected luxuries per head following what they have. Comfort is
        /// luxuries against that expectation: people get used to their standard of living.
        /// </summary>
        public double ExpectationTauMonths = 120;

        /// <summary>Hunger at or above which a famine begins.</summary>
        public double FamineHunger = 0.15;

        /// <summary>Hunger at or below which a famine is over.</summary>
        public double FamineEnds = 0.05;

        /// <summary>
        /// A nation starves (is lost, D-0019) when one famine has killed this fraction of the people alive when it began.
        /// </summary>
        public double StarvationCollapse = 0.25;
    }
}
