---
name: decide
description: Record a design or technical decision for Statecraft Simulator as a numbered record in Docs/decisions. Use when a choice with alternatives is made or proposed, or when Norm answers an open question.
argument-hint: "<short imperative title>"
---

# Record a decision

Title: $ARGUMENTS

1. Run `python Tools/docs/doctool.py new-decision "<title>"`. It prints the new file's path and refreshes the index.
2. Fill in the record (format in Docs/DOCS_GUIDE.md): Status `Accepted (Norm, <date>)` only if Norm decided it,
   otherwise `Proposed`; Area; Context (why a decision was needed); Decision; Because (the main alternative
   rejected, and why); Consequences if any. Keep it under 30 lines, no TODO left, no em dashes.
3. If it answers an open question, delete that question from Docs/OPEN_QUESTIONS.md and cite its Q number in Context.
4. If it supersedes an earlier record, set that record's status to `Superseded by D-NNNN`. That is the only edit
   ever made to an accepted record.
5. Update the docs the decision affects, then run `python Tools/docs/doctool.py decisions`.
