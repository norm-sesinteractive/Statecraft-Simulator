# Roadmap

> **Owns:** milestones and their checklists. **Budget:** 250 lines.
> Milestones after M0 are [PROPOSED] until Norm accepts the order. Tick items as they ship. When every box in
> a milestone is ticked, replace the milestone with one line under "Done".

## Done

(none yet)

## M0 Foundations (2026-10-14)

- [x] Unity 6.3 URP project, mobile-first settings, trimmed packages
- [x] Assemblies: Sim (pure C#), Game, UI, Editor, Sim.Tests
- [x] Sim core model v0 with 43 tests; headless harness and runner
- [x] Generated scene (MainSceneBuilder), dev HUD, placeholder map
- [x] Self-updating docs: CLAUDE.md, Docs/, decisions, doctool, hooks, skills, rules
- [ ] Norm: first editor open, Play check, first commit (STATUS.md)

## M1 Core loop prototype: one nation, deep [PROPOSED]

Goal: steering one nation is interesting for 30 minutes, with real danger of losing it from within.
- [x] Design session: no goals, defeats, suspicion, politics, reach (D-0018 to D-0022, 2026-10-14)
- [ ] Politics v1 (D-0021): government type lever; Democracy, Monarchy, Communism, Military junta, Theocracy with traits;
      fomented revolutions; democratic votes started by the player or the people; emergent revolts that install
      the type the people are most aligned with (D-0024)
- [ ] Contentment and unrest per generation (D-0032); any generation can revolt, with its own consequences
- [ ] Suspicion v1 (D-0020): government and shadow tiers, drivers and dampers, leak between tiers, venting by revolution
- [ ] Defeat v1 (D-0019): a revolt that topples the hidden hand; game over when the home nation is lost (Q-017)
- [ ] Puppet government v1: decisions it makes on its own (crackdowns, reforms), driven by society state
- [ ] Events v1 with consequences (revolt, revolution, crime wave, whistleblowers), as text keys for Norm
- [ ] Balance pass with the runner: unrest reachable; each lever matters within its own time horizon
- [ ] Explanation ("why?") in the real UI, not only the dev HUD
- [ ] Text catalog: a key for every player-facing string, values from Norm
- [ ] Verify RESEARCH.md citations before any reach game text

## M2 Nations and geography [PROPOSED]

- [ ] Map model: regions with terrain and resources; nation territory
- [ ] Population dynamics: births, deaths, cohort sizes (Lust, food, Medicine, war); starvation as a defeat (D-0019)
- [ ] Economy v1: production, consumption (Gluttony), wealth (Greed), resource access by terrain
- [ ] Map presentation: placeholder regions, then Norm's Blender assets
- [ ] World generation: procedural or authored (Q-004)

## M3 The world stage [PROPOSED]

- [ ] Trade between nations
- [ ] Culture through trade imports (D-0025) and cultural conquest as a defeat
- [ ] Relations between peoples, -100 to 100 (D-0026)
- [ ] War: puppet declarations, abstract fronts, territory, war weariness; military conquest as a defeat
- [ ] Diplomacy, only as much as the design needs
- [ ] Rival hands with personalities; the player gains nations by conquest (D-0022)

## M4 Goals and structure [PROPOSED]

- [ ] Scenarios, win and loss conditions, game length (Q-001, Q-011)
- [ ] Save and load (sim snapshot, RNG state, command log)
- [ ] Onboarding and tutorial

## M5 Mobile production [PROPOSED]

- [ ] Shipping UI (touch-first) with Norm's text and icons
- [ ] Device performance and battery pass; Android builds on device, then iOS
- [ ] Settings, accessibility, localization readiness
- [ ] Store preparation (Android first)
