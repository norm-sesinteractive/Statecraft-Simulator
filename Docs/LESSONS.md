# Lessons

> **Owns:** traps and root causes, grouped by area, one dated bullet each. **Budget:** 150 lines.
> Grep the area before debugging or changing it. When an area's own doc is next rewritten, fold its lessons in
> and delete them here.

## Unity batch mode

- 2026-10-14 (from Sol's Box): batch Unity fails while the editor holds the project. `Tools/unity.ps1` checks
  first; the headless harness is the alternative.
- 2026-10-14 (from Sol's Box): headless runs skip some editor housekeeping (the URP material upgrader runs on
  `EditorApplication.update`). Expect first-open migrations after a package change and commit them.
- 2026-10-14: `InputSystemUIInputModule.AssignDefaultActions()` called from editor code serializes a private copy
  of the default actions into the scene (37 KB). Reference the package asset and its action sub-assets instead
  (`MainSceneBuilder.WireDefaultUiActions`).

## Git

- 2026-10-14: `git lfs env` run outside a repository creates empty `lfs/objects` and `lfs/tmp` folders in the
  current directory. Do not run it from the project root before the repo exists.

## Serialized values

- 2026-10-14 (from InsignificAnts): a serialized value in a scene or asset wins over the code default. Here
  MainSceneBuilder writes those values from code, but still grep `Assets/Scenes/Main.unity` before quoting one.

## Mobile UI

- 2026-10-14 (from InsignificAnts): Unicode symbol glyphs render as boxes on device fonts. ASCII text, PNG icons.
- 2026-10-14 (from InsignificAnts): screen thresholds in raw pixels misfire on phones (3 to 4 times the editor's
  pixels). Use the reference height.

## C# and the headless harness

- 2026-10-14: a class named like its namespace (`Scenarios.Scenarios`) breaks name lookup inside
  `Statecraft.Sim.*`. It became `ScenarioLibrary`.
- 2026-10-14: Unity compiles C# 9 against .NET Standard 2.1. Records and `init` need `IsExternalInit`, which
  Unity lacks; the harness enforces C# 9 so such code fails there first.

## Docs

- 2026-10-14 (from InsignificAnts): docs without budgets grew to 400-600 KB and could no longer be read in one
  call. Hence per-doc budgets, generated references and the linter here.
