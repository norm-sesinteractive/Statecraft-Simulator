namespace Statecraft.Sim.Economy
{
    /// <summary>
    /// What a nation's land and people produce (D-0036). Food keeps people alive; materials (timber, stone, ore) are
    /// the needs of daily life; luxuries (spices, wine, silks, gems) are its comforts. Amounts are in units of one
    /// person's monthly food need. The order is fixed: per-resource arrays follow it.
    /// </summary>
    public enum Resource
    {
        Food,
        Materials,
        Luxuries,
    }
}
