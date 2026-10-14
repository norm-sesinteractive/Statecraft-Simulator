using Statecraft.Sim.Nations;

namespace Statecraft.Sim.Institutions
{
    /// <summary>
    /// The levers of one nation that the hidden hand can pull. Politics joins this set when its
    /// design is settled (Docs/design/INSTITUTIONS.md).
    /// </summary>
    public sealed class InstitutionSet
    {
        internal InstitutionSet(NationSetup setup)
        {
            Media = new MediaMix(setup.MediaWeights);
            Curriculum = new Curriculum(setup.CurriculumWeights);
            Law = new LawPolicy(setup.LawSeverity);
        }

        public MediaMix Media { get; }

        public Curriculum Curriculum { get; }

        public LawPolicy Law { get; }
    }
}
