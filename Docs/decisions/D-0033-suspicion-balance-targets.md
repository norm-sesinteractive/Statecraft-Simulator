# D-0033: Balance targets for suspicion and the hand's survival; drift measured against tradition

**Status:** Accepted (Norm, 2026-10-15: "The pace feels right") | **Date:** 2026-10-15 | **Area:** Simulation

## Context
Norm, after playing model v1: the Politics tab works, "but is nearly impossible to overload government or shadow org
suspicion as predicted." Runner measurements agreed: one extreme lever stalled near 0.4, every lever at an extreme
needed a decade, and a fully suspicious people never revolted, so the hand could not fall.

## Decision
Balance to these targets, pinned by `Assets/Tests/Sim/BalanceTests.cs` with default tuning:
1. An untouched world stays calm for a century (suspicion below 0.3, shadow below 0.05, no revolts).
2. A moderate tilt (2:1 weights) stays below the suspicion alarm.
3. One extreme lever (all airtime on one vice, or one curriculum focus) raises the alarm within a decade.
4. Every lever at an extreme overloads suspicion (above 0.85) within five years.
5. Sustained suspicion breaks shadow suspicion within 15 years, and the hand falls within 30 in most games.

"Vices out of whack" is measured as cultural drift: each vice's distance from the nation's own tradition (its
starting culture, slowly accepting lasting change) beyond a tolerance. A warrior culture is not suspicious for
being warlike; it grows suspicious when someone changes it.

## Because
The tension Norm wants (D-0019, D-0020) needs extremes to be dangerous on a playable time scale, while normal play
stays safe. Measuring drift against human nature (model v1) diluted a single vice's distortion across seven and
would have made distinctive cultures suspicious of themselves.
