using Statecraft.Sim.Core;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Systems;

namespace Statecraft.Sim.Commands
{
    /// <summary>
    /// The hidden hand calls a vote on the government type; only types that can vote (Democracy) allow it.
    /// Ignored unless <see cref="Simulation.CanCallVote"/> allows it; offer it only then.
    /// </summary>
    public sealed class CallVote : SimCommand
    {
        public CallVote(int nationIndex, GovernmentType target)
        {
            NationIndex = nationIndex;
            Target = target;
        }

        public int NationIndex { get; }

        public GovernmentType Target { get; }

        internal override void Apply(SimContext context)
        {
            var nation = ResolveNation(context.World, NationIndex);
            var government = nation.Government;
            if (!nation.PlayerControlled || !government.CanFoment(Target)) return;
            if (!context.Tuning.Politics.Types[(int)government.Type].CanVote) return;
            PoliticsSystem.Start(government, PendingChange.Vote, Target, context.Tuning.Politics.VoteMonths, byPlayer: true);
        }
    }
}
