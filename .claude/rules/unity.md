---
paths:
  - "Assets/Scripts/Game/**"
  - "Assets/Scripts/UI/**"
  - "Assets/Scripts/Editor/**"
  - "Assets/UI/**"
  - "Assets/Scenes/**"
---

# Unity-side rules

- Read sim state; change it only by enqueuing `SimCommand`s through `SimulationHost`. No sim logic in Update():
  `SimulationHost` alone calls `Simulation.Advance`.
- Scenes are generated: change `MainSceneBuilder` or `ProjectSetup`, then run
  `powershell -ExecutionPolicy Bypass -File Tools/unity.ps1 -Mode setup` (editor closed) or the Statecraft menu.
  Never hand-edit Main.unity.
- Never quote a serialized field's code default as live; grep the scene or asset.
- UI Toolkit only. UI strings, UXML and USS are ASCII. Player-facing text is a placeholder (`[LIKE_THIS]`) unless
  Norm wrote it; dev HUD text may be plain English (D-0015).
- Sizes in reference pixels at 1080 height; touch targets of at least about 72 px; respect the safe area.
- Placeholder art from primitives only; Norm's assets arrive as FBX and PNG under Assets/Art.
- Verify with `Tools/unity.ps1 -Mode compile` or `-Mode test` when the editor is closed; otherwise tell Norm
  exactly what to check in the editor.
