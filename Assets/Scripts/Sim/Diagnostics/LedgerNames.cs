using Statecraft.Sim.Economy;
using Statecraft.Sim.Institutions;
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

        /// <summary>The government type in power (its traits).</summary>
        public const string Government = "Government";

        /// <summary>A revolution being fomented.</summary>
        public const string Revolution = "Revolution";

        public const string Inertia = "Inertia";
        public const string GovernmentSuspicion = "GovernmentSuspicion";
        public const string CulturalDrift = "Cultural drift";
        public const string ShadowSuspicion = "ShadowSuspicion";
        public const string CurriculumConcentration = "Curriculum concentration";
        public const string AirtimeConcentration = "Airtime concentration";
        public const string LeverShocks = "Lever shocks";
        public const string MediaNeutral = "Media.Neutral";
        public const string SatisfiedGluttony = "Satisfaction.Gluttony";
        public const string Decay = "Decay";

        // Economy (D-0036). "Hunger", "Comfort" and the like are economy readings, not ledger quantities.
        public const string Hunger = "Hunger";
        public const string MaterialsShortage = "Materials shortage";
        public const string Comfort = "Comfort";
        public const string Unrest = "Unrest";
        public const string ElderNeglect = "Elder neglect";
        public const string Famine = "Famine";
        public const string Crowding = "Crowding";
        public const string OpenLand = "Open land";
        public const string Upkeep = "Upkeep";
        public static readonly string[] Surplus = PerResource("Surplus");

        public static readonly string[] Media = PerVice("Media");
        public static readonly string[] Channels = PerVice("Channels");
        public static readonly string[] Disposition = PerVice("Disposition");
        public static readonly string[] Mood = PerVice("Mood");
        public static readonly string[] Expressed = PerVice("Expressed");
        public static readonly string[] Outlet = PerVice("Outlet");
        public static readonly string[] Frustration = PerVice("Frustration");
        public static readonly string[] Schooling = PerFocus("Schooling");

        static string[] PerVice(string prefix)
        {
            var names = new string[ViceVector.Count];
            for (int i = 0; i < names.Length; i++) names[i] = prefix + "." + (Vice)i;
            return names;
        }

        static string[] PerResource(string prefix)
        {
            var names = new string[EconomyState.ResourceCount];
            for (int i = 0; i < names.Length; i++) names[i] = prefix + "." + (Resource)i;
            return names;
        }

        static string[] PerFocus(string prefix)
        {
            var names = new string[Curriculum.FocusCount];
            for (int i = 0; i < names.Length; i++) names[i] = prefix + "." + (CurriculumFocus)i;
            return names;
        }
    }
}
