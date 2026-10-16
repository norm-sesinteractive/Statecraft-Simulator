using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Scenarios;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.SimRunner
{
    /// <summary>The "run" command: one scripted game, printed as a yearly table, optionally as CSV.</summary>
    internal static class RunCommand
    {
        public static int Execute(Options options)
        {
            string scriptName = options.Get("script", "baseline");
            int years = options.GetInt("years", 50);
            int every = options.GetInt("every", 5);
            ulong seed = options.GetULong("seed", 1);
            string csvPath = options.Get("csv", null);
            bool trace = options.Get("trace", null) != null;
            string why = options.Get("why", null);
            bool economy = options.Get("economy", null) != null;
            if (why != null && WhyTarget(why, out _, out _) == false)
                throw new ArgumentException("--why takes suspicion, shadow, unrest (youth), contentment (adults), births, deaths (elders), productivity or wealth.");

            if (!LeverScripts.TryGet(scriptName, out var onYearStart, out string description))
                throw new ArgumentException($"Unknown script '{scriptName}'. Known: {string.Join(", ", LeverScripts.Names())}.");

            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), seed);
            int home = simulation.World.PlayerHomeIndex;

            Console.WriteLine($"Script '{scriptName}': {description}");
            Console.WriteLine($"Three-nation scenario, seed {seed}, {years} years. Contentment runs -1..+1, the rest 0..1.");
            Console.WriteLine();
            PrintHeader(economy);
            PrintRows(simulation, economy);

            StreamWriter csv = null;
            if (csvPath != null)
            {
                csv = new StreamWriter(csvPath, false, new UTF8Encoding(false));
                csv.WriteLine(CsvHeader());
                WriteCsv(csv, simulation);
            }

            long eventsSeen = simulation.Events.TotalRaised;
            var eventCounts = new Dictionary<SimEventKind, int>();
            try
            {
                for (int year = 1; year <= years; year++)
                {
                    onYearStart(simulation, simulation.Date.Year, home);
                    for (int month = 0; month < SimDate.MonthsPerYear; month++)
                    {
                        simulation.Advance();
                        if (trace) Trace(simulation, home);
                    }
                    if (csv != null) WriteCsv(csv, simulation);
                    eventsSeen = PrintNewEvents(simulation, eventsSeen, eventCounts);
                    if (year % every == 0 || year == years)
                    {
                        PrintRows(simulation, economy);
                        if (why != null) PrintWhy(simulation, home, why);
                    }
                }
            }
            finally
            {
                csv?.Dispose();
            }

            Console.WriteLine();
            if (eventCounts.Count == 0) Console.WriteLine("No alarms fired.");
            foreach (var pair in eventCounts) Console.WriteLine($"{pair.Key}: {pair.Value}");
            if (csvPath != null) Console.WriteLine($"CSV written to {csvPath}");
            return 0;
        }

        static void PrintHeader(bool economy)
        {
            if (economy)
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "{0,5} {1,-12} {2,8} {3,6} {4,6} {5,6} {6,6} {7,6} {8,6} {9,6} {10,6} {11,6} {12,6} {13,8}",
                    "Year", "Nation", "People", "Births", "Deaths", "Press", "Hunger", "Store", "Farm", "Short", "Lux", "Comf", "Prod", "Wealth"));
            else
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,5} {1,-12} {2,-5} {3,8} {4,6} {5,7} {6,6} {7,6} {8,5} {9,5}  {10,-16} {11}",
                    "Year", "Nation", "Gov", "Content", "Crime", "Unrest", "Fear", "Aggr", "Susp", "Shad", "Loudest vice", "Worst frustration"));
        }

        // Economy view: people (millions), births and deaths per 1000 a year, land pressure, hunger, months of food stored, share of
        // workers farming, materials shortage, luxuries per head, comfort, productivity, wealth per head.
        static void PrintEconomyRows(Simulation simulation, int year)
        {
            foreach (var nation in simulation.World.Nations)
            {
                var e = nation.Economy;
                double people = nation.Society.Population;
                double labor = e.Workers(Statecraft.Sim.Economy.Resource.Food) + e.Workers(Statecraft.Sim.Economy.Resource.Materials) +
                    e.Workers(Statecraft.Sim.Economy.Resource.Luxuries);
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "{0,5} {1,-12} {2,8:0.000} {3,6:0.0} {4,6:0.0} {5,6:0.00} {6,6:0.00} {7,6:0.0} {8,6:0.00} {9,6:0.00} {10,6:0.000} {11,6:+0.00;-0.00;0.00} {12,6:0.00} {13,8:0.0}",
                    year, nation.NameKey + (nation.PlayerControlled ? "" : (nation.Index == simulation.World.PlayerHomeIndex ? "!" : "")),
                    people / 1e6, 1000 * e.BirthsPerYear / people, 1000 * e.DeathsPerYear / people, e.LandPressure, e.Hunger, e.StoreMonths,
                    labor > 0 ? e.Workers(Statecraft.Sim.Economy.Resource.Food) / labor : 0, e.MaterialsShortage, e.LuxuriesPerHead,
                    e.Comfort, e.Productivity, e.Wealth / people));
            }
        }

        static void PrintRows(Simulation simulation, bool economy)
        {
            if (economy)
            {
                PrintEconomyRows(simulation, simulation.Date.Tick == 0 ? 0 : (int)((simulation.Date.Tick - 1) / SimDate.MonthsPerYear) + 1);
                return;
            }
            // Rows show the state after the last simulated month.
            int year = simulation.Date.Tick == 0 ? 0 : (int)((simulation.Date.Tick - 1) / SimDate.MonthsPerYear) + 1;
            foreach (var nation in simulation.World.Nations)
            {
                var s = nation.Society;
                Vice loudest = s.Expressed.ArgMax();
                Vice worst = s.Frustration.ArgMax();
                string frustration = s.Frustration[worst] > 0.0005 ? $"{worst} {s.Frustration[worst]:0.00}" : "-";
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "{0,5} {1,-12} {2,-5} {3,8:+0.00;-0.00} {4,6:0.00} {5,7:0.00} {6,6:0.00} {7,6:0.00} {8,5:0.00} {9,5:0.00}  {10,-16} {11}",
                    year, nation.NameKey + (nation.PlayerControlled ? "" : (nation.Index == simulation.World.PlayerHomeIndex ? "!" : "")),
                    Short(nation.Government.Type), s.Contentment, s.Crime, s.Unrest, s.Fear, s.Aggression,
                    nation.Government.Suspicion, nation.Government.ShadowSuspicion,
                    $"{loudest} {s.Expressed[loudest]:0.00}", frustration));
            }
        }

        static long PrintNewEvents(Simulation simulation, long seen, Dictionary<SimEventKind, int> counts)
        {
            long fresh = simulation.Events.TotalRaised - seen;
            int available = (int)Math.Min(fresh, simulation.Events.Count);
            for (int age = available - 1; age >= 0; age--)
            {
                var e = simulation.Events.GetRecent(age);
                string name = simulation.World.Nations[e.NationIndex].NameKey;
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "      ! {0} {1} {2}{3} ({4:0.00})", e.Date, name, e.Kind, Detail(e), e.Value));
                counts.TryGetValue(e.Kind, out int count);
                counts[e.Kind] = count + 1;
            }
            return simulation.Events.TotalRaised;
        }

        static bool WhyTarget(string name, out Quantity quantity, out int cohort)
        {
            cohort = -1;
            switch (name.ToLowerInvariant())
            {
                case "suspicion": quantity = Quantity.GovernmentSuspicion; return true;
                case "shadow": quantity = Quantity.ShadowSuspicion; return true;
                case "unrest": quantity = Quantity.Unrest; cohort = (int)Cohort.Youth; return true;
                case "contentment": quantity = Quantity.Contentment; cohort = (int)Cohort.Adult; return true;
                case "births": quantity = Quantity.Births; return true;
                case "deaths": quantity = Quantity.Deaths; cohort = (int)Cohort.Elder; return true;
                case "productivity": quantity = Quantity.Productivity; return true;
                case "wealth": quantity = Quantity.Wealth; return true;
                default: quantity = Quantity.Mood; return false;
            }
        }

        // Diagnostic: the terms behind one of the home nation's values in the last simulated month (from the ledger).
        static void PrintWhy(Simulation simulation, int home, string name)
        {
            WhyTarget(name, out var quantity, out int cohort);
            var terms = new List<Contribution>();
            simulation.Ledger.Collect(home, quantity, -1, cohort, terms);
            var parts = new List<string>();
            double sum = 0;
            foreach (var term in terms)
            {
                sum += term.Amount;
                parts.Add(string.Format(CultureInfo.InvariantCulture, "{0} {1:+0.000;-0.000}", term.Source, term.Amount));
            }
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "      why {0} (sum {1:+0.000;-0.000}): {2}", name, sum, string.Join(", ", parts)));
        }

        // Diagnostic: the home nation's politics each month while a revolution or vote is under way.
        static void Trace(Simulation simulation, int home)
        {
            var nation = simulation.World.Nations[home];
            var g = nation.Government;
            if (g.Pending == PendingChange.None) return;
            var s = nation.Society;
            double lead = g.Alignment(g.PendingTarget) - g.Alignment(g.Type);
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "      ~ {0} {1} toward {2}, {3} month(s) left: unrest {4:0.000} (youth {5:0.000} adult {6:0.000} elder {7:0.000}), " +
                "lead {8:+0.000;-0.000}, fear {9:0.000}, suspicion {10:0.000}",
                simulation.Date, g.Pending, g.PendingTarget, g.PendingMonths, s.Unrest, s.GetCohort(Cohort.Youth).Unrest,
                s.GetCohort(Cohort.Adult).Unrest, s.GetCohort(Cohort.Elder).Unrest, lead, s.Fear, g.Suspicion));
        }

        static string Short(GovernmentType type) => type == GovernmentType.MilitaryJunta ? "Junta" : type.ToString().Substring(0, 5);

        static string Detail(SimEvent e)
        {
            if (e.Detail < 0) return "";
            switch (e.Kind)
            {
                case SimEventKind.RevoltSucceeded: return " by " + (Cohort)e.Detail + " -> " + (GovernmentType)(int)e.Value;
                case SimEventKind.RevoltCrushed:
                case SimEventKind.HiddenHandOverthrown: return " by " + (Cohort)e.Detail;
                default: return " " + (GovernmentType)e.Detail;
            }
        }

        static string CsvHeader()
        {
            var header = new StringBuilder("tick,year,month,nation,government,suspicion,shadow,contentment,crime,unrest,fear,aggression,youth_unrest,adult_unrest,elder_unrest");
            foreach (string group in new[] { "expressed", "outlet", "frustration", "mood" })
                for (int v = 0; v < ViceVector.Count; v++)
                    header.Append(',').Append(group).Append('_').Append(((Vice)v).ToString().ToLowerInvariant());
            return header.ToString();
        }

        static void WriteCsv(StreamWriter csv, Simulation simulation)
        {
            foreach (var nation in simulation.World.Nations)
            {
                var s = nation.Society;
                var line = new StringBuilder();
                line.Append(simulation.Date.Tick).Append(',').Append(simulation.Date.Year).Append(',').Append(simulation.Date.Month)
                    .Append(',').Append(nation.NameKey).Append(',').Append(nation.Government.Type);
                var g = nation.Government;
                foreach (double value in new[] { g.Suspicion, g.ShadowSuspicion, s.Contentment, s.Crime, s.Unrest, s.Fear, s.Aggression,
                    s.GetCohort(Cohort.Youth).Unrest, s.GetCohort(Cohort.Adult).Unrest, s.GetCohort(Cohort.Elder).Unrest }) Append(line, value);
                foreach (var vector in new[] { s.Expressed, s.Outlets, s.Frustration, s.Mood })
                    for (int v = 0; v < ViceVector.Count; v++) Append(line, vector[v]);
                csv.WriteLine(line.ToString());
            }
        }

        static void Append(StringBuilder line, double value) =>
            line.Append(',').Append(value.ToString("0.#####", CultureInfo.InvariantCulture));
    }
}
