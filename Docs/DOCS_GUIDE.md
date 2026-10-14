# Documentation guide

> **Owns:** how the docs are kept: principles, where things go, formats, budgets, the session ritual and the
> tools that enforce it. **Budget:** 220 lines. **Read when:** before editing any doc.

Goal: any future session picks up the project cold and knows what the game is, why it is built this way and
what to do next, without re-deriving anything and without reading more than it needs.

## Principles

1. **One owner per fact.** Every fact lives in one doc (its "Owns" line says what it owns); other docs link to
   it. Duplicates drift, and then one of them lies.
2. **Docs lead, code follows.** Write the design or convention down first, or at the latest in the same session.
3. **Generate what can be generated.** Anything derivable from code (the type list, parameter values, the causal
   graph, the decision index) is generated, never typed. Hand edits to `Docs/generated/` are overwritten.
4. **Budgets keep docs readable.** Each doc declares a line budget and the linter warns past it. Over budget
   means split by topic or archive, never "just this once" (InsignificAnts' docs reached 600 KB without one).
5. **Absolute dates.** Write "2026-10-14", never "recently" or "the new system".
6. **Status markers.** Unmarked text describes what exists. `[PROPOSED]`: Claude's idea awaiting Norm.
   `[PLANNED]`: agreed, not built. `[OPEN]`: undecided (see OPEN_QUESTIONS.md). `[DEPRECATED]`: on its way out.
7. **Current intent, not history.** Design docs describe today's plan. History lives in decision records and the
   worklog. When a plan is dropped, delete it from the design doc and record why in a decision.
8. **Norm's words are attributed.** Quotes from the pitch say "Norm"; Claude's interpretations are [PROPOSED].
9. **Style.** ASCII punctuation and no em dashes. Tables where they scan better than prose. Code blocks under
   about five lines.

## Where things go

| You have | It goes in |
|---|---|
| A choice with alternatives that a later session might undo | a decision record (`/decide`), Proposed until Norm accepts |
| A question only Norm can answer | OPEN_QUESTIONS.md, with Claude's recommendation |
| Work left unverified or owed | STATUS.md "Not yet verified", or ROADMAP.md |
| A trap or root cause | LESSONS.md, under its area |
| A new term | GLOSSARY.md |
| How a system behaves | its design doc (the player's view) and MODEL.md (the math) |
| A new code convention | ARCHITECTURE.md |
| What a session did | WORKLOG.md, one entry per session |
| Project knowledge of any kind | never Claude's memory (memory holds only Norm's profile and working style) |

## Formats

**Doc header** (every hand-written doc under Docs/, except decision records):

```
# Title
> **Owns:** what this doc is the single source of truth for. **Budget:** N lines.
```

**Worklog entry** (newest first; 3 to 8 bullets):

```
## 2026-10-14: session N, short title
- What was done, with decision ids. Placeholders added. Subagent delegation, if any.
- Pick up at: where the next session should start.
```

**Decision record** (`Docs/decisions/D-0007-short-slug.md`, created by `/decide`; never edit an accepted
record: supersede it with a new one):

```
# D-0007: Short imperative title
**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Simulation
## Context / ## Decision / ## Because (the main alternative rejected, and why) / ## Consequences (optional)
```

Statuses: Proposed (Claude's, awaiting Norm), Accepted, Superseded by D-xxxx, Rejected.

**Open question:** `### Q-017: Question` then Why it matters, Options, Claude recommends. Removed when answered.

**Lesson:** `- 2026-10-14: what happened, the root cause, and the rule that prevents it.`

## Budgets (lines; the linter reads each doc's own Budget line)

CLAUDE.md 160 (fixed in the linter). STATUS 80. GLOSSARY 120. LESSONS 150. OPEN_QUESTIONS 220. DOCS_GUIDE 220.
ROADMAP 250. Design docs 150 to 350. MODEL 500. RESEARCH 400. ARCHITECTURE 400. TOOLING 300. WORKLOG: 10 entries.

## The session ritual

**Start (automatic):** the SessionStart hook regenerates CODEMAP and the decision index, then prints a brief: git
state, whether the Unity editor holds the project, docs drift, doc lint. CLAUDE.md imports STATUS.md.
**Start (Claude):** read the docs that own the task's area. After a long break, `/catch-up`.

**During:** update the owning doc when behavior changes; `/decide` for choices with alternatives; add placeholders
for any player-facing text and keep a list; record traps in LESSONS. The PostToolUse hook tracks which code files
changed and warns when `Docs/generated/` is hand-edited.

**End:** `/wrap-up` (definition of done):
- [ ] Generated docs regenerated (`doctool generate --sim`).
- [ ] Owning docs updated for every behavior or convention change, or the worklog says why not.
- [ ] Decisions recorded; ROADMAP ticked; OPEN_QUESTIONS updated.
- [ ] STATUS.md rewritten; one WORKLOG entry added (placeholders and subagent use listed).
- [ ] `doctool lint` clean; `doctool mark-synced` run.
- [ ] Git commands handed to Norm (Norm runs them).

## Tools

All in [architecture/TOOLING.md](architecture/TOOLING.md): doctool commands, hooks, skills (`/catch-up`,
`/wrap-up`, `/decide`, `/design-review`, `/doc-health`) and path-scoped rules.

## Health check (`/doc-health`, every few sessions)

- Is any fact stated in two places? Collapse it to one and link.
- Has any [PLANNED] or [PROPOSED] item shipped or been decided? Update its marker.
- Do the docs match the code? (Cheap subagents read and compare; the main model decides.)
- Are budgets respected and the CLAUDE.md map complete? (`doctool lint`)
- Are lessons ready to fold into their area docs?
