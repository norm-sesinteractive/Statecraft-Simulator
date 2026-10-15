namespace Statecraft.Sim.Tuning
{
    /// <summary>Coefficients for how airtime stirs mood now and shapes culture over decades.</summary>
    public sealed class MediaTuning
    {
        /// <summary>Mood shift (on the 0-1 vice scale) when one vice gets all the airtime.</summary>
        public double MoodGain = 0.5;

        /// <summary>Time constant in months for mood to follow the airtime mix. Short: media works fast.</summary>
        public double MoodTauMonths = 4;

        /// <summary>Standard deviation of the monthly news-cycle noise added to each mood value. 0 disables it.</summary>
        public double MoodNoise = 0.01;

        /// <summary>Monthly disposition drift of youth per unit of airtime push (youth are the most susceptible).</summary>
        public double CultivationYouth = 0.004;

        /// <summary>Monthly disposition drift of adults per unit of airtime push.</summary>
        public double CultivationAdult = 0.0015;

        /// <summary>Monthly disposition drift of elders per unit of airtime push.</summary>
        public double CultivationElder = 0.0005;
    }
}
