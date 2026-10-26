# Noted by the creator

Things the creator has pointed out. **Fix only when the creator asks.** Remove an item once it's fixed (and say so in the commit).

| Date | Note | Where |
|---|---|---|
| 2026-10-19 | It looks weird when the camera shifts and the portrait suddenly sits inside the black text bar. | Conversation bar, camera lift (`game/Main.cs`, Layout) |
| 2026-10-19 | Magda's portrait: her clothes end too low (said before the off-shoulder dress; check whether it still applies). | `art/proposals/magda_young.png` |
| 2026-10-19 | When Teo talks, the portrait doesn't change: it's still the other person (e.g. Stojan). Teo has no portrait; his lines show his name next to the listener's portrait at full brightness. | `game/Main.cs`, NextLine |
| 2026-10-19 | Portraits need a lot of work overall in the UI: the frame and how they sit on screen. UI work comes later. | Conversation bar (`game/Main.cs`) |
| 2026-10-19 | Changing location: a black screen that says where we are (and probably when), instead of cutting straight to the new place. Later. | `game/Main.cs`, EnterPhase / Arrive |
| 2026-10-19 | "Viktor comes out of his office and claps once" (office arrival): he should actually do it on screen, walk out of his office, not stay where he is. Same principle wherever the text describes someone moving. | `content/ink/tuesday_office.ink:23`, `content/rooms/office.json` |
| 2026-10-19 | "VIKTOR: Teo. In here a second." (11:30): Teo should go to Viktor's office, preferably as a short sequence of him walking there, and the conversation happen inside. Build together with "people move when the text says so" (Viktor's clap above). | `content/ink/tuesday_office.ink:329` (viktor_office) |
| 2026-10-19 | The floors are completely open: tables, desks etc. should stand out in the room, not everything pushed against the walls. (Possible now with props: furniture as depth-sorted sprites, like the bar counter.) | All rooms |
| 2026-10-19 | Portraits are end-of-project work: one expression per character is enough for now. | `design/portraits_flat.md` |
| 2026-10-26 | Very annoying that you can't click text onwards with the mouse button. (Code listens for left click in `_UnhandledInput`, but the conversation bar most likely swallows the click first.) | `game/Main.cs`, `_UnhandledInput` |
| 2026-10-26 | Doesn't like how text is handled at all: the box. Asked for alternatives. | Conversation bar (`game/Main.Ui.cs`) |
| 2026-10-26 | When "Get up" is the only choice left, it should just happen. Same wherever a single option remains. | Choices with one option (engine or front end) |
| 2026-10-26 | The clock in the office doesn't move. | `content/rooms/office.json` (clock), office room art |
| 2026-10-26 | Too little variation, and "Look busy" takes too long before things start happening in the office. | `content/ink/tuesday_office.ink`, `look_busy` (45 minutes, four lines) |
| 2026-10-26 | The game looks bad, but that's known; later. Things must not stand along the walls (see the 2026-10-19 note on open floors). | All rooms |
| 2026-10-26 | Waking at Kornel's looks like Teo's own flat. (Wednesday morning is always shown as Teo's flat room; the scene pictures for Kornel's, Magda's and the station are never used.) | `content/art.json` (`wed_wake` is a room), `content/ink/wednesday.ink` |
| 2026-10-26 | Go through all the text critically and rewrite it. The story is the game's most important ingredient. (In progress: `design/text_review.md`.) | `content/ink/*.ink` |
