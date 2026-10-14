# Vision

> **Owns:** the player fantasy, the design pillars, what the game is and is not, and the core loop.
> **Budget:** 200 lines. **Read when:** making any design call, or judging whether a feature belongs.

This doc distills [PITCH.md](PITCH.md). Lines marked "Norm" restate the pitch; anything marked [PROPOSED] is
Claude's interpretation awaiting Norm's review ([../OPEN_QUESTIONS.md](../OPEN_QUESTIONS.md)).

## The fantasy

You are a hidden society (Norm: "an all powerful illuminati like organization") that shapes nations through
"seemingly subtle everyday means": what the media airs, what the schools teach, how the courts judge, which
political forms take root. You never command the nations. They grow, trade, quarrel, invade, decay or
flourish on their own. Your skill is reading the current early and nudging it, "lest they make a monster
they can no longer control or are overwhelmed by outside forces" (Norm).

## Pillars

1. **Simple controls, complex outcomes** (Norm). Every lever is a slider or a small choice. Depth comes from
   interactions and time, never from more buttons.
2. **Influence, never command** (Norm): "The illuminati do not directly control the actions of their
   puppets." Nations decide for themselves; the player shapes what they want and what they are able to do.
3. **The river** (Norm). The player influences a flow and watches its consequences. [PROPOSED] Every lever has
   its own momentum and delay (media in months, courts in about a year, schools over a generation), and
   pressure always goes somewhere: into outlets, or over the banks as crime, unrest and war.
4. **Every strength casts a shadow** (Norm's examples). Violent media makes a nation more warlike, better at
   war, and more violent at home. A purely peaceful nation is happy and becomes prey. A people "so happy and
   so well supplied that crime no longer takes place" risks growing "weak and decadent".
5. **Legible causality** [PROPOSED]. Complex outcomes are only fun when the player can ask "why?" and get a
   true answer. The simulation records every term behind every value (D-0013).
6. **A leg in realism** [PROPOSED, from Norm's earlier projects]. Mechanics trace to social science
   ([../simulation/RESEARCH.md](../simulation/RESEARCH.md)); deliberate departures are named as such.

## What it is not

- Not Civilization (Norm): no unit-by-unit orders, no per-city build queues, no tech-tree clicking, none of the
  "tedious individual choices".
- Not a god game with direct powers [PROPOSED]: you cannot declare war, but you can make war likely.
- Not a puzzle with one right answer [PROPOSED]: the same levers play out differently in different lands
  and beside different neighbors.

## Core loop [PROPOSED]

1. **Read** a nation: its moods, its outlets, what is overflowing, the threats beyond its borders.
2. **Adjust** a few levers: the airtime mix, the curriculum, the courts' severity, political nudges.
3. **Let time flow**: real-time with pause, months ticking by (D-0010).
4. **Watch consequences arrive on their own schedules**: media within months, courts within a year, schooling
   a generation later.
5. **Respond** to what the puppets do on their own: wars, revolts, booms, decadence, rival influence.

## The institutions in one breath [PROPOSED, D-0011]

| Institution | River image | Acts on | Speed |
|---|---|---|---|
| Media | what pours into the river | vices: the mood now, the culture over decades | months |
| Education | the channels dug for it | outlets: what people are trained to do with their drives | a generation |
| Law and order | the levees | the overflow: crime, fear, deterrence | about a year |
| Politics | who steers, and the spillways | decisions, legitimacy, how grievance is voiced | years |

Details: [INSTITUTIONS.md](INSTITUTIONS.md).

## Presentation [OPEN]

- Mobile first (D-0002): touch-first, landscape (provisional, Q-012). A session has to make sense in minutes.
- Art: Norm's Blender models and icons; Unity primitives until then. The look of the map is open (Q-004). One
  possibility: the player's view is the society's secret map room, with nations as pieces on a table.
- All player-facing text is Norm's (D-0005).

## Inspirations [PROPOSED, Claude's suggestions]

- **Democracy** (Positech): policy sliders feeding a visible web of simulated causes; the closest cousin to
  "simple controls, complex outcomes".
- **Plague Inc.**: indirect influence spreading across a world map, on mobile.
- **Crusader Kings**: stories that emerge from characters acting on their own (Norm's agency north star in
  InsignificAnts).
- **Frostpunk**: a society's discontent as a gauge to manage, not control.

## Stakes (D-0018 to D-0022)

No goals and no score: the player plays for their own designs. Nothing limits the levers; the risk is defeat.
A nation is lost to military conquest, to cultural conquest by a rival shadow organization, to starvation, or to a
revolt that topples the hidden hand itself. The government is the shield that takes the fall first. Details:
[HIDDEN_HAND.md](HIDDEN_HAND.md).

## Open

Setting, map and pacing are undecided: Q-003, Q-004, Q-011 in [../OPEN_QUESTIONS.md](../OPEN_QUESTIONS.md).
