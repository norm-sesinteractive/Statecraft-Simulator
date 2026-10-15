# D-0030: Infrastructure efficiency is a transit loss on each side of a deal, 50% to 100%

**Status:** Accepted (Norm, 2026-10-14); the buyer's overpayment formula is Proposed | **Date:** 2026-10-14 | **Area:** Simulation
**Supersedes:** the revenue rule of D-0029 (its "efficiency, not appeal" principle stands).

## Context
Norm clarified how D-0029's infrastructure efficiency applies to money.

## Decision
- **Efficiency** = 50% at 0 infrastructure, rising linearly to 100% at 100 infrastructure: 2 points per 1%
  (`efficiency = 0.5 + infrastructure / 200`). Each nation uses its own infrastructure.
- **The deal itself is unchanged:** the agreed amount is what the buyer pays and what the supplier is paid.
- **Buyer:** "more than the agreed upon rate will be deducted from the paying/receiving party if they don't have
  100 infrastructure." [PROPOSED] cost to the buyer = agreed x (2 - efficiency): at 50% efficiency the buyer pays
  1.5x; at 100%, exactly the agreed amount.
- **Supplier:** gets the agreed amount, "but then lose a % of it in transit": it keeps agreed x efficiency.
- Military efficiency (D-0029) uses the same 50% to 100% curve [PROPOSED].

## Because
Norm's call: a softer, linear version of D-0029 that keeps prices honest and puts the loss in logistics.
