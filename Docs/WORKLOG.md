# Worklog

> **Owns:** what each session did, newest first. **Budget:** 10 entries; older ones move to `Docs/archive/` with
> `python Tools/docs/doctool.py rotate-worklog`. Format: [DOCS_GUIDE.md](DOCS_GUIDE.md), "Worklog entry".
> Narrative only: decisions go to `decisions/`, open work to STATUS.md and ROADMAP.md.

## 2026-10-14: session 1, project foundations

- Created the Unity 6000.3.13f1 project in batch mode from the Universal 3D template; removed template samples;
  trimmed packages (D-0016); applied mobile-first player settings from code (D-0017).
- Recorded Norm's answers: mobile first (D-0002), Norm runs git (D-0003), .blend outside git (D-0004), Norm
  writes game text (D-0005).
- Built the pure C# sim, model v0 (D-0006, D-0010 to D-0013): 43 tests passing headless and in Unity; the
  headless harness and SimRunner (D-0007).
- Built the Unity side: GameRoot, SimulationHost, WorldView placeholder, DevHud on UI Toolkit (D-0008),
  MainSceneBuilder and ProjectSetup (D-0009).
- Built the docs system (D-0014): CLAUDE.md, Docs/, decision records, doctool, hooks, skills, path-scoped rules.
- Runner: baseline is stable; "stoke-then-channel" shows frustration building for decades, then easing a
  generation after schools switch to Military. Unrest never rose (noted in STATUS).
- Placeholders added for Norm to replace: `[NATION_1]`, `[NATION_2]`, `[NATION_3]` (scenario nation names).
- Subagents: one Sonnet agent researched Claude Code hook and skill formats.
- Pick up at STATUS.md, "Next up".
