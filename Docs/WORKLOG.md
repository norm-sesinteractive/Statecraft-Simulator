# Worklog

> **Owns:** what each session did, newest first. **Budget:** 10 entries; older ones move to `Docs/archive/` with
> `python Tools/docs/doctool.py rotate-worklog`. Format: [DOCS_GUIDE.md](DOCS_GUIDE.md), "Worklog entry".
> Narrative only: decisions go to `decisions/`, open work to STATUS.md and ROADMAP.md.

## 2026-10-15: session 4, economy v1

- Norm: the game ends when the player's hand falls; another nation's hand falling is pegged (Q-026); balance is hard
  to judge without conquest and clearer relations. Norm's answers: full M2 economy first, whole-nation conquest,
  puppets decide trade and war, no rival hands yet (D-0035, accepted).
- Built economy v1 (D-0036, proposed): `Statecraft.Sim.Economy` (Terrain, Resource, EconomyState), EconomySystem,
  population numbers in GenerationSystem (births, deaths, land pressure, famine), hunger and materials hardship in
  contentment, comfort as a luxury venue for outlets, violence that kills, `Simulation.Defeat` (overthrow or
  starvation). Scenario nations got distinct lands and populations.
- Tuning found with the runner: labor-bound farming starved luxuries (raised food per worker, reserved 5% of labor
  for luxuries); a fixed growth gap sent [NATION_3] into Malthusian famine and revolts and [NATION_2] into endless
  decline (added crowding and open-land terms to births); farmers now plan against the year's harvest.
- 17 economy tests (83 total, all pass). One older test now switches off Sloth idleness, which is a separate effect.
- Runner: `--economy` view, `--why births|deaths|productivity|wealth`, scripts baby-boom, farmers, merchants, famine;
  PARAMETERS.md now labels per-resource arrays and prints the terrain yield table. Dev HUD: economy section,
  generation shares, a game-over line naming the defeat.
- Placeholders added for Norm: none (dev HUD text only).
- Pick up at: STATUS.md, "Next up" (Norm's look at economy v1 first).

## 2026-10-15: session 3, balance passes 1 and 2

- Norm: the Politics tab works, but suspicion is "nearly impossible to overload". Measured first: new runner option
  `--why` (ledger terms behind a value) and scripts propaganda, monoculture, extremist, whiplash.
- Findings: concentration terms capped too low, "imbalance" diluted across seven vices, and suspicion barely fed
  unrest, so even total suspicion never caused the revolt that topples the hand.
- Changes (D-0033, Proposed): stronger concentration, cultural drift against each nation's tradition, faster
  suspicion and leak, suspicion and shadow suspicion feed unrest, shadow resets when the hand falls.
- Six balance-target tests with default tuning (63 tests total, all pass). Dev HUD: "Only" and "Even" buttons.
- Norm approved the pace ("The pace feels right"); D-0033 accepted. Then balance pass 2: runner `sweep` (each lever
  extreme and 3:1, at its horizon) and a slow-burn script showed moderate play did nothing. Contentment now sums
  frustration per vice; stronger media and schooling; smaller unrest cushion (D-0034, Proposed). Three more
  balance tests (66 total). Two older tests now switch revolts off where revolts blur what they measure.
- Placeholders added for Norm: none.
- Pick up at: STATUS.md, "Next up" (Norm's feel check of pass 2 first).

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
