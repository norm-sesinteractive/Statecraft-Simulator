using System;
using System.Globalization;
using Statecraft.Sim.Core;

namespace Statecraft.Sim.Society
{
    /// <summary>
    /// One number per <see cref="Vice"/>. A small value type: copy freely, and mutate through the
    /// indexer only on locals and fields (never through a property, which would mutate a copy).
    /// </summary>
    public struct ViceVector : IEquatable<ViceVector>
    {
        public const int Count = 7;

        public double Wrath;
        public double Sloth;
        public double Greed;
        public double Lust;
        public double Pride;
        public double Envy;
        public double Gluttony;

        public ViceVector(double wrath, double sloth, double greed, double lust, double pride, double envy, double gluttony)
        {
            Wrath = wrath;
            Sloth = sloth;
            Greed = greed;
            Lust = lust;
            Pride = pride;
            Envy = envy;
            Gluttony = gluttony;
        }

        public static ViceVector Zero => default;

        public static ViceVector Uniform(double value) => new ViceVector(value, value, value, value, value, value, value);

        public double this[Vice vice]
        {
            get => this[(int)vice];
            set => this[(int)vice] = value;
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Wrath;
                    case 1: return Sloth;
                    case 2: return Greed;
                    case 3: return Lust;
                    case 4: return Pride;
                    case 5: return Envy;
                    case 6: return Gluttony;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
            set
            {
                switch (index)
                {
                    case 0: Wrath = value; break;
                    case 1: Sloth = value; break;
                    case 2: Greed = value; break;
                    case 3: Lust = value; break;
                    case 4: Pride = value; break;
                    case 5: Envy = value; break;
                    case 6: Gluttony = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public double Sum() => Wrath + Sloth + Greed + Lust + Pride + Envy + Gluttony;

        public double Mean() => Sum() / Count;

        /// <summary>The vice with the largest value (the first one on ties).</summary>
        public Vice ArgMax()
        {
            int best = 0;
            for (int i = 1; i < Count; i++)
                if (this[i] > this[best]) best = i;
            return (Vice)best;
        }

        public bool IsFinite()
        {
            for (int i = 0; i < Count; i++)
            {
                double v = this[i];
                if (double.IsNaN(v) || double.IsInfinity(v)) return false;
            }
            return true;
        }

        public ViceVector Clamp(double min, double max)
        {
            var result = this;
            for (int i = 0; i < Count; i++) result[i] = SimMath.Clamp(result[i], min, max);
            return result;
        }

        public ViceVector Clamp01() => Clamp(0, 1);

        public static ViceVector operator +(ViceVector a, ViceVector b) => new ViceVector(
            a.Wrath + b.Wrath, a.Sloth + b.Sloth, a.Greed + b.Greed, a.Lust + b.Lust,
            a.Pride + b.Pride, a.Envy + b.Envy, a.Gluttony + b.Gluttony);

        public static ViceVector operator -(ViceVector a, ViceVector b) => new ViceVector(
            a.Wrath - b.Wrath, a.Sloth - b.Sloth, a.Greed - b.Greed, a.Lust - b.Lust,
            a.Pride - b.Pride, a.Envy - b.Envy, a.Gluttony - b.Gluttony);

        public static ViceVector operator *(ViceVector a, double s) => new ViceVector(
            a.Wrath * s, a.Sloth * s, a.Greed * s, a.Lust * s, a.Pride * s, a.Envy * s, a.Gluttony * s);

        public static ViceVector operator *(double s, ViceVector a) => a * s;

        public static ViceVector Lerp(ViceVector a, ViceVector b, double t) => a + (b - a) * t;

        public bool Equals(ViceVector other) =>
            Wrath.Equals(other.Wrath) && Sloth.Equals(other.Sloth) && Greed.Equals(other.Greed) &&
            Lust.Equals(other.Lust) && Pride.Equals(other.Pride) && Envy.Equals(other.Envy) &&
            Gluttony.Equals(other.Gluttony);

        public override bool Equals(object obj) => obj is ViceVector other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Wrath, Sloth, Greed, Lust, Pride, Envy, Gluttony);

        public static bool operator ==(ViceVector a, ViceVector b) => a.Equals(b);

        public static bool operator !=(ViceVector a, ViceVector b) => !a.Equals(b);

        public override string ToString() => string.Format(CultureInfo.InvariantCulture,
            "W{0:0.00} S{1:0.00} G{2:0.00} L{3:0.00} P{4:0.00} E{5:0.00} Gl{6:0.00}",
            Wrath, Sloth, Greed, Lust, Pride, Envy, Gluttony);
    }
}
