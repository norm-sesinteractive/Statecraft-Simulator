using Statecraft.Sim.Society;

namespace Statecraft.Sim.Tuning
{
    /// <summary>Coefficients for aging: how culture and training pass from youth to adults to elders.</summary>
    public sealed class GenerationTuning
    {
        /// <summary>Years a person spends in the Youth cohort.</summary>
        public double YouthYears = 18;

        /// <summary>Years a person spends in the Adult cohort before becoming an Elder.</summary>
        public double AdultYears = 42;

        /// <summary>The disposition every culture drifts back toward without pressure ("human nature"), 0 to 1 per vice.</summary>
        public ViceVector HumanNature = ViceVector.Uniform(0.35);

        /// <summary>Time constant in months of the drift back toward human nature. Long: cultures are sticky.</summary>
        public double NatureTauMonths = 600;

        // Births and base death rates are the steady state of a 30/55/15 population with the cohort lengths above:
        // a nation at human nature, an even curriculum and enough food neither grows nor shrinks (D-0036).

        /// <summary>Births per adult per year, before modifiers.</summary>
        public double BirthsPerAdult = 0.0330303;

        /// <summary>Deaths per person per year in each cohort (Youth, Adult, Elder), before modifiers.</summary>
        public double[] Mortality = { 0.005, 0.0064935, 0.0873016 };

        /// <summary>Births multiplier per unit of adult expressed Lust above human nature.</summary>
        public double LustBirths = 1.0;

        /// <summary>Births lost per unit of adult expressed Pride above human nature (individualism).</summary>
        public double PrideBirths = 0.5;

        /// <summary>Births lost per unit of hunger.</summary>
        public double HungerBirths = 2.0;

        /// <summary>Land pressure (food need over what the land could yield) above which families have fewer children.</summary>
        public double CrowdingStart = 0.7;

        /// <summary>
        /// Births lost per unit of land pressure above <see cref="CrowdingStart"/>: as the land fills up, people marry
        /// later and have fewer children, so a nation usually slows before it starves.
        /// </summary>
        public double CrowdingBirths = 1.5;

        /// <summary>Land pressure below which families have more children (open land to settle).</summary>
        public double OpenLandBelow = 0.5;

        /// <summary>
        /// Births gained per unit of land pressure below <see cref="OpenLandBelow"/>: a dwindling people with room to
        /// grow recovers, so populations find a level instead of shrinking forever.
        /// </summary>
        public double OpenLandBirths = 1.5;

        /// <summary>Deaths prevented per unit of adult Medicine schooling above an even curriculum.</summary>
        public double MedicineLife = 1.0;

        /// <summary>Lowest death-rate multiplier medicine can reach.</summary>
        public double MinMortalityScale = 0.4;

        /// <summary>
        /// Extra elder deaths per unit of the nation's expressed Pride above human nature: a society that does not
        /// take care of its elders (Norm's example).
        /// </summary>
        public double ElderNeglect = 0.5;

        /// <summary>How much expressed Wrath above human nature worsens elder neglect (x per unit).</summary>
        public double NeglectWrath = 1.0;

        /// <summary>
        /// Extra deaths per month at total hunger, per cohort (Youth, Adult, Elder); scales with hunger squared, so a
        /// thin harvest kills few and a real famine many.
        /// </summary>
        public double[] FamineMortality = { 0.12, 0.06, 0.15 };
    }
}
