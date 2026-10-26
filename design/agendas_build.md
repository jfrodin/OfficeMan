# Step 3: A world that moves

**Status: approved by the creator and built, 2026-10-26.** Content: `content/ink/tuesday_people.ink`, plus small hooks
in the party, the night stairwell and Wednesday. Who stands where: `content/rooms/terrace.json`.

Creator's decisions (2026-10-26):
- All the plans below go in.
- People may come to Teo on their own, **at most one per evening** (`came_to_teo`).
- When nobody takes Magda home, she may leave with someone else. **Who it is, is left to chance.**
- **The party goes on until 01:00** (was 23:30).

## The problem it solves

Before this, the people at the party only acted inside Teo's storylets, plus four fixed moments (Ramona arrives, the
open bar, Kornel's poem, Magda and Viktor at 20:15). If Teo just stood at the bar, nothing happened to anyone.

## What the engine does

- **Offstage moments** (`# kind: offstage`). Something a person does on their own, at a time or when something is
  true, whether Teo is there or not. Takes no time. No choices. The only text allowed is what Teo overhears
  (`#heard`), shown as a caption by the person. Everything else shows in the world: who is where in the room, which
  conversations are on offer, what people say later.
- **What each person is doing**: `set_doing("magda", "river")`, read as `doing(magda, river)` in conditions and room
  files.
- **How their own evening is going**: `feel("ramona", -2)`, read as `feels(ramona)`. Separate from what they think of
  Teo.
- **Coming to Teo** is an ordinary interrupt that fires when someone's plan says so.
- The simulator reports where everyone's evening ended up, and what happened to the lives still at the party at ten.
- Look at a room in a given state: `... --room terrace --at 22:15 --doing "magda=bar,viktor=bar,magda_crushed"`.

## The plans

### Magda: get Viktor tonight
*"Thirty-four tickets. I had a plan."*

| # | When | What happens |
|---|---|---|
| M1 | The open bar (18:40) | She starts using the tickets herself. Overheard: *MAGDA: Laci! I'm the bank! The bank says yes!* |
| M2 | 20:15 (existed) | Viktor turns away; she goes down to the river (`doing: river`) |
| M3 | An hour later, if she's still waiting | She comes back up: to the hen party if they're here (*DENISA: Girls! She's one of us now!*), otherwise the bar. New conversation: "Magda is back." |
| M4 | 22:00, likes Teo, never walked home | She comes to him: *Walk me home? I don't want the tram looking like this.* Yes goes straight to her building |
| M5 | From 22:00 if she doesn't like Teo much, from 22:30 otherwise | Nobody is taking her home, so she leaves with someone: a consultant, Adrian Kovál or a stranger, by chance and depending on who's there. Wednesday: yesterday's earrings |

### Viktor: tonight's target, then the club

| # | When | What happens |
|---|---|---|
| V1 | 21:30 (22:30 if he has invited Teo) | Leaves for Club Schengen with his target. *VIKTOR: Schengen! Who's young? Nobody? Fine. Come anyway.* "Go with Viktor" is gone once he is |
| V2 | 22:00, if Teo ruined his evening | He has been drinking alone at the bar. Half the time he comes for Teo: *You. You're coming. You owe me an evening.* Otherwise he calls himself a car at 22:45 |

### Ramona: be seen not caring, then leave with dignity

| # | When | What happens |
|---|---|---|
| R1 | When Viktor leaves | She orders a bottle. *RAMONA: The bottle, please. Not a glass. I'm not a child.* Her conversation notices |
| R2 | 22:30 | She leaves. If her evening has gone badly, half the time she offers Teo a lift on the way out: *You. Permits. I'm leaving. You look like a man who needs a lift.* |

### Kornel: be liked

| # | When | What happens |
|---|---|---|
| K1 | 20:45, poem went badly, Teo not beside him | Sits with Zdenka. *ZDENKA: It rhymed, Kornel. That's more than most of them manage.* Wednesday: *Zdenka doesn't say things.* |
| K2 | 21:00, poem went well | His first beer. *KORNEL: It tastes like bread that's given up!* He can still drive Teo home: one beer, two hours ago, he's done the maths |

### Zdenka: see everyone together, leave at ten

| # | When | What happens |
|---|---|---|
| Z1 | 22:00 | She leaves. Her table can be looked at: chair pushed in square. In lives where she's dark, a folded napkin left exactly in the middle |

### Stojan: get the balcony through, one way or another

| # | When | What happens |
|---|---|---|
| S1 | During the party, if Teo refused him | He builds anyway. Through the wall that night: *DANA: Stojan, it's midnight. STOJAN: Concrete doesn't know what time it is!* |
| S2 | During the party, if Teo promised to look | He celebrates early. *STOJAN: To Teo! To Permits!* *LENKA: Some people need their sleep! For their skin!* |

### Stopped at the gate (creator's request, 2026-10-26)

Before ten, when Teo picks "Go home", there's a 60% chance someone stops him, once an evening. Who it is depends on
who's there and what's still to come: Magda at the door (*You've still got a ticket!*), Kornel before the poem (*I'm
reading at eight. It's a short thing. It rhymes.*), Viktor at the railing (*Nobody leaves before me.*), the hen party.
He can always go anyway; going is remembered. Engine: `stay()` in an exit storylet means he doesn't leave after all.

## Result (simulator, 1000 lives per play style, 2026-10-26)

The plans only touch lives that are still at the party when they happen. For those, the night is now often something
other than going straight home. There was no "at ten" measure before; for comparison, 88% of all random lives went
straight home before step 3.

| Play style | At the party at ten | Of those, straight home | All lives, straight home |
|---|---|---|---|
| random | 12% | 63% | 82% |
| drinker | 24% | 38% | 59% |
| cautious | 5% | 62% | 94% |
| kind | 13% | 40% | 73% |
| bold | 19% | 46% | 73% |
| late (stays to the end) | 76% | 36% | 42% |

**Open:** most simulated lives still go home before ten. The simulated players pick "Go home" at random and pick it
again right after being stopped; a real player who stays usually has a reason. The playtest will tell. If real
players still leave early, "Go home" could open later.
