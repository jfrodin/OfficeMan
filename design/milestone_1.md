# Milestone 1: A Tuesday worth replaying

**Status: APPROVED (2026-10-18).**

**Goal:** one day in Teo's life that the creator can play in a proper window and evaluate as a narrative
simulation. Five playthroughs should feel like five different Tuesdays, and a disaster should make you want to see
Wednesday rather than reload.

Scope stays fixed: Tuesday plus Wednesday morning, the existing locations, the existing cast. No new story or
characters without the creator.

---

## 1. Status (updated 2026-10-27)

The list to check before starting work. Update it whenever something is done or decided.

### Done

| What | When | Where |
|---|---|---|
| Engine: phases, storylets, director, conditions, seeded randomness, save/load | 2026-10-18 | `src/OfficeMan.Core` |
| Full Tuesday and Wednesday morning in ink (6 night routes, 3 outsider threads, 15 closing hooks) | 2026-10-18 | `content/ink` |
| Text player, simulator, tests | 2026-10-18 | `src/OfficeMan.Cli`, `tools/OfficeMan.Sim`, `tests` |
| Step 1: walkable rooms (flat, stairwell day and night, office, terrace), sprites, conversations over the scene | 2026-10-19 | `game/`, `content/rooms` |
| Step 2: traits (four, nudged per life, conditions, nine big moments, choices by trait, inner voice, hindsight, end-of-day words) | 2026-10-19 | `design/traits_build.md` |
| Clean-up: old art experiments removed, `Main.cs` split by job | 2026-10-19 | |
| Step 3: everyone at the party follows a plan of their own; one person at most comes to Teo per evening; Magda may leave with someone, by chance; Stojan's evening heard through the wall | 2026-10-26 | `design/agendas_build.md` |
| The party runs until 01:00 | 2026-10-26 | `content/phases.json` |
| Lasting trait shifts made rarer (one a day, a second sign) | 2026-10-26 | `design/traits_build.md` |
| Simulator: a sixth play style, "late", that stays to the end | 2026-10-26 | `tools/OfficeMan.Sim` |
| Stopped at the gate: before ten, someone may talk Teo into staying (once an evening, he can always go) | 2026-10-26 | `design/agendas_build.md` |
| Text, first pass: every line read critically and rewritten (fewer punchlines and similes, people in their own voices); the bar changes with drink | 2026-10-26 | `design/text_review.md` |
| A text column on the right (a log you can scroll back), now the default; the old bar in Settings / F2. Click moves text on; a single choice just happens | 2026-10-26 | `game/Main.Column.cs` |
| Office: a wall clock that shows the time; Look busy shorter and varied; the dead afternoon can be skipped | 2026-10-26 | |
| Waking at Magda's, Kornel's or the station shows that place | 2026-10-26 | |
| Furniture out on the floor, first try: desks in the office, tables on the terrace, as sprites people stand behind and in front of; Teo walks round them | 2026-10-27 | `content/rooms`, `design/visual_standard.md` |
| Close-up in conversations, first try: the camera moves in on Teo and whoever he's talking to (column layout; F3 or Settings to turn it off) | 2026-10-27 | `game/Main.Column.cs` |

Health on 2026-10-26: 55 tests pass. 1000 simulated lives per play style (six styles): 0 crashes, 0 dead ends, every
storylet reached.

### Next

| # | What | Waiting for |
|---|---|---|
| 1 | **Playtest of step 3** in the window: stay at the party past ten and see who comes to Teo | The creator |
| 2 | Step 1, last part: spoken lines floating above people (the overheard lines from step 3 are the first use) | Nothing; can start |
| 3 | People move when the text says so (Viktor's clap, Teo walking to Viktor's office) | The creator asking (`design/noted.md`) |
| 4 | Step 4: memory that spreads (gossip, reputation) | Nothing; next big step |
| 5 | Step 5: every bad outcome opens something; end-of-day "where Teo's life stands" | Step 4 |
| 6 | Step 6: the creator plays several lives, then we tune | Step 5 |

### Needs the creator

1. **Play it.** The text (first pass) and the text column are both new. Is the voice right? Column or bar?
   (`design/text_review.md`, F2 to switch.) Also new: furniture out on the floor, and the close-up (F3).
2. **Money.** Euro is agreed. Proposed: no number on screen, Teo checks his balance on the phone. Still to decide:
   his pay, payday, what he has on Tuesday morning, and what the gift costs (500 next to an 8-euro beer).
3. **When can Teo go home from the party?** "Go home" is on offer from 18:30; someone may now stop him once. Still,
   most simulated lives leave before ten (5 to 24% are there at ten). The playtest will show whether a real player
   does the same. If so: open "Go home" later.
4. **Your notes** (`design/noted.md`): the open ones, fixed only when you ask.

### Known weak spots (evidence)

| Problem | Evidence | Fixed by |
|---|---|---|
| Most lives leave the party early | 59 to 94% of lives go home through the stairwell; of those still there at ten, 36 to 63% | Partly: stopped at the gate. Open (see "Needs the creator") |
| The same headline every time | "The two-ticket rule lasted one hour and forty minutes": 94 to 99% of lives | Open: the open bar could depend on Viktor's evening |
| Memory stays private | By Wednesday only witnesses know anything | Step 4 |
| Some failures just close a door | Several refusals and failed checks lead nowhere | Step 5 |
| ~~The world doesn't move without Teo~~ | | Step 3, done |
| ~~Lasting trait shifts too common~~ | | 2026-10-26, done |
| ~~Traits barely matter~~ | | Step 2, done |
| ~~Played in a terminal~~ | | Step 1, done |

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
rooms too. The party at Terasa Velka is walkable from 17:00 to 23:30 (01:00 since 2026-10-26): the light goes from golden sun through dusk
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

**Done (2026-10-19):** designed with the creator and built. Four traits (Nerve, Warmth, Appetite, Honesty), a
slightly different Teo each life, conditions that push him for the moment, nine big moments that shift him for good,
nine choices that depend on who he is, inner-voice variants, hindsight lines, and an end screen that says in words
who Teo was. See `design/traits_build.md`.
- **A design session first:** the final hidden trait list (4-5), fixed or slightly random starting values, how each trait *shows* without numbers.
- Traits change what Teo notices, which options appear, and how he talks to himself.
- Conditions v2: drunk, hungover, tired, mood, humiliated, plus whatever the session adds. Each has a monologue voice, so his state can be read from how he thinks.
- **Legible in hindsight:** after an important check, one line of monologue names the cause ("I said it because of the third zhivka"), or a person does ("You were drunk, Teo").
- Small permanent shifts: a humiliation or a triumph can change a trait for the rest of the life.

### Step 3: A world that moves (NPC agendas)

**Done (2026-10-26).** Approved by the creator and built; see `design/agendas_build.md`.
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

See "Needs the creator" in section 1. Playtests are planned after Steps 3 and 5.
