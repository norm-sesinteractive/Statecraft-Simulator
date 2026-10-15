# Worklog

> **Owns:** what each session did, newest first. **Budget:** 10 entries; older ones move to `Docs/archive/` with
> `python Tools/docs/doctool.py rotate-worklog`. Format: [DOCS_GUIDE.md](DOCS_GUIDE.md), "Worklog entry".
> Narrative only: decisions go to `decisions/`, open work to STATUS.md and ROADMAP.md.

## 2026-10-15: session 3, balance pass 1 (suspicion overload)

- Norm: the Politics tab works, but suspicion is "nearly impossible to overload". Measured first: new runner option
  `--why` (ledger terms behind a value) and scripts propaganda, monoculture, extremist, whiplash.
- Findings: concentration terms capped too low, "imbalance" diluted across seven vices, and suspicion barely fed
  unrest, so even total suspicion never caused the revolt that topples the hand.
- Changes (D-0033, Proposed): stronger concentration, cultural drift against each nation's tradition, faster
  suspicion and leak, suspicion and shadow suspicion feed unrest, shadow resets when the hand falls.
- Six balance-target tests with default tuning (63 tests total, all pass). Dev HUD: "Only" and "Even" buttons.
- Placeholders added for Norm: none.
- Pick up at: STATUS.md, "Next up" (Norm's feel check first).

## 2026-10-14: session 2, design rounds and politics model v1

- Design with Norm, recorded verbatim in PITCH.md and as D-0018 to D-0032: no goals, four defeats, two-tier
  suspicion, the government as a shield, reach and rivals, revolts by alignment, culture through trade imports with
  educational overlap and a Commerce floor, infrastructure as transit efficiency, relations between peoples,
  contentment and revolts per generation. New doc: Docs/design/HIDDEN_HAND.md.
- Built model v1: `Statecraft.Sim.Politics` (GovernmentType, GovernmentState), PoliticsSystem, per-cohort contentment,
  unrest and schooling, FomentRevolution and CallVote commands, lever-shock tracking, IsGameOver, ledger by cohort.
- 14 new tests (57 total, all pass headless). Runner: government and suspicion columns, four new scripts, `--trace`.
- Balance changes after runner experiments: revolt threshold 0.5 to 0.3, unrest from suffering 0.8 to 1.2, stronger
  fomenting (MODEL.md, "Observations"). A stale runner build first looked like a balance bug; LESSONS has the trap.
- Dev HUD: Politics tab (foment, vote; offered only when allowed), generation rows, suspicion meters, game-over banner.
  Not yet seen in the editor.
- Placeholders added for Norm: none (dev HUD text only).
- Pick up at: STATUS.md, "Next up".

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
