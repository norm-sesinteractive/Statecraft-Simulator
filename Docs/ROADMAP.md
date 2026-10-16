# Roadmap

> **Owns:** milestones and their checklists. **Budget:** 250 lines.
> Milestones after M0 are [PROPOSED] until Norm accepts the order. Tick items as they ship. When every box in
> a milestone is ticked, replace the milestone with one line under "Done".

## Done

- M0 Foundations (2026-10-14): Unity project, pure C# sim v0, headless harness, generated scene and dev HUD,
  self-updating docs; verified in the editor by Norm.

## M1 Core loop prototype: one nation, deep [PROPOSED]

Goal: steering one nation is interesting for 30 minutes, with real danger of losing it from within.
- [x] Design session: no goals, defeats, suspicion, politics, reach (D-0018 to D-0022, 2026-10-14)
- [x] Politics v1 (D-0021, 2026-10-14): government type lever; Democracy, Monarchy, Communism, Military junta, Theocracy with traits;
      fomented revolutions; democratic votes started by the player or the people; emergent revolts that install
      the type the people are most aligned with (D-0024)
- [x] Contentment and unrest per generation (D-0032); any generation can revolt, with its own consequences
- [x] Suspicion v1 (D-0020): government and shadow tiers, drivers and dampers, leak between tiers, venting by revolution
- [x] Defeat v1 (D-0019): a revolt that topples the hidden hand; game over when the home nation is lost (sim and dev HUD)
- [x] Dev HUD politics controls verified in the editor (Norm, 2026-10-15: "Politics tab functions")
- [ ] Puppet government v1: decisions it makes on its own (crackdowns, reforms), driven by society state
- [ ] Events v1: politics events exist in the sim (revolutions, votes, revolts, suspicion alarms); player-facing
      event text keys for Norm still to come
- [x] Balance pass 1 (2026-10-15): suspicion overload and the hand's fall on a playable time scale (D-0033)
- [x] Balance pass 2 (2026-10-15): moderate levers felt within their horizon; misery revolts at the shield (D-0034)
- [ ] Explanation ("why?") in the real UI, not only the dev HUD
- [ ] Text catalog: a key for every player-facing string, values from Norm
- [ ] Verify RESEARCH.md citations before any reach game text

## M2 Nations and geography [PROPOSED]

- [x] Economy order and scope (D-0035, 2026-10-15): full economy before trade and war
- [ ] Map model: regions with terrain and resources; nation territory (economy v1 holds terrain areas per nation)
- [x] Population dynamics (D-0036, 2026-10-15): births, deaths, cohort sizes (Lust, Pride, Medicine, land, famine,
      violence); starvation as a defeat (D-0019). War deaths wait for M3.
- [x] Economy v1 (D-0036, 2026-10-15): production by terrain and skill, consumption (Gluttony), stores, comfort,
      wealth (Greed, Commerce); dev HUD economy readouts
- [ ] Norm's review of economy v1 (D-0036) in the editor
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
