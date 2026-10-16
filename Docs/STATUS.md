# Status

> **Owns:** where the project stands right now: phase, what is verified, loose ends, next steps.
> **Budget:** 80 lines. Rewritten, not appended, by every `/wrap-up`.

**Last updated:** 2026-10-15, session 4 (economy v1). **Phase:** M2 nations and geography; economy v1 built.

## Verified working

- M0 done (2026-10-14). Model v1 (politics, two-tier suspicion, generational revolts, the hand's overthrow) built
  2026-10-14; Politics tab confirmed in the editor by Norm (2026-10-15).
- Balance pass 1 (D-0033, accepted: "The pace feels right"). Balance pass 2 (D-0034, proposed).
- Economy v1 (D-0036, proposed, 2026-10-15): land as terrain areas, labor, food, materials, luxuries, stores,
  comfort, wealth, people per generation with births and deaths, famine and starvation as a defeat.
  83 tests pass headless (17 new in EconomyTests). In the runner the untouched world stays calm for 300 years and
  populations level off; balance pass 1 pace holds (extremist overthrow year 13, whiplash 12, propaganda 16).
- All Unity assemblies compile against the real Unity libraries (`dotnet build Statecraft.Editor.csproj`, 0 errors).

## Not yet verified (needs Norm)

- [ ] Economy section of the dev HUD in the editor: people, food, goods and the four meters for each nation;
      "GAME OVER: the nation starved" if the home nation starves.
- [ ] Feel check of pass 2 (a 3:1 tilt felt within a year) and now of the economy (try Lust plus Medicine, or
      Gluttony plus Sloth, at 8x speed and watch people, land pressure and comfort).
- [ ] Unity's Test Runner has not run the tests added since session 1 (editor open blocks batch mode). Unity must
      refresh once so `EconomyTests.cs.meta` exists before the commit.
- [ ] No Android build or device check yet.

## Provisional, for Norm to confirm

- D-0036 economy v1: three resources, needs-first labor, which schooling raises which output (Infrastructure raising
  materials is Claude's addition), which vices drive births, deaths and consumption, starvation at a 25% famine toll.
- Levers alone rarely starve a nation (crowding slows births first); starvation becomes live with war and imports.
- D-0034: frustration counted per vice in contentment, and the pass 2 tuning.
- Company "SES Interactive", bundle id `com.SESInteractive.StatecraftSimulator`, landscape only (D-0017, Q-012).
- Dev tooling may use plain English while game text is Norm's (D-0015).

## Next up (suggested order, D-0035)

1. Norm's look at economy v1 (and pass 2) in the editor; adjust D-0036.
2. M3 on top of the economy: trade between nations (goods, prices, wealth, transit efficiency D-0029 to D-0031),
   culture through imports (D-0025 to D-0028), relations (D-0026) with a clear readout.
3. Puppet decisions (D-0035): when to trade and when to go to war; whole-nation military conquest.
4. Cultural conquest without rival hands (an absorbed nation leaves the player); rival hands later (Q-026).
5. Still open from M1: puppet crackdowns and reforms, events with text keys for Norm.

## Open questions worth Norm's time next

Q-004 the map (terrain areas wait for it), Q-014 how puppets decide on war, Q-003 setting and era, Q-011 pacing,
Q-026 another nation's hand falling (Docs/OPEN_QUESTIONS.md).
