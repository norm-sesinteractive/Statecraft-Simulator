using Statecraft.Sim.Core;

namespace Statecraft.Sim.Institutions
{
    /// <summary>
    /// How hard the courts strike. 0 pardons all but the most guilty; 1 cracks down ruthlessly.
    /// Severity deters crime but breeds fear (Norm's pitch: the pardon-versus-crackdown trade).
    /// </summary>
    public sealed class LawPolicy
    {
        double severity;

        internal LawPolicy(double severity) => Severity = severity;

        /// <summary>Court severity, 0 to 1.</summary>
        public double Severity
        {
            get => severity;
            internal set => severity = SimMath.Clamp01(double.IsNaN(value) ? 0.5 : value);
        }
    }
}
