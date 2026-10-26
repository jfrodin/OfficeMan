# The Misadventures of Office Man

A narrative adventure about an ordinary life in a city that can turn absurd without warning.

**Status:** prototype. One Tuesday (the office summer party) and the Wednesday morning after, playable as a walkable
pixel world (Godot) or as text in the terminal.

- Creative vision: [GAME_VISION.md](GAME_VISION.md)
- Design documents: [design/](design/)

## Requirements

- .NET SDK 9 (the engine library targets .NET 8 for Godot compatibility)
- Windows (other platforms likely work but are not targeted)

## Play the prototype

**In a window:** double-click `Play.bat` (needs Godot 4.6 .NET; edit the path at the top of the file if Godot
lives elsewhere).

| | Keyboard | Controller |
|---|---|---|
| Walk | Arrows / WASD | Left stick / d-pad |
| Interact, continue | E / Enter / Space | A |
| Choose | 1-9, mouse, or arrows + Enter | d-pad + A |
| Back out of a menu | Esc | B |
| Pause menu (save, load, settings) | Esc | Start |
| Quick save / load | F5 / F9 | |

Every choice autosaves. Teo's flat, the stairwell, the office and the party are walkable rooms; the night scenes are
still pictures with a list of choices.

**Review the look:** double-click `Review.bat`. It captures every room and every way text appears, then opens a viewer
(← → to look through, Esc to close) and saves them all on one sheet, `art/out/review.png`.

**In the terminal** (fast for testing):

```
dotnet run --project src/OfficeMan.Cli
dotnet run --project src/OfficeMan.Cli -- --seed 42 --debug
```

Type a number to choose. `s` saves, `l` loads, `d` toggles the debug view (Teo's hidden state and recent checks), `q` quits.
Text types out at reading pace; press any key to show the rest of a passage at once, or start with `--fast` to turn it off.

The same seed with the same choices always produces the same life; that includes the luck.

## Simulate thousands of lives

```
dotnet run --project tools/OfficeMan.Sim -- --runs 1000
dotnet run --project tools/OfficeMan.Sim -- --runs 500 --persona drinker
```

Reports crashes, dead ends, unreachable storylets, how the night and the closing hook are
distributed, and how many distinct lives were produced, for five play styles
(random, drinker, cautious, kind, bold, and late: a player who stays to the end of everything). Exits non-zero on any crash or dead end.

## Tests

```
dotnet test
```

## Layout

| Path | What |
|---|---|
| `content/phases.json` | The shape of the day: phases, times, what follows what |
| `content/ink/*.ink` | All writing. Each knot tagged `# phase:` is a storylet |
| `src/OfficeMan.Core` | The life engine: world state, director, conditions, checks, saves |
| `src/OfficeMan.Cli` | Text front end |
| `game/` | Godot 4.6 (C#) front end: rooms, walking, text, menus. Not in the .sln; build with `dotnet build` inside `game/` |
| `content/art.json` | Which image is used for each speaker + mood, each location, and which phases are walkable rooms |
| `content/rooms/*.json` | Walkable rooms: picture, walkable floor, and the people and things in them (anchors for `# where:`) |
| `tools/OfficeMan.Sim` | Headless life simulator |
| `tools/OfficeMan.ArtGen` | All the art, built from code: rooms, sprites, temporary portraits (see its `Program.cs`) |
| `art/game/` | The generated in-game art (committed) |
| `art/reference/` | The two reference images the creator chose. Never change them or their sources |
| `design/` | Design documents. Start with `design/milestone_1.md` section 1: what's done, what's next, what waits for the creator. `design/noted.md` lists what the creator has pointed out, to fix when asked |
| `tests/OfficeMan.Tests` | Unit and content tests |
| `third_party/ink` | Vendored ink runtime and compiler (MIT) |

## Writing content

Storylets are ordinary ink knots with metadata tags directly under the header:

```ink
=== magda_river
# phase: party
# kind: hub
# choice: Go down to the river walk, where Magda went.
# when: magda_crushed and not played(leave_home)
# duration: 30
She's on a bench...
```

`kind` is one of `scene`, `enter`, `hub`, `exit`, `interrupt`, `timeout`, `offstage` (something other people do on
their own; no choices, only overheard lines). See
`src/OfficeMan.Core/Content/Storylet.cs` for every tag and `content/ink/lib.ink` for the
functions ink can call (`check`, `remember`, `like`, `drink`, `headline`, …).

Dialogue lines are written `NAME: words`. Everything else is Teo's inner monologue.
Add `#mood:sad` to a line to change the speaker's portrait, and `#scene:station` to change the backdrop.
Moods and scenes are resolved through `content/art.json`, so artwork can be replaced without touching the writing.

Third-party software: see [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
