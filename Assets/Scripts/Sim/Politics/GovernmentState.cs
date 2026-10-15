namespace Statecraft.Sim.Politics
{
    /// <summary>
    /// A nation's political state: its government type (the hidden hand's shield, D-0021), how the people
    /// align with each type, the two tiers of suspicion (D-0020), and any revolution or vote under way.
    /// </summary>
    public sealed class GovernmentState
    {
        public const int TypeCount = 5;

        internal GovernmentType type;
        internal GovernmentType previousType;
        internal int monthsInPower;
        internal int cooldown;
        internal double recentChanges;

        internal double suspicion;
        internal double suspicionTarget;
        internal double shadowSuspicion;
        internal double shadowDrift;
        internal double leverShock;
        internal readonly double[] alignment = new double[TypeCount];

        internal PendingChange pending;
        internal GovernmentType pendingTarget;
        internal int pendingMonths;
        internal bool pendingByPlayer;
        internal bool pendingAnnounced;

        internal bool suspicionAlarm;
        internal bool shadowAlarm;
        internal bool hiddenHandOverthrown;

        internal GovernmentState(GovernmentType type)
        {
            this.type = type;
            previousType = type;
        }

        public GovernmentType Type => type;

        /// <summary>The type in power before the last change (the same as Type until one happens).</summary>
        public GovernmentType PreviousType => previousType;

        public int MonthsInPower => monthsInPower;

        /// <summary>Months before another revolution or vote can start.</summary>
        public int Cooldown => cooldown;

        /// <summary>Recent government changes, decaying over years. Each one vents less suspicion than the last.</summary>
        public double RecentChanges => recentChanges;

        /// <summary>Suspicion that the rulers do not serve the people, 0 to 1.</summary>
        public double Suspicion => suspicion;

        public double SuspicionTarget => suspicionTarget;

        /// <summary>Suspicion that someone behind the rulers pulls the strings, 0 to 1. The hidden hand's exposure.</summary>
        public double ShadowSuspicion => shadowSuspicion;

        /// <summary>Recent abrupt lever changes, decaying over months.</summary>
        public double LeverShock => leverShock;

        /// <summary>How strongly the whole population favors a type (inertia for the current type included).</summary>
        public double Alignment(GovernmentType candidate) => alignment[(int)candidate];

        public PendingChange Pending => pending;

        public GovernmentType PendingTarget => pendingTarget;

        public int PendingMonths => pendingMonths;

        public bool PendingByPlayer => pendingByPlayer;

        /// <summary>True once a revolt has toppled the hidden hand in this nation (D-0019).</summary>
        public bool HiddenHandOverthrown => hiddenHandOverthrown;

        /// <summary>Whether a revolution toward <paramref name="target"/> can be fomented now.</summary>
        public bool CanFoment(GovernmentType target) =>
            !hiddenHandOverthrown && pending == PendingChange.None && cooldown <= 0 && target != type;
    }
}
