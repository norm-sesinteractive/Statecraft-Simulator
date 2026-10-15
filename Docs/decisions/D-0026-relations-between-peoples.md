# D-0026: Relations between peoples: -100 to 100, sticky at the extremes

**Status:** Accepted (Norm, 2026-10-14); formula Proposed | **Date:** 2026-10-14 | **Area:** Simulation

## Context
Norm: "Nations should track a contentment score between their peoples."

## Decision
Every pair of nations has a relations score from -100 (hated rivalry) to +100 (lasting bond).
- Trade pulls it up; military action pulls it down, "a balanced and opposite relationship with trade".
- With no action underway it drifts back toward 0, slowest at the extremes and faster near 0: "The worse things
  get, the longer it takes to cool down and the reverse."
Proposed drift: each month the score moves toward 0 by `rate * (1 - |score| / 100)^2 + floor`, never overshooting 0.

## Because
Norm's call. It makes history matter: old wars and old friendships outlast the events that made them.
