using Statecraft.Sim.Society;

namespace Statecraft.Sim.Tuning
{
    /// <summary>Coefficients for government types, alignment, two-tier suspicion, revolutions, votes and revolts.</summary>
    public sealed class PoliticsTuning
    {
        /// <summary>Traits per government type, in GovernmentType order: Democracy, Monarchy, Communism, MilitaryJunta, Theocracy.</summary>
        public GovernmentTraits[] Types =
        {
            new GovernmentTraits // Democracy
            {
                OutletBonus = new ViceVector(0, 0, 0, 0, 0.05, 0.05, 0),
                AlignmentBase = 0.10,
                AlignmentFromVices = new ViceVector(-0.1, 0, 0, 0, 0.3, 0.3, 0),
                AlignmentFromSchooling = new[] { 0, 0, 0, 0.2, 0.3, 0.3, 0 },
                UnrestVenting = 0.04, FearSufferingScale = 1.3, FearDampeningScale = 0.7, SeveritySuspicion = 0.15,
                ScienceSuspicionScale = 0.5, LeverSwingSuspicionScale = 1.5, RevoltChanceScale = 0.7,
                AggressionBonus = -0.05, CanVote = true,
            },
            new GovernmentTraits // Monarchy
            {
                OutletBonus = new ViceVector(0, 0, 0, 0, 0.08, 0, 0),
                AlignmentBase = 0.10,
                AlignmentFromVices = new ViceVector(0, 0.4, 0, 0, 0.4, 0, 0),
                AlignmentFromSchooling = new[] { 0.2, 0.1, 0, 0, 0, 0, 0 },
                SeveritySuspicion = 0.05, RevoltChanceScale = 0.9,
            },
            new GovernmentTraits // Communism
            {
                OutletBonus = new ViceVector(0, 0, -0.10, 0, 0, 0.12, 0),
                AlignmentBase = 0.05,
                AlignmentFromVices = new ViceVector(0, 0, -0.3, 0, 0, 0.7, 0),
                AlignmentFromSchooling = new[] { 0.2, 0, 0, 0.2, 0, 0, 0 },
                FearSufferingScale = 0.75, FearDampeningScale = 1.2, SeveritySuspicion = 0.02,
                ScienceSuspicionScale = 1.5, RevoltChanceScale = 1.2,
            },
            new GovernmentTraits // MilitaryJunta
            {
                OutletBonus = new ViceVector(0.10, 0, 0, 0, 0.05, 0, 0),
                AlignmentBase = 0.05,
                AlignmentFromVices = new ViceVector(0.6, 0, 0, 0, 0.4, 0, 0),
                AlignmentFromSchooling = new[] { 0, 0.5, 0, 0, 0, 0, 0 },
                FearSufferingScale = 0.5, FearDampeningScale = 1.5, LeverSwingSuspicionScale = 0.7,
                RevoltChanceScale = 1.5, AggressionBonus = 0.10,
            },
            new GovernmentTraits // Theocracy
            {
                OutletBonus = new ViceVector(0, 0.08, 0, -0.08, 0, 0, -0.08),
                AlignmentBase = 0.05,
                AlignmentFromVices = new ViceVector(0, 0.5, 0, -0.2, 0, 0, -0.2),
                AlignmentFromSchooling = new[] { 0, 0, 0, 0, -0.4, 0, 0.1 },
                UnrestVenting = 0.04, FearSufferingScale = 0.8, SeveritySuspicion = 0.05,
                ScienceSuspicionScale = 2.0, RevoltChanceScale = 0.9, AggressionBonus = 0.03,
            },
        };

        /// <summary>Lowest chance any revolution, vote or revolt can have: nothing in politics is certain.</summary>
        public double ChanceFloor = 0.05;

        /// <summary>Highest chance any revolution, vote or revolt can have.</summary>
        public double ChanceCeiling = 0.95;

        /// <summary>Alignment bonus for the type already in power (habit), applied when people choose.</summary>
        public double InertiaBonus = 0.05;

        /// <summary>Government suspicion per unit of the adult generation's Science schooling share.</summary>
        public double ScienceSuspicion = 0.6;

        /// <summary>Government suspicion per unit of expressed Pride (people who question authority).</summary>
        public double PrideSuspicion = 0.15;

        /// <summary>Government suspicion per unit of frustrated Envy or Greed (the system visibly blocking ambition).</summary>
        public double BlockedAmbitionSuspicion = 0.5;

        /// <summary>Government suspicion per unit of cultural drift: the sum over vices of disposition distance from tradition beyond the tolerance.</summary>
        public double DriftSuspicion = 1.0;

        /// <summary>Per-vice distance from tradition that nobody notices.</summary>
        public double DriftTolerance = 0.1;

        /// <summary>Time constant in months for tradition to accept a lasting change of culture as the new normal.</summary>
        public double TraditionTauMonths = 600;

        /// <summary>Government suspicion per unit of the largest curriculum or vice airtime share beyond its start point.</summary>
        public double ConcentrationSuspicion = 0.8;

        /// <summary>Largest curriculum share that raises no suspicion.</summary>
        public double CurriculumConcentrationStart = 0.4;

        /// <summary>Largest single-vice airtime share that raises no suspicion.</summary>
        public double AirtimeConcentrationStart = 0.35;

        /// <summary>Government suspicion per unit of recent lever shock.</summary>
        public double LeverShockSuspicion = 0.5;

        /// <summary>Time constant in months for lever shocks to fade.</summary>
        public double LeverShockTauMonths = 12;

        /// <summary>Government suspicion per unit of suffering (negative contentment).</summary>
        public double SufferingSuspicion = 0.4;

        /// <summary>Suspicion removed per unit of expressed Sloth (acceptance).</summary>
        public double SlothDamping = 0.15;

        /// <summary>Suspicion removed per unit of satisfied Gluttony (bread and circuses).</summary>
        public double GluttonyDamping = 0.2;

        /// <summary>Suspicion removed per unit of neutral airtime share.</summary>
        public double NeutralAirtimeDamping = 0.2;

        /// <summary>Time constant in months for government suspicion to follow its target.</summary>
        public double SuspicionTauMonths = 12;

        /// <summary>Government suspicion above which it leaks into shadow suspicion.</summary>
        public double LeakThreshold = 0.5;

        /// <summary>Monthly shadow suspicion per unit of government suspicion above the threshold.</summary>
        public double LeakRate = 0.02;

        /// <summary>Time constant in months for shadow suspicion to fade with nothing feeding it. Long: people remember.</summary>
        public double ShadowDecayTauMonths = 600;

        /// <summary>Shadow suspicion added per recent government change when another happens ("new masters, same strings").</summary>
        public double ChangeLeak = 0.03;

        /// <summary>Shadow suspicion at which revolts aim at the hidden hand itself instead of the government.</summary>
        public double ShadowBreakingPoint = 0.7;

        /// <summary>Unrest per unit of government suspicion.</summary>
        public double UnrestFromSuspicion = 0.5;

        /// <summary>Unrest per unit of shadow suspicion: a people that believes in a hidden hand grows restless.</summary>
        public double UnrestFromShadowSuspicion = 0.4;

        /// <summary>Government suspicion above which a SuspicionRising event fires.</summary>
        public double SuspicionAlarm = 0.5;

        /// <summary>Government suspicion below which the alarm clears.</summary>
        public double SuspicionCalm = 0.4;

        /// <summary>Shadow suspicion above which a ShadowRumors event fires.</summary>
        public double ShadowAlarm = 0.3;

        /// <summary>Shadow suspicion below which the rumors fade.</summary>
        public double ShadowCalm = 0.2;

        /// <summary>Share of government suspicion a successful revolution vents (before diminishing).</summary>
        public double RevolutionVent = 0.7;

        /// <summary>Share of government suspicion a passed vote vents (before diminishing).</summary>
        public double VoteVent = 0.4;

        /// <summary>Venting multiplier per recent change: each change vents this fraction of the one before.</summary>
        public double VentDiminishing = 0.6;

        /// <summary>Time constant in months for recent changes to be forgotten.</summary>
        public double RecentChangeTauMonths = 120;

        /// <summary>Months after any revolution, vote or revolt before another can start.</summary>
        public int ChangeCooldownMonths = 12;

        /// <summary>Months a fomented revolution takes.</summary>
        public int FomentMonths = 6;

        /// <summary>Unrest added to every generation's target while a revolution is fomented.</summary>
        public double FomentUnrest = 0.5;

        /// <summary>Success chance of a fomented revolution with no unrest and equal alignment.</summary>
        public double FomentBaseSuccess = 0.35;

        /// <summary>Success chance added per unit of national unrest ("needs some unrest to work with").</summary>
        public double FomentUnrestSuccess = 1.5;

        /// <summary>Success chance added per unit of alignment the target has over the current type.</summary>
        public double FomentAlignmentSuccess = 1.0;

        /// <summary>Success chance removed per unit of fear.</summary>
        public double FomentFearPenalty = 0.3;

        /// <summary>Government suspicion added by a failed plot.</summary>
        public double FailedPlotSuspicion = 0.08;

        /// <summary>Shadow suspicion added by a failed plot (it leaves traces).</summary>
        public double FailedPlotShadow = 0.02;

        /// <summary>Violence of a fomented revolution, on the same scale as RevoltViolence.</summary>
        public double FomentViolence = 0.6;

        /// <summary>Months a vote takes.</summary>
        public int VoteMonths = 3;

        /// <summary>Pass chance added per unit of alignment the target has over the current type (from 50%).</summary>
        public double VoteSensitivity = 4;

        /// <summary>National unrest at which the people may call a vote on their own.</summary>
        public double PeopleVoteUnrest = 0.25;

        /// <summary>Alignment lead another type needs before the people call a vote for it.</summary>
        public double PeopleVoteMargin = 0.05;

        /// <summary>Monthly chance the people call a vote once the conditions hold.</summary>
        public double PeopleVoteChance = 0.03;

        /// <summary>A generation's unrest at which it can revolt.</summary>
        public double RevoltThreshold = 0.3;

        /// <summary>Monthly revolt chance per unit of a generation's unrest above the threshold.</summary>
        public double RevoltChancePerUnrest = 0.15;

        /// <summary>Revolt frequency multiplier by generation: youth, adults, elders (D-0032).</summary>
        public double[] RevoltFrequency = { 1.5, 1.0, 0.4 };

        /// <summary>Base revolt success chance by generation: youth, adults, elders.</summary>
        public double[] RevoltSuccess = { 0.45, 0.7, 0.35 };

        /// <summary>Revolt violence by generation: youth, adults, elders (elder revolts are mostly bloodless).</summary>
        public double[] RevoltViolence = { 1.0, 0.6, 0.2 };

        /// <summary>Success chance added per unit of the revolting generation's unrest above the threshold.</summary>
        public double RevoltUnrestSuccess = 0.5;

        /// <summary>Success chance removed per unit of fear.</summary>
        public double RevoltFearPenalty = 0.4;

        /// <summary>Weight of the revolting generation's alignment when the new type is chosen (others weigh 1).</summary>
        public double RevoltingCohortWeight = 2;

        /// <summary>Alignment bonus that elder revolts give the previous type, Monarchy and Theocracy (restoration).</summary>
        public double ElderRestorationBonus = 0.1;

        /// <summary>Contentment lost by every generation per unit of violence.</summary>
        public double ViolenceContentmentShock = 0.2;

        /// <summary>Crime added per unit of violence.</summary>
        public double ViolenceCrimeShock = 0.1;

        /// <summary>Share of every generation killed per unit of violence (D-0036: revolutions cost lives).</summary>
        public double ViolenceMortality = 0.01;

        /// <summary>Share of the revolting generation's unrest released by a successful revolt.</summary>
        public double RevoltUnrestRelief = 0.7;

        /// <summary>Contentment lost by a generation whose revolt is crushed.</summary>
        public double CrushedContentmentShock = 0.1;

        /// <summary>Government suspicion added when a revolt is crushed.</summary>
        public double CrushedSuspicion = 0.05;
    }
}
