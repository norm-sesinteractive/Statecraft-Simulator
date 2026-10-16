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
  violence (Norm: "increasing acts of internal violence"); with Pride, elder neglect (economy v1).

### Sloth
Norm: "complacency, acceptance."
- Outlets: leisure, comfort and care (v0: Agriculture, Medicine, Arts).
- Frustrated: little crime, but listlessness: expressed Sloth costs labor (economy v1).
- Effects: softens frustration into acceptance and dampens unrest (v0). A slothful people endures more and
  achieves less; too much is the road to decadence.

### Greed
Norm: "many obvious implications with financial systems and desire of conquest."
- Outlets: commerce, markets, property, trade (v0: Commerce, Infrastructure, some Agriculture).
- Frustrated: theft, fraud and corruption; a strong source of crime (v0).
- Effects: economic drive (economy v1: more materials and luxuries) and trade appetite [PLANNED]; desire for
  conquest when blocked (v0 aggression weight). Luxuries are one of its venues (comfort).

### Lust
Norm: "influence reproductive rate and potentially culture spread."
- Outlets: family life, romance, the arts (v0: Arts, Medicine).
- Frustrated: vice crime and scandal (v0, small).
- Effects: birth rate (economy v1); luxuries are a venue (comfort); cultural output that spreads to neighbors [PLANNED].

### Pride
Norm: "A little pride helps drive a population, a lot can drive them into individualism. This can create high
highs in technological growth, but also drives people apart. An aggressive and individualistic society would
suffer from acts of violence and a society that doesn't take care of its elder's, etc."
- Outlets: achievement, science, monuments, prestige and honor (v0: Science, Military, Arts, Infrastructure).
- Frustrated: defiance; amplifies unrest when people suffer (v0).
- Effects: fewer children and elder neglect at high levels, worst with Wrath (economy v1); technological growth and
  weaker cohesion [PLANNED].

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
- Effects: food eaten per person (economy v1), so land fills sooner; luxuries are a venue (comfort); scarcity
  pressure that becomes appetite for neighbors' land [PLANNED].

## Contentment and suffering

Norm: "Weaved with all of these is the contentment/suffering of the average citizen."
- One axis in v0: -1 is suffering, +1 is content. Q-010 asks whether two gauges would play better.
- Per generation (model v1, D-0032): youth, adults and elders each have their own contentment (their expressed vices
  against the nation's outlets) and their own unrest; the national value is the population-weighted average.
- Raised by satisfaction. Lowered by frustration of each vice, counted separately so one starved drive hurts on its
  own (D-0034), softened by Sloth; by fear of the courts; by crime; by hunger and a shortage of materials (D-0036).
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
- Economy v1 (D-0036, proposed) chose: Lust and Pride for births, Pride and Wrath for elder deaths, Gluttony for
  consumption, Greed for production and Sloth for lost labor. Norm to confirm.
