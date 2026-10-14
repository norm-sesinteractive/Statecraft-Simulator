using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// How much of each vice a nation can channel productively: a base every society has, plus
    /// what the adult workforce was trained to do. Venues (an army, markets, an arts scene) join
    /// in a later model version.
    /// </summary>
    internal sealed class OutletSystem : ISimSystem
    {
        public void Tick(SimContext context)
        {
            var tuning = context.Tuning.Outlets;
            var ledger = context.Ledger;
            var nations = context.World.Nations;

            for (int n = 0; n < nations.Count; n++)
            {
                var society = nations[n].Society;
                var adult = society.cohorts[(int)Cohort.Adult];

                for (int v = 0; v < ViceVector.Count; v++)
                {
                    double outlet =
                        ledger.Add(n, Quantity.Outlet, v, LedgerNames.Base, tuning.BaseOutlet[v]) +
                        ledger.Add(n, Quantity.Outlet, v, LedgerNames.Channels[v], tuning.ChannelGain * adult.channeling[v]);
                    society.outlets[v] = SimMath.Clamp01(outlet);
                }
            }
        }
    }
}
