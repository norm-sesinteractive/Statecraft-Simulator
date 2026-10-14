# D-0010: Real-time with pause over monthly ticks

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Design

## Context
Norm's river: "influencing the flow of a river and watching as the consequences of your river take place." Q-005.

## Decision
Time flows continuously at 0 (paused), 1, 2, 4 or 8 months per second; the simulation advances in whole monthly
ticks underneath. The app pauses itself when sent to the background.

## Because
Watching consequences unfold is the fantasy, and turns would chop the river into steps. The tick engine does not
care either way, so turns remain possible if Norm prefers them.
