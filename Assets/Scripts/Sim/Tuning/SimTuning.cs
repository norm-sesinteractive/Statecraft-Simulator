namespace Statecraft.Sim.Tuning
{
    /// <summary>
    /// Every tunable coefficient of the model, grouped by system. Values are provisional until a
    /// balance pass; the generated table in Docs/generated/PARAMETERS.md always shows what is live.
    /// </summary>
    public sealed class SimTuning
    {
        public MediaTuning Media = new MediaTuning();
        public EducationTuning Education = new EducationTuning();
        public GenerationTuning Generations = new GenerationTuning();
        public OutletTuning Outlets = new OutletTuning();
        public WellbeingTuning Wellbeing = new WellbeingTuning();
        public PoliticsTuning Politics = new PoliticsTuning();
        public EconomyTuning Economy = new EconomyTuning();

        public static SimTuning CreateDefault() => new SimTuning();
    }
}
