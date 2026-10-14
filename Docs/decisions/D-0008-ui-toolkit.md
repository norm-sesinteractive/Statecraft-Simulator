# D-0008: UI Toolkit for all UI

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Architecture

## Context
Claude writes all code and cannot drag things around in the editor. Norm used UI Toolkit for panels in
InsignificAnts.

## Decision
UI is UI Toolkit: UXML layouts, USS styles and C# controllers. PanelSettings scale with the screen at a 1920x1080
reference, matching height. The Input System UI module drives input through an EventSystem, as in InsignificAnts.

## Because
UXML and USS are plain text that Claude can author and review, unlike uGUI prefabs. Alternative rejected: uGUI
(prefab and scene YAML wiring, harder to generate and diff).
