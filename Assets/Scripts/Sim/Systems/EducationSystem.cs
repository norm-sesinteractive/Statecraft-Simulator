using System;
using Statecraft.Sim.Core;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Society;
using Statecraft.Sim.Tuning;

namespace Statecraft.Sim.Systems
{
    /// <summary>
    /// Education digs the channels. Schools train the youth cohort's channeling toward the curriculum and
    /// give it a schooling profile (what it was taught); both reach the adult workforce only as the young
    /// grow up.
    /// </summary>
    internal sealed class EducationSystem : ISimSystem
    {
        /// <summary>Channeling per vice that a curriculum trains toward (share-weighted rows of the focus table).</summary>
        public static ViceVector CurriculumTarget(Curriculum curriculum, EducationTuning tuning)
        {
            var target = ViceVector.Zero;
            int rows = Math.Min(Curriculum.FocusCount, tuning.FocusChanneling.Length);
            for (int f = 0; f < rows; f++)
                target = target + tuning.FocusChanneling[f] * curriculum.Share((CurriculumFocus)f);
            return target.Clamp01();
        }

        public void Tick(SimContext context)
        {
            var tuning = context.Tuning.Education;
            double alpha = SimMath.AlphaFromTau(tuning.SchoolTauMonths);
            var nations = context.World.Nations;

            for (int n = 0; n < nations.Count; n++)
            {
                var youth = nations[n].Society.cohorts[(int)Cohort.Youth];
                var curriculum = nations[n].Institutions.Curriculum;
                youth.channeling = ViceVector.Lerp(youth.channeling, CurriculumTarget(curriculum, tuning), alpha);
                for (int f = 0; f < Curriculum.FocusCount; f++)
                    youth.schooling[f] += (curriculum.Share((CurriculumFocus)f) - youth.schooling[f]) * alpha;
            }
        }
    }
}
