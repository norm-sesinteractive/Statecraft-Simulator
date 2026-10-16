using Statecraft.Sim.Economy;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Nations
{
    /// <summary>
    /// One country: its identity, the institutions the hidden hand can steer, the society those institutions
    /// act on, and its government (the hand's shield). Its rulers act on their own; the player never orders them.
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
            Government = new GovernmentState(setup.Government);
            Economy = new EconomyState(setup);
        }

        /// <summary>Position in <see cref="Core.World.Nations"/>; the nation's id.</summary>
        public int Index { get; }

        /// <summary>Display-name key; holds a placeholder such as "[NATION_1]" until the text catalog exists.</summary>
        public string NameKey { get; }

        /// <summary>Whether the hidden hand holds this nation's levers. Turns false if a revolt topples it.</summary>
        public bool PlayerControlled { get; internal set; }

        public InstitutionSet Institutions { get; }

        public SocietyState Society { get; }

        public GovernmentState Government { get; }

        /// <summary>Land, work, goods, food and wealth (D-0036).</summary>
        public EconomyState Economy { get; }
    }
}
