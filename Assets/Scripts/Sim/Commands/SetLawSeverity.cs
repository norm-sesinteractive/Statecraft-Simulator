using Statecraft.Sim.Core;

namespace Statecraft.Sim.Commands
{
    /// <summary>Sets how hard a nation's courts strike, 0 (pardon all but the most guilty) to 1 (ruthless).</summary>
    public sealed class SetLawSeverity : SimCommand
    {
        public SetLawSeverity(int nationIndex, double severity)
        {
            NationIndex = nationIndex;
            Severity = severity;
        }

        public int NationIndex { get; }

        public double Severity { get; }

        internal override void Apply(World world) => ResolveNation(world, NationIndex).Institutions.Law.Severity = Severity;
    }
}
