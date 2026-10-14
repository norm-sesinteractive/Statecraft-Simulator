using System;

namespace Statecraft.Sim.Core
{
    /// <summary>
    /// A position on the game calendar. One tick is one month; a new game starts at Year 1,
    /// Month 1 (tick 0). The calendar is fictional until the setting is decided.
    /// </summary>
    public readonly struct SimDate : IEquatable<SimDate>, IComparable<SimDate>
    {
        public const int MonthsPerYear = 12;

        public SimDate(long tick)
        {
            if (tick < 0) throw new ArgumentOutOfRangeException(nameof(tick), "Ticks start at 0.");
            Tick = tick;
        }

        /// <summary>Months elapsed since the game began.</summary>
        public long Tick { get; }

        public int Year => (int)(Tick / MonthsPerYear) + 1;

        /// <summary>Month of the year, 1 to 12.</summary>
        public int Month => (int)(Tick % MonthsPerYear) + 1;

        public SimDate Next => new SimDate(Tick + 1);

        public static SimDate FromYearMonth(int year, int month)
        {
            if (year < 1) throw new ArgumentOutOfRangeException(nameof(year));
            if (month < 1 || month > MonthsPerYear) throw new ArgumentOutOfRangeException(nameof(month));
            return new SimDate((long)(year - 1) * MonthsPerYear + (month - 1));
        }

        public bool Equals(SimDate other) => Tick == other.Tick;
        public override bool Equals(object obj) => obj is SimDate other && Equals(other);
        public override int GetHashCode() => Tick.GetHashCode();
        public int CompareTo(SimDate other) => Tick.CompareTo(other.Tick);
        public static bool operator ==(SimDate a, SimDate b) => a.Tick == b.Tick;
        public static bool operator !=(SimDate a, SimDate b) => a.Tick != b.Tick;

        /// <summary>Compact technical form, for logs and dev tools: "Y12 M03".</summary>
        public override string ToString() => $"Y{Year} M{Month:00}";
    }
}
