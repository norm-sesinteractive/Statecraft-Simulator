using System;

namespace Statecraft.Sim.Core
{
    /// <summary>
    /// Bounded history of recent <see cref="SimEvent"/>s plus a running total, so views can poll
    /// for what is new since they last looked without subscribing mid-tick.
    /// </summary>
    public sealed class EventLog
    {
        public const int Capacity = 64;

        readonly SimEvent[] ring = new SimEvent[Capacity];
        int next;
        int count;

        /// <summary>Number of events raised since the game began (never decreases).</summary>
        public long TotalRaised { get; private set; }

        /// <summary>Number of events currently held (at most <see cref="Capacity"/>).</summary>
        public int Count => count;

        /// <summary>The <paramref name="age"/>-th most recent event; 0 is the newest.</summary>
        public SimEvent GetRecent(int age)
        {
            if (age < 0 || age >= count) throw new ArgumentOutOfRangeException(nameof(age));
            int index = (next - 1 - age + Capacity) % Capacity;
            return ring[index];
        }

        internal void Raise(SimEvent simEvent)
        {
            ring[next] = simEvent;
            next = (next + 1) % Capacity;
            if (count < Capacity) count++;
            TotalRaised++;
        }
    }
}
