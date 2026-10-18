# Third-party software

The game's creative content (writing, art, music) is original to this project. The following
software is used under its licence. These notices must ship with any distributed build
(e.g. in a `licenses` folder or the credits screen).

| Component | Version | Licence | Use | Obligation |
|---|---|---|---|---|
| [ink](https://github.com/inkle/ink) runtime and compiler, by inkle Ltd | 1.2.1 | MIT | Dialogue scripting (vendored in `third_party/ink`) | Include the copyright and licence text (`third_party/ink/LICENSE.txt`) |
| [.NET](https://github.com/dotnet/runtime) runtime | 8 / 9 | MIT | Runtime | Include licence text when the runtime is bundled |
| [Godot Engine](https://godotengine.org) | 4.6 | MIT | Game engine (graphical client, upcoming) | Include Godot's licence and its third-party notices (`Engine.get_license_text()` etc.) |
| [xUnit](https://xunit.net) | 2.9 | Apache-2.0 | Tests only, not shipped | None for builds |

No fonts, images, audio or other creative assets from third parties are used.
