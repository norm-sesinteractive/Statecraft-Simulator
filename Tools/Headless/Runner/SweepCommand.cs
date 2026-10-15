using System;
using System.Collections.Generic;
using System.Globalization;
using Statecraft.Sim.Commands;
using Statecraft.Sim.Core;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Scenarios;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.SimRunner
{
    /// <summary>
    /// The "sweep" command: pushes one lever of the home nation to an extreme at a time and reports how far each
    /// outcome moved at that lever's own horizon (media 1 year, courts 1 year, schools 25 years), against an
    /// untouched run of the same length. Answers "does every lever matter within its time horizon?"
    /// </summary>
    internal static class SweepCommand
    {
        sealed class Probe
        {
            public Probe(string name, int years, Action<Simulation> apply)
            {
                Name = name;
                Years = years;
                Apply = apply;
            }

            public string Name { get; }
            public int Years { get; }
            public Action<Simulation> Apply { get; }
        }

        public static int Execute(Options options)
        {
            ulong seed = options.GetULong("seed", 1);
            var probes = new List<Probe>();
            for (int v = 0; v < ViceVector.Count; v++)
            {
                int vice = v;
                probes.Add(new Probe("media all " + (Vice)v, 1, sim => sim.Enqueue(new SetMediaMix(0, OneHot(MediaMix.CategoryCount, vice)))));
            }
            for (int v = 0; v < ViceVector.Count; v++)
            {
                int vice = v;
                probes.Add(new Probe("media tilt " + (Vice)v, 1, sim => sim.Enqueue(new SetMediaMix(0, Tilt(MediaMix.CategoryCount, vice)))));
            }
            probes.Add(new Probe("media all Neutral", 1, sim => sim.Enqueue(new SetMediaMix(0, OneHot(MediaMix.CategoryCount, MediaMix.NeutralIndex)))));
            probes.Add(new Probe("courts 0 (pardon)", 1, sim => sim.Enqueue(new SetLawSeverity(0, 0))));
            probes.Add(new Probe("courts 1 (ruthless)", 1, sim => sim.Enqueue(new SetLawSeverity(0, 1))));
            for (int f = 0; f < Curriculum.FocusCount; f++)
            {
                int focus = f;
                probes.Add(new Probe("school all " + (CurriculumFocus)f, 25, sim => sim.Enqueue(new SetCurriculum(0, OneHot(Curriculum.FocusCount, focus)))));
                probes.Add(new Probe("school tilt " + (CurriculumFocus)f, 25, sim => sim.Enqueue(new SetCurriculum(0, Tilt(Curriculum.FocusCount, focus)))));
            }

            Console.WriteLine($"Lever sweep on the home nation (three-nation scenario, seed {seed}). Deltas against an untouched run");
            Console.WriteLine("of the same length. Contentment and unrest per generation are youth/adult/elder.");
            Console.WriteLine();
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,-26} {1,3} {2,8} {3,7} {4,7} {5,7} {6,7}  {7,-20} {8}",
                "Probe", "Yrs", "Content", "Crime", "Unrest", "Susp", "Aggr", "Contentment Y/A/E", "Largest frustration change"));

            var baselines = new Dictionary<int, Simulation>();
            foreach (var probe in probes)
            {
                if (!baselines.TryGetValue(probe.Years, out var baseline))
                {
                    baseline = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), seed);
                    baseline.Advance(probe.Years * SimDate.MonthsPerYear);
                    baselines[probe.Years] = baseline;
                }
                var simulation = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), seed);
                probe.Apply(simulation);
                simulation.Advance(probe.Years * SimDate.MonthsPerYear);

                var a = simulation.World.Nations[0];
                var b = baseline.World.Nations[0];
                var sa = a.Society;
                var sb = b.Society;
                int worst = 0;
                for (int v = 1; v < ViceVector.Count; v++)
                    if (Math.Abs(sa.Frustration[v] - sb.Frustration[v]) > Math.Abs(sa.Frustration[worst] - sb.Frustration[worst])) worst = v;
                string generations = string.Format(CultureInfo.InvariantCulture, "{0:+0.00;-0.00}/{1:+0.00;-0.00}/{2:+0.00;-0.00}",
                    sa.GetCohort(Cohort.Youth).Contentment - sb.GetCohort(Cohort.Youth).Contentment,
                    sa.GetCohort(Cohort.Adult).Contentment - sb.GetCohort(Cohort.Adult).Contentment,
                    sa.GetCohort(Cohort.Elder).Contentment - sb.GetCohort(Cohort.Elder).Contentment);
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "{0,-26} {1,3} {2,8:+0.00;-0.00} {3,7:+0.00;-0.00} {4,7:+0.00;-0.00} {5,7:+0.00;-0.00} {6,7:+0.00;-0.00}  {7,-20} {8} {9:+0.00;-0.00}",
                    probe.Name, probe.Years, sa.Contentment - sb.Contentment, sa.Crime - sb.Crime, sa.Unrest - sb.Unrest,
                    a.Government.Suspicion - b.Government.Suspicion, sa.Aggression - sb.Aggression, generations,
                    (Vice)worst, sa.Frustration[worst] - sb.Frustration[worst]));
            }
            return 0;
        }

        // Weight 3 on one category, 1 on the rest: a moderate setting below the concentration thresholds.
        static double[] Tilt(int length, int index)
        {
            var weights = new double[length];
            for (int i = 0; i < length; i++) weights[i] = i == index ? 3 : 1;
            return weights;
        }

        static double[] OneHot(int length, int index)
        {
            var weights = new double[length];
            weights[index] = 1;
            return weights;
        }
    }
}
