using System.Collections.Generic;
using Statecraft.Sim.Nations;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Scenarios
{
    /// <summary>
    /// Built-in starting worlds. Nation names are placeholders until Norm names them. Weight
    /// arrays follow the orders in <see cref="NationSetup"/>.
    /// </summary>
    public static class ScenarioLibrary
    {
        /// <summary>
        /// The player's home nation at human-nature defaults, plus two contrasting rivals: a proud,
        /// wrathful warrior culture under harsh courts, and a comfortable, indulgent one under lenient courts.
        /// </summary>
        public static IReadOnlyList<NationSetup> ThreeNations() => new[]
        {
            new NationSetup { NameKey = "[NATION_1]", PlayerControlled = true },
            new NationSetup
            {
                NameKey = "[NATION_2]",
                Disposition = new ViceVector(0.60, 0.20, 0.40, 0.35, 0.55, 0.45, 0.30),
                //               Wrath Sloth Greed Lust Pride Envy Glut  Neutral
                MediaWeights = new[] { 3.0, 0.5, 1.0, 0.5, 2.0, 1.0, 0.5, 1.0 },
                //                    Agri Mil  Med  Infra Sci  Comm Arts
                CurriculumWeights = new[] { 0.2, 0.5, 0.0, 0.2, 0.1, 0.0, 0.0 },
                LawSeverity = 0.8,
                Government = GovernmentType.MilitaryJunta,
            },
            new NationSetup
            {
                NameKey = "[NATION_3]",
                Disposition = new ViceVector(0.20, 0.60, 0.35, 0.50, 0.30, 0.30, 0.60),
                MediaWeights = new[] { 0.5, 2.0, 1.0, 2.0, 0.5, 0.5, 3.0, 1.0 },
                CurriculumWeights = new[] { 0.4, 0.0, 0.3, 0.0, 0.0, 0.0, 0.3 },
                LawSeverity = 0.2,
                Government = GovernmentType.Monarchy,
            },
        };

        /// <summary>One player-controlled nation at defaults: the baseline for tests and experiments.</summary>
        public static IReadOnlyList<NationSetup> SingleNation() => new[]
        {
            new NationSetup { NameKey = "[NATION_1]", PlayerControlled = true },
        };
    }
}
