# Status

> **Owns:** where the project stands right now: phase, what is verified, loose ends, next steps.
> **Budget:** 80 lines. Rewritten, not appended, by every `/wrap-up`.

**Last updated:** 2026-10-15, session 3 (balance pass 1). **Phase:** M1 core loop; model v1 built and balanced once.

## Verified working

- M0 done (2026-10-14). Model v1 (politics, two-tier suspicion, generational revolts, the hand's overthrow) built
  2026-10-14; Norm confirmed the Politics tab works in the editor (2026-10-15).
- Balance pass 1 (2026-10-15, D-0033, Proposed): extremes now overload suspicion on a playable time scale and can
  topple the hand; untouched nations stay calm for a century. 63 tests pass headless, including 6 balance-target
  tests that use default tuning.
- All Unity assemblies compile against the real Unity libraries (`dotnet build Statecraft.Editor.csproj`, 0 errors).

## Not yet verified (needs Norm)

- [ ] In the editor: Media and Education rows now have "Only" buttons and each group an "Even" button. Try an
      extreme (Only on one vice, Only on one school focus, courts at 1) at 8x speed: the suspicion alarm should come
      within about two game years, shadow rumors within about six, and the hand should be in danger after a decade.
- [ ] Does that pace feel right in play? The targets are in D-0033 and MODEL.md, "Balance targets".
- [ ] Unity's Test Runner has not run the 20 tests added since session 1 (editor open blocks batch mode).
- [ ] No Android build or device check yet.

## Provisional, for Norm to confirm

- D-0033 balance targets and the "cultural drift against tradition" reading of "vices out of whack".
- Company "SES Interactive", bundle id `com.SESInteractive.StatecraftSimulator`, landscape only (D-0017, Q-012).
- Dev tooling may use plain English while game text is Norm's (D-0015).

## Next up (suggested order)

1. Norm plays the balance pass and says whether the pace feels right.
2. Balance pass 2: each lever within its own time horizon; revolts from hardship (not only from suspicion).
3. Puppet government v1 (crackdowns and reforms on its own; resetting levers after the hand falls), then events with
   text keys for Norm.
4. Then M2 (land, population, food: starvation as a defeat) or M3 (trade, culture, relations, war), Norm's choice.

## Open questions worth Norm's time next

Q-003 setting and era, Q-004 the map, Q-006 media control shape, Q-011 pacing (Docs/OPEN_QUESTIONS.md).
