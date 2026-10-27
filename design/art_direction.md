# Art direction: choosing the look (2026-10-27)

The creator wants a world with depth: things in the way, not everything pushed against the walls like a stage set.
This "tech demo" sets the standard for every location, so the rest of the work can be about events and the story.
Portraits are a separate track and not part of this choice.

**Status: decided 2026-10-27: A, the pixel art as the game has it now.** The creator chose it after talking it over at home. The rest of this page is the record of how we got there; the 3D demo (`game/demo/StyleDemo.cs`) and the high-resolution people (ArtGen `people`) are no longer used by anything.

## What code is good and bad at

Code is good at geometry, light and colour: rooms, furniture, shadows, window light, fog. It's bad at organic things:
faces, hands, plants, cloth. So the best-looking option leans on what code does well and keeps people simple. That's
also why Disco Elysium can't be copied: everything there rests on hand-painted surfaces.

## The options, as pictures of the same office

Made by `game/demo/StyleDemo.cs` (a dev tool, not part of the game):

    godot --path game --rendering-method forward_plus res://demo/StyleDemo.tscn -- --style c1 --shot out.png

Pictures in `art/out/style/` (scratch, not committed): `a.png`, `c1.png`, `c2.png`, `c3.png`, `h.png`, `hp.png`, `hf.png`. All four in one numbered sheet (1 A, 2 H, 3 HP, 4 HF): `--compare a.png h.png hp.png hf.png --shot compare.png`. The redrawn people: `art/out/people_sheet.png` (ArtGen `people`).

| | Look | Depth | Cost |
|---|---|---|---|
| A | Pixel art as today, furniture out on the floor | Faked: layers and sorted sprites | Least |
| C1 | Real 3D, flat colours, strong light (Kentucky Route Zero, A Short Hike) | Real: walk round a desk, rooms wider than the screen, light and shadow for free | Most, and the base that carries the rest |
| C2 | Real 3D drawn in two inks with a print dither (Return of the Obra Dinn) | Real | As C1; very distinctive, harder to read |
| C3 | Real 3D rendered small, shown as big pixels | Real | As C1; keeps the pixel feel |
| H | C1's 3D room, with the game's drawn people standing in it as flat figures facing the camera (Octopath Traveler, Paper Mario) | Real for the room | Rooms as C1; people stay 2D drawings, no 3D modelling or animation |
| HP | H rendered small like C3, so the room and the people share one pixel size | Real for the room | As H; today's sprites fit without redrawing |
| HF | H with the people redrawn at high resolution as smooth flat shapes (`tools/OfficeMan.ArtGen/Sprites/FlatPeople.cs`, from the same descriptions as the pixel people) | Real for the room | As H, plus walk frames and sitting poses still to draw |

The 3D pictures are a quick sketch: boxes, cylinders and spheres, no textures, people as faceless mannequins. They
show the light and the depth, not the finished detail.

2026-10-27, the creator on C1: looks completely unfinished, people as cylinders are not acceptable. Good 3D people
are the most expensive thing to make with our means (modelling, rigging, animation), so H was made: 3D rooms, 2D people.

The creator likes the feel of H (2026-10-27); the original pixel art has its charm too. The choice is between A and H.

The creator asked for the people to be redrawn rather than the room made smaller: HF.

Decided (2026-10-27): A. The game stays on the compatibility renderer.

## Logo and icon

Proposals made by ArtGen (`logos`, `icons`; scratch pictures in `art/out/logos_sheet.png`, `art/out/icons_sheet.png`).
Letters are the game's own, drawn in code (`tools/OfficeMan.ArtGen/Icons/Lettering.cs`), so no font licence is needed.
Byline: "A game by Joakim Frödin", no studio name (the creator, 2026-10-27).

- Logo: the creator leans towards the embossing label tape (red tape for OFFICE MAN), 2026-10-27. Not yet decided.
- Icon: the stamp mark on a slip of paper was the favourite before the logo leaned towards the label tape.
- Icon to match the label tape: OM on a piece of red tape (`tape`, first row of the icon sheet). Taller tape at 32 px and below.
