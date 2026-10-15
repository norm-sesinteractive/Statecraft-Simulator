# The world stage

> **Owns:** everything between nations: war, trade, culture, diplomacy and rival influence.
> **Budget:** 300 lines. **Read when:** designing anything that crosses a border.

Nothing here is built yet [PLANNED M3]. Norm: "Will you flood the banks in just such a spot to overflow across
a great land? Will you smooth the rapids and peacefully exist within your boundaries? How will you survive when
a rival river tries to run into you?"

## War

Norm: "increasing violence in the media increases aggression in the society. This would make a country more
likely to undertake wars on neighboring countries and also to be better at doing so, but it would also have
repercussions on the aggressive country, increasing acts of internal violence. Conversely a country that
focuses only on peace and no defensive activities may have a very happy population, but become open to
military conquest from jealous neighbor countries."

- [PROPOSED] Only puppet governments declare war. Appetite comes from aggression (Wrath, Greed, Pride, Envy x
  Wrath), opportunity (a weaker, richer, reachable neighbor) and scarcity (Gluttony's pressure).
- [PROPOSED] Wars resolve abstractly, month by month. Strength comes from population, wealth, training
  (Military schooling) and morale (Pride, and Wrath that has an outlet); terrain favors defenders.
- [PROPOSED] Consequences: territory, casualties, war weariness, surges of Wrath and Pride after victory, and
  humiliation after defeat (feeding Envy and Wrath: the seed of the next war).
- War is itself an outlet for Wrath: a nation at war has somewhere to put its aggression.

## Trade

Norm: "Positive interactions between nations will also need to exist, primarily in the forms of trade and culture."

- [PROPOSED] Trade flows between nations with complementary resources, driven by Greed and enabled by peace. It
  gives Greed an outlet, builds wealth, and creates dependence that makes war costlier.
- Every act of trade carries culture to the buyer (below) and improves relations between the two peoples (D-0026).

## Culture through trade (D-0025)

Norm: "An act of trade will bring the receiving/paying country slightly closer towards the cultural levels of the
supplying nation. Nations never change culture based on exports of goods or services, only on receiving them."

- **Direction:** only the importer changes.
- **What moves:** vices only (cohort dispositions). Education does not travel; it sets how much gets through (D-0027).
- **Source:** always the supplier's adult (active) generation.
- **Receivers:** youth take it at 2x, adults at 1x, elders at 0.5x.
- **Strength, part 1, educational overlap (D-0027):** for each receiving cohort, the sum over curriculum focuses of
  the smaller share between that cohort and the supplier's adults. Norm's example: supplier adults 75 Agriculture
  and 25 Science against receivers at 100 Agriculture gives 75%; against receivers with Science and no Agriculture,
  25%. The overlap scales both the vice change and the goodwill gained.
- **Commerce floor (D-0028):** the rate is never below half the receiving cohort's Commerce share, so a fully
  commercial people reaches 50% with anyone; the floor never adds to a real match.
- **Infrastructure (D-0029 to D-0031):** changes efficiency, not appeal. The score is the adult generation's
  Infrastructure schooling share x 100; efficiency runs linearly from 50% at 0 to 100% at 100 (2 points per 1%). Deals keep their agreed price; the loss is in transit:
  the supplier keeps agreed x its efficiency, and the buyer pays more than agreed ([PROPOSED] agreed x (2 - its
  efficiency), so 1.5x at worst). The same curve scales the efficiency of military activity [PROPOSED].
- **Strength, part 2, contentment:** each receiving cohort compares its own contentment with the supplier's adults'
  ("not against their would-be generational counterparts"). A happier supplier persuades more.

[PROPOSED] formulas, to tune with the runner:
- Per month of trade, each receiving cohort's dispositions move toward the supplier's adult dispositions by
  `base * tradeVolume * cohortWeight * max(overlap, 0.5 * cohortCommerce) * clamp(1 + (supplierAdultContentment - cohortContentment), 0, 2)`.
- Goodwill from that trade (D-0026) is scaled by the importer's population-weighted overlap.
- Needs contentment per cohort (D-0032) and schooling per cohort by curriculum focus (D-0027).
- Optional modifiers from the pitch, off by default: the receiver's Envy raises receptiveness ("susceptible to other
  cultures"), the supplier's Lust raises cultural reach ("potentially culture spread").

## Cultural conquest (D-0019, D-0025)

Norm: "Cultural conquests will go along with trade and the interplay between the vices and educations of
neighboring nations."
[PROPOSED] tipping point: a nation falls to the hand that holds its supplier when, for several years in a row, its
adult culture lies within a small distance of that supplier's adult culture, that supplier provides most of its
imports, and relations with that supplier are strongly positive. The player sees the drift coming as a "cultural
pull" gauge toward each trading partner.

## Relations between peoples (D-0026)

Norm: "Nations should track a contentment score between their peoples."

- Every pair of nations has a score from -100 (hated rivalry) to +100 (lasting bond).
- Trade pulls it up; military action pulls it down, "a balanced and opposite relationship with trade".
- With no action underway it drifts back to 0, slowest at the extremes and faster near 0: "The worse things get,
  the longer it takes to cool down and the reverse."
- [PROPOSED] Drift per month toward 0: `rate * (1 - |score| / 100)^2 + floor`, never overshooting 0.
- [PROPOSED] Effects: low scores raise the puppets' appetite for war against that neighbor; high scores make
  trade more likely and war less so; strongly positive scores are one condition of cultural conquest.

## Diplomacy

- [OPEN] Alliances, threats and treaties as puppet decisions, or left implicit? Keep it minimal unless it earns
  its place under "simple controls".

## Rival rivers (D-0022)

- Rival shadow organizations steer other nations. They can take the player's nations by military or cultural
  conquest, and the player can take theirs the same ways; a conquered nation joins the conqueror's reach.
- Cultural conquest is a defeat (D-0019): "your population becomes controlled by some other shadow organization".
  It rides on trade-borne culture, above.
- [PROPOSED] Rival hands run on the same levers as the player, with personalities (a warmonger, a decadent,
  a schoolmaster), so their nations read as distinct rivers.
