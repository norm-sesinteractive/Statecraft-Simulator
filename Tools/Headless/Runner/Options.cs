using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Statecraft.SimRunner
{
    /// <summary>Minimal "--key value" argument parser; a bare "--flag" reads as "true".</summary>
    internal sealed class Options
    {
        readonly Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static Options Parse(string[] args, int start)
        {
            var options = new Options();
            for (int i = start; i < args.Length; i++)
            {
                string arg = args[i];
                if (!arg.StartsWith("--", StringComparison.Ordinal))
                    throw new ArgumentException($"Unexpected argument '{arg}'. Options look like --years 50.");
                string key = arg.Substring(2);
                bool hasValue = i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal);
                options.values[key] = hasValue ? args[++i] : "true";
            }
            return options;
        }

        public string Get(string key, string fallback) => values.TryGetValue(key, out var value) ? value : fallback;

        public int GetInt(string key, int fallback)
        {
            if (!values.TryGetValue(key, out var raw)) return fallback;
            if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) || value <= 0)
                throw new ArgumentException($"--{key} needs a positive whole number, got '{raw}'.");
            return value;
        }

        public ulong GetULong(string key, ulong fallback)
        {
            if (!values.TryGetValue(key, out var raw)) return fallback;
            if (!ulong.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong value))
                throw new ArgumentException($"--{key} needs a whole number, got '{raw}'.");
            return value;
        }

        /// <summary>The repository root: the nearest ancestor of the working directory holding Assets, Docs and Tools.</summary>
        public static string FindRepoRoot()
        {
            foreach (string start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                var dir = new DirectoryInfo(start);
                while (dir != null)
                {
                    if (Directory.Exists(Path.Combine(dir.FullName, "Assets")) &&
                        Directory.Exists(Path.Combine(dir.FullName, "Tools")) &&
                        File.Exists(Path.Combine(dir.FullName, "CLAUDE.md")))
                        return dir.FullName;
                    dir = dir.Parent;
                }
            }
            throw new ArgumentException("Could not find the repository root (a folder with Assets, Tools and CLAUDE.md). Pass --out.");
        }
    }
}
