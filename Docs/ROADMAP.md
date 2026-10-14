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

Goal: steering one nation is interesting for 30 minutes.
- [ ] Design session answers Q-001, Q-002, Q-003, Q-006, Q-007, Q-013
- [ ] Player constraints v1 (influence budget, exposure, or both), if chosen
- [ ] Politics v1: government forms and how they shift (ideology from schools, unrest, reform or revolution)
- [ ] Puppet government v1: decisions it makes on its own (crackdowns, reforms), driven by society state
- [ ] Events v1 with consequences (revolt, golden age, crime wave), as text keys for Norm
- [ ] Balance pass with the runner: unrest reachable; each lever matters within its own time horizon
- [ ] Explanation ("why?") in the real UI, not only the dev HUD
- [ ] Text catalog: a key for every player-facing string, values from Norm
- [ ] Verify RESEARCH.md citations before any reach game text

## M2 Nations and geography [PROPOSED]

- [ ] Map model: regions with terrain and resources; nation territory
- [ ] Population dynamics: births, deaths, cohort sizes (Lust, food, Medicine, war)
- [ ] Economy v1: production, consumption (Gluttony), wealth (Greed), resource access by terrain
- [ ] Map presentation: placeholder regions, then Norm's Blender assets
- [ ] World generation: procedural or authored (Q-004)

## M3 The world stage [PROPOSED]

- [ ] Trade between nations
- [ ] Culture flow (Lust and Pride export, Envy imports)
- [ ] War: puppet declarations, abstract fronts, territory, war weariness
- [ ] Diplomacy, only as much as the design needs
- [ ] Rival hidden hands (Q-009)

## M4 Goals and structure [PROPOSED]

- [ ] Scenarios, win and loss conditions, game length (Q-001, Q-011)
- [ ] Save and load (sim snapshot, RNG state, command log)
- [ ] Onboarding and tutorial

## M5 Mobile production [PROPOSED]

- [ ] Shipping UI (touch-first) with Norm's text and icons
- [ ] Device performance and battery pass; Android builds on device, then iOS
- [ ] Settings, accessibility, localization readiness
- [ ] Store preparation (Android first)
