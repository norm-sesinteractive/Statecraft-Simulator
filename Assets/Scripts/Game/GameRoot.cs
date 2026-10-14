using Statecraft.Sim.Core;
using Statecraft.Sim.Scenarios;
using Statecraft.Sim.Tuning;
using UnityEngine;

namespace Statecraft.Game
{
    /// <summary>
    /// Composition root: builds the simulation for the starting scenario and hands it to the
    /// <see cref="SimulationHost"/>. The only place the game constructs simulation objects.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class GameRoot : MonoBehaviour
    {
        [SerializeField] SimulationHost host;

        [Tooltip("Seed for the simulation's random generator. Same seed and same lever changes give the same game.")]
        [SerializeField] int seed = 1;

        void Awake()
        {
            if (host == null) host = GetComponent<SimulationHost>();
            if (host == null)
            {
                Debug.LogError("[Statecraft] GameRoot needs a SimulationHost.", this);
                enabled = false;
                return;
            }

            var simulation = new Simulation(ScenarioLibrary.ThreeNations(), SimTuning.CreateDefault(), (ulong)Mathf.Max(0, seed));
            host.Attach(simulation);
        }
    }
}
