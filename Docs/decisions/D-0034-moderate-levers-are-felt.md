# D-0034: Moderate levers are felt; frustration counts per vice, not averaged

**Status:** Proposed | **Date:** 2026-10-15 | **Area:** Simulation

## Context
Balance pass 2. After pass 1 (D-0033), only extremes had consequences, through suspicion. The runner's `sweep` showed a
3:1 media tilt moved contentment by 0.01 in a year and a 3:1 school tilt by nothing after 25 years; 80 years of
moderate mismatch ("slow-burn") left unrest at 0. Two causes: contentment averaged frustration over seven vices, so one
frustrated drive was diluted sevenfold, and a fixed cushion (election venting, Sloth, fear) swallowed small unrest.

## Decision
- Contentment loses `FrustrationGain * frustration` for each vice, summed (gain 0.5), instead of the mean (1.2 / 7).
- MoodGain 0.25 to 0.5; ChannelGain 0.6 to 0.8; SlothDampening 0.15 to 0.10; Democracy's UnrestVenting 0.08 to 0.04.
- New targets, pinned in BalanceTests.cs: a 3:1 media tilt is felt within a year (aggression, contentment); courts
  change crime and contentment within a year; schooling that fits a culture eases it within 25 years; real misery
  revolts against the government within five years, not against the hand.

## Because
Play needs a middle ground between "nothing happens" and "suspicion doom". Summing matches the pitch: a culture
without outlets "for their share of the 7" suffers, one drive at a time. The pass 1 pace Norm approved holds: extreme
play still topples the hand in years 11 to 14; an untouched world stays calm for a century.

## Consequences
Moderate mismatch makes people sour (contentment falls) but rarely rebellious; revolts need real misery or suspicion.
Hardship from famine and war arrives with M2 and M3.
