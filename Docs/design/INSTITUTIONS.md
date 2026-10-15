# Institutions: the levers

> **Owns:** every lever the hidden hand can pull, what each acts on, and how fast. **Budget:** 350 lines.
> **Read when:** designing or changing anything the player controls.

The organizing idea [PROPOSED, D-0011]: each institution acts on a different part of the river, on its own
time scale. Media fills it, education digs the channels, law builds the levees, politics decides who steers.
That keeps each lever distinct and makes timing the strategy. Math: [../simulation/MODEL.md](../simulation/MODEL.md).

## Media (built, model v0)

Norm: "Control over the media is the most simple to understand, it would effectively allow a balance of the 7
to be aired to the population."

- **Control:** an airtime mix: a share for each vice plus neutral programming, summing to 100%.
- **Fast effect:** mood follows the mix within months.
- **Slow effect:** years of airtime cultivate disposition, the young fastest and elders slowest. Taking a
  message off the air leaves a cultural residue (pinned by a test).
- **Asymmetry:** inflaming is easier than calming. All the airtime is a push of +1; none of it is -1/7.
- [OPEN] Q-006: keep airtime shares, or seven virtue-to-vice sliders?
- [PLANNED] Media technology by era (criers, print, radio, screens) changing reach and speed (Q-003).

## Education (built, model v0)

Norm: "Control over the educational system would be very powerful in influencing the outlets/outcomes of the
7 emotional influencers. The school might push for democratic elections, or communism, or any common form of
government. It might push for agricultural growth, military growth, medical growth, infrastructural growth,
potentially others."

- **Control:** curriculum shares across focuses: Agriculture, Military, Medicine, Infrastructure (Norm), plus
  Science, Commerce and Arts [PROPOSED]. Spreading the school thin trains each thing thinly.
- **Effect:** schools train the youth cohort's channeling, the habits and skills that give a vice a
  productive outlet. Adults change only as the trained young come of age: about a decade to show, a
  generation to complete (pinned by a test).
- **Which focus channels which vice** [PROPOSED]: the FocusChanneling table in
  [../generated/PARAMETERS.md](../generated/PARAMETERS.md). Norm should review it.
- [PLANNED] Civic ideology: which form of government the young come to favor (feeds Politics).
- [PLANNED] Schooling between nations: shared schooling lets trade carry vices and goodwill (D-0027); Commerce sets a
  floor under that (D-0028); Infrastructure makes revenue and military activity efficient (D-0029). See WORLD_STAGE.md.
- [PLANNED] Sector growth: trained farmers grow food, trained engineers build (M2 economy).
- [OPEN] Q-016: should schools also shape values (dispositions), not only outlets?

## Law and order (built, model v0)

Norm: "Should courts crack down ruthlessly? Should they pardon all but the most guilty? Pardoning the minor
offenders may make a society suffer less, but the reduction in fear of punishment may lead to increased crime."

- **Control:** court severity, from 0 (pardon all but the most guilty) to 1 (ruthless crackdown).
- **Effects:** deterrence lowers crime; fear lowers contentment; fear also suppresses unrest in the short run,
  a pressure cooker. Pinned: harsh courts cut crime but breed fear and suffering.
- Norm: "Influence over Law & Order would go hand in hand with the political system of the land."
  [PLANNED] which severities a political form tolerates, and what crackdowns cost in legitimacy.
- [PLANNED] Where the courts look (which crimes they pursue) as a second simple control.

## Politics (D-0021, built in model v1)

Norm: "the political system chosen by the player is the government system of the people. If the player wants,
they should be able to change the government type, but not without instigating a revolution of some kind.
Democracies can vote to change to another type, but they can also experience revolts/revolutions."

- **Control:** the government type. The player chooses it, but a change only happens through a revolution the
  player foments, or, in a democracy, a vote (started by the player or by the people). Revolts also break out on
  their own when the people become unruly.
- **Role:** the government is the shield: it absorbs or stokes the people's whims and takes the fall, venting
  government suspicion when it is replaced ([HIDDEN_HAND.md](HIDDEN_HAND.md)).
- **First types** (Norm): Democracy, Monarchy, Communism, Military junta, Theocracy (unspecified religion).
  Meritocracy is a candidate, left out of politics v1 as possibly overpowered (D-0023).

What each type changes (accepted starting design, D-0023; tune by playing):

| Type | How grievance vents | Courts | War | Vices it channels or frustrates | Suspicion |
|---|---|---|---|---|---|
| Democracy | elections vent unrest; can vote itself into another type | harsh courts cost legitimacy | slow to declare | gives Pride and Envy a voice | free press: abrupt lever swings are noticed sooner |
| Monarchy | revolts topple the crown | moderate | at the crown's pride | Pride in the crown | suspicion fixes on the ruler; stable while revered |
| Communism | suppressed, then explosive | harsh courts tolerated | moderate | equality channels Envy; Greed frustrated (no markets) | an educated population turns on the planners |
| Military junta | suppressed by fear; coups are common | harsh courts are cheap | eager | channels Wrath and Pride | fear holds suspicion down until it breaks |
| Theocracy | absorbed by faith (Sloth as acceptance) | harsh on vice crimes | holy wars possible | condemns Lust and Gluttony | high Science raises suspicion sharply |
| Meritocracy (candidate) | advancement vents ambition | moderate | moderate | channels Pride and Envy through advancement | Science raises little suspicion: maybe too strong |

Revolutions (D-0023, D-0024): the player picks a target type and foments; it needs some unrest to work with, takes
months, costs lives and stability, and can fail. Revolts the people start install the type the population is most
aligned with, possibly the same one. [PROPOSED] first draft of alignment: Wrath and Pride lean to a junta, Envy to
communism, Sloth and Pride to monarchy, Sloth with low Science to theocracy, Pride and Envy with high schooling to
democracy; the current type gets a small inertia bonus.

## Faith [OPEN]

Theocracy is now a government type with an unspecified religion. Whether faith also becomes a lever of its own (a
church that condemns or blesses each vice) stays open: Q-008.

## The player's constraints (D-0019)

None: Norm: "Nothing limits the levers persay." Every lever is free and takes effect next month. The check on the
player is risk: extreme settings raise suspicion and invite the four defeats in [HIDDEN_HAND.md](HIDDEN_HAND.md).
