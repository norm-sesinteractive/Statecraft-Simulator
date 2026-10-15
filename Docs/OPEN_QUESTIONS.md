# Open questions

> **Owns:** design questions only Norm can settle, each with Claude's recommendation. **Budget:** 220 lines.
> Answered questions are removed here and recorded as decisions (`/design-review` does both).
> Answered so far: Q-001 (D-0018), Q-002 (D-0019, D-0020), Q-009 (D-0022), Q-013 (D-0021),
> Q-017 to Q-019 (D-0023), Q-020 (D-0024), Q-021 (D-0025), Q-022 (D-0032), Q-023 (D-0028), Q-024 (D-0031).

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

### Q-008: Faith as its own lever?
Theocracy is now a government type (D-0021). Should faith also be a lever, a church that condemns or blesses each vice?
**Claude recommends:** not yet; let Theocracy carry religion until the four institutions are fun.

### Q-010: Contentment and suffering: one axis or two gauges?
One axis is built. Two gauges would let a nation be prosperous and frustrated at once.
**Claude recommends:** keep one axis until playtests show the need.

### Q-011: Pacing of an open-ended game on mobile
With no goals (D-0018), a game lasts until defeat. How long should a typical run feel, and is there offline progression?
**Claude recommends:** design for 5 to 10 minute sittings with autosave; no offline progression at first.

### Q-012: Identity and orientation
Provisional: company "SES Interactive", bundle id `com.SESInteractive.StatecraftSimulator`, landscape only.
Bundle ids are permanent once published. **Claude recommends:** confirm or change before the first store build.

### Q-014: How do the puppets decide on war?
**Options:** a threshold on aggression plus opportunity; a utility-based planner per government.
**Claude recommends:** threshold plus opportunity with randomness, readable as a forecast.

### Q-015: Business model
Premium or free-to-play affects pacing, session design and progression. **Claude recommends:** premium.

### Q-016: Should schools also shape values?
Model v0 has schools train outlets only. Norm's pitch has schools pushing forms of government too.
**Claude recommends:** add civic ideology (which government types the young favor), feeding votes and emergent revolts.

### Q-025: Whose Commerce sets the floor?
D-0028: "Half of a nation's commerce value should be the floor."
**Options:** (a) each receiving cohort's own Commerce share; (b) the importing nation's adults; (c) the smaller of
importer and supplier.
**Claude recommends:** (a), matching how overlap is measured per receiving cohort (recorded provisionally).
