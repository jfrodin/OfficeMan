# Milestone 1: A Tuesday worth replaying

**Status: APPROVED (2026-10-18).**

**Goal:** one day in Teo's life that the creator can play in a proper window and evaluate as a narrative
simulation. Five playthroughs should feel like five different Tuesdays, and a disaster should make you want to see
Wednesday rather than reload.

Scope stays fixed: Tuesday plus Wednesday morning, the existing locations, the existing cast. No new story or
characters without the creator.

---

## 1. Where we are

### Works today (tested)

| Area | State |
|---|---|
| Engine | Phases, scene and hub storylets, a director, conditions in tags, seeded randomness (reloading replays the same luck), save/load |
| World state | Qualities, per-character regard and memories, alcohol/tiredness/mood/shame, provisional traits, hidden tiered checks |
| Content | Full Tuesday and Wednesday morning in ink: 6 night routes, 3 outsider threads, 15 closing hooks, memories paid off on Wednesday |
| Tools | Text player with reading pace; simulator (5 play styles, coverage, outcome spread); 33 tests |
| Health | 10,000 simulated lives: 0 crashes, 0 dead ends, every storylet reachable, generic ending 1-4% |

### Weak (evidence)

| Problem | Evidence |
|---|---|
| **Traits barely matter** | Four provisional traits with fixed values; they only nudge a few checks. Teo doesn't feel like a specific person. |
| **The world doesn't move without Teo** | Magda, Viktor, Ramona and the rest only act inside Teo's storylets. If you stand at the bar all night, nothing happens to them. |
| **Causes aren't legible** | Checks resolve silently. A bad outcome rarely tells you *why* (drunk? tactless? unlucky?), which is what makes a disaster feel fair. |
| **Conditions are mostly alcohol** | Mood, tiredness and shame exist but rarely change what Teo thinks or can do. |
| **Memory stays private** | Characters remember Teo, but nothing spreads. By Wednesday the whole office should "know" things, rightly or wrongly. |
| **The big threads are hard to reach** | Viktor's club, Ramona, a night at Magda's: 1-10% of random lives. Some of that is right (rare is good), some is just hidden. |
| **Some failures are simply bad** | Several refusals and failed checks close a door without opening another. That breaks *No Optimal Life*. |
| **It's played in a terminal** | Fine for testing, poor for evaluating a game. No portraits, no sense of place. |

---

## 2. What to build, in order

Each step ends in something the creator can play.

### Step 1: A playable window (Godot) (REVISED 2026-10-18 · first walkable room done)

The first build was a visual-novel-style dialogue screen. The creator wants a **walkable pixel world** like
*Deadhouse*. Step 1 becomes:

- Pixel rooms at about 480×270, scaled up crisply: Teo's flat, the stairwell, the office and the terrace first.
  Night locations can stay short conversation scenes for now.
- Teo walks with arrows/WASD/controller; people and objects in the room are interactable (prompt + E / A button).
- Hub storylets are anchored to people and things in the room; exits are doors; interrupts happen in the room.
- Floating spoken lines above characters.
- Conversations over the live scene: portrait bottom-left, text and choices bottom-right (Deadhouse layout).
- Code-built pixel sprites: Teo with a walk cycle, the cast with idle poses.

**Done (2026-10-18):** the office is walkable (Tuesday and Wednesday) with Teo, Magda, Kornel, Zdenka and Viktor as
pixel sprites; things to look at; interaction prompts; conversations over the live scene (portrait bottom-left,
words and choices bottom-right); keyboard and controller. Story moves between places mid-conversation are marked
in the text, so the scene switches exactly where the story does.

**Done (2026-10-19):** the flat (Teo in bed), the stairwell (day and night, Stojan smoking) and the terrace are
rooms too. The party at Terasa Velka is walkable from 17:00 to 23:30: the light goes from golden sun through dusk
(the string lights come on) to night. Every party situation is tied to a person or place: the bar (Laci), Magda by
the door, Viktor at the railing with whoever he's after tonight, Ramona at the bar, Zdenka at her table, the
consultants, the hen party, the developer, the railing, the steps down to the river; ways out at the door (home),
with Viktor (the club), with Ramona (her lift) and down the steps (Magda). People appear only when the story has
them there. Sprites for Ramona, Stojan, Laci and all the outsiders, with sitting poses for people at tables.
Conversations: the bottom bar with portraits, compact notes for thoughts, overheard captions.

**Still to do in Step 1:** floating spoken lines above people (the mechanism exists for overheard lines placed
at a person; no lines are written for it yet).

The original Step 1 notes below still describe the conversation layer, which is kept.

#### Original Step 1 (conversation layer, built)
- One dialogue screen: location background, **large speaker portrait**, text box with reading pace and skip, choices, the hub menu, save/load and autosave.
- **Portraits are data, not code.** Ink lines can carry a mood tag (`MAGDA: Oh. Okay. #mood:hurt`). A portrait manifest (`content/portraits.json`) maps *character + mood → image*, falling back to the character's neutral, then to a generic silhouette. Replacing art means replacing files; the dialogue system never changes.
- Temporary art: the existing portraits (animation Magda, the four cast portraits), silhouettes for everyone else, Teo's flat as a background, plain titled panels for other locations.
- The text player stays, for fast testing.

### Step 2: Who Teo is (traits and conditions, designed with the creator first)
- **A design session first:** the final hidden trait list (4-5), fixed or slightly random starting values, how each trait *shows* without numbers.
- Traits change what Teo notices, which options appear, and how he talks to himself.
- Conditions v2: drunk, hungover, tired, mood, humiliated, plus whatever the session adds. Each has a monologue voice, so his state can be read from how he thinks.
- **Legible in hindsight:** after an important check, one line of monologue names the cause ("I said it because of the third zhivka"), or a person does ("You were drunk, Teo").
- Small permanent shifts: a humiliation or a triumph can change a trait for the rest of the life.

### Step 3: A world that moves (NPC agendas)
- Each main character at the party (Magda, Viktor, Ramona, Kornel, Zdenka) and Stojan at home gets **a small plan for the day**: goals, a mood, and states that advance with time and events, whether or not Teo is there.
- Their storylets key off their state as well as Teo's actions, so the evening unfolds differently even for a passive Teo.
- The director picks between "what Teo seeks out" and "what happens to him".

### Step 4: Memory that spreads (gossip and reputation)
- Memories gain a weight and a witness list. Strong, public memories spread overnight: by Wednesday, people know things they didn't see, sometimes wrongly.
- A few reputation qualities (e.g. *reliable*, *fun*, *creep*, *Viktor's guy*), shown only through how people treat him.

### Step 5: Consequences and *No Optimal Life* audit
- Go through every bad outcome: each must open at least one new possibility (a person, a place, a favour owed, a secret).
- The simulator gets a check that flags outcomes that only close doors.
- **End-of-day screen:** not a score. A short, plain-language "where Teo's life stands" (who's angry, who's in love, what's in motion) and the open questions Wednesday afternoon would answer.

### Step 6: Playtest with the creator, then tune
- Creator plays several lives. We adjust pacing, odds, writing, and which threads are too hidden or too easy.

---

## 3. Postponed on purpose

| Postponed | Why |
|---|---|
| Final character art, art direction, environment art | References kept in `art/reference/`; temporary art is enough to evaluate the game |
| Animation, audio, music | Not needed to judge the narrative |
| Chapters 2-4, time jumps, "years pass" | Prove one day first |
| City map, new locations, new main characters | Scope; and the creator leads the cast |
| Magda's darker storyline | Creator wants to develop it later |
| Localisation, Steam, achievements, settings menus | Shipping concerns |
| Combat or skill systems | Not part of this game |

---

## 4. Needs the creator

1. **Approve this milestone** (or change the order). I'd start with Step 1 so every later playtest is in a real window.
2. **Trait design session** (before Step 2): proposal in `design/traits_proposal.md`.
3. ~~Magda's new age~~: decided (21; children 5 and 2 with their fathers). Content updated.
4. **Playtests** after Steps 1, 3 and 5.
