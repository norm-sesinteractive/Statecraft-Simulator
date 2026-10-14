---
name: doc-health
description: Deep audit of Statecraft Simulator's docs against the code. Finds facts stated twice, stale status markers, drift between docs and code, budget problems, and lessons ready to fold in. Use when Norm asks to tidy or audit the docs, or every few sessions.
---

# Doc health check

1. Run `python Tools/docs/doctool.py generate --sim`, then `python Tools/docs/doctool.py lint`, and fix the warnings.
2. Delegate the reading to cheaper subagents (model "sonnet", or "haiku" for pure extraction), about five at once,
   never in worktrees. Give each one area: design docs against Docs/simulation/MODEL.md; MODEL.md against
   Assets/Scripts/Sim; ARCHITECTURE.md against the code; GLOSSARY.md against code names; STATUS.md and ROADMAP.md
   against reality. Ask each for concrete mismatches with file and line, written to a scratchpad file, returning
   only counts.
3. Read those lists and decide on the main model: fix what is plainly wrong, collapse facts stated twice into one
   owner plus links, update [PLANNED] and [PROPOSED] markers for what shipped or was decided, and fold LESSONS
   entries into area docs that are being edited anyway.
4. Report what changed and what needs Norm's decision. Flag stale code or comments in chat; do not delete them.
