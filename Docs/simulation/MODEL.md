# Simulation model

> **Owns:** how the simulation computes: state, the tick pipeline, the equations and their time scales.
> **Budget:** 500 lines. **Read when:** changing or explaining simulation behavior.

Live coefficient values: [../generated/PARAMETERS.md](../generated/PARAMETERS.md) (numbers in this doc are
illustrations, never live values). What feeds what, observed from a run:
[../generated/SOCIETY_GRAPH.md](../generated/SOCIETY_GRAPH.md). Player-facing meaning: [../design/VICES.md](../design/VICES.md).

**Status: model v0 (2026-10-14).** Media, education, generations, outlets and wellbeing, for any number of
nations that do not yet interact. The whole model is [PROPOSED] (D-0011, D-0012) until Norm reviews it. The
tests in `Assets/Tests/Sim/RiverBehaviorTests.cs` pin the intended behaviors; if a change breaks one, the
design changed and this doc must say so.

## Conventions

- One tick is one month. Time constants (tau) are in months.
- Vice values, outlets, channeling, crime, unrest, fear and aggression run 0 to 1. Mood runs -1 to 1.
  Contentment runs -1 (suffering) to +1 (content).
- **Lag:** a stock follows its target as `x += (target - x) * (1 - exp(-1 / tau))` per tick: about 63% of the
  gap closes in one tau.
- **Targets are sums of recorded terms.** Each additive term passes through the ledger, so the "why?" panel and
  the generated graph show exactly what the code computed (D-0013).
- Randomness only through `SimRandom` (seeded PCG32). Model v0 uses it only for news-cycle noise on mood.

## State per nation

| Quantity | Code | Kind |
|---|---|---|
| Airtime shares (7 vices + neutral) | `MediaMix` | lever |
| Curriculum shares (7 focuses) | `Curriculum` | lever |
| Court severity | `LawPolicy.Severity` | lever |
| Cohort share, disposition, channeling (Youth, Adult, Elder) | `CohortState` | slow stocks |
| Mood per vice | `SocietyState.Mood` | fast stock |
| National disposition | `SocietyState.Disposition` | derived: share-weighted cohorts |
| Expressed, outlets, frustration per vice | `SocietyState` | derived each tick |
| Fear, aggression | `SocietyState` | derived each tick |
| Contentment, crime, unrest | `SocietyState` | stocks following their targets |

## Tick pipeline (`Simulation.Step`; the order is part of the model)

1. Apply queued commands (lever changes).
2. **Media** (`MediaSystem`): mood moves toward its target; each cohort's disposition is cultivated.
3. **Education** (`EducationSystem`): youth channeling moves toward the curriculum target.
4. **Generations** (`GenerationSystem`): births, graduation and retirement move disposition and channeling
   between cohorts; every culture drifts toward human nature; national disposition is recomputed.
5. **Outlets** (`OutletSystem`): outlet per vice from a base plus adult channeling.
6. **Wellbeing** (`WellbeingSystem`): expressed vice, frustration and fear, then the targets for contentment,
   crime, unrest and aggression; the stocks move; alarms raise events.
7. Advance the date.

## Equations (model v0)

### Media
- Push on vice v: `p_v = (share_v - 1/8) / (1 - 1/8)`. 0 for an even mix, +1 with all the airtime, -1/7 with none.
- Mood target: `MoodGain * p_v`. Mood follows with tau `MoodTauMonths`, plus noise `MoodNoise * N(0, 1)`.
- Cultivation of cohort c each month: `rate = p_v * Cultivation_c`. Disposition gains `rate * (1 - d)` when the
  rate is positive and loses `|rate| * d` when negative, so it saturates and never leaves 0 to 1.

### Education
- Curriculum target: `T_v = sum over focuses f of share_f * FocusChanneling[f][v]`, clamped to 0..1.
- Youth channeling follows `T` with tau `SchoolTauMonths`.

### Generations (cohort shares are fixed in v0)
- Monthly birth fraction `b = 1 / (12 * YouthYears)`; retirement fraction `r = 1 / (12 * AdultYears)`.
- Youth disposition moves toward adult disposition by `b` (newborns inherit their parents' culture). Youth
  channeling decays by `b` (newborns are untrained).
- Adults move toward youth by `b * youthShare / adultShare` (the graduates); elders move toward adults by
  `r * adultShare / elderShare`.
- Every cohort's disposition drifts toward `HumanNature` with tau `NatureTauMonths`.
- With the defaults the adult cohort replaces itself over roughly 33 years: the generational lag of schooling.

### Outlets
- `outlet_v = clamp01(BaseOutlet_v + ChannelGain * adultChanneling_v)`.
- [PLANNED] Venues: an army at war, open markets, an arts scene, abundance. Outlets will need both trained
  people and somewhere for the drive to go.

### Wellbeing
- Expressed `e_v = clamp01(disposition_v + mood_v)`; frustration `f_v = max(0, e_v - outlet_v)`; satisfied
  `min(e_v, outlet_v)`.
- Fear: `FearPerSeverity * severity`.
- Contentment target, clamped to -1..1:
  `ContentmentBase + SatisfactionGain * mean(satisfied) - FrustrationGain * mean(f) * (1 - SlothAcceptance * e_Sloth) - FearSuffering * fear - CrimeSuffering * crime`
- Crime target, clamped to 0..1:
  `CrimeBase + sum_v CrimeFromFrustration_v * f_v - Deterrence * severity - ContentmentCrimeRelief * max(0, contentment)`
- Unrest target, clamped to 0..1, with suffering `s = max(0, -contentment)`:
  `UnrestFromSuffering * s * (1 + PrideAmplifier * e_Pride + EnvyAmplifier * e_Envy) + UnrestFromWrath * f_Wrath - SlothDampening * e_Sloth - FearDampening * fear`
- Aggression, clamped: `sum_v AggressionWeights_v * e_v + EnvyWrathAggression * e_Envy * e_Wrath`.
- Contentment, crime and unrest follow their targets with their own taus.
- Alarms with hysteresis raise events: UnrestRising and UnrestEased, SufferingSevere and SufferingEased,
  CrimeWave and CrimeEased.

## Game start

`Simulation.Settle` starts each nation at equilibrium for its starting levers (steady-state channeling, mood at
its target, stocks at their targets), so a new game opens without a transient (pinned by a test).

## Behaviors pinned by tests

| Test | Design statement |
|---|---|
| MediaStirsMoodWithinAYear | media works within months |
| YearsOfAirtimeLeaveACulturalResidueAfterTheMoodFades | media held for years changes the culture |
| SchoolingReachesTheWorkforceAGenerationLate | education is slow and deep |
| OutletsTurnWrathIntoContentmentInsteadOfCrime | outlets turn a vice into contentment |
| AnAggressiveCultureWithOutletsCanBeHappy | Norm: aggressive cultures with outlets can be happy |
| HarshCourtsCutCrimeButBreedFear | Norm: the pardon-versus-crackdown trade |
| ASufficientlyContentPeopleNeedsNoCourts | Norm: so happy that crime no longer takes place |
| SlothSoftensSufferingIntoAcceptance | Sloth is complacency and acceptance |

## Observations from the runner (2026-10-14, default tuning)

- Baseline: the three nations stay stable for 60 years and keep their personalities.
- All-Wrath media at home: expressed Wrath 0.35 to 0.85 within 30 years; crime 0.05 to 0.27; aggression 0.33 to 0.60.
- Switching schools to Military in year 30 eases Wrath frustration from 0.57 to 0.34 by year 80: the lag works.
- Unrest never rises in the built-in scripts because contentment never turns negative. Balance question for M1.

## Known simplifications (v0)

- Fixed cohort shares: no population numbers, births or deaths yet.
- Outlets have no venues; nations do not interact; there is no politics and no puppet decision-making.
- Mood noise is independent month to month (no news stories yet).
