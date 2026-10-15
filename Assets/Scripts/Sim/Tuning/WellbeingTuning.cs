using Statecraft.Sim.Society;

namespace Statecraft.Sim.Tuning
{
    /// <summary>Coefficients for contentment, fear, crime, unrest, aggression and their alarm thresholds.</summary>
    public sealed class WellbeingTuning
    {
        /// <summary>Contentment of a society with no satisfaction, frustration, fear or crime.</summary>
        public double ContentmentBase = 0.15;

        /// <summary>Contentment per unit of mean satisfied vice (expressed and channeled).</summary>
        public double SatisfactionGain = 0.5;

        /// <summary>
        /// Suffering per unit of frustration, summed over vices (not averaged): one badly frustrated drive hurts on its
        /// own rather than being diluted by six satisfied ones.
        /// </summary>
        public double FrustrationGain = 0.5;

        /// <summary>How much expressed Sloth softens frustration into acceptance (0 = none, 1 = Sloth 1.0 cancels it).</summary>
        public double SlothAcceptance = 0.5;

        /// <summary>Fear created per unit of court severity.</summary>
        public double FearPerSeverity = 0.6;

        /// <summary>Suffering per unit of fear.</summary>
        public double FearSuffering = 0.3;

        /// <summary>Suffering per unit of crime.</summary>
        public double CrimeSuffering = 0.4;

        /// <summary>Time constant in months for contentment to follow its target.</summary>
        public double ContentmentTauMonths = 6;

        /// <summary>Crime present in any society regardless of frustration.</summary>
        public double CrimeBase = 0.12;

        /// <summary>Crime per unit of frustration, per vice.</summary>
        public ViceVector CrimeFromFrustration = new ViceVector(0.35, 0.05, 0.35, 0.10, 0.10, 0.30, 0.10);

        /// <summary>Crime prevented per unit of court severity (deterrence).</summary>
        public double Deterrence = 0.12;

        /// <summary>
        /// Crime prevented per unit of positive contentment. Larger than CrimeBase on purpose, so a
        /// sufficiently content people can reach zero crime ("so happy that crime no longer takes place").
        /// </summary>
        public double ContentmentCrimeRelief = 0.15;

        /// <summary>Time constant in months for crime to follow its target.</summary>
        public double CrimeTauMonths = 6;

        /// <summary>Unrest per unit of suffering (negative contentment).</summary>
        public double UnrestFromSuffering = 1.2;

        /// <summary>Extra unrest from suffering per unit of expressed Pride (proud people defy).</summary>
        public double PrideAmplifier = 0.5;

        /// <summary>Extra unrest from suffering per unit of expressed Envy (relative deprivation).</summary>
        public double EnvyAmplifier = 0.5;

        /// <summary>Unrest per unit of Wrath frustration.</summary>
        public double UnrestFromWrath = 0.3;

        /// <summary>Unrest removed per unit of expressed Sloth (complacency, acceptance).</summary>
        public double SlothDampening = 0.10;

        /// <summary>Unrest removed per unit of fear (suppression, short-term).</summary>
        public double FearDampening = 0.2;

        /// <summary>Time constant in months for unrest to follow its target.</summary>
        public double UnrestTauMonths = 12;

        /// <summary>Aggression per unit of expressed vice. Negative weights calm (Sloth).</summary>
        public ViceVector AggressionWeights = new ViceVector(0.45, -0.15, 0.20, 0.0, 0.15, 0.10, 0.05);

        /// <summary>Aggression per unit of Envy times Wrath ("taking what others have if aggression is high").</summary>
        public double EnvyWrathAggression = 0.4;

        /// <summary>Unrest above which an UnrestRising event fires.</summary>
        public double UnrestAlarm = 0.7;

        /// <summary>Unrest below which an alarmed nation is considered calm again.</summary>
        public double UnrestCalm = 0.6;

        /// <summary>Contentment below which a SufferingSevere event fires.</summary>
        public double SufferingAlarm = -0.5;

        /// <summary>Contentment above which severe suffering is considered eased.</summary>
        public double SufferingCalm = -0.4;

        /// <summary>Crime above which a CrimeWave event fires.</summary>
        public double CrimeAlarm = 0.5;

        /// <summary>Crime below which a crime wave is considered over.</summary>
        public double CrimeCalm = 0.4;
    }
}
