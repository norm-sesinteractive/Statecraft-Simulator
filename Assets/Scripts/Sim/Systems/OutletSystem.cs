using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// How much of each vice a nation can channel productively: a base every society has, what the adult
    /// workforce was trained to do, and what the government type channels or starves. Venues (an army,
    /// markets, an arts scene) join in a later model version.
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
                var traits = context.Tuning.Politics.Types[(int)nations[n].Government.type];

                for (int v = 0; v < ViceVector.Count; v++)
                {
                    double outlet =
                        ledger.Add(n, Quantity.Outlet, v, LedgerNames.Base, tuning.BaseOutlet[v]) +
                        ledger.Add(n, Quantity.Outlet, v, LedgerNames.Channels[v], tuning.ChannelGain * adult.channeling[v]);
                    double bonus = traits.OutletBonus[v];
                    if (bonus != 0) outlet += ledger.Add(n, Quantity.Outlet, v, LedgerNames.Government, bonus);
                    society.outlets[v] = SimMath.Clamp01(outlet);
                }
            }
        }
    }
}
