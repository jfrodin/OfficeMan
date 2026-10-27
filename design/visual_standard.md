# Visual standard for locations

**Status: working standard (2026-10-18), written after the creator's review of the flat and the hallway.**
Final art direction is still open; this keeps every scene in the prototype coherent until then.

## Why the scenes were inconsistent

| | Office | Flat (before) | Hallway (before) |
|---|---|---|---|
| Made for | The walkable room system | The first art test, weeks of iterations earlier | A stop-gap gradient |
| Native size / scale on screen | 480×270, ×4 | 320×180, ×6 (bigger pixels) | 320×180, ×6 |
| Perspective | Side-on elevation: flat back wall, a strip of floor | One-point perspective with converging walls; the bed drawn in a different perspective again | None |
| Lighting | Unlit art + real lights + post-processing | Light baked into the picture, no shader | None |
| Texture | Material ramps, restrained | Heavy ordered dithering everywhere ("pixel noise") | Dithered gradient |
| People | Pixel sprites in the scene | None | An empty silhouette card |

The root cause was process, not technique: the office was built and reviewed for the walkable system; the
other two were never rebuilt for it, and I didn't look at them in the game before handing the build over.

## The standard

**Canvas and camera**
- Every location is **480×270 native**, shown at **×4**. Same pixel size everywhere.
- **Side-on elevation**: the back wall is flat and parallel to the screen. No converging walls, no second perspective inside a scene. Depth comes from the floor strip and from overlap (things in front, things behind).
- Wall base (where floor meets wall) at **y ≈ 176-178**; the floor strip below is where people stand.

**Scale and proportions** (characters ~50 px tall ≈ 1.7 m; roughly 30 px per metre)
- Doors **66 px** tall, ~30 px wide. Tables and desks **22-26 px**, counters **30 px**, beds **20-24 px** high and about **60-70 px** long.
- Windows start around **1 character height** above the floor line.
- Ceilings are stylised high (2.5-3.3 character heights) so walls carry the storytelling; that applies to all rooms alike.

**Lighting**
- Every scene is drawn **unlit** and lit by the room shader. Every visible light has a source in the picture (a window, a bulb, a screen, a door gap).
- One **key light** (the strongest, sets the mood), one or two **practical** lights, a tinted **ambient**. People are lit by the same lights and cast floor shadows.
- Time of day is shown by light, not by text: dawn, office morning, night.

**Pixel density and texture**
- Materials use short hand-picked ramps (4-7 steps). Texture only where a material has it (wood grain, tile seams, wallpaper print, terrazzo chips), as regular patterns, never random noise.
- **No large dithered fields.** Dithering only for soft gradients in light, sky and glass.
- Outlines only where needed for readability (characters); environment shapes are read by value contrast.

**Composition**
- Each scene has a clear focal point (where the story happens), a readable silhouette for every important object, breathing space around them, and a foreground element for depth where it helps.
- Objects sit on the floor line or the wall plane, never floating.

**Furniture on the floor** (2026-10-27, first try)
- Not everything against the walls. Desks, tables and trolleys stand out on the floor as **prop sprites**: one frame, anchored at the front foot, sorted by depth with the people, so someone can stand behind a desk and Teo in front of it.
- A prop's `block` in the room json (half width, depth) is the floor it takes up. Teo walks round it, and the art generator puts a soft shadow there, so the json is the one place that says where furniture stands.
- People who work at a desk stand behind it. Things that belong on a wall (shelves, the cabinet, the noticeboard) stay there.
- Screens on a prop can't use the emissive layer (they move with the sprite). They're drawn bright, a small light in the room json lights round them, and the room's glow layer gives the bloom.

**Environmental storytelling**
- Every object earns its place by saying something about who lives or works there. No random decoration.

**People**
- Anyone present in a scene appears in it as a sprite, at scene scale, with at least idle animation.
- Describing a visible action in text is a fallback, not a substitute.

## Presentation modes for text

One typography everywhere (same font, sizes 26-30, warm name colour, cool italic for thoughts). Three layouts, so the player knows at a glance what is happening:

| Situation | Presentation |
|---|---|
| Someone speaks **to Teo** (a conversation) | The **bar**: full width along the bottom, at least 210 px high, growing upwards with its content. A head-and-shoulders portrait (250×270, cropped from the full portrait) stands in its left end. Name, words, choices. The scene behind dims slightly. Three or more choices go in two columns so the bar stays low. |
| Teo's **thoughts and observations**, nobody talking to him | A compact **note** low in the frame, centred, only as wide as the words. No name, no portrait, no dimming. |
| Teo's thoughts **during** a conversation | Stay in the bar, in italics; the other person's portrait dims. The layout doesn't jump mid-conversation. |
| Someone **overheard** | A small **caption** with name and source ("STOJAN · through the wall"), placed towards the sound: ink tag `#from:left`, `#from:right` or `#from:<thing id>` (above that thing); top centre without one. No portrait, no bar. |

Characters without portrait art speak in the bar with their name only, never behind an empty silhouette.

**The text column** (2026-10-27). Slides in when there's text, out when Teo walks. A warm, dark, paper-like colour that fades into the picture over 150 px instead of a hard edge. The speaker's portrait hangs on the column's left edge, over the picture, level with the newest line (never low in the frame, where people stand). Names in one warm family with small differences per person (Magda coral, Viktor gold, Zdenka grey-green, Kornel mustard, Ramona pearl, Stojan olive; Teo muted); the differences stay small on purpose. Teo's thoughts: italic, one fixed cool colour; his state shows in the words, not the colour (agreed with the creator, so it doesn't become a gauge). Choices in stamp blue, like an office stamp.

**Close-up** (2026-10-27, first try, column layout). When Teo talks to someone in the room, the camera moves in on the two of them (×2, or ×1.5 if they stand apart, so the pixels stay square) and back out when he walks again. F3 or Settings turns it off. If someone talks to Teo from across the room, he walks over to them first, so a conversation always has the two of them together; they're framed in the space left of the portrait (the room may slide further than the column to make that possible).

**Camera (bottom bar).** It doesn't move when dialogue opens. It rises only when the bar would cover Teo or the person talking to him, only by as much as needed (at most 120 px), eases there smoothly, ignores small changes, and settles back when Teo walks again.

**Resolutions.** The game renders at 1920×1080 and scales (checked at 1024×768, 1280×720, 1600×900, 2560×1080). Other aspect ratios get black bars. Body text is about 16 px high at 1024 wide: readable, but the smallest sensible window.

## Where the scenes stand (2026-10-18, second pass)

Checked by screenshot. `Review.bat` puts every room and text mode on one sheet: `art/out/review.png`.

| Scene | Meets the standard | Still open |
|---|---|---|
| Flat (dawn) | Side-on, to scale, dawn key light, storytelling objects (clock 06:47, dishes, envelopes, diploma in the wardrobe, a moving box and an unlabelled bottle on the cupboards, the smoke alarm with its battery out). Teo is in bed as a sprite, breathing. | Teo in bed is small and simple at this scale. |
| Stairwell (day, night) | Same camera and scale as the office; doors, stairs, flickering bulb, Stojan smoking (sprite with cigarette and smoke); TV light under his door at night; tiled terrazzo; a neighbour's bicycle in the foreground. | |
| Office | Door 66 px, desks 24 px, no dithering, real office chairs, keyboards. | Some desk props are still small blocks. |
| Terrace (17:00-23:30) | Same camera and scale; the river, the far bank and the sky behind the railing; the bar under an awning with Laci behind the counter (a prop, so people stand behind and in front of it); string lights; sitting poses at tables. The evening is a timeline: ambient light and glow change with the clock, the sunset is a light so night can put it out. | The deck's lower half is plain; the outsiders' tables look the same whether or not they're here. |

People are now **47-52 px** tall (frame 28×60), which matches the standard next to 66 px doors.

Screens emit light on top of everything in the frame except people and the foreground: the shader compares the frame with the room's own albedo and holds back emissive light wherever something stands in front.
