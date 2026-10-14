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
        internal abstract void Apply(World world);

        internal static Nation ResolveNation(World world, int nationIndex)
        {
            if (nationIndex < 0 || nationIndex >= world.Nations.Count)
                throw new InvalidOperationException($"No nation with index {nationIndex}.");
            return world.Nations[nationIndex];
        }
    }
}
