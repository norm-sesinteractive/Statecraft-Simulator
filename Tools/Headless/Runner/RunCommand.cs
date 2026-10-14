using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Statecraft.Sim.Core;
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

            if (!LeverScripts.TryGet(scriptName, out var onYearStart, out string description))
                throw new ArgumentException($"Unknown script '{scriptName}'. Known: {string.Join(", ", LeverScripts.Names())}.");

            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), seed);
            int home = simulation.World.PlayerHomeIndex;

            Console.WriteLine($"Script '{scriptName}': {description}");
            Console.WriteLine($"Three-nation scenario, seed {seed}, {years} years. Contentment runs -1..+1, the rest 0..1.");
            Console.WriteLine();
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,5} {1,-12} {2,8} {3,6} {4,7} {5,6} {6,6}  {7,-16} {8}",
                "Year", "Nation", "Content", "Crime", "Unrest", "Fear", "Aggr", "Loudest vice", "Worst frustration"));
            PrintRows(simulation);

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
                    simulation.Advance(SimDate.MonthsPerYear);
                    if (csv != null) WriteCsv(csv, simulation);
                    eventsSeen = PrintNewEvents(simulation, eventsSeen, eventCounts);
                    if (year % every == 0 || year == years) PrintRows(simulation);
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

        static void PrintRows(Simulation simulation)
        {
            // Rows show the state after the last simulated month.
            int year = simulation.Date.Tick == 0 ? 0 : (int)((simulation.Date.Tick - 1) / SimDate.MonthsPerYear) + 1;
            foreach (var nation in simulation.World.Nations)
            {
                var s = nation.Society;
                Vice loudest = s.Expressed.ArgMax();
                Vice worst = s.Frustration.ArgMax();
                string frustration = s.Frustration[worst] > 0.0005 ? $"{worst} {s.Frustration[worst]:0.00}" : "-";
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "{0,5} {1,-12} {2,8:+0.00;-0.00} {3,6:0.00} {4,7:0.00} {5,6:0.00} {6,6:0.00}  {7,-16} {8}",
                    year, nation.NameKey, s.Contentment, s.Crime, s.Unrest, s.Fear, s.Aggression,
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
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "      ! {0} {1} {2} ({3:0.00})", e.Date, name, e.Kind, e.Value));
                counts.TryGetValue(e.Kind, out int count);
                counts[e.Kind] = count + 1;
            }
            return simulation.Events.TotalRaised;
        }

        static string CsvHeader()
        {
            var header = new StringBuilder("tick,year,month,nation,contentment,crime,unrest,fear,aggression");
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
                    .Append(',').Append(nation.NameKey);
                foreach (double value in new[] { s.Contentment, s.Crime, s.Unrest, s.Fear, s.Aggression }) Append(line, value);
                foreach (var vector in new[] { s.Expressed, s.Outlets, s.Frustration, s.Mood })
                    for (int v = 0; v < ViceVector.Count; v++) Append(line, vector[v]);
                csv.WriteLine(line.ToString());
            }
        }

        static void Append(StringBuilder line, double value) =>
            line.Append(',').Append(value.ToString("0.#####", CultureInfo.InvariantCulture));
    }
}
