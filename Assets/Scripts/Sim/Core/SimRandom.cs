namespace Statecraft.Sim.Core
{
    /// <summary>
    /// Deterministic PCG32 generator (O'Neill 2014, XSH-RR variant). The only source of randomness
    /// the simulation may use: never System.Random (its algorithm differs between runtimes) or
    /// UnityEngine.Random. Same seed and same commands give the same game.
    /// </summary>
    public sealed class SimRandom
    {
        const ulong Multiplier = 6364136223846793005UL;
        const ulong DefaultStream = 54UL;

        ulong state;
        readonly ulong increment;

        /// <summary>Seeds the generator exactly like the reference pcg32_srandom_r(seed, stream).</summary>
        public SimRandom(ulong seed, ulong stream = DefaultStream)
        {
            state = 0UL;
            increment = (stream << 1) | 1UL;
            NextUInt();
            state = unchecked(state + seed);
            NextUInt();
        }

        SimRandom(ulong rawState, ulong rawIncrement, bool _)
        {
            state = rawState;
            increment = rawIncrement | 1UL;
        }

        /// <summary>Internal state, for save files.</summary>
        public ulong State => state;

        /// <summary>Stream selector (always odd), for save files.</summary>
        public ulong Increment => increment;

        /// <summary>Recreates a generator from <see cref="State"/> and <see cref="Increment"/>.</summary>
        public static SimRandom FromState(ulong state, ulong increment) => new SimRandom(state, increment, true);

        public uint NextUInt()
        {
            ulong old = state;
            state = unchecked(old * Multiplier + increment);
            uint xorShifted = (uint)(((old >> 18) ^ old) >> 27);
            int rotation = (int)(old >> 59);
            return (xorShifted >> rotation) | (xorShifted << ((-rotation) & 31));
        }

        /// <summary>Uniform double in [0, 1) with 32 bits of randomness.</summary>
        public double NextDouble() => NextUInt() * (1.0 / 4294967296.0);

        /// <summary>Uniform double in [min, max).</summary>
        public double Range(double min, double max) => min + (max - min) * NextDouble();

        /// <summary>
        /// Approximately standard-normal deviate (Irwin-Hall: twelve uniforms minus six). Uses
        /// only addition, so results match bit for bit across Mono and .NET.
        /// </summary>
        public double NextGaussian()
        {
            double sum = 0;
            for (int i = 0; i < 12; i++) sum += NextDouble();
            return sum - 6.0;
        }
    }
}
