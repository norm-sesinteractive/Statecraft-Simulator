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
    }
}
