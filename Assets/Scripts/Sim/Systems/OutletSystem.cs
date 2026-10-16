using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Society;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// How much of each vice a nation can channel productively: a base every society has, what the adult
    /// workforce was trained to do, what the government type channels or starves, and luxuries as a venue
    /// for Greed, Lust and Gluttony (comfort against expectation, D-0036). Other venues (an army at war,
    /// open markets) join in a later model version.
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
                double comfort = nations[n].Economy.Comfort;

                for (int v = 0; v < ViceVector.Count; v++)
                {
                    double outlet =
                        ledger.Add(n, Quantity.Outlet, v, LedgerNames.Base, tuning.BaseOutlet[v]) +
                        ledger.Add(n, Quantity.Outlet, v, LedgerNames.Channels[v], tuning.ChannelGain * adult.channeling[v]);
                    double bonus = traits.OutletBonus[v];
                    if (bonus != 0) outlet += ledger.Add(n, Quantity.Outlet, v, LedgerNames.Government, bonus);
                    double venue = tuning.LuxuryVenue[v];
                    if (venue != 0 && comfort != 0) outlet += ledger.Add(n, Quantity.Outlet, v, LedgerNames.Comfort, venue * comfort);
                    society.outlets[v] = SimMath.Clamp01(outlet);
                }
            }
        }
    }
}
