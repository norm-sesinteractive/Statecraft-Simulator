---
paths:
  - "Assets/Scripts/Sim/**"
  - "Assets/Tests/**"
  - "Tools/Headless/**"
---

# Simulation rules (Statecraft.Sim)

- Pure C#: no UnityEngine (the asmdef enforces it). C# 9 and .NET Standard 2.1 only: no records, `init`,
  file-scoped namespaces or global usings.
- Determinism: randomness only from the Simulation's `SimRandom`. No `System.Random`, `DateTime.Now`, dependence
  on Dictionary order, or threads.
- State changes only inside systems (internal fields) or through `SimCommand`s; the public surface stays read-only.
- Every additive term of a computed target goes through `Ledger.Add` with a `LedgerNames` source. A new computed
  quantity gets a `Quantity` value.
- Every coefficient is a public field with an XML `<summary>` in `Tuning/*Tuning.cs`; no magic numbers in systems.
- A mechanic ships with a behavior test in Assets/Tests/Sim (NUnit 3.5 APIs only) and its section in
  Docs/simulation/MODEL.md.
- Verify: `dotnet test Tools/Headless/Statecraft.Headless.slnx --nologo -v q` (works with the editor open).
- After changing tuning or ledger terms, regenerate: `python Tools/docs/doctool.py generate --sim`.
