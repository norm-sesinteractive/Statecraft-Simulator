# D-0007: A headless .NET harness compiles the Unity sim and test sources

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Tooling

## Context
Unity batch mode needs the editor closed and takes 20 to 60 seconds per run. In InsignificAnts, compile checks
went through Unity-generated csproj files, which miss new files until Unity regenerates them.

## Decision
`Tools/Headless` holds SDK-style projects that compile `Assets/Scripts/Sim/**` and `Assets/Tests/Sim/**` in
place (netstandard2.1 and C# 9 for the sim, net10.0 with NUnit 3.14 for tests), plus a SimRunner console tool for
experiments and generated docs. Unity's Test Runner runs the same test files.

## Because
Seconds instead of minutes, works with the editor open, globs pick up new files automatically, and the compiler
settings match Unity's limits so incompatible code fails early. Alternative rejected: running every check through
Unity batch mode (slow, blocked whenever Norm has the editor open).
