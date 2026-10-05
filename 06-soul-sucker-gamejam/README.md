# Soul Sucker

Soul Sucker is a 2D puzzle-platformer for Web built in Unity. You play as a fragile parasite that can survive alone only briefly, so progress depends on possessing hosts, using their single ability, solving the level, and reaching the exit.

## Gameplay Summary

- 8 side-scrolling levels with increasing difficulty
- 5 host types, each solving a distinct obstacle class:
  - `Jumper` - high vertical burst
  - `Dasher` - horizontal dash burst
  - `Shooter` - ranged projectile to break walls and hit remote switches
  - `Phase` - phase through spirit barriers
  - `Anchor` - heavy body to push blocks and activate pressure plates
- Side-scrolling camera with smooth follow and horizontal look-ahead
- Code-driven levels built from C# data blueprints
- Persistent bottom HUD card showing the equipped host, what it does, and its controls

## Controls

- `A / D` or `Left / Right`: move
- `Space`: host ability (jump / dash / phase) or parasite hop
- `E`: possess nearest host or release current host
- `J`: shoot while inside `Shooter`
- `R`: restart current level

## Campaign Progression

1. **Jumper Tutorial** - possess Jumper, climb a vertical route
2. **Dasher Tutorial** - possess Dasher and time a clean dash lane
3. **Shooter Tutorial** - break walls and hit a remote switch
4. **Anchor Tutorial** - push a heavy block onto a plate to open the route
5. **Phase Tutorial** - learn timed phasing through spirit barriers
6. **Three-Host Puzzle** - Jumper -> Shooter -> Dasher with a deliberate return
7. **Four-Host Chain** - Anchor -> Phase -> Shooter, then back to Jumper
8. **Final Exam** - all five hosts in sequence

## Open In Unity

1. Open this folder in Unity `6.3 LTS`.
2. Let Unity finish importing packages and generating metadata.
3. Run `Tools/Soul Sucker/Scaffold/Generate Or Repair Phase 1`.
4. Open `Assets/Scenes/Bootstrap.unity`.
5. Enter Play Mode.

Expected flow:

`Bootstrap -> MainMenu -> Gameplay -> Victory`

## Local Testing

- In the editor, use Play Mode from `Bootstrap`.
- For browser testing, use Unity `Build and Run` or serve the generated Web build from a local server.
- Do **not** test a Web build by double-clicking `index.html`, because browser security restrictions can break Web content loaded directly from disk.

## Web Builds

Editor menu entries:

- `Tools/Soul Sucker/Build/Web Development`
- `Tools/Soul Sucker/Build/Web Release`

Output folders:

- `Builds/WebDev`
- `Builds/WebRelease`

## Project Notes

- No third-party gameplay packages are required for the jam build.
- Levels are defined in code under `Assets/Scripts/Data/LevelCatalog.cs`.
- The game is intentionally scoped to one central loop only:
  `leave host -> possess host -> gain ability -> solve level -> exit or die`
- The project has diverged from the original early jam scope (3 hosts, 5 rooms, fixed camera) to an expanded redesign (5 hosts, 8 levels, scrolling camera).

## Manual Unity Steps Before Upload

- Open the project once in Unity so `.meta` files, package lock data, and scene assets are generated.
- Run the scaffold menu if the scenes need to be regenerated after script changes.
- Run EditMode smoke tests in Unity Test Runner.
- Build a Web release and test it through a local server or Unity `Build and Run`.
