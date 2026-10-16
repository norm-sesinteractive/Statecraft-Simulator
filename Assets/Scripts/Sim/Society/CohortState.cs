using Statecraft.Sim.Institutions;

namespace Statecraft.Sim.Society
{
    /// <summary>
    /// One generation band of a nation: its share of the population, the drives it carries (disposition),
    /// the outlets it was trained to use (channeling), what it was taught (schooling), and its own
    /// contentment and unrest (D-0032). Culture moves between cohorts as people age, which is why
    /// schooling pays off a generation late.
    /// </summary>
    public sealed class CohortState
    {
        internal double share;
        internal double population;
        internal ViceVector disposition;
        internal ViceVector channeling;
        internal readonly double[] schooling = new double[Curriculum.FocusCount];
        internal ViceVector expressed;
        internal ViceVector frustration;
        internal double contentment;
        internal double contentmentTarget;
        internal double unrest;
        internal double unrestTarget;

        internal CohortState(Cohort cohort, double share, double population, ViceVector disposition)
        {
            Cohort = cohort;
            this.share = share;
            this.population = population;
            this.disposition = disposition;
        }

        public Cohort Cohort { get; }

        /// <summary>Fraction of the nation's people in this cohort; follows births, aging and deaths (D-0036).</summary>
        public double Share => share;

        /// <summary>People in this cohort.</summary>
        public double Population => population;

        /// <summary>How strongly this cohort carries each vice, 0 to 1 (deep culture, slow to change).</summary>
        public ViceVector Disposition => disposition;

        /// <summary>Trained habits and skills that give each vice a productive outlet, 0 to 1.</summary>
        public ViceVector Channeling => channeling;

        /// <summary>Share of this cohort's education in a curriculum focus; shares sum to 1.</summary>
        public double Schooling(CurriculumFocus focus) => schooling[(int)focus];

        /// <summary>This cohort's disposition plus the national mood, 0 to 1.</summary>
        public ViceVector Expressed => expressed;

        /// <summary>This cohort's expressed vice beyond the nation's outlets, 0 to 1.</summary>
        public ViceVector Frustration => frustration;

        /// <summary>This generation's contentment, -1 (suffering) to +1.</summary>
        public double Contentment => contentment;

        public double ContentmentTarget => contentmentTarget;

        /// <summary>This generation's unrest, 0 to 1. Any generation can revolt (D-0032).</summary>
        public double Unrest => unrest;

        public double UnrestTarget => unrestTarget;
    }
}
