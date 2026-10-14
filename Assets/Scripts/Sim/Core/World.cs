using System;
using System.Collections.Generic;
using Statecraft.Sim.Nations;

namespace Statecraft.Sim.Core
{
    /// <summary>Every nation in the game, in a fixed order; a nation's index in <see cref="Nations"/> is its id.</summary>
    public sealed class World
    {
        readonly List<Nation> nations;

        internal World(IReadOnlyList<NationSetup> setups)
        {
            if (setups == null || setups.Count == 0)
                throw new ArgumentException("A world needs at least one nation.", nameof(setups));

            nations = new List<Nation>(setups.Count);
            for (int i = 0; i < setups.Count; i++)
            {
                if (setups[i] == null) throw new ArgumentException($"Nation setup {i} is null.", nameof(setups));
                nations.Add(new Nation(i, setups[i]));
            }
        }

        public IReadOnlyList<Nation> Nations => nations;

        /// <summary>Index of the first player-controlled nation, or -1 when the player controls none.</summary>
        public int PlayerHomeIndex
        {
            get
            {
                for (int i = 0; i < nations.Count; i++)
                    if (nations[i].PlayerControlled) return i;
                return -1;
            }
        }
    }
}
