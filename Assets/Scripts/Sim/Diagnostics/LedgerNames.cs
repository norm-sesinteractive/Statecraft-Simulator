using Statecraft.Sim.Society;

namespace Statecraft.Sim.Diagnostics
{
    /// <summary>
    /// Source names used in ledger entries. Names of quantities match <see cref="Ledger.NameOf"/>
    /// so the causal graph connects; levers and constants have their own prefixes. Built once.
    /// </summary>
    internal static class LedgerNames
    {
        public const string Base = "Base";
        public const string Satisfaction = "Satisfaction";
        public const string Fear = "Fear";
        public const string Crime = "Crime";
        public const string Contentment = "Contentment";

        /// <summary>Negative contentment, as a positive amount. Named apart so "why unrest?" reads correctly.</summary>
        public const string Suffering = "Suffering";
        public const string LawSeverity = "Law.Severity";
        public const string EnvyTimesWrath = "Envy x Wrath";

        public static readonly string[] Media = PerVice("Media");
        public static readonly string[] Channels = PerVice("Channels");
        public static readonly string[] Disposition = PerVice("Disposition");
        public static readonly string[] Mood = PerVice("Mood");
        public static readonly string[] Expressed = PerVice("Expressed");
        public static readonly string[] Outlet = PerVice("Outlet");
        public static readonly string[] Frustration = PerVice("Frustration");

        static string[] PerVice(string prefix)
        {
            var names = new string[ViceVector.Count];
            for (int i = 0; i < names.Length; i++) names[i] = prefix + "." + (Vice)i;
            return names;
        }
    }
}
