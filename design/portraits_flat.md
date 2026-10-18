# Portraits in the drawn_flat style (proposal v1, 2026-10-18)

**Status: proposal for the creator. Not in the game yet; the game still uses the temporary portraits.**

Preview: `art/proposals/flat_cast_v1.png`. Regenerate: `dotnet run --project tools/OfficeMan.ArtGen -- flat` (about a minute) → `art/out/flat/<name>_<mood>.png` and `art/out/flat_cast.png`.

## How it's built

- **Magda** is drawn by the reference code itself (`MagdaDrawing` + `DrawnStyles.Flat`), unchanged, only called with other expressions (happy, irritated, sad). She is exactly the reference.
- **Viktor, Zdenka, Kornel, Stojan** use a new kit, `Drawn/Flat/FlatFace.cs`, that follows the reference's rules: front view on the same 600×760 page, light from the upper left, two tones per colour with the shadow side on the right, soft blush, a thin warm line round the face, the same evening sky, fine grain. Each character is a description (`Drawn/Flat/FlatCast.cs`): face shape, age, eyes, brows, nose, mouth, hair, glasses, clothes.
- Moods use the names already in `content/art.json`, so switching the game over is a change to that file only.

## Honest state

| Character | Works | Not yet |
|---|---|---|
| Magda | Is the reference | Happy has no open smile (the reference has no teeth) |
| Viktor | Tanned, greying temples, open collar, stubble | Reads stiff and polite, not macho. The hair is a flat-top, not swept back. Needs more jaw, a heavier brow, a smirk. |
| Zdenka | Grey bun, half-moon glasses on a chain, brooch; the smile that doesn't reach the eyes | Face a little washed out |
| Kornel | Combover, round glasses, short sleeves and tie, pen in the pocket; overjoyed reads | Could be more awkward in posture (all are straight-on) |
| Stojan | Bald dome, wiry grey fringe, moustache, cigarette, vest, laughing with eyes shut | Neck and shoulders a bit block-like |

All four new faces share one construction, so they look related, but also a little alike. The reference's hand-placed shapes (Magda) are still more alive than the kit's generated ones. Next steps, if the direction is right: hand-tune each face's silhouette, give each a slight head angle, push the expressions further.
