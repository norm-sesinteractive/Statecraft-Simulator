namespace Statecraft.Sim.Core
{
    /// <summary>
    /// Kinds of notable change the simulation reports. Alarms come in pairs with an all-clear. For politics
    /// events, <see cref="SimEvent.Detail"/> carries a government type or a cohort (see each member).
    /// </summary>
    public enum SimEventKind
    {
        UnrestRising,
        UnrestEased,
        SufferingSevere,
        SufferingEased,
        CrimeWave,
        CrimeEased,

        /// <summary>Government suspicion crossed its alarm (pamphlets, investigations).</summary>
        SuspicionRising,
        SuspicionEased,

        /// <summary>Shadow suspicion crossed its alarm (whispers of a hidden hand).</summary>
        ShadowRumors,
        ShadowRumorsFaded,

        /// <summary>The player began fomenting a revolution. Detail: target GovernmentType.</summary>
        RevolutionStarted,

        /// <summary>A fomented revolution won. Detail: the new GovernmentType.</summary>
        RevolutionSucceeded,

        /// <summary>A fomented revolution failed. Detail: the target GovernmentType.</summary>
        RevolutionFailed,

        /// <summary>A vote on the government type began. Detail: target GovernmentType. Value: 1 if the player called it, 0 if the people did.</summary>
        VoteCalled,

        /// <summary>A vote passed. Detail: the new GovernmentType.</summary>
        VotePassed,

        /// <summary>A vote failed. Detail: the target GovernmentType.</summary>
        VoteFailed,

        /// <summary>A generation's revolt toppled the government. Detail: the Cohort. Value: the new GovernmentType.</summary>
        RevoltSucceeded,

        /// <summary>A generation's revolt was crushed. Detail: the Cohort.</summary>
        RevoltCrushed,

        /// <summary>A revolt toppled the hidden hand itself in this nation (D-0019). Detail: the Cohort.</summary>
        HiddenHandOverthrown,
    }
}
