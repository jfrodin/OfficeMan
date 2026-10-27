# Art direction: choosing the look (2026-10-27)

The creator wants a world with depth: things in the way, not everything pushed against the walls like a stage set.
This "tech demo" sets the standard for every location, so the rest of the work can be about events and the story.
Portraits are a separate track and not part of this choice.

**Status: comparison pictures made; waiting for the creator to choose.**

## What code is good and bad at

Code is good at geometry, light and colour: rooms, furniture, shadows, window light, fog. It's bad at organic things:
faces, hands, plants, cloth. So the best-looking option leans on what code does well and keeps people simple. That's
also why Disco Elysium can't be copied: everything there rests on hand-painted surfaces.

## The options, as pictures of the same office

Made by `game/demo/StyleDemo.cs` (a dev tool, not part of the game):

    godot --path game --rendering-method forward_plus res://demo/StyleDemo.tscn -- --style c1 --shot out.png

Pictures in `art/out/style/` (scratch, not committed): `a.png`, `c1.png`, `c2.png`, `c3.png`.

| | Look | Depth | Cost |
|---|---|---|---|
| A | Pixel art as today, furniture out on the floor | Faked: layers and sorted sprites | Least |
| C1 | Real 3D, flat colours, strong light (Kentucky Route Zero, A Short Hike) | Real: walk round a desk, rooms wider than the screen, light and shadow for free | Most, and the base that carries the rest |
| C2 | Real 3D drawn in two inks with a print dither (Return of the Obra Dinn) | Real | As C1; very distinctive, harder to read |
| C3 | Real 3D rendered small, shown as big pixels | Real | As C1; keeps the pixel feel |

The 3D pictures are a quick sketch: boxes, cylinders and spheres, no textures, people as faceless mannequins. They
show the light and the depth, not the finished detail.

Recommendation: C1, or C3 if the pixel feel matters. If 3D is chosen, the game switches from the compatibility
renderer to Forward+ (needs a graphics card from roughly the last eight years).
