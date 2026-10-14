# D-0005: Player-facing text is Norm's; Claude uses placeholders

**Status:** Accepted (Norm, 2026-10-14) | **Date:** 2026-10-14 | **Area:** Process

## Context
The game will be text-heavy (names, tooltips, events, news). InsignificAnts has the same rule.

## Decision
"I write it, Claude uses placeholders." Any player-facing text Claude adds is an obvious placeholder such as
`[NATION_1]` or `[EVENT_TITLE]`, and every session summary lists the placeholders it added. The simulation holds
no text at all, only keys and ids. Dev tooling is covered separately by D-0015.

## Because
Norm's call. A text catalog (keys to Norm's text) is planned in M1 for the shipping UI.
