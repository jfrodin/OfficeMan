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
| 2026-10-19 | Portraits are end-of-project work: one expression per character is enough for now. | `design/portraits_flat.md` |
