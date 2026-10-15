# Status

> **Owns:** where the project stands right now: phase, what is verified, loose ends, next steps.
> **Budget:** 80 lines. Rewritten, not appended, by every `/wrap-up`.

**Last updated:** 2026-10-14, session 2 (design, then politics build). **Phase:** M1 core loop; model v1 built.

## Verified working (2026-10-14)

- M0 done: Unity 6000.3.13f1 project, pure C# sim, headless harness, generated scene, docs system; Norm confirmed
  the first build runs in the editor.
- Model v1 (headless): per-generation contentment and unrest (D-0032); politics with five government types,
  alignment, fomented revolutions, votes, generational revolts (D-0021, D-0024); two-tier suspicion (D-0020);
  the hidden hand's overthrow and game over (D-0019). 57 tests pass headless.
- All Unity assemblies compile against the real Unity libraries (`dotnet build Statecraft.Editor.csproj`, 0 errors),
  using throwaway includes for files the open editor had not yet picked up.
- Runner experiments behave as designed (MODEL.md, "Observations"): false revolutions succeed about 30% toward an
  unpopular type and 75% toward a popular one; tyranny ends in the people voting in a junta.

## Not yet verified (needs Norm)

- [ ] In the editor (focus it so it recompiles): Play `Main.unity`, open the **Politics** tab, foment a revolution
      and (in the Democracy) call a vote; watch the Generations rows and the two suspicion meters.
- [ ] Unity's Test Runner has not run the 14 new tests (batch mode is blocked while the editor is open; run
      Window > General > Test Runner > EditMode, or close the editor and `Tools/unity.ps1 -Mode test`).
- [ ] No Android build or device check yet.

## Provisional, for Norm to confirm

- Company "SES Interactive", bundle id `com.SESInteractive.StatecraftSimulator`, landscape only (D-0017, Q-012).
- Dev tooling may use plain English while game text is Norm's (D-0015).
- Balance changes made while building (MODEL.md): revolt threshold 0.3, unrest from suffering 1.2, stronger
  fomenting. All tuning lives in Docs/generated/PARAMETERS.md.

## Design settled in session 2 (2026-10-14)

D-0018 to D-0032: no goals; four defeats and free levers; two-tier suspicion; the government as the shield,
changed by revolution or vote; reach by conquest and rival hands; revolts install the people's preferred type;
culture through trade imports gated by educational overlap with a Commerce floor; infrastructure as transit
efficiency from adult schooling; relations between peoples; contentment and revolts per generation.

## Next up (suggested order)

1. Norm's editor check of the Politics tab (above).
2. Balance pass: make generational revolts reachable in bad times but not routine; tune suspicion speed.
3. Puppet government v1 (crackdowns, reforms on its own), then events with text keys for Norm.
4. Then M2 (land, population, food: starvation as a defeat) or M3 (trade, culture, relations, war), Norm's choice.

## Open questions worth Norm's time next

Q-003 setting and era, Q-004 the map, Q-006 media control shape, Q-011 pacing (Docs/OPEN_QUESTIONS.md).
