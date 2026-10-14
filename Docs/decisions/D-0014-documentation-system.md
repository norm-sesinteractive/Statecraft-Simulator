# D-0014: A self-updating documentation system

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Process

## Context
Norm asked for "a self-updating documentation structure" so Claude understands the project across many sessions.
Lessons from Norm's projects: Sol's Box's single DECISIONS.md reached 50 KB in two months; InsignificAnts' docs
grew to 400-600 KB and a hand-kept class reference to 430 KB.

## Decision
- CLAUDE.md (rules, map, workflow) imports STATUS.md, so every session starts oriented.
- One owner per fact, each doc with an "Owns" line and a line budget the linter enforces.
- Decisions as one small file each, with a generated index; worklog capped at ten entries.
- Generated docs: CODEMAP (from code), PARAMETERS and SOCIETY_GRAPH (from the live model), the decision index.
- Hooks: a SessionStart brief that also regenerates the code map; PostToolUse drift tracking; a non-blocking Stop
  notice to Norm. Skills: `/catch-up`, `/wrap-up`, `/decide`, `/design-review`, `/doc-health`.
- Path-scoped rules in `.claude/rules/` instead of CLAUDE.md files inside `Assets/`.

## Because
What can be generated cannot go stale; what cannot is kept small, owned and checked. The Stop hook informs rather
than blocks, so mid-task turns are not interrupted; `/wrap-up` is where docs catch up. If drift becomes a
problem, the Stop hook can be made blocking.
