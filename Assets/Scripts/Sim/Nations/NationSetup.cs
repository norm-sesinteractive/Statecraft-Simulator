using Statecraft.Sim.Institutions;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Nations
{
    /// <summary>
    /// Plain data describing one nation at game start; a scenario is a list of these. Weight
    /// arrays need not sum to 1 (they are normalized into shares).
    /// </summary>
    public sealed class NationSetup
    {
        /// <summary>Display-name key or placeholder. Never player-facing prose (Norm writes all game text).</summary>
        public string NameKey = "[NATION]";

        /// <summary>Whether the hidden hand starts with its levers in this nation.</summary>
        public bool PlayerControlled;

        /// <summary>Starting disposition of every cohort, 0 to 1 per vice.</summary>
        public ViceVector Disposition = ViceVector.Uniform(0.35);

        /// <summary>Airtime weights: the seven vices in <see cref="Vice"/> order, then neutral programming.</summary>
        public double[] MediaWeights = MediaMix.EvenWeights();

        /// <summary>School weights in <see cref="CurriculumFocus"/> order.</summary>
        public double[] CurriculumWeights = Curriculum.EvenWeights();

        /// <summary>Court severity, 0 (pardon all but the most guilty) to 1 (ruthless crackdown).</summary>
        public double LawSeverity = 0.5;

        /// <summary>The government type in power at game start.</summary>
        public GovernmentType Government = GovernmentType.Democracy;

        public double YouthShare = 0.30;
        public double AdultShare = 0.55;
        public double ElderShare = 0.15;

        /// <summary>People alive at game start, split by the cohort shares above.</summary>
        public double Population = 1000000;

        /// <summary>Area of each terrain in <see cref="Economy.Terrain"/> order (abstract units until the map exists).</summary>
        public double[] LandAreas = DefaultLand();

        /// <summary>A balanced homeland: mostly plains, some forest and hills, a river valley, a coast, a few mountains.</summary>
        public static double[] DefaultLand() => new double[] { 35, 20, 15, 10, 0, 10, 10, 0 };
        //                                                      Plain For Hill Mtn Des Coast River Tundra
    }
}
