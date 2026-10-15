# Simulation model

> **Owns:** how the simulation computes: state, the tick pipeline, the equations and their time scales.
> **Budget:** 500 lines. **Read when:** changing or explaining simulation behavior.

Live coefficient values: [../generated/PARAMETERS.md](../generated/PARAMETERS.md) (numbers in this doc are
illustrations, never live values). What feeds what, observed from a run:
[../generated/SOCIETY_GRAPH.md](../generated/SOCIETY_GRAPH.md). Player-facing meaning: [../design/VICES.md](../design/VICES.md).

**Status: model v1 (2026-10-14).** Model v0 (media, education, generations, outlets, wellbeing) plus
per-generation contentment and unrest (D-0032) and politics (D-0019 to D-0024): government types, alignment,
two-tier suspicion, fomented revolutions, votes, generational revolts and the hidden hand's overthrow. Nations do
not interact yet. Tests in `Assets/Tests/Sim/RiverBehaviorTests.cs` and `PoliticsTests.cs` pin the intended
behaviors; if a change breaks one, the design changed and this doc must say so.

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
| Fear, aggression, crime | `SocietyState` | fear and aggression derived; crime a stock |
| Schooling profile per cohort (7 focuses) | `CohortState.Schooling` | slow stock, follows the curriculum |
| Contentment and unrest per cohort | `CohortState` | stocks following their targets |
| National contentment and unrest | `SocietyState` | share-weighted averages of the cohorts |
| Government type, previous type, months in power | `GovernmentState` | changed only by revolution, vote or revolt |
| Alignment with each type | `GovernmentState.Alignment` | derived each tick |
| Government suspicion; shadow suspicion | `GovernmentState` | stock following a target; slow accumulator |
| Lever shock, recent changes, cooldown, pending change | `GovernmentState` | decaying memories and timers |

## Tick pipeline (`Simulation.Step`; the order is part of the model)

1. Apply queued commands (lever changes).
2. **Media** (`MediaSystem`): mood moves toward its target; each cohort's disposition is cultivated.
3. **Education** (`EducationSystem`): youth channeling moves toward the curriculum target.
4. **Generations** (`GenerationSystem`): births, graduation and retirement move disposition and channeling
   between cohorts; every culture drifts toward human nature; national disposition is recomputed.
5. **Outlets** (`OutletSystem`): outlet per vice from a base plus adult channeling.
6. **Wellbeing** (`WellbeingSystem`): national expressed vice, frustration, fear, crime and aggression; then each
   generation's expressed vice, frustration, contentment and unrest; national averages; alarms.
7. **Politics** (`PoliticsSystem`): alignment, government suspicion, shadow drift and alarms; then at most one of:
   resolve a pending revolution or vote, the people call a vote, a generation revolts.
8. Advance the date.

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
  `ContentmentBase + SatisfactionGain * mean(satisfied) - FrustrationGain * sum(f) * (1 - SlothAcceptance * e_Sloth) - FearSuffering * fear - CrimeSuffering * crime`
  (frustration is summed over vices, not averaged: D-0034)
- Crime target, clamped to 0..1:
  `CrimeBase + sum_v CrimeFromFrustration_v * f_v - Deterrence * severity - ContentmentCrimeRelief * max(0, contentment)`
- Unrest target, clamped to 0..1, with suffering `s = max(0, -contentment)`:
  `UnrestFromSuffering * s * (1 + PrideAmplifier * e_Pride + EnvyAmplifier * e_Envy) + UnrestFromWrath * f_Wrath - SlothDampening * e_Sloth - FearDampening * fear`
- Aggression, clamped: `sum_v AggressionWeights_v * e_v + EnvyWrathAggression * e_Envy * e_Wrath`.
- Contentment, crime and unrest follow their targets with their own taus.
- Alarms with hysteresis raise events: UnrestRising and UnrestEased, SufferingSevere and SufferingEased,
  CrimeWave and CrimeEased.

### Generations (model v1, D-0032)
- Contentment and unrest above are computed **per cohort**: each cohort's expressed vice is its own disposition
  plus the national mood, measured against the nation's outlets; its suffering is its own. Fear and crime are national.
- Unrest also gains `UnrestFromSuspicion * governmentSuspicion`, `FomentUnrest` while a revolution is fomented,
  and loses the government type's `UnrestVenting`. Fear's effects scale by the type's `FearSufferingScale` and
  `FearDampeningScale`.
- National contentment and unrest are the share-weighted averages of the cohorts.
- Schooling profile: youth schooling follows the curriculum shares with tau `SchoolTauMonths`; it graduates to
  adults and elders like channeling, without newborn dilution (it records what the schooled were taught).

### Politics (model v1)
- **Alignment** with type k: `AlignmentBase + sum_v AlignmentFromVices[v] * (expressed_v - humanNature_v) +
  sum_f AlignmentFromSchooling[f] * (schooling_f - 1/7)`, plus `InertiaBonus` for the type in power.
- **Government suspicion target**, clamped 0..1: Science schooling of adults (scaled by the type), expressed
  Pride, frustrated Envy and Greed, cultural drift (per vice, disposition distance from the nation's tradition
  beyond `DriftTolerance`, summed), curriculum or single-vice airtime concentration, recent lever shocks (scaled by
  the type), suffering, court severity (by type); minus expressed Sloth, satisfied Gluttony and neutral airtime.
  Stock follows with tau `SuspicionTauMonths`. Tradition starts as the setup disposition and follows the national
  disposition with tau `TraditionTauMonths`.
- Unrest gains `UnrestFromShadowSuspicion * shadowSuspicion` as well: a people that believes in a hidden hand revolts.
- When a revolt topples the hand, shadow suspicion resets to 0 and government suspicion is vented.
- **Shadow suspicion** drifts each month by `LeakRate * max(0, suspicion - LeakThreshold)` minus a slow decay, and
  jumps on each government change by `ChangeLeak * recentChanges` and on failed plots.
- **Changing the government** (revolution success, passed vote, successful revolt): government suspicion is cut by
  `vent * VentDiminishing^recentChanges` (`RevolutionVent` or `VoteVent`); then recentChanges += 1 (decaying with
  tau `RecentChangeTauMonths`); a cooldown starts.
- **Fomented revolution:** `FomentMonths` of stoked unrest, then success with chance `FomentBaseSuccess +
  FomentUnrestSuccess * unrest + FomentAlignmentSuccess * (alignment lead of the target) - FomentFearPenalty * fear`,
  clamped to `ChanceFloor..ChanceCeiling`. Violence either way; failure raises both suspicions.
- **Vote** (types that can vote): passes with chance `0.5 + VoteSensitivity * lead`. The people call one themselves
  when unrest passes `PeopleVoteUnrest` and another type leads by `PeopleVoteMargin` (monthly chance `PeopleVoteChance`).
- **Revolt:** a cohort past `RevoltThreshold` revolts with monthly chance `RevoltChancePerUnrest * excess *
  RevoltFrequency[cohort] * RevoltChanceScale[type]`; succeeds with `RevoltSuccess[cohort] + RevoltUnrestSuccess *
  excess - RevoltFearPenalty * fear`. Violence by cohort. Success installs the type with the best share-weighted
  cohort alignment, the revolting cohort counted `RevoltingCohortWeight` times, elders adding `ElderRestorationBonus`
  to the previous type, Monarchy and Theocracy. Failure: the cohort is crushed and suspicion rises.
- **Overthrow of the hidden hand:** a successful revolt while shadow suspicion is at least `ShadowBreakingPoint`
  also topples the hand; the nation leaves the player's control; if it is the home nation, the game is over.

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
| EachScenarioNationIsMostAlignedWithItsOwnGovernment | scenarios start politically stable |
| AnEducatedPeopleGrowsSuspicious | Norm: "extremely high science" raises suspicion |
| AbruptLeverSwingsRaiseSuspicion | subtlety pays; abrupt swings are noticed |
| LastingGovernmentSuspicionLeaksIntoShadowSuspicion | suspicion of the government becomes suspicion of you |
| AFalseRevolutionReplacesTheShieldAndVentsSuspicion | Norm: foment a false revolution to reduce suspicion |
| EachFalseRevolutionVentsLessAndLeaksMore | "new masters, same strings" |
| AFailedPlotRaisesSuspicion | revolutions are a gamble |
| OnlyDemocraciesVote, AVotePassesWhenThePeopleAlignWithTheTarget | Norm: democracies can vote to change type |
| ARevoltInstallsTheTypeThePeopleAlignWith | D-0024 |
| ElderRevoltsRestoreWhatYouthRevoltsOverthrew | D-0032 generational consequences |
| ARevoltPastTheBreakingPointTopplesTheHiddenHand, BelowTheBreakingPointTheGovernmentTakesTheFall | D-0019, D-0021 |
| EachGenerationHasItsOwnContentment | D-0032 |

## Observations from the runner (2026-10-14, default tuning)

- Baseline: the three nations stay stable for 60 years and keep their personalities.
- All-Wrath media at home: expressed Wrath 0.35 to 0.85 within 30 years; crime 0.05 to 0.27; aggression 0.33 to 0.60.
- Switching schools to Military in year 30 eases Wrath frustration from 0.57 to 0.34 by year 80: the lag works.
- Model v1 balance changes (2026-10-14): `RevoltThreshold` 0.5 to 0.3 and `UnrestFromSuffering` 0.8 to 1.2 (60
  years of tyranny never reached the old threshold); `FomentUnrest` 0.25 to 0.5, `FomentBaseSuccess` 0.2 to 0.35,
  `FomentAlignmentSuccess` 2 to 1 (fomented revolutions always failed at the 5% floor).
- After them (seeds 1 to 3): "false-revolution" revolutions toward an unpopular Monarchy succeed about 30% of the
  time, back toward a popular Democracy about 75%; failures raise suspicion, successes vent it. "tyranny" ends with
  the people voting in a Military Junta (aligned with their Wrath), venting suspicion; shadow suspicion reaches
  about 0.5 first. "people-vote" for Monarchy passed at 30%. Generational revolts stay rare in these scripts.

## Balance targets (D-0033, pinned by BalanceTests.cs)

Untouched world calm for a century; a 2:1 tilt below the alarm; one extreme lever raises the alarm within a decade;
every lever at an extreme overloads suspicion within five years; sustained suspicion breaks shadow suspicion within
15 years and topples the hand within 30 in most games.

Balance pass 1 (2026-10-15): `ConcentrationSuspicion` 0.3 to 0.8, concentration starts 0.5 and 0.4 to 0.4 and 0.35,
drift against tradition (new), `SuspicionTauMonths` 18 to 12, `LeakThreshold` 0.4 to 0.5, `LeakRate` 0.01 to 0.02,
`UnrestFromSuspicion` 0.2 to 0.5, `UnrestFromShadowSuspicion` 0.4 (new). Runner results: extremist alarm in year 2,
rumors year 6, overthrow year 13; whiplash overthrow year 10; one-vice propaganda overthrow around year 20; a
one-focus monoculture holds at the alarm without leaking; the AI nations stay calm for 100 years.

Balance pass 2 (2026-10-15, D-0034): frustration summed in contentment (gain 0.5), `MoodGain` 0.25 to 0.5,
`ChannelGain` 0.6 to 0.8, `SlothDampening` 0.15 to 0.10, Democracy `UnrestVenting` 0.08 to 0.04. Sweep (3:1 tilts,
deltas against untouched): Wrath tilt in a year contentment -0.04, crime +0.03, aggression +0.05; all-Wrath airtime
-0.21, +0.14, +0.24; courts 0 versus 1: contentment 0.18 apart, crime 0.08. Extreme play topples the hand in years 11
to 14 (pass 1 pace kept); slow-burn sours contentment from +0.18 to +0.06 over 80 years without revolts.

## Known simplifications (v1)

- After the hand falls, the nation's levers stay where the player left them (no one resets them until puppet
  governments exist), so an abandoned extreme keeps that nation in turmoil.

- Fixed cohort shares: no population numbers, births or deaths yet (revolutions "cost lives" only as contentment).
- Outlets have no venues; nations do not interact; no puppet decision-making beyond votes and revolts.
- Rival hidden hands are not modeled, so only the player's hand can be overthrown.
- Mood noise is independent month to month (no news stories yet).
