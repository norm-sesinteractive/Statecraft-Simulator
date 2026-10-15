using System;
using Statecraft.Sim.Commands;
using Statecraft.Sim.Core;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Society;

namespace Statecraft.SimRunner
{
    /// <summary>
    /// Named, scripted player behavior for headless experiments: which levers the hidden hand pulls
    /// in its home nation, and in which year. The other nations keep their starting levers.
    /// </summary>
    internal static class LeverScripts
    {
        /// <summary>A named script and what it changes at the start of each simulated year.</summary>
        sealed class Script
        {
            public Script(string name, string description, Action<Simulation, int, int> onYearStart)
            {
                Name = name;
                Description = description;
                OnYearStart = onYearStart;
            }

            public string Name { get; }
            public string Description { get; }

            /// <summary>Called before each simulated year with (simulation, year, home nation index).</summary>
            public Action<Simulation, int, int> OnYearStart { get; }
        }

        static readonly Script[] All =
        {
            new Script("baseline", "Touch nothing; watch the three nations drift.", (sim, year, home) => { }),
            new Script("wrath-media", "From year 2 the home media airs nothing but Wrath.", (sim, year, home) =>
            {
                if (year == 2) sim.Enqueue(new SetMediaMix(home, Airtime(Vice.Wrath)));
            }),
            new Script("military-school", "From year 2 every school hour at home goes to Military.", (sim, year, home) =>
            {
                if (year == 2) sim.Enqueue(new SetCurriculum(home, Focus(CurriculumFocus.Military)));
            }),
            new Script("iron-fist", "From year 2 the home courts crack down ruthlessly (severity 1).", (sim, year, home) =>
            {
                if (year == 2) sim.Enqueue(new SetLawSeverity(home, 1));
            }),
            new Script("velvet-glove", "From year 2 the home courts pardon all but the most guilty (severity 0).", (sim, year, home) =>
            {
                if (year == 2) sim.Enqueue(new SetLawSeverity(home, 0));
            }),
            new Script("bread-and-circuses", "From year 2 home media airs Gluttony, Lust and Sloth; schools teach Agriculture and Arts.", (sim, year, home) =>
            {
                if (year != 2) return;
                var media = new double[MediaMix.CategoryCount];
                media[(int)Vice.Gluttony] = 3;
                media[(int)Vice.Lust] = 2;
                media[(int)Vice.Sloth] = 2;
                sim.Enqueue(new SetMediaMix(home, media));
                var school = new double[Curriculum.FocusCount];
                school[(int)CurriculumFocus.Agriculture] = 1;
                school[(int)CurriculumFocus.Arts] = 1;
                sim.Enqueue(new SetCurriculum(home, school));
            }),
            new Script("stoke-then-channel", "Years 2-29 Wrath media with Arts schools; from year 30 schools switch to Military. Frustration first, outlets a generation later.", (sim, year, home) =>
            {
                if (year == 2)
                {
                    sim.Enqueue(new SetMediaMix(home, Airtime(Vice.Wrath)));
                    sim.Enqueue(new SetCurriculum(home, Focus(CurriculumFocus.Arts)));
                }
                if (year == 30) sim.Enqueue(new SetCurriculum(home, Focus(CurriculumFocus.Military)));
            }),
            new Script("scholars", "From year 2 every school hour at home goes to Science: an educated people grows suspicious.", (sim, year, home) =>
            {
                if (year == 2) sim.Enqueue(new SetCurriculum(home, Focus(CurriculumFocus.Science)));
            }),
            new Script("false-revolution", "Scholars from year 2; whenever government suspicion passes 0.4, foment a revolution to whichever type the people like best.", (sim, year, home) =>
            {
                if (year == 2) sim.Enqueue(new SetCurriculum(home, Focus(CurriculumFocus.Science)));
                var government = sim.World.Nations[home].Government;
                if (government.Suspicion < 0.4) return;
                var target = BestAlternative(government);
                if (sim.CanFoment(home, target)) sim.Enqueue(new FomentRevolution(home, target));
            }),
            new Script("people-vote", "In year 3 the hand calls a vote for Monarchy; in year 20, for whatever the people like best.", (sim, year, home) =>
            {
                var government = sim.World.Nations[home].Government;
                if (year == 3 && sim.CanCallVote(home, GovernmentType.Monarchy)) sim.Enqueue(new CallVote(home, GovernmentType.Monarchy));
                var best = BestAlternative(government);
                if (year == 20 && sim.CanCallVote(home, best)) sim.Enqueue(new CallVote(home, best));
            }),
            new Script("propaganda", "From year 2 the home media airs nothing but Envy (one extreme lever).", (sim, year, home) =>
            {
                if (year == 2) sim.Enqueue(new SetMediaMix(home, Airtime(Vice.Envy)));
            }),
            new Script("monoculture", "From year 2 every school hour at home goes to Commerce (one extreme lever).", (sim, year, home) =>
            {
                if (year == 2) sim.Enqueue(new SetCurriculum(home, Focus(CurriculumFocus.Commerce)));
            }),
            new Script("extremist", "From year 2 all-Envy media, all-Science schools and ruthless courts (every lever at an extreme).", (sim, year, home) =>
            {
                if (year != 2) return;
                sim.Enqueue(new SetMediaMix(home, Airtime(Vice.Envy)));
                sim.Enqueue(new SetCurriculum(home, Focus(CurriculumFocus.Science)));
                sim.Enqueue(new SetLawSeverity(home, 1));
            }),
            new Script("whiplash", "From year 2 the home media flips every year between all-Wrath and all-Sloth.", (sim, year, home) =>
            {
                if (year >= 2) sim.Enqueue(new SetMediaMix(home, Airtime(year % 2 == 0 ? Vice.Wrath : Vice.Sloth)));
            }),
            new Script("slow-burn", "From year 2 moderate tilts only: media leans Wrath, Greed, Envy; schools lean Arts, Medicine, Agriculture (no outlets for them).", (sim, year, home) =>
            {
                if (year != 2) return;
                sim.Enqueue(new SetMediaMix(home, new[] { 2.0, 1, 2, 1, 1, 2, 1, 1 }));
                sim.Enqueue(new SetCurriculum(home, new[] { 2.0, 1, 2, 0.5, 1, 0.5, 2 }));
            }),
            new Script("tyranny", "From year 2 ruthless courts, nothing but Wrath on the air, and Arts schools: frustrated youth and revolts.", (sim, year, home) =>
            {
                if (year != 2) return;
                sim.Enqueue(new SetLawSeverity(home, 1));
                sim.Enqueue(new SetMediaMix(home, Airtime(Vice.Wrath)));
                sim.Enqueue(new SetCurriculum(home, Focus(CurriculumFocus.Arts)));
            }),
        };

        static GovernmentType BestAlternative(GovernmentState government)
        {
            var best = government.Type == GovernmentType.Democracy ? GovernmentType.Monarchy : GovernmentType.Democracy;
            for (int k = 0; k < GovernmentState.TypeCount; k++)
            {
                var type = (GovernmentType)k;
                if (type != government.Type && government.Alignment(type) > government.Alignment(best)) best = type;
            }
            return best;
        }

        public static bool TryGet(string name, out Action<Simulation, int, int> onYearStart, out string description)
        {
            foreach (var script in All)
            {
                if (!string.Equals(script.Name, name, StringComparison.OrdinalIgnoreCase)) continue;
                onYearStart = script.OnYearStart;
                description = script.Description;
                return true;
            }
            onYearStart = null;
            description = null;
            return false;
        }

        /// <summary>Every script name, in listing order.</summary>
        public static string[] Names()
        {
            var names = new string[All.Length];
            for (int i = 0; i < All.Length; i++) names[i] = All[i].Name;
            return names;
        }

        public static void PrintList()
        {
            foreach (var script in All) Console.WriteLine($"  {script.Name,-20} {script.Description}");
        }

        static double[] Airtime(Vice vice)
        {
            var weights = new double[MediaMix.CategoryCount];
            weights[(int)vice] = 1;
            return weights;
        }

        static double[] Focus(CurriculumFocus focus)
        {
            var weights = new double[Curriculum.FocusCount];
            weights[(int)focus] = 1;
            return weights;
        }
    }
}
