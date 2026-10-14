# D-0016: Trim template packages; add mobile helpers

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Engine

## Context
The Universal 3D template ships packages this project does not use.

## Decision
Removed Visual Scripting (all logic is C# written by Claude), Unity Version Control (collab-proxy; git is used),
Multiplayer Center (single player) and AI Navigation (no NavMesh on an abstract map). Added the Device Simulator
device pack and Android Logcat, as in InsignificAnts. Kept URP, Input System, Test Framework, uGUI (EventSystem),
Timeline and both IDE packages.

## Because
Fewer packages mean faster imports and less to read. Each removal is one line in `Packages/manifest.json` to undo.
