# D-0027: Educational overlap gates trade's cultural influence and goodwill

**Status:** Accepted (Norm, 2026-10-14); the zero-overlap case is open (Q-023) | **Date:** 2026-10-14 | **Area:** Simulation
**Supersedes:** the "what moves" part of D-0025 (education no longer travels with trade).

## Context
D-0025 had trade carry both vices and education. Norm corrected it: "I made an error in equating aggression and
science."

## Decision
- Trade moves **vices only**. Education is not transferred; it sets how much gets through.
- **Overlap** between the supplier's adults and each receiving cohort = the sum, over every curriculum focus, of
  the smaller of the two shares. Example (Norm): supplier adults 75 Agriculture and 25 Science; receivers 100
  Agriculture gives 75%; receivers with 25 to 100 Science and no Agriculture gives 25%.
- The overlap scales both the vice change and the goodwill (relations gain, D-0026) from that trade. Zero overlap
  means no vice change and no goodwill, pending Q-023.
- The cohort weights (2x youth, 1x adults, 0.5x elders) and the contentment comparison from D-0025 still apply.

## Because
Norm's call. Shared schooling is shared understanding: nations taught alike influence and befriend each other.

## Consequences
Each cohort needs an education profile per curriculum focus (today the model only has channeling per vice):
[PROPOSED] a per-cohort schooling vector that follows the curriculum for youth and graduates to adults and elders the
same way channeling does.
