# The seven vices and contentment

> **Owns:** what each vice means in play, its outlets, what it does when satisfied or frustrated, and the
> contentment and suffering axis. **Budget:** 300 lines.
> **Read when:** touching vices, outlets, contentment, crime, unrest or aggression.

The math lives in [../simulation/MODEL.md](../simulation/MODEL.md); real-world grounding in
[../simulation/RESEARCH.md](../simulation/RESEARCH.md); live values in
[../generated/PARAMETERS.md](../generated/PARAMETERS.md).

## The core idea (Norm)

Every society carries all seven vices in some measure. "Aggressive cultures, that have outlets for that
aggression, can still be happy cultures. Cultures that don't have outlets for their share of the 7 experience
an increase in suffering. This may lead to revolt at the worst, or just leave the country less effective and
more likely to be invaded."

## Anatomy of a vice [PROPOSED, D-0012; built in model v0]

| Part | Meaning | Changes |
|---|---|---|
| Disposition | how strongly the people carry the vice: their culture | over years and generations |
| Mood | how stirred up it is right now (media, later events) | within months |
| Expressed | disposition plus mood: what acts on the nation now | every month |
| Outlet | how much of it the society can channel productively | as each trained generation comes of age |
| Satisfaction | expressed vice that fits its outlet: makes people content | every month |
| Frustration | expressed vice beyond its outlet: becomes suffering, crime, unrest | every month |

A strongly expressed vice with a large outlet satisfies more than a weak one. That is how a warlike people can
be a happy people (pinned by the test `AnAggressiveCultureWithOutletsCanBeHappy`).

## The seven

Each entry gives Norm's definition, then [PROPOSED] outlets, frustration and effects. "v0" marks what the
model already does; outlets come from curriculum channeling in v0, and venues are planned.

### Wrath
Norm: "Aggression, vengeance, punishment."
- Outlets: military service and war (v0: Military schooling); a public appetite for harsh justice; sport and spectacle.
- Frustrated: violent crime, feuds and riots; feeds unrest directly (v0).
- Effects: war appetite (v0: the largest aggression weight) and military effectiveness [PLANNED]; internal
  violence (Norm: "increasing acts of internal violence").

### Sloth
Norm: "complacency, acceptance."
- Outlets: leisure, comfort and care (v0: Agriculture, Medicine, Arts).
- Frustrated: little crime, but listlessness [PLANNED: lower productivity].
- Effects: softens frustration into acceptance and dampens unrest (v0). A slothful people endures more and
  achieves less; too much is the road to decadence.

### Greed
Norm: "many obvious implications with financial systems and desire of conquest."
- Outlets: commerce, markets, property, trade (v0: Commerce, Infrastructure, some Agriculture).
- Frustrated: theft, fraud and corruption; a strong source of crime (v0).
- Effects: economic drive and trade appetite [PLANNED]; desire for conquest when blocked (v0 aggression weight).

### Lust
Norm: "influence reproductive rate and potentially culture spread."
- Outlets: family life, romance, the arts (v0: Arts, Medicine).
- Frustrated: vice crime and scandal (v0, small).
- Effects [PLANNED]: birth rate; cultural output that spreads to neighbors.

### Pride
Norm: "A little pride helps drive a population, a lot can drive them into individualism. This can create high
highs in technological growth, but also drives people apart. An aggressive and individualistic society would
suffer from acts of violence and a society that doesn't take care of its elder's, etc."
- Outlets: achievement, science, monuments, prestige and honor (v0: Science, Military, Arts, Infrastructure).
- Frustrated: defiance; amplifies unrest when people suffer (v0).
- Effects [PLANNED]: technological growth; weaker cohesion and elder care at high levels, worst with Wrath.

### Envy
Norm: "Susceptible to other cultures. Also creates a more conquest oriented society, taking what others have
if aggression is high."
- Outlets: social mobility, emulation, status competition (v0: Infrastructure, Commerce, some Science and Arts).
- Frustrated: resentment and crime; amplifies unrest when people suffer (v0).
- Effects: with Wrath, appetite for conquest (v0: the Envy x Wrath aggression term); openness to foreign
  culture [PLANNED].

### Gluttony
Norm: "influence resource consumption. There may be times when it is beneficial to the player's desire to
consume more resources and drive a higher desire in a country's people to invade their neighboring countries."
- Outlets: abundance and feasting (v0: Agriculture, some Commerce).
- Frustrated: hoarding and black markets (v0, small crime weight).
- Effects [PLANNED]: consumption per person; scarcity pressure that becomes appetite for neighbors' land.

## Contentment and suffering

Norm: "Weaved with all of these is the contentment/suffering of the average citizen."
- One axis in v0: -1 is suffering, +1 is content. Q-010 asks whether two gauges would play better.
- Per generation (model v1, D-0032): youth, adults and elders each have their own contentment (their expressed vices
  against the nation's outlets) and their own unrest; the national value is the population-weighted average.
- Raised by satisfaction. Lowered by frustration (softened by Sloth), by fear of the courts, and by crime.
- Suffering feeds unrest, amplified by Pride and Envy. Contentment starves crime: a content enough people can
  reach zero crime (pinned by `ASufficientlyContentPeopleNeedsNoCourts`).

## What the vices produce (v0)

| Outcome | Rises with | Falls with |
|---|---|---|
| Crime | frustration (Wrath, Greed and Envy most) | court severity (deterrence), contentment |
| Unrest | suffering, amplified by Pride and Envy; Wrath frustration | Sloth, fear |
| Aggression | Wrath, Greed, Pride, Envy, Envy x Wrath | Sloth |
| Fear | court severity | |

## Open

- Should each vice show the player a virtue mirror (Q-006)?
- Which vices should first drive births, production and consumption (M2)?
