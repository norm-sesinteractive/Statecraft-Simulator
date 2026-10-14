namespace Statecraft.Sim.Society
{
    /// <summary>
    /// The seven drives the hidden hand steers (Norm's pitch, Docs/design/VICES.md). The numeric
    /// order is stable and indexes every <see cref="ViceVector"/>; never reorder.
    /// </summary>
    public enum Vice
    {
        Wrath = 0,
        Sloth = 1,
        Greed = 2,
        Lust = 3,
        Pride = 4,
        Envy = 5,
        Gluttony = 6,
    }
}
