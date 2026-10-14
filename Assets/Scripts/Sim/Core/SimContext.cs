using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Core
{
    /// <summary>Everything a system needs during one tick. One instance per simulation, reused.</summary>
    internal sealed class SimContext
    {
        public SimContext(World world, SimTuning tuning, SimRandom random, Ledger ledger, EventLog events)
        {
            World = world;
            Tuning = tuning;
            Random = random;
            Ledger = ledger;
            Events = events;
        }

        public World World { get; }

        public SimTuning Tuning { get; }

        public SimRandom Random { get; }

        public Ledger Ledger { get; }

        public EventLog Events { get; }

        /// <summary>The month being simulated.</summary>
        public SimDate Date { get; set; }
    }
}
