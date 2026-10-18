# Art test v1

**Status: AWAITING THE CREATOR'S VERDICT.** Generated 2026-10-18 by `tools/OfficeMan.ArtGen`.
Regenerate with `dotnet run --project tools/OfficeMan.ArtGen`; images land in `art/out/`.

## What was made

| Image | Native size | Notes |
|---|---|---|
| `magda_neutral/laughing/hopeful/crushed/tipsy.png` | 112×136 (shown ×4) | One face, five expressions, all from parameters |
| `magda_sheet.png` | 5 × 112×136 | All five side by side |
| `teo_flat.png` | 320×180 (shown ×4) | Teo's flat, Tuesday 06:47 |

Everything is drawn by code. No image files are loaded; no external assets are used.

## How it works

- **Portraits:** volumes (head, cheeks, nose, hair mass, body) are smooth 3D height fields, lit by one key light and quantised onto small hand-picked colour ramps. Facial features are drawn precisely from an `Expression` record (smile, mouth open, lids, squint, brows, gaze, blush, tears). A new emotion is a new set of numbers, not a new drawing.
- **Scenes:** every pixel gets a *material* and a *light level*; lights (the window sun, bounce light, the dust shaft) add to the light level; the final pass quantises onto each material's ramp. That keeps one consistent light across the room.

## Honest assessment

What works:
- Expressions read clearly, especially *crushed* (running mascara) and *laughing*.
- Character identity: permed burgundy hair, blue eyeshadow, gold hoops, red dress. It's Magda.
- The room has mood: dawn through the window, the sun patch, the details from the writing (06:47, dying ficus, the crack like Italy).

What doesn't yet:
- Portraits look "rendered" (soft 3D shading) more than hand-placed pixel art. Hand-drawn pixel portraits use flatter planes and deliberate clusters. This can be pushed further: fewer ramp steps, hand-tuned clusters, stronger shapes.
- Faces are symmetrical and front-on. A three-quarter view would add a lot of life but is harder to generate.
- The body and dress are simple. Hands are not attempted yet.
- Dithering on large walls is still a bit busy.
- No text box / UI yet, so it's not a full dialogue-screen mock-up.

## Questions for the creator

1. **Overall:** is this in the right neighbourhood for the game, or should we change technique?
2. **Portrait style:** keep the soft, painterly shading, or push towards flatter, chunkier "classic" pixel art?
3. **Portrait resolution:** 112×136 now. Bigger means more expressive faces and more work per character.
4. **Room:** is the level of detail and mood right for locations?
