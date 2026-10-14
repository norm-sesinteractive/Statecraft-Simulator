# D-0003: Norm runs every git write

**Status:** Accepted (Norm, 2026-10-14) | **Date:** 2026-10-14 | **Area:** Process

## Context
Norm's projects differ: in Sol's Box Norm runs all git operations; InsignificAnts later let Claude commit to main.

## Decision
"I run git, you give commands." Claude never runs git writes (init, add, commit, push, remotes, repo creation).
Read-only git (status, log, diff) is fine. Claude hands over copy-paste commands, one per bash block, with a
drafted commit message.

## Because
Norm's call. `/wrap-up` ends with the commands. No worktrees or side branches either (CLAUDE.md rule 4).
