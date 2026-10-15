# Pitch (Norm's words, verbatim)

> **Owns:** Norm's own statement of the game, exactly as written. **Budget:** 150 lines.
> Frozen: never edited. Later pitches or changes of direction from Norm are appended below with their dates.
> When another doc disagrees with this one, this one wins until Norm says otherwise.

## 2026-10-14: the original pitch

New game idea. Working title is Statecraft Simulator. The point of the game is to play as an all powerful illuminati like organization that manipulates a country or countries through seemingly subtle everyday means. Control the content of the media, the education system, law & order, semblances of political systems. Changing these controls changes the pressures and outputs of the society. The illuminati do not directly control the actions of their puppets, they need to be strategic about how they manipulate the masses, lest they make a monster they can no longer control or are overwhelmed by outside forces (other countries).

Some outcomes are obvious, increasing violence in the media increases aggression in the society. This would make a country more likely to undertake wars on neighboring countries and also to be better at doing so, but it would also have repercussions on the aggressive country, increasing acts of internal violence. Conversely a country that focuses only on peace and no defensive activities may have a very happy population, but become open to military conquest from jealous neighbor countries. Positive interactions between nations will also need to exist, primarily in the forms of trade and culture.

I don't want this to turn into a game like Civilization 4,5,6,7, etc.. That game is full of tedious individual choices. This game is more like influencing the flow of a river and watching as the consequences of your river take place. Will you flood the banks in just such a spot to overflow across a great land? Will you smooth the rapids and peacefully exist within your boundaries? How will you survive when a rival river tries to run into you?

Controls should always be simple for this game, while outcomes can be complex.

Regarding the mental states to influence, why not go with the widely known 7:
Wrath (Aggression, vengeance, punishment),
Sloth (complacency, acceptance),
Greed (many obvious implications with financial systems and desire of conquest),
Lust (influence reproductive rate and potentially culture spread),
Pride (A little pride helps drive a population, a lot can drive them into individualism. This can create high highs in technological growth, but also drives people apart. An aggressive and individualistic society would suffer from acts of violence and a society that doesn't take care of its elder's, etc.),
Envy (Susceptible to other cultures. Also creates a more conquest oriented society, taking what others have if aggression is high),
Gluttony (influence resource consumption. There may be times when it is beneficial to the player's desire to consume more resources and drive a higher desire in a country's people to invade their neighboring countries)

Weaved with all of these is the contentment/suffering of the average citizen. Aggressive cultures, that have outlets for that aggression, can still be happy cultures. Cultures that don't have outlets for their share of the 7 experience an increase in suffering. This may lead to revolt at the worst, or just leave the country less effective and more likely to be invaded.

Countries should have different resources and different landscapes that change how those resources are accessed and how their populations live and grow.

Control over the media is the most simple to understand, it would effectively allow a balance of the 7 to be aired to the population. Control over the educational system would be very powerful in influencing the outlets/outcomes of the 7 emotional influencers. The school might push for democratic elections, or communism, or any common form of government. It might push for agricultural growth, military growth, medical growth, infrastructural growth, potentially others. Influence over Law & Order would go hand in hand with the political system of the land. Should courts crack down ruthlessly? Should they pardon all but the most guilty? Pardoning the minor offenders may make a society suffer less, but the reduction in fear of punishment may lead to increased crime. Perhaps a society can be so happy and so well supplied that crime no longer takes place. Can they be that happy without becoming weak and decadent and thereby being overthrown by their neighbors?

### Process notes from the same message

I want to build this game in Unity. I want all coding to be done through Claude Code. I want a self-updating documentation structure to be implemented that will make it easy for Claude Code to understand the project between many different sessions. This first session will focus on setting up the foundations of the project and future session will refine the gameplay concepts.

I'm happy to make Blender content or icons as needed.

### Answers Norm gave in session 1 (2026-10-14)

- Git: "I run git, you give commands." (D-0003)
- Binary assets: exports in git, .blend sources outside it, no LFS. (D-0004)
- Game text: "I write it, Claude uses placeholders." (D-0005)
- Platform: mobile first. (D-0002)

## 2026-10-14: goals, defeat, suspicion and politics (session 2)

The shadow organization has no game goals, it's simply driven by the desire of the player to manipulate the population(s) of the game.
Nothing limits the levers persay, but the available countries should be able to be defeated via at least two means. 1. Conquered by another country via military means. 2. Culturally conquered by another country (effectively your population becomes controlled by some other shadow organization). The population can run out of resources and starve/die. Or they can revolt and end up overthrowing the selected government type and possibly overthrow the shadow government itself. Things like extremely high science and frustrations with infrastructure would increase the likelihood of a country figuring out something is wrong. There could be many other situations in which one or more of the 7 vices and or education are out of whack that lead to suspicion of the government and eventually suspicion of you. Perhaps you foment a false revolution to change government types and try to reduce suspicions.

More specifically about how politics works, the political system chosen by the player is the government system of the people. If the player wants, they should be able to change the government type, but not without instigating a revolution of some kind. Democracies can vote to change to another type, but they can also experience revolts/revolutions. The government is just a shield for the shadow organization of the player. They are another tool to absorb or stoke the whims of the people and there to take the fall if the people become unruly.

### Follow-up answers (same session)

- Losing a nation: "Single nation, loss ends game."
- Reach: "Start with one, expand."
- Democratic votes: both the player and the people can start one.
- Government types for the first politics version: Theocracy (unspecified religion), Democracy, Monarchy,
  Communism, Military junta; "Possibly Meritocracy but that might be over-powered."

## 2026-10-14: revolts, culture through trade, relations between peoples (session 2)

The proposals are reasonable and acceptable to get the project off the ground. A new government post-revolt is selected by the desires of the people. Whatever government type the current population would be most aligned with, even if it's the same one they just revolted from. Cultural conquests will go along with trade and the interplay between the vices and educations of neighboring nations. An act of trade will bring the receiving/paying country slightly closer towards the cultural levels of the supplying nation. Nations never change culture based on exports of goods or services, only on receiving them. For example if the supplying nation's active generation was strong in aggression and weak in science, relative to the receiver, the receiving nation's aggression would nudge slightly up and science slightly down. The amount of influence received should be affected by the contentment of the supplying nation's people, relative to the receiving nation's. If contentment is measured separately by generation, then all influence will come from the active/adult generation of the supplying nation and be received by the receiving nation at 2X for youth, 1X for adults, and .5x for the older generation, each generation being affected by their own contentment balance against the adult generation of the supplying nation (not against their would-be generational counterparts).
Nations should track a contentment score between their peoples, benefited by trade and slowly move towards 0 when nations aren't engaged in trade. Military action can pull the score negative having a balanced and opposite relationship with trade. Each side is pulled back to 0 when no action is underway. Picturing the min and max relationships as -100 through 100, nations at either extreme will move to 0, naturally, the slowest. Moving back to 0 faster as their scores become closer to 0. This signifies a lasting bond or a hated rivalry. The worse things get, the longer it takes to cool down and the reverse.

## 2026-10-14: educational overlap gates trade influence (session 2)

To your Science interpretation, I made an error in equating aggression and science, but have a clearer vision now. The change amount in vices should be affected by the educational cross-over of the adult supplying and individual receiving generations. IE if supplying adults are educated as 75 agriculture and 25 science (0 everything else) and the receiving population was 100 agriculture, the change in vice values and the gain in good will cross country would be at 75% of the maximum rate. If the receiving country was at 25 through 100 science and 0 agriculture, then the yields would instead be 25% of max. The minimum of each shared category, cumulated across all educational types. If the receiver had 0 education in everything but Military or Medicine, or anything that's 0 in the supplying nation, then 0% goodwill benefit and 0% vice adjustment will take place. We might have to debate that part out a little further.

## 2026-10-14: commerce floor, infrastructure efficiency (session 2)

Ah, commerce is an angle I hadn't fully considered. Half of a nation's commerce value should be the floor, so they can never get higher than 50% with commerce alone (no added benefit on top of education matches, just a floor). Infrastructure can represent an increase in efficiency of trade an military activity. Low infrastructure leads to a incoming and outgoing revenue being reduced by a %, say 50% max and high infrastructure (good) leads to no reduction in financial comings and goings. Making trade more efficient, but not necessarily more appealing.

## 2026-10-14: infrastructure as transit loss (session 2)

One more clarification around infrastructure, the amount paid to a supplying nation is still the agreed upon amount, but more than the agreed upon rate will be deducted from the paying/receiving party if they don't have 100 infrastructure. likewise the receiving party will get the agreed upon amount, but then lose a % of it in transit based on infrastructure. Let's soften the original approach and set a floor of 50% efficiency at 0 infrastructure and 100% efficiency at 100 infrastructure. Increased at a linear rate (2 points = 1 % efficiency gain).

## 2026-10-14: infrastructure score, contentment per generation (session 2)

Infrastructure score comes from the education of the active/adult generation. No to the 30%. 0 =50% 100 = 100%, linear growth, as mentioned. Separate contentment per generation is acceptable. Any given generation could power a revolt, potentially with different consequences.
