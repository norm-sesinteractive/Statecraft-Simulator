# D-0036: Economy v1: land, labor, food, goods, wealth and people

**Status:** Proposed | **Date:** 2026-10-15 | **Area:** Simulation

## Context
D-0035: the full M2 economy comes before trade and war. The map is still open (Q-004), so land must work without
geography. Starvation is a defeat (D-0019). Trade (D-0025 to D-0031) and war will need goods, prices, money and people.

## Decision
- **Land without a map:** each nation holds an area of eight terrains (plains, forest, hills, mountains, desert, coast,
  river valley, tundra). Each terrain yields food, materials and luxuries up to a ceiling; the map will later supply
  the areas.
- **Labor on land:** adults plus a share of elders work, slowed by Sloth, unrest and hunger. Output approaches the
  land's ceiling with diminishing returns. Labor goes to needs first (food and refilling the stores, then materials),
  and the rest makes luxuries, which always keep a small reserved share. Farmers work more fields in a poor year.
- **Every curriculum focus earns its keep:** Agriculture raises food, Infrastructure raises materials, Arts raises
  luxuries, Science raises all three a little, Commerce turns surplus into more wealth, Medicine lowers deaths.
  Military waits for war (M3). Infrastructure's effect on output is Claude's addition to Norm's transit efficiency
  (D-0029 to D-0031).
- **Vices in the economy:** Gluttony raises what each person eats; Greed drives materials and luxuries; Sloth costs
  labor; Lust raises births; Pride lowers births and, with Wrath, neglects elders (Norm's example).
- **People:** real numbers per generation. Births per adult; deaths per generation (less with Medicine, more in
  famine). The base rates are the steady state of the default 30/55/15 population, so an untouched default nation
  neither grows nor shrinks. Land pressure (food need over the land's ceiling) slows births as land fills and speeds
  them where land is open, so populations find a level instead of growing into famine or shrinking forever.
- **Food and starvation:** stores hold six months of need; the harvest varies by year. Hunger hurts contentment,
  births, health and labor. A famine begins at hunger 0.15; a nation **starves** (is lost) when one famine kills a
  quarter of the people alive when it began.
- **Comfort:** luxuries per head against what the people have come to expect (expectation follows with a ten-year
  lag). Comfort is a venue for Greed, Lust and Gluttony outlets. A materials shortage is hardship.
- **Wealth:** what is produced beyond need, valued at prices (food 1, materials 2, luxuries 6), times Commerce; it
  wears away with a ten-year time constant. It does nothing else yet: trade and war will spend it.
- Revolutions and revolts now cost lives.

## Because
Norm: "Countries should have different resources and different landscapes that change how those resources are
accessed and how their populations live and grow." Needs-first labor and adapting expectations keep the model readable
and keep the balance passes intact (the untouched world stays calm for a century and pass 1 pace holds). The three
scenario nations get distinct lands: the warrior junta is rich in materials and short of food, the decadent monarchy
rich in food and luxuries and crowded, which gives M3 trade its motives.

## Open
- Levers alone rarely starve a nation: crowding slows births first. Starvation becomes a live threat with war and
  food imports (M3). Norm may want it reachable sooner.
- Whether Infrastructure should raise materials output, or stay purely transit efficiency.
