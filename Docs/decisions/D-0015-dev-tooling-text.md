# D-0015: Dev tooling may use plain technical English

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Process

## Context
D-0005 makes all player-facing text Norm's. Developer tools (the dev HUD, editor menus, logs, runner output)
need readable labels to be usable at all.

## Decision
Text that only developers see may use plain technical English in ASCII: vice and lever names, "Contentment",
"Year 3, Month 7". Anything shipped to players goes through the text catalog with Norm's words, or an obvious
placeholder until Norm writes it.

## Because
A dev HUD full of `[LABEL_7]` placeholders would be unusable for testing. The boundary is "would a player see this
in a release build?" Norm may reject this and require placeholders everywhere.
