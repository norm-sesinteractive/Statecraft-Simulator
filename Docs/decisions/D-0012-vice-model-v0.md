# D-0012: Vice model v0: disposition plus mood, outlets, frustration, one contentment axis

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Simulation

## Context
The pitch needs vices that rise and fall, outlets that make them harmless or even pleasant, and suffering where
they have nowhere to go. Education should work across generations.

## Decision
Each nation has three cohorts (Youth, Adult, Elder) with a disposition and a channeling vector each, plus a
national mood. Expressed vice is disposition plus mood; outlets come from adult channeling; frustration is
expressed beyond outlet. Contentment is one axis from -1 to +1. Crime, unrest and aggression follow from these
(MODEL.md has the equations).

## Because
Cohorts make education's lag emerge naturally and match research on generational value change (Inglehart). Fast
mood and slow disposition separate media's two effects. One contentment axis matches the pitch's
"contentment/suffering"; Q-010 keeps two gauges open.
