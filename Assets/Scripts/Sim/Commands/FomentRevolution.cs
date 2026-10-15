using Statecraft.Sim.Core;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Systems;

namespace Statecraft.Sim.Commands
{
    /// <summary>
    /// The hidden hand foments a revolution toward a government type (D-0021): months of stoked unrest, then
    /// success or failure. Ignored unless <see cref="Simulation.CanFoment"/> allows it; offer it only then.
    /// </summary>
    public sealed class FomentRevolution : SimCommand
    {
        public FomentRevolution(int nationIndex, GovernmentType target)
        {
            NationIndex = nationIndex;
            Target = target;
        }

        public int NationIndex { get; }

        public GovernmentType Target { get; }

        internal override void Apply(SimContext context)
        {
            var nation = ResolveNation(context.World, NationIndex);
            if (!nation.PlayerControlled || !nation.Government.CanFoment(Target)) return;
            PoliticsSystem.Start(nation.Government, PendingChange.Revolution, Target, context.Tuning.Politics.FomentMonths, byPlayer: true);
        }
    }
}
