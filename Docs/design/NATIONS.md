# Nations

> **Owns:** what a nation is: identity, land and resources, population and generations, the puppet government.
> **Budget:** 300 lines. **Read when:** working on the map, geography, demographics or nation AI.

## Anatomy

| Part | Built? | Notes |
|---|---|---|
| Identity | yes | index plus a name key; names are Norm's to write (`[NATION_1]` placeholders) |
| Institutions | yes: media, curriculum, courts | [INSTITUTIONS.md](INSTITUTIONS.md) |
| Society | yes | cohorts, vices, outlets, contentment, crime, unrest, aggression: [VICES.md](VICES.md) |
| Land and resources | [PLANNED M2] | below |
| Population numbers | [PLANNED M2] | v0 has fixed cohort shares only |
| Economy | [PLANNED M2] | production, consumption, wealth |
| Military | [PLANNED M3] | strength from people, wealth, Wrath and training |
| Puppet government | [PLANNED M1] | below |

## Generations (built, model v0)

Three cohorts carry the culture. Youth are shaped by family, school and media; adults are the workforce and the
voice of the nation; elders keep the culture of their youth. Newborns inherit their parents' disposition and
arrive untrained. Youth graduate into adults, adults retire into elders. That flow is why schooling changes a
nation a generation late, and why media's long cultivation reaches the young first.

## Land and resources [PLANNED M2]

Norm: "Countries should have different resources and different landscapes that change how those resources are
accessed and how their populations live and grow."

[PROPOSED] Regions with one terrain each (plains, forest, hills, mountains, desert, coast, river valley,
tundra) and resource deposits (food, timber, ore, energy, luxuries). Terrain sets what can be reached and at
what cost: mountains defend but isolate, coasts and rivers trade, plains feed. Land shapes growth, Gluttony's
pressure, Greed's opportunities, and who is worth invading.

## Population [PLANNED M2]

[PROPOSED] Births rise with Lust, food and contentment, and fall with Pride (individualism) and hardship.
Deaths rise with age, violence, famine and war, and fall with Medicine. Elder neglect under high Pride and
Wrath (Norm's example) adds elder suffering.

## The puppet government [PLANNED M1]

Norm: "The illuminati do not directly control the actions of their puppets."

The player chooses the government type, and it changes only through revolution or, in a democracy, a vote (D-0021).
Norm: "The government is just a shield for the shadow organization of the player. They are another tool to absorb
or stoke the whims of the people and there to take the fall if the people become unruly." Types and suspicion:
[INSTITUTIONS.md](INSTITUTIONS.md) and [HIDDEN_HAND.md](HIDDEN_HAND.md).

Starvation is one of the four ways to lose a nation (D-0019), so food and resources must be modeled (M2).

[PROPOSED] Each nation's rulers act on their own every month, reading their society. They crack down when unrest
rises (if their political form allows it), reform when pressure builds (if it allows that), declare war when
aggression, opportunity and appetite line up, and sue for peace when war weariness wins. The player shapes
those conditions; the rulers pull the trigger. Their likely next moves should be readable as a forecast, so
the player can steer ahead of them.
