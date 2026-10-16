using System;
using System.Collections.Generic;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Nations;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Society;
using Statecraft.Sim.Systems;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Core
{
    /// <summary>
    /// The whole game state plus the monthly tick. Owns the world, the random generator, the
    /// command queue, the ledger and the event log; nothing in the sim is static. In the game,
    /// Unity's SimulationHost is the only caller of <see cref="Advance"/>; tests and the headless
    /// runner drive it directly.
    /// </summary>
    public sealed class Simulation
    {
        readonly List<SimCommand> pending = new List<SimCommand>();
        readonly ISimSystem[] systems;
        readonly SimContext context;

        public Simulation(IReadOnlyList<NationSetup> setups, SimTuning tuning, ulong seed)
        {
            Tuning = tuning ?? SimTuning.CreateDefault();
            Seed = seed;
            World = new World(setups);
            Random = new SimRandom(seed);
            Ledger = new Ledger();
            Events = new EventLog();
            context = new SimContext(World, Tuning, Random, Ledger, Events);

            // The order is part of the model: see Docs/simulation/MODEL.md, "Tick pipeline".
            systems = new ISimSystem[]
            {
                new MediaSystem(),
                new EducationSystem(),
                new EconomySystem(),
                new GenerationSystem(),
                new OutletSystem(),
                new WellbeingSystem(),
                new PoliticsSystem(),
            };

            Settle();
        }

        public World World { get; }

        public SimTuning Tuning { get; }

        public ulong Seed { get; }

        /// <summary>The month about to be simulated. Starts at Year 1, Month 1.</summary>
        public SimDate Date { get; private set; }

        /// <summary>Every term behind the most recent tick's computed values.</summary>
        public Ledger Ledger { get; }

        public EventLog Events { get; }

        internal SimRandom Random { get; }

        /// <summary>Commands waiting for the next tick.</summary>
        public int PendingCommandCount => pending.Count;

        /// <summary>True once the home nation is lost (D-0019, D-0023); the game ends there.</summary>
        public bool IsGameOver => Defeat != Defeat.None;

        /// <summary>How the home nation was lost, or None.</summary>
        public Defeat Defeat
        {
            get
            {
                int home = World.PlayerHomeIndex;
                if (home < 0) return Defeat.None;
                var nation = World.Nations[home];
                if (nation.Government.HiddenHandOverthrown) return Defeat.HiddenHandOverthrown;
                if (nation.Economy.Starved) return Defeat.Starvation;
                return Defeat.None;
            }
        }

        /// <summary>Whether the player may foment a revolution toward <paramref name="target"/> in a nation now.</summary>
        public bool CanFoment(int nationIndex, GovernmentType target)
        {
            if (nationIndex < 0 || nationIndex >= World.Nations.Count) return false;
            var nation = World.Nations[nationIndex];
            return nation.PlayerControlled && nation.Government.CanFoment(target);
        }

        /// <summary>Whether the player may call a vote toward <paramref name="target"/> (only where the type can vote).</summary>
        public bool CanCallVote(int nationIndex, GovernmentType target) =>
            CanFoment(nationIndex, target) && Tuning.Politics.Types[(int)World.Nations[nationIndex].Government.Type].CanVote;

        /// <summary>Queues a lever change; it applies at the start of the next tick.</summary>
        public void Enqueue(SimCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            pending.Add(command);
        }

        /// <summary>Simulates <paramref name="ticks"/> months.</summary>
        public void Advance(int ticks = 1)
        {
            if (ticks < 0) throw new ArgumentOutOfRangeException(nameof(ticks));
            for (int i = 0; i < ticks; i++) Step();
        }

        void Step()
        {
            context.Date = Date;
            for (int i = 0; i < pending.Count; i++) pending[i].Apply(context);
            pending.Clear();

            Ledger.Clear();
            for (int i = 0; i < systems.Length; i++) systems[i].Tick(context);
            Date = Date.Next;
        }

        // Starts every stock at its equilibrium for the starting levers, so a new game opens
        // without a transient. Uses no randomness.
        void Settle()
        {
            double alpha = SimMath.AlphaFromTau(Tuning.Education.SchoolTauMonths);

            var nations = World.Nations;
            for (int n = 0; n < nations.Count; n++)
            {
                var society = nations[n].Society;
                // Newborns as a share of the youth each month, at the base birth rate (see GenerationSystem).
                double youthPeople = society.cohorts[(int)Cohort.Youth].population;
                double birth = youthPeople > 0
                    ? SimMath.Clamp01(society.cohorts[(int)Cohort.Adult].population * Tuning.Generations.BirthsPerAdult / SimDate.MonthsPerYear / youthPeople)
                    : 0;
                // Steady state of "school pulls youth toward the target, newborns dilute it" (see EducationSystem, GenerationSystem).
                double trainedFraction = alpha / (1.0 - (1.0 - alpha) * (1.0 - birth));
                ViceVector trained = EducationSystem.CurriculumTarget(nations[n].Institutions.Curriculum, Tuning.Education) * trainedFraction;
                society.cohorts[(int)Cohort.Youth].channeling = trained * (1.0 - birth);
                society.cohorts[(int)Cohort.Adult].channeling = trained;
                society.cohorts[(int)Cohort.Elder].channeling = trained;
                society.mood = MediaSystem.MoodTarget(nations[n].Institutions.Media, Tuning.Media);
                society.disposition = GenerationSystem.NationalDisposition(society);
                for (int c = 0; c < society.cohorts.Length; c++)
                    for (int f = 0; f < Institutions.Curriculum.FocusCount; f++)
                        society.cohorts[c].schooling[f] = nations[n].Institutions.Curriculum.Share((Institutions.CurriculumFocus)f);
            }

            context.Date = Date;
            new OutletSystem().Tick(context);
            for (int iteration = 0; iteration < 50; iteration++)
            {
                for (int n = 0; n < nations.Count; n++)
                {
                    EconomySystem.Evaluate(context, nations[n], settle: true);
                    WellbeingSystem.Evaluate(context, nations[n], settle: true);
                    PoliticsSystem.Evaluate(context, nations[n], settle: true);
                }
            }
            Ledger.Clear();
        }
    }
}
