using System.Runtime.CompilerServices;

// Tests read and seed internal state directly; everything else goes through the public API.
[assembly: InternalsVisibleTo("Statecraft.Sim.Tests")]
