# Glossary

> **Owns:** the project's terms and their names in code. **Budget:** 120 lines.
> Use these words in docs, code and conversation; add a row before introducing a new term.

| Term | Meaning | In code |
|---|---|---|
| Hidden hand | The player's secret society (working name; Norm names it) | (the player) |
| Nation | A country: institutions, society, later land and people | `Nation` |
| Puppet government | A nation's rulers, who act on their own; never commanded | [PLANNED] |
| Government type | The political system the player chooses: Democracy, Monarchy, Communism, Military junta, Theocracy | `GovernmentType`, `GovernmentState` |
| Shield | The government's role: absorbs the people's anger and takes the fall for the hidden hand | `PoliticsSystem.ChangeGovernment` |
| Government suspicion | The people's sense that their rulers do not serve them | `GovernmentState.Suspicion` |
| Shadow suspicion | The people's sense that someone behind the rulers pulls the strings | `GovernmentState.ShadowSuspicion` |
| Revolution | A change of government type the player foments (months, can fail) | `FomentRevolution` |
| False revolution | A revolution the player foments to replace the shield and vent suspicion | `FomentRevolution` |
| Revolt | A generation rising on its own when its unrest passes the threshold | `PoliticsSystem` (RevoltSucceeded, RevoltCrushed) |
| Vote | A democracy's peaceful change of government type, started by the player or the people | `CallVote` |
| Home nation | The nation the player starts with; losing it ends the game (D-0023) | `World.PlayerHomeIndex`, `Simulation.IsGameOver` |
| Alignment | How strongly a population favors each government type; picks the type after a revolt | `GovernmentState.Alignment` |
| Tradition | What a nation considers its normal culture: its starting disposition, slowly accepting lasting change | `SocietyState.Tradition` |
| Cultural drift | Vices pushed away from tradition; raises suspicion (D-0033) | `PoliticsSystem` (Cultural drift term) |
| Lever shock | Recent abrupt lever changes, decaying; raises suspicion | `GovernmentState.LeverShock` |
| Culture | A cohort's vices (disposition); carried to importers by trade | [PLANNED] |
| Schooling profile | A cohort's education share per curriculum focus | `CohortState.Schooling` |
| Educational overlap | Sum over focuses of the smaller share between two cohorts; gates trade influence and goodwill | [PLANNED] |
| Commerce floor | Half a cohort's Commerce share: the minimum trade influence and goodwill rate (D-0028) | [PLANNED] |
| Infrastructure efficiency | 50% at 0 infrastructure to 100% at 100; sets transit loss on trade and military efficiency (D-0030) | [PLANNED] |
| Relations | Score between two peoples, -100 hated rivalry to +100 lasting bond | [PLANNED] |
| Rival hand | Another shadow organization steering other nations | [PLANNED] |
| Cultural conquest | A nation's people falling under a rival hand through culture rather than war | [PLANNED] |
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
