# D-0001: Build on Unity 6000.3.13f1 with URP from the Universal 3D template

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Engine

## Context
Norm wants the game in Unity. Two editors are installed (2022.3.62f1 and 6000.3.13f1); InsignificAnts and Sol's
Box both use 6000.3.13f1 with URP.

## Decision
Unity 6000.3.13f1, project created in batch mode from the bundled Universal 3D template (URP 17.3, Mobile and PC
renderer assets, Input System).

## Because
Same editor as Norm's other projects (one install, known behavior). URP is the mobile-friendly pipeline, and the
3D template supports both a 3D map and flat UI, so it does not foreclose the open map-look question (Q-004).
The Universal 2D template was the alternative; it would make Norm's Blender models harder to use.
