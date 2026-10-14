---
name: wrap-up
description: End-of-session documentation ritual for Statecraft Simulator. Regenerates the generated docs, updates STATUS, WORKLOG, ROADMAP, decisions and open questions for this session's changes, lints, marks the docs synced, and hands Norm the git commands. Use when Norm says wrap up, end the session, sync the docs, or before a long pause.
argument-hint: "[short session title]"
---

# Wrap up the session

Session title (optional): $ARGUMENTS

Follow Docs/DOCS_GUIDE.md. Work through these steps in order. Keep edits small and factual; no em dashes.

1. **See what changed.**
   - `python Tools/docs/doctool.py pending` lists code files edited since the last sync.
   - If a git repository exists: `git status --short` and `git diff --stat` (read-only).
   - Recall the decisions made, questions answered, placeholders added and subagents used this session.
2. **Verify** with the cheapest checks that cover the changes (CLAUDE.md, "Verify"). Note anything unverified.
3. **Update the owning docs** (map in CLAUDE.md) for every change in behavior, design or convention: design
   docs, Docs/simulation/MODEL.md (equations, pinned tests), ARCHITECTURE.md, TOOLING.md, GLOSSARY.md (new terms),
   LESSONS.md (traps). If a change needs no doc update, say why in the worklog entry.
4. **Decisions:** one record per new choice (`python Tools/docs/doctool.py new-decision "Title"`, then fill it in).
   For each answered open question, record an Accepted decision and delete the question from OPEN_QUESTIONS.md.
5. **Regenerate:** `python Tools/docs/doctool.py generate --sim`.
6. **ROADMAP.md:** tick what shipped; add new items.
7. **STATUS.md:** rewrite it within its 80-line budget: last updated, phase, verified, not yet verified,
   provisional choices, next up.
8. **WORKLOG.md:** add one entry at the top (format in DOCS_GUIDE.md) that lists every placeholder added for Norm
   and any subagent delegation. If there are more than ten entries, run `python Tools/docs/doctool.py rotate-worklog`.
9. **Lint:** `python Tools/docs/doctool.py lint`, then fix every warning.
10. **Mark synced:** `python Tools/docs/doctool.py mark-synced`.
11. **Hand Norm the git commands.** Norm runs them; never run git writes yourself. One command per bash block:
    - before the very first commit only: `git init -b main`
    - `git add -A`
    - `git commit -m "<summary line>" -m "<2 to 6 bullet lines>"` (draft the message; add the co-author
      trailer from the harness's attribution instructions, if any, as the last paragraph)
    - `git push`, only if a remote exists
    Remind Norm to glance at `git status` first if anything unexpected may be staged.
12. **Report** to Norm: what changed in the docs, what to verify in the editor or on device, and every
    placeholder added this session.
