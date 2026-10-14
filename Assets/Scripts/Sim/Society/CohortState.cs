namespace Statecraft.Sim.Society
{
    /// <summary>
    /// One generation band of a nation: its share of the population, the drives it carries
    /// (disposition) and the outlets it was trained to use (channeling). Culture moves between
    /// cohorts as people age, which is why schooling pays off a generation late.
    /// </summary>
    public sealed class CohortState
    {
        internal double share;
        internal ViceVector disposition;
        internal ViceVector channeling;

        internal CohortState(Cohort cohort, double share, ViceVector disposition)
        {
            Cohort = cohort;
            this.share = share;
            this.disposition = disposition;
        }

        public Cohort Cohort { get; }

        /// <summary>Fraction of the population in this cohort (fixed in model v0).</summary>
        public double Share => share;

        /// <summary>How strongly this cohort carries each vice, 0 to 1 (deep culture, slow to change).</summary>
        public ViceVector Disposition => disposition;

        /// <summary>Trained habits and skills that give each vice a productive outlet, 0 to 1.</summary>
        public ViceVector Channeling => channeling;
    }
}
