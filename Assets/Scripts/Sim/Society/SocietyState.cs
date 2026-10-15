using System;
using Statecraft.Sim.Nations;

namespace Statecraft.Sim.Society
{
    /// <summary>
    /// The river itself: a nation's cohorts, its current mood, how much of each vice is expressed
    /// and channeled, and the resulting contentment, crime, unrest and aggression. Systems write
    /// the internal fields; everything outside the sim reads the public properties.
    /// </summary>
    public sealed class SocietyState
    {
        internal readonly CohortState[] cohorts;

        internal ViceVector mood;
        internal ViceVector disposition;
        internal ViceVector expressed;
        internal ViceVector outlets;
        internal ViceVector frustration;

        internal double contentment;
        internal double contentmentTarget;
        internal double crime;
        internal double crimeTarget;
        internal double unrest;
        internal double unrestTarget;
        internal double fear;
        internal double aggression;

        // Event hysteresis: true while a nation sits beyond an alarm threshold.
        internal bool unrestAlarm;
        internal bool sufferingAlarm;
        internal bool crimeAlarm;

        internal SocietyState(NationSetup setup)
        {
            double total = setup.YouthShare + setup.AdultShare + setup.ElderShare;
            if (!(total > 0)) throw new ArgumentException("Cohort shares must sum to a positive number.", nameof(setup));

            ViceVector start = setup.Disposition.Clamp01();
            cohorts = new[]
            {
                new CohortState(Cohort.Youth, setup.YouthShare / total, start),
                new CohortState(Cohort.Adult, setup.AdultShare / total, start),
                new CohortState(Cohort.Elder, setup.ElderShare / total, start),
            };
            disposition = start;
        }

        public CohortState GetCohort(Cohort cohort) => cohorts[(int)cohort];

        /// <summary>Short-lived stirring of each vice by media and events, about -1 to 1. Fades within months.</summary>
        public ViceVector Mood => mood;

        /// <summary>Population-weighted disposition of all cohorts, 0 to 1.</summary>
        public ViceVector Disposition => disposition;

        /// <summary>How much of each vice is acting on the nation now: disposition plus mood, 0 to 1.</summary>
        public ViceVector Expressed => expressed;

        /// <summary>How much of each vice the nation can channel productively, 0 to 1.</summary>
        public ViceVector Outlets => outlets;

        /// <summary>Expressed vice with nowhere to go (expressed beyond outlet), 0 to 1.</summary>
        public ViceVector Frustration => frustration;

        /// <summary>Contentment of the average citizen, -1 (suffering) to +1: the population-weighted average of the generations.</summary>
        public double Contentment => contentment;

        /// <summary>Population-weighted average of the generations' contentment targets.</summary>
        public double ContentmentTarget => contentmentTarget;

        /// <summary>Crime level, 0 to 1.</summary>
        public double Crime => crime;

        public double CrimeTarget => crimeTarget;

        /// <summary>Organized discontent, 0 to 1: the population-weighted average of the generations' unrest.</summary>
        public double Unrest => unrest;

        public double UnrestTarget => unrestTarget;

        /// <summary>Fear of punishment created by the courts, 0 to 1.</summary>
        public double Fear => fear;

        /// <summary>Appetite for war, 0 to 1. Display-only in model v0; the puppet government will read it.</summary>
        public double Aggression => aggression;
    }
}
