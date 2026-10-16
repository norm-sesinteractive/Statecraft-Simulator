namespace Statecraft.Sim.Core
{
    /// <summary>How the player's home nation was lost (D-0019), or None while the game goes on.</summary>
    public enum Defeat
    {
        None,

        /// <summary>A revolt went past the government and toppled the hidden hand.</summary>
        HiddenHandOverthrown,

        /// <summary>A famine killed so many of the people that the nation collapsed (D-0036).</summary>
        Starvation,
    }
}
