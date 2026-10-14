# Glossary

> **Owns:** the project's terms and their names in code. **Budget:** 120 lines.
> Use these words in docs, code and conversation; add a row before introducing a new term.

| Term | Meaning | In code |
|---|---|---|
| Hidden hand | The player's secret society (working name; Norm names it) | (the player) |
| Nation | A country: institutions, society, later land and people | `Nation` |
| Puppet government | A nation's rulers, who act on their own; never commanded | [PLANNED] |
| Institution | A social machine the player steers: media, education, law and order, politics | `InstitutionSet` |
| Lever | A control the player sets on an institution | `MediaMix`, `Curriculum`, `LawPolicy` |
| Vice | One of seven drives: Wrath, Sloth, Greed, Lust, Pride, Envy, Gluttony | `Vice` |
| Disposition | How strongly a cohort carries a vice: deep culture, slow to change | `CohortState.Disposition` |
| Mood | Short-lived stirring of a vice by media (later events) | `SocietyState.Mood` |
| Expressed vice | Disposition plus mood: what acts on the nation now | `SocietyState.Expressed` |
| Outlet | Capacity to channel a vice productively | `SocietyState.Outlets` |
| Channeling | Trained habits and skills that create outlets, taught by schools | `CohortState.Channeling` |
| Venue | A real place for a drive to go: an army, markets, an arts scene | [PLANNED] |
| Satisfaction | Expressed vice that fits its outlet | computed in `WellbeingSystem` |
| Frustration | Expressed vice beyond its outlet | `SocietyState.Frustration` |
| Contentment, suffering | One axis: -1 suffering to +1 content | `SocietyState.Contentment` |
| Fear | Dread of punishment created by court severity | `SocietyState.Fear` |
| Crime | Lawbreaking from frustration, net of deterrence and contentment | `SocietyState.Crime` |
| Unrest | Organized discontent that can become revolt | `SocietyState.Unrest` |
| Aggression | A nation's appetite for war | `SocietyState.Aggression` |
| Cohort | A generation band: Youth, Adult, Elder | `Cohort`, `CohortState` |
| Push | How hard the media presses a vice relative to an even mix | `MediaMix.Push` |
| Cultivation | Slow reshaping of disposition by years of airtime | `MediaTuning.Cultivation*` |
| Human nature | The disposition a culture drifts back to without pressure | `GenerationTuning.HumanNature` |
| Tick | One simulated month | `SimDate` |
| Command | The only way to change sim state from outside | `SimCommand` |
| Event | A notable change the sim reports outward | `SimEvent`, `EventLog` |
| Ledger, contribution | The record of every term behind a computed value ("why?") | `Ledger`, `Contribution` |
| Tuning | The model's coefficients | `SimTuning` |
| Scenario | A starting world: a list of nation setups | `NationSetup`, `ScenarioLibrary` |
| Lever script | Scripted player behavior for headless experiments | `LeverScripts` (runner) |
| Dev HUD | The developer overlay; not the player UI | `DevHud` |
