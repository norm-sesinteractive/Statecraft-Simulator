using Statecraft.Sim.Core;

namespace Statecraft.Sim.Systems
{
    /// <summary>One stage of the monthly tick. Systems run in a fixed order set by <see cref="Simulation"/>.</summary>
    internal interface ISimSystem
    {
        void Tick(SimContext context);
    }
}
