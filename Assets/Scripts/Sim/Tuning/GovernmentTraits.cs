using Statecraft.Sim.Society;

namespace Statecraft.Sim.Tuning
{
    /// <summary>What one government type changes in the model (INSTITUTIONS.md traits table, D-0023).</summary>
    public sealed class GovernmentTraits
    {
        /// <summary>Outlet capacity this type adds or removes per vice (Communism channels Envy and starves Greed, and so on).</summary>
        public ViceVector OutletBonus;

        /// <summary>Appeal of this type before the population's culture is considered.</summary>
        public double AlignmentBase;

        /// <summary>Alignment per unit of expressed vice above human nature.</summary>
        public ViceVector AlignmentFromVices;

        /// <summary>Alignment per unit of schooling share above an even curriculum, in CurriculumFocus order.</summary>
        public double[] AlignmentFromSchooling = new double[7];

        /// <summary>Unrest removed from every generation (elections, faith).</summary>
        public double UnrestVenting;

        /// <summary>Multiplier on the suffering that fear of the courts causes.</summary>
        public double FearSufferingScale = 1;

        /// <summary>Multiplier on how much fear holds unrest down.</summary>
        public double FearDampeningScale = 1;

        /// <summary>Government suspicion per unit of court severity.</summary>
        public double SeveritySuspicion;

        /// <summary>Multiplier on the suspicion an educated (Science-schooled) population brings.</summary>
        public double ScienceSuspicionScale = 1;

        /// <summary>Multiplier on the suspicion abrupt lever swings bring (a free press notices sooner).</summary>
        public double LeverSwingSuspicionScale = 1;

        /// <summary>Multiplier on how often revolts break out (coups under a junta).</summary>
        public double RevoltChanceScale = 1;

        /// <summary>Aggression added by this type.</summary>
        public double AggressionBonus;

        /// <summary>Whether the people can vote to change the government type.</summary>
        public bool CanVote;
    }
}
