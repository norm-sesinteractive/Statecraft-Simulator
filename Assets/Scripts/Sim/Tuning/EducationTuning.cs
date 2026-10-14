using Statecraft.Sim.Society;

namespace Statecraft.Sim.Tuning
{
    /// <summary>Coefficients for how schooling trains the channels (outlets) of the next generation.</summary>
    public sealed class EducationTuning
    {
        /// <summary>Time constant in months for the youth cohort's trained channels to follow the curriculum.</summary>
        public double SchoolTauMonths = 72;

        /// <summary>
        /// Channels each curriculum focus trains, per vice. Rows follow CurriculumFocus order:
        /// Agriculture, Military, Medicine, Infrastructure, Science, Commerce, Arts. Provisional
        /// proposal awaiting Norm's review (Docs/design/INSTITUTIONS.md).
        /// </summary>
        public ViceVector[] FocusChanneling =
        {
            //               Wrath Sloth Greed Lust  Pride Envy  Gluttony
            new ViceVector(0.0, 0.4, 0.2, 0.0, 0.0, 0.0, 1.0), // Agriculture
            new ViceVector(1.0, 0.0, 0.0, 0.0, 0.5, 0.0, 0.0), // Military
            new ViceVector(0.0, 0.5, 0.0, 0.6, 0.0, 0.0, 0.0), // Medicine
            new ViceVector(0.0, 0.0, 0.5, 0.0, 0.3, 0.6, 0.0), // Infrastructure
            new ViceVector(0.0, 0.0, 0.0, 0.0, 1.0, 0.3, 0.0), // Science
            new ViceVector(0.0, 0.0, 1.0, 0.0, 0.0, 0.5, 0.3), // Commerce
            new ViceVector(0.0, 0.2, 0.0, 1.0, 0.5, 0.2, 0.0), // Arts
        };
    }
}
