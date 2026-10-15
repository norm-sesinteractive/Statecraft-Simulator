using System;
using Statecraft.Sim.Nations;

namespace Statecraft.Sim.Core
{
    /// <summary>
    /// An instruction to change a lever. Commands are the only public way to mutate the
    /// simulation: they queue on <see cref="Simulation.Enqueue"/> and apply at the start of the
    /// next tick, which keeps saves and replays honest. Only the sim assembly can define them.
    /// </summary>
    public abstract class SimCommand
    {
        internal abstract void Apply(SimContext context);

        internal static Nation ResolveNation(World world, int nationIndex)
        {
            if (nationIndex < 0 || nationIndex >= world.Nations.Count)
                throw new InvalidOperationException($"No nation with index {nationIndex}.");
            return world.Nations[nationIndex];
        }

        /// <summary>Records how abruptly a lever moved (0 to 1); abrupt swings raise suspicion (D-0020).</summary>
        internal static void RecordShock(Nation nation, double[] before, double[] after)
        {
            double change = 0;
            for (int i = 0; i < before.Length; i++) change += Math.Abs(after[i] - before[i]);
            nation.Government.leverShock += change / 2;
        }
    }
}
