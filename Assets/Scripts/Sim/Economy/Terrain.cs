namespace Statecraft.Sim.Economy
{
    /// <summary>
    /// Kinds of land. A nation holds an area of each; the terrain sets what the land can yield (D-0036). The order
    /// is fixed: arrays indexed by terrain (setup areas, the yield table) follow it.
    /// </summary>
    public enum Terrain
    {
        Plains,
        Forest,
        Hills,
        Mountains,
        Desert,
        Coast,
        RiverValley,
        Tundra,
    }
}
