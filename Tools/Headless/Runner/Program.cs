using System;
using System.Globalization;

namespace Statecraft.SimRunner
{
    /// <summary>
    /// Command-line entry for headless runs of the pure C# simulation (balancing experiments) and
    /// for the docs generated from the live model (parameters table, causal graph).
    /// </summary>
    public static class Program
    {
        const string Usage = @"Statecraft SimRunner: the simulation without Unity.

Commands:
  run      [--script NAME] [--years N] [--seed N] [--every N] [--csv PATH] [--economy] [--trace] [--why WHAT]
           Runs the three-nation scenario under a lever script and prints a table every N years.
           --economy prints people, food, goods and wealth instead of society and politics.
           --why prints the ledger terms behind a home value: suspicion, shadow, unrest, contentment,
           births, deaths, productivity or wealth.
  scripts  Lists the built-in lever scripts.
  sweep    [--seed N]
           Pushes each home lever to an extreme, one at a time, and reports effects at that lever's horizon.
  docs     [--out DIR]
           Writes PARAMETERS.md and SOCIETY_GRAPH.md (default: Docs/generated under the repo root).

Examples (from the repo root):
  dotnet run --project Tools/Headless/Runner -- run --script wrath-media --years 60
  dotnet run --project Tools/Headless/Runner -- docs";

        public static int Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
            {
                Console.WriteLine(Usage);
                return 0;
            }

            try
            {
                var options = Options.Parse(args, 1);
                switch (args[0])
                {
                    case "run": return RunCommand.Execute(options);
                    case "scripts": LeverScripts.PrintList(); return 0;
                    case "docs": return DocsCommand.Execute(options);
                    case "sweep": return SweepCommand.Execute(options);
                    default:
                        Console.Error.WriteLine($"Unknown command '{args[0]}'.");
                        Console.Error.WriteLine(Usage);
                        return 2;
                }
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine(e.Message);
                return 2;
            }
        }
    }
}
