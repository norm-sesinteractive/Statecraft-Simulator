# Nations

> **Owns:** what a nation is: identity, land and resources, population and generations, the puppet government.
> **Budget:** 300 lines. **Read when:** working on the map, geography, demographics or nation AI.

## Anatomy

| Part | Built? | Notes |
|---|---|---|
| Identity | yes | index plus a name key; names are Norm's to write (`[NATION_1]` placeholders) |
| Institutions | yes: media, curriculum, courts | [INSTITUTIONS.md](INSTITUTIONS.md) |
| Society | yes | cohorts, vices, outlets, contentment, crime, unrest, aggression: [VICES.md](VICES.md) |
| Land and resources | yes, without a map (D-0036, proposed) | below |
| Population numbers | yes: people per generation, births and deaths (D-0036) | below |
| Economy | yes: labor, food, materials, luxuries, stores, comfort, wealth (D-0036) | below |
| Military | [PLANNED M3] | strength from people, wealth, Wrath and training |
| Government | yes: type, alignment, suspicion, revolutions, votes, revolts | [INSTITUTIONS.md](INSTITUTIONS.md), [HIDDEN_HAND.md](HIDDEN_HAND.md) |
| Puppet decisions | [PLANNED M1] | below |

## Generations (built, model v0)

Three cohorts carry the culture. Youth are shaped by family, school and media; adults are the workforce and the
voice of the nation; elders keep the culture of their youth. Newborns inherit their parents' disposition and
arrive untrained. Youth graduate into adults, adults retire into elders. That flow is why schooling changes a
nation a generation late, and why media's long cultivation reaches the young first.

## Land and resources (economy v1, D-0036, proposed)

Norm: "Countries should have different resources and different landscapes that change how those resources are
accessed and how their populations live and grow."

- Until the map exists (Q-004), a nation holds an area of each of eight terrains: plains, forest, hills, mountains,
  desert, coast, river valley, tundra. The map will supply these areas later.
- Three resources: **food** keeps people alive; **materials** (timber, stone, ore) are the needs of daily life;
  **luxuries** (spices, wine, silks, gems) are its comforts. Plains and river valleys feed; forests, hills and
  mountains build; coasts, deserts and mountains bring luxuries. Yields per terrain: the TerrainYields table in
  [../generated/PARAMETERS.md](../generated/PARAMETERS.md).
- People work needs first (food, then materials) and make luxuries with what is left. Land is a ceiling: more workers
  on crowded land add less and less. Skill from schooling and drive from Greed raise the ceiling; Sloth, unrest and
  hunger cost labor.
- Food stores carry a nation through poor harvests; surplus becomes wealth. Wealth will pay for trade and war (M3).
- Planned: terrain as defense and isolation (mountains), trade routes (coasts, rivers), and who is worth invading (M3).

## Population (economy v1, D-0036, proposed)

- People are counted per generation. Births rise with Lust and open land, and fall with Pride (individualism),
  crowded land and hunger. Deaths fall with Medicine and rise with famine and violence; a proud and wrathful people
  neglects its elders (Norm's example).
- Land pressure (what the people need to eat against what the land could yield) is the natural limit: as land fills,
  families have fewer children, so a nation usually slows before it starves. A dwindling people with open land recovers.
- Starvation (D-0019): a famine that kills a quarter of the people alive when it began loses the nation.

## The puppet government [PLANNED M1]

Norm: "The illuminati do not directly control the actions of their puppets."

The player chooses the government type, and it changes only through revolution or, in a democracy, a vote (D-0021).
Norm: "The government is just a shield for the shadow organization of the player. They are another tool to absorb
or stoke the whims of the people and there to take the fall if the people become unruly." Types and suspicion:
[INSTITUTIONS.md](INSTITUTIONS.md) and [HIDDEN_HAND.md](HIDDEN_HAND.md).

Starvation is one of the four ways to lose a nation (D-0019); it is built (above).

[PROPOSED] Each nation's rulers act on their own every month, reading their society. They crack down when unrest
rises (if their political form allows it), reform when pressure builds (if it allows that), declare war when
aggression, opportunity and appetite line up, and sue for peace when war weariness wins. The player shapes
those conditions; the rulers pull the trigger. Their likely next moves should be readable as a forecast, so
the player can steer ahead of them.
