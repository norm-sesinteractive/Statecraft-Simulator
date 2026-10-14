namespace Statecraft.Sim.Core
{
    /// <summary>Kinds of notable change the simulation reports. Each alarm has a matching all-clear.</summary>
    public enum SimEventKind
    {
        UnrestRising,
        UnrestEased,
        SufferingSevere,
        SufferingEased,
        CrimeWave,
        CrimeEased,
    }
}
