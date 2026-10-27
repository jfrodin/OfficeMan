# Noted by the creator

Things the creator has pointed out. **Fix only when the creator asks.** Remove an item once it's fixed (and say so in the commit).

| Date | Note | Where |
|---|---|---|
| 2026-10-19 | It looks weird when the camera shifts and the portrait suddenly sits inside the black text bar. | Conversation bar, camera lift (`game/Main.cs`, Layout) |
| 2026-10-19 | Magda's portrait: her clothes end too low (said before the off-shoulder dress; check whether it still applies). | `art/proposals/magda_young.png` |
| 2026-10-19 | When Teo talks, the portrait doesn't change: it's still the other person (e.g. Stojan). Teo has no portrait; his lines show his name next to the listener's portrait at full brightness. | `game/Main.cs`, NextLine |
| 2026-10-19 | Portraits need a lot of work overall in the UI: the frame and how they sit on screen. UI work comes later. | Conversation bar (`game/Main.cs`) |
| 2026-10-19 | "Viktor comes out of his office and claps once" (office arrival): he should actually do it on screen, walk out of his office, not stay where he is. Same principle wherever the text describes someone moving. | `content/ink/tuesday_office.ink:23`, `content/rooms/office.json` |
| 2026-10-19 | "VIKTOR: Teo. In here a second." (11:30): Teo should go to Viktor's office, preferably as a short sequence of him walking there, and the conversation happen inside. Build together with "people move when the text says so" (Viktor's clap above). | `content/ink/tuesday_office.ink:329` (viktor_office) |
| 2026-10-19 | The floors are completely open: tables, desks etc. should stand out in the room, not everything pushed against the walls. (2026-10-27, on the creator's request: first try built in the office and on the terrace. The flat and the stairwell are unchanged. Waiting for the creator's verdict.) | All rooms |
| 2026-10-19 | Portraits are end-of-project work: one expression per character is enough for now. | `design/portraits_flat.md` |
| 2026-10-26 | Doesn't like how text is handled at all: the box. Asked for alternatives. (A text column is built and is now the default; the old bar is still in Settings / F2. Waiting for the creator's verdict.) | `game/Main.Column.cs`, `game/Main.Ui.cs` |
| 2026-10-26 | The game looks bad, but that's known; later. Things must not stand along the walls (see the 2026-10-19 note on open floors). | All rooms |
| 2026-10-26 | Go through all the text critically and rewrite it. The story is the game's most important ingredient. (First pass done for everything, 2026-10-26: `design/text_review.md`. Waiting for the creator to play it.) | `content/ink/*.ink` |
| 2026-10-27 | Should we show how much money Teo has? Euro is fine (new in the EU). Pay etc. to be decided; the creator leans towards money not showing unless Teo checks his balance. (Money is already counted in the engine, gift money and beers, but starts at 0 and nothing reacts to it.) | `content/ink/tuesday_office.ink`, `tuesday_party.ink` |
| 2026-10-27 | Could the game be more like Disco Elysium: walking around a world with more depth (3D), while the look stays 2D? Paused for now. | `design/visual_standard.md`, `game/RoomView.cs` |
| 2026-10-27 | When Teo talks to someone, should the camera zoom in on them, and only be zoomed out when walking around? (First try built: F3 or Settings turns it off. Waiting for the creator's verdict.) | `game/Main.Column.cs` |
| 2026-10-27 | Maybe Teo has to go home if he has no money left? (Agreed instead: being broke changes how the evening goes rather than ending it. Built 2026-10-27: Teo starts with 70 euro, the bank tells him so in the morning; out of money at the bar he can borrow from Kornel, run a tab with Laci or take water; broke after the club he walks home at dawn; Wednesday remembers. Rare so far, because the bar opens for everyone at 18:40.) | `content/ink/tuesday_party.ink` |
| 2026-10-27 | The text column covers about 30% of the picture. (2026-10-27, on the creator's request: the column slides in only when there's text and out when Teo walks, and fades into the picture instead of a hard edge. Still a third of the screen while it's open. Waiting for the creator's verdict.) | `game/Main.Column.cs` |
| 2026-10-27 | The speaker's portrait sits at the very top of the text column; doesn't look good. Asked how Disco Elysium does it. (Built, on the creator's request: the portrait hangs on the column's left edge, over the picture, level with the newest line. Names in one warm family with small differences per person, choices in stamp blue, Teo's thoughts one fixed colour. Waiting for the creator's verdict.) | `game/Main.Column.cs` |
| 2026-10-27 | Frozen points: in Disco Elysium you can't talk to anyone without walking to them. We shouldn't have scenes on a fixed picture Teo hasn't walked to. Rewrite them. (Principle agreed. 2026-10-27: the morning stairwell is now a room Teo walks through; Stojan calls out when he comes near, and Teo can go over or walk on. Still frozen: the night stairwell, the interrupts at the office and the party, the night places.) | `content/ink`, `content/phases.json` |
| 2026-10-27 | The 3D comparison (C1) looks completely unfinished: the people are just cylinders. That is absolutely not acceptable. | `game/demo/StyleDemo.cs`, `art/out/style/c1.png` |
| 2026-10-27 | Playthrough 2026-10-27: Teo's flat looks bad. | `art/game/rooms`, ArtGen `FlatRoom` |
| 2026-10-27 | "50 each" for the gift: the creator thought we had lowered it further. (50 euro is about 550 kronor; the creator's own office gives about 100 kronor a year, roughly 9 euro.) | `content/ink/tuesday_home.ink:36`, `tuesday_office.ink` |
| 2026-10-27 | A bank that texts you your balance is unrealistic. | `content/ink/tuesday_home.ink:38` |
| 2026-10-27 | The stairwell looks bad. | `StairwellRoom` |
| 2026-10-27 | Magda's portrait already hints at the red dress, though at the office she's clearly in a pink top. | `art/game/portraits/magda_*`, `content/art.json` |
| 2026-10-27 | Are the tickets right? Had a drink with Magda, then could buy a beer and then a spirit before they ran out. Do tickets only go when Teo buys at the bar himself? (Yes: only the bar takes tickets; Magda is the bank, so a drink with her is free.) | `content/ink/tuesday_party.ink` (use_ticket) |
| 2026-10-27 | Felicity is with the alignment team but stands talking to Viktor: how does that fit? She also looks a lot like Ramona. Unclear who is who and why people stand where they stand. | `content/rooms/terrace.json`, `PixelPeople` |
| 2026-10-27 | The river looks bad. | `art/game/scenes/river.png` |
| 2026-10-27 | The sobering-up station's room looks terrible. | `art/game/scenes/station.png` |
| 2026-10-27 | The sobering-up invoice is far too high (it's 4,200). It probably shouldn't cost anything, or a bill comes home later. The creator doesn't know how it works. | `content/ink/wednesday.ink:125` |
| 2026-10-27 | Viktor got the letter the same morning Teo was checked out of the station: impossibly fast post. Needs handling in a completely different way. | `content/ink/wednesday.ink` |
| 2026-10-27 | "I'd like the floor to open": what does it mean? A lot of the language needs rewriting, better and more narrative. Feels so-so now. | `content/ink/wednesday.ink:449`, all text |
| 2026-10-27 | Maybe end with a recap of your choices, like "5% of players chose this" (as in The Walking Dead); or maybe that's uninteresting. (The clearer "start again" is done.) | `game/Main.cs` (ShowEnding) |
