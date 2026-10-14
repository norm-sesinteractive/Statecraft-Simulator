namespace Statecraft.Sim.Core
{
    /// <summary>
    /// A notable change the simulation reports outward (events out, commands in). Plain data with
    /// no text: presentation decides how, and in whose words, to show it.
    /// </summary>
    public readonly struct SimEvent
    {
        public SimEvent(SimDate date, int nationIndex, SimEventKind kind, double value)
        {
            Date = date;
            NationIndex = nationIndex;
            Kind = kind;
            Value = value;
        }

        public SimDate Date { get; }

        public int NationIndex { get; }

        public SimEventKind Kind { get; }

        /// <summary>The value of the watched quantity when the event fired.</summary>
        public double Value { get; }
    }
}
