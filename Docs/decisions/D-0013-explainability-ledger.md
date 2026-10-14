# D-0013: Every computed value records its terms (the ledger)

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Simulation

## Context
"Controls should always be simple for this game, while outcomes can be complex." Complex outcomes are only fun
when the player can see why they happened, and Claude needs the same visibility to balance them.

## Decision
Every additive term of a computed target passes through `Ledger.Add(nation, target, source, amount)`. The UI
answers "why?" from it, tests check that terms add up to every value, and SimRunner builds the causal graph doc
from it.

## Because
Explanations can never drift from the math, because they are the math. Cost: one struct per term per tick, which
is negligible. The alternative, a hand-maintained list of influences, goes stale.
