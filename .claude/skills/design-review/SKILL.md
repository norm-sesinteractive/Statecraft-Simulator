---
name: design-review
description: Run a design session with Norm for Statecraft Simulator. Walks open questions and Proposed decisions, asks Norm with clear options and a recommendation, and records the answers as decisions and doc updates. Use when Norm wants to refine gameplay, settle open questions, or review proposals.
argument-hint: "[topic or question ids, e.g. Q-001 Q-002, or politics]"
---

# Design review

Focus: $ARGUMENTS (if empty, take the questions listed under "Next up" in STATUS.md).

1. Read Docs/design/PITCH.md first (Norm's intent wins), then Docs/OPEN_QUESTIONS.md and the design docs the topic
   touches. For model questions, also Docs/simulation/MODEL.md.
2. If numbers would help Norm decide, run a quick experiment
   (`dotnet run --project Tools/Headless/Runner -- run --script <name> --years <n>`) and summarize it in a few lines.
3. Ask Norm with AskUserQuestion: at most four questions per round, two to four options each, the recommended
   option first and marked "(Recommended)", each with a one-line tradeoff. Follow up when an answer opens a new question.
4. Record every answer: an Accepted decision (see /decide), the question removed from OPEN_QUESTIONS.md, and the
   owning design doc updated (drop [PROPOSED] or [OPEN] markers that are now settled). Quote Norm's wording when given.
5. New questions that came up go into OPEN_QUESTIONS.md, each with a recommendation.
6. Finish with a short summary of what was decided and what it changes in ROADMAP.md, and offer to start building.
