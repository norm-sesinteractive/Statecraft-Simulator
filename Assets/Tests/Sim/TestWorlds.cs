using System;
using Statecraft.Sim.Core;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Nations;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Tests
{
    /// <summary>Builders for small test worlds. Mood and harvest noise are off unless a test turns them on, so results are exact.</summary>
    internal static class TestWorlds
    {
        public static Simulation Single(Action<NationSetup> configure = null, Action<SimTuning> tune = null, ulong seed = 7)
        {
            var setup = new NationSetup { NameKey = "[TEST]", PlayerControlled = true };
            configure?.Invoke(setup);
            return new Simulation(new[] { setup }, QuietTuning(tune), seed);
        }

        /// <summary>Default tuning with mood and harvest noise off, optionally adjusted.</summary>
        public static SimTuning QuietTuning(Action<SimTuning> tune = null)
        {
            var tuning = SimTuning.CreateDefault();
            tuning.Media.MoodNoise = 0;
            tuning.Economy.HarvestNoise = 0;
            tune?.Invoke(tuning);
            return tuning;
        }

        /// <summary>Media weights that give all airtime to one vice.</summary>
        public static double[] AllAirtimeOn(Vice vice)
        {
            var weights = new double[MediaMix.CategoryCount];
            weights[(int)vice] = 1;
            return weights;
        }

        /// <summary>Curriculum weights that put every school hour into one focus.</summary>
        public static double[] OnlyFocus(CurriculumFocus focus)
        {
            var weights = new double[Curriculum.FocusCount];
            weights[(int)focus] = 1;
            return weights;
        }

        public static int Years(int years) => years * SimDate.MonthsPerYear;

        public static SocietyState Society(Simulation simulation, int nation = 0) => simulation.World.Nations[nation].Society;
    }
}
