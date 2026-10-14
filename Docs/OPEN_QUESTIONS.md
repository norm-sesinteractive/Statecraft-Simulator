# Open questions

> **Owns:** design questions only Norm can settle, each with Claude's recommendation. **Budget:** 220 lines.
> Answered questions are removed here and recorded as decisions (`/design-review` does both).

### Q-001: What is the hidden hand trying to achieve?
**Why it matters:** gives every system a purpose; sets game length and pacing.
**Options:** (a) open sandbox with optional goals; (b) scenarios with objectives ("make [NATION] the strongest
power by year 150", "a crime-free utopia that survives its neighbors"); (c) a hidden agenda score plus survival.
**Claude recommends:** (b) and (c) together: scenario goals, and failure if your nation turns on you or is overrun.

### Q-002: What limits the player's levers?
**Why it matters:** today every slider is free and instant, so there is little tension.
**Options:** (a) influence budget earned over time; (b) exposure: abrupt or extreme changes raise suspicion,
discovery is a loss or a crisis; (c) rate limits per lever; (d) reach: only nations where you hold a foothold.
**Claude recommends:** (b) first, maybe (d) later. Exposure rewards subtlety, which is the fantasy.

### Q-003: Setting and era
**Options:** (a) fictional world, modern day; (b) fictional world over centuries, media evolving from criers to
screens; (c) alternate history of Earth.
**Claude recommends:** (b). Eras make media reach and speed a progression, and fiction avoids real-world politics.

### Q-004: The world map
**Options:** procedural or authored; how many nations (3 to 12?); the look (flat map, 3D terrain, a tabletop
"map room" with nations as pieces).
**Claude recommends:** authored small maps first (easier to balance), procedural later; the map room look suits
the fantasy and Blender pieces.

### Q-005: Time model
Real-time with pause, months ticking (built, D-0010), or turns of a year?
**Claude recommends:** keep real-time with pause; it is the river. Add a "run until something happens" speed.

### Q-006: Shape of the media control
**Options:** (a) airtime shares over seven vices plus neutral (built); (b) seven sliders from virtue to vice
(Patience-Wrath, Diligence-Sloth, Charity-Greed, Chastity-Lust, Humility-Pride, Kindness-Envy,
Temperance-Gluttony). Also: should calming stay weaker than inflaming?
**Claude recommends:** (a) for its built-in trade-off, showing the virtue name when a share is below even.

### Q-007: Do the four institutions act on different layers?
Media acts on vices, education on outlets, law on overflow, politics on steering (D-0011, built for three).
**Claude recommends:** yes; it keeps each lever distinct and makes time horizons the strategy.

### Q-008: Faith as a fifth institution?
A church that condemns or blesses each vice fits the seven-sins framing.
**Claude recommends:** later (M3 or after), once the four are fun; it overlaps media.

### Q-009: The player's reach and rivals
**Options:** one home nation whose influence spreads; several nations from the start; rival hidden societies.
**Claude recommends:** one home nation at first, with at least one rival hand later ("a rival river").

### Q-010: Contentment and suffering: one axis or two gauges?
One axis is built. Two gauges would let a nation be prosperous and frustrated at once.
**Claude recommends:** keep one axis until playtests show the need.

### Q-011: Game length and mobile sessions
How long is a game (an hour, an evening, a week of short sessions)? Offline progression?
**Claude recommends:** 60 to 90 minute games playable in 5 to 10 minute sittings; no offline progression at first.

### Q-012: Identity and orientation
Provisional: company "SES Interactive", bundle id `com.SESInteractive.StatecraftSimulator`, landscape only.
Bundle ids are permanent once published. **Claude recommends:** confirm or change before the first store build.

### Q-013: What does the politics lever look like?
**Options:** (a) pick a government form directly; (b) forms shift through ideology (schools), unrest
(revolution) and reform, with the player backing factions.
**Claude recommends:** (b); choosing a form outright contradicts "influence, never command".

### Q-014: How do the puppets decide on war?
**Options:** a threshold on aggression plus opportunity; a utility-based planner per government.
**Claude recommends:** threshold plus opportunity with randomness, readable as a forecast.

### Q-015: Business model
Premium or free-to-play affects pacing, session design and progression. **Claude recommends:** premium.

### Q-016: Should schools also shape values?
Model v0 has schools train outlets only. Norm's pitch has schools pushing forms of government too.
**Claude recommends:** add civic ideology (for politics), keep vice dispositions to media and family.
