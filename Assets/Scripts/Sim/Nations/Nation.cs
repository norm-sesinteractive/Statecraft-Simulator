using Statecraft.Sim.Institutions;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Nations
{
    /// <summary>
    /// One country: its identity, the institutions the hidden hand can steer, and the society
    /// those institutions act on. Its government will act on its own; the player never orders it.
    /// </summary>
    public sealed class Nation
    {
        internal Nation(int index, NationSetup setup)
        {
            Index = index;
            NameKey = setup.NameKey;
            PlayerControlled = setup.PlayerControlled;
            Institutions = new InstitutionSet(setup);
            Society = new SocietyState(setup);
        }

        /// <summary>Position in <see cref="Core.World.Nations"/>; the nation's id.</summary>
        public int Index { get; }

        /// <summary>Display-name key; holds a placeholder such as "[NATION_1]" until the text catalog exists.</summary>
        public string NameKey { get; }

        public bool PlayerControlled { get; }

        public InstitutionSet Institutions { get; }

        public SocietyState Society { get; }
    }
}
