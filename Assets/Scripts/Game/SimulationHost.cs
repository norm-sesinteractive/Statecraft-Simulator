using System;
using Statecraft.Sim.Core;
using UnityEngine;

namespace Statecraft.Game
{
    /// <summary>
    /// Runs the simulation in real time with pause: turns unscaled frame time into whole monthly
    /// ticks at the chosen speed. The only caller of Simulation.Advance in the game, so frame rate
    /// never reaches the model; speed only changes how many months pass per second.
    /// </summary>
    public sealed class SimulationHost : MonoBehaviour
    {
        // Months per real second at each speed step; step 0 is paused.
        static readonly float[] SpeedSteps = { 0f, 1f, 2f, 4f, 8f };

        [Tooltip("Speed step at start: 0 paused, then 1, 2, 4 or 8 months per second.")]
        [SerializeField] int startSpeed = 1;

        [Tooltip("Cap on months simulated in one frame, so a hitch never stalls the game.")]
        [SerializeField] int maxTicksPerFrame = 8;

        float accumulator;

        public Simulation Simulation { get; private set; }

        public int SpeedIndex { get; private set; }

        public int SpeedCount => SpeedSteps.Length;

        public bool IsPaused => SpeedIndex == 0;

        /// <summary>Raised after one or more ticks (at most once per frame), and once when a simulation is attached.</summary>
        public event Action Ticked;

        public event Action SpeedChanged;

        public static float MonthsPerSecondAt(int speedIndex) => SpeedSteps[Mathf.Clamp(speedIndex, 0, SpeedSteps.Length - 1)];

        public void Attach(Simulation simulation)
        {
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
            accumulator = 0;
            SetSpeed(startSpeed);
            Ticked?.Invoke();
        }

        public void SetSpeed(int speedIndex)
        {
            SpeedIndex = Mathf.Clamp(speedIndex, 0, SpeedSteps.Length - 1);
            SpeedChanged?.Invoke();
        }

        /// <summary>Queues a lever change; it takes effect at the next tick.</summary>
        public void Enqueue(SimCommand command)
        {
            if (Simulation != null) Simulation.Enqueue(command);
        }

        void Update()
        {
            if (Simulation == null || IsPaused) return;

            accumulator += Time.unscaledDeltaTime * SpeedSteps[SpeedIndex];
            int ticks = Mathf.Min((int)accumulator, maxTicksPerFrame);
            if (ticks <= 0) return;

            accumulator = Mathf.Min(accumulator - ticks, maxTicksPerFrame);
            Simulation.Advance(ticks);
            Ticked?.Invoke();
        }

        // Mobile: leaving the app pauses the world (no offline progression in model v0).
        void OnApplicationPause(bool paused)
        {
            if (paused) SetSpeed(0);
        }
    }
}
