using Statecraft.Sim.Society;

namespace Statecraft.Sim.Tuning
{
    /// <summary>Coefficients for how much of each vice a society can channel productively.</summary>
    public sealed class OutletTuning
    {
        /// <summary>Outlet capacity every society has before any training (family life, ordinary work, feasts), per vice.</summary>
        public ViceVector BaseOutlet = ViceVector.Uniform(0.25);

        /// <summary>Outlet capacity added per unit of the adult cohort's trained channels.</summary>
        public double ChannelGain = 0.6;
    }
}
