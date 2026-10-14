# D-0006: The simulation is pure C#, deterministic and tick-based (one tick is one month)

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Architecture

## Context
The game is a simulation first. Claude cannot see the game running, so the model must be verifiable by tests and
headless runs. Sol's Box proved the pattern.

## Decision
`Statecraft.Sim` has no UnityEngine reference (`noEngineReferences: true`). One `Simulation` object owns the
world, a seeded PCG32 generator, the command queue, the ledger and the event log. Systems run in a fixed order
each monthly tick. Commands in, events out, read-only state outside the assembly.

## Because
Testable in milliseconds, reproducible from a seed (saves, replays, regression tests), and buildable outside Unity
(D-0007). Monthly ticks suit social dynamics: 100 years is 1,200 ticks. The alternative, logic in MonoBehaviours,
cannot be tested headless and couples the model to frame rate.
