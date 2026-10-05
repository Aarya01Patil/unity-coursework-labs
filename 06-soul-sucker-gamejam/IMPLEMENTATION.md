

Your playbook says the jam entry must have one complete gameplay loop, stay very small in scope, be playable on the web, and only add polish after the core loop is stable. It also gives a concrete fun check: 3 fresh players, 3 minutes each, and at least 2 of 3 wanting another round. 

For the Codex side, I’m shaping this around the workflow OpenAI actually documents: Codex works in the current directory, reads `AGENTS.md` before it starts work, and OpenAI recommends Git checkpoints plus putting build/test/done criteria into `AGENTS.md`. If you want multiple agents working in parallel, you have to ask for subagents explicitly. ([OpenAI Developers][1])

I’m assuming a Unity 6.3 LTS 2D URP project. Unity’s 2D setup docs recommend starting from the 2D template with URP, Unity’s Unity 6 support page positions 6.3 as the current LTS line, and Unity’s web docs cover the browser build flow we need. For the MVP, I’d stay on the built-in Input Manager because Unity documents it as the default when you do not install the newer Input System package. ([Unity Documentation][2])

## 1) The exact product brief Codex should follow

**Game:** Soul Sucker
**Genre:** 2D fixed-camera puzzle-platformer
**Theme:** Dependency
**Core loop:** leave host → possess another host → gain that host’s single ability → solve the room → reach exit or die
**Host types:** 3 only

* Jumper
* Flyer
* Shooter

**Non-negotiables**

* One complete loop only.
* Browser-playable build required.
* Five rooms max.
* Three core abilities max.
* No story systems, inventory, crafting, dialogue trees, save system, or complex enemy AI.
* Placeholder visuals/audio are fine; loop completeness is not negotiable.
* Polish only after all five rooms are playable end to end. 

## 2) The most Codex-friendly technical strategy

Do **not** ask Codex to build this as a big pile of hand-edited Unity scenes and prefabs.

The reliable path is:

* 1 `Bootstrap` scene
* 1 `MainMenu` scene
* 1 `Gameplay` scene
* All five puzzle rooms generated **from C# level data at runtime**
* Placeholder sprites and basic sounds generated in code or created by tiny editor scripts

That keeps most of the project in normal C# files, which is where Codex is strongest. It also avoids fragile `.unity`, `.prefab`, and `ProjectSettings` churn.

## 3) High-level architecture

### Scenes

`Bootstrap`

* Initializes persistent systems
* Loads `MainMenu`

`MainMenu`

* Title
* Play button
* Controls panel
* Quit omitted for Web

`Gameplay`

* Fixed orthographic camera
* Builds one room at a time from a `LevelBlueprint`
* Uses overlays for fail/win states instead of extra scenes

### Runtime flow

1. Boot
2. Main menu
3. Load room 1
4. Play room
5. On death: rebuild current room
6. On exit: next room
7. After room 5: show victory panel

### Why fixed-camera single-screen rooms

This is the cleanest jam scope:

* no scrolling camera bugs
* no checkpoint system
* no large world loading
* easier Web build performance
* easier Codex automation

Use one room size for every level: **32 units wide × 18 units tall**, which matches a 16:9 fixed camera nicely.

## 4) Concrete gameplay spec

### Controls

* `A/D` or Left/Right: move
* `Space`: jump or ascend if in flyer
* `E`: possess nearest host / leave current host
* `J`: fire projectile when in shooter host
* `R`: restart room
* `Esc`: pause optional, not required

### Parasite form

* Small blob
* Can move, but weakly
* Can survive outside a host for only **3.0 seconds**
* Dies on hazards
* Cannot shoot
* Has only a tiny hop or no jump

### Host behavior

Unpossessed hosts do **not** need AI for MVP. They can just stand idle. That is good scope discipline.

When possessed:

* host becomes the controlled body
* parasite sprite hides
* parasite collider/rigidbody disables
* input routes into host controller

When released:

* parasite spawns at the host’s release anchor
* air timer starts
* host freezes again
* player must find a new host or die

### Win condition

Touch the exit door while alive.

### Lose conditions

* parasite timer reaches zero outside a host
* active body touches hazard
* optionally projectile self-damage is off for MVP

## 5) Project folder structure

```text
Assets/
  Art/
    Generated/
    UI/
  Audio/
    Generated/
  Editor/
    Build/
    Scaffold/
  Materials/
  Prefabs/
    Runtime/
  Scenes/
    Bootstrap.unity
    MainMenu.unity
    Gameplay.unity
  Scripts/
    Core/
    Data/
    Player/
    Hosts/
    World/
    UI/
    Audio/
    Utilities/
  Tests/
    EditMode/
    PlayMode/
  Resources/
    Generated/
```

Also create assembly definitions:

```text
Assets/Scripts/ParasitePlayer.Runtime.asmdef
Assets/Editor/ParasitePlayer.Editor.asmdef
Assets/Tests/EditMode/ParasitePlayer.Tests.EditMode.asmdef
Assets/Tests/PlayMode/ParasitePlayer.Tests.PlayMode.asmdef
```

## 6) Put this in `AGENTS.md`

```md
# AGENTS.md

## Project
Build "Parasite Player", a 2D fixed-camera puzzle-platformer for Web.
Theme is dependency: the player is a weak parasite that must possess hosts to gain abilities.

## Design rules
- Preserve one gameplay loop only: leave host -> possess host -> use ability -> reach exit or die.
- Hard cap: 3 host abilities, 5 rooms, 3 UI screens.
- Browser build is mandatory.
- Do not add story systems, inventory, dialogue, save/load, complex enemy AI, or procedural generation beyond room instantiation from data.
- Polish only after all 5 rooms are playable end to end.

## Technical rules
- Use Unity 6.3 LTS, 2D URP project.
- Prefer C# runtime builders and editor scripts over direct large YAML edits to .unity/.prefab files.
- Keep dependencies minimal. No third-party packages for MVP.
- Use the built-in Input Manager for MVP.
- Keep the game fixed-camera and single-screen per room.
- Use placeholder/generated art and simple generated audio where practical.

## Architecture rules
- Scenes: Bootstrap, MainMenu, Gameplay only.
- Gameplay scene must build rooms from code-defined level blueprints.
- No service locator, no DI container, no ECS, no reflection-heavy framework.
- Prefer boring, direct C# with clear references.
- If a system can be a single MonoBehaviour and a small data class, keep it that way.

## Workflow
- Create a git checkpoint before and after each major task.
- Work in phases:
  1. bootstrap and docs
  2. runtime shell and room builder
  3. parasite movement and possession
  4. host abilities
  5. room content
  6. UI and polish
  7. tests
  8. Web build and final fixes
- After each phase:
  - summarize files changed
  - list manual Unity steps, if any
  - run relevant tests/checks
  - do not silently expand scope

## Done means
- Room 1 through Room 5 fully playable
- All 3 host types work
- Death and respawn work
- Win progression works
- HUD is readable
- Web build runs in browser
- EditMode and PlayMode smoke tests pass
```

That matches OpenAI’s recommendation that `AGENTS.md` stay practical and encode repo layout, run/test commands, constraints, and what “done” means. ([OpenAI Developers][3])

## 7) Exact system-by-system implementation plan

### Phase 0 — one-time human setup

Do this once before Codex starts changing gameplay code:

1. Install Unity 6.3 LTS in Unity Hub.
2. Add the **Web** module.
3. Create a **2D URP** project named `ParasitePlayer`.
4. Initialize Git.
5. Open the project once so Unity imports packages.

Unity’s docs say to install the Editor from Hub, choose the 2D template with URP for 2D work, and add the Web module for browser builds. ([Unity Documentation][4])

### Phase 1 — bootstrap the repo

Codex should create:

`README.md`

* one-paragraph game summary
* controls
* how to run
* how to build web

`AGENTS.md`

* the file above

`Assets/Editor/Scaffold/ProjectScaffoldGenerator.cs`

* optional editor menu to:

  * create the 3 scenes
  * set build settings
  * create core roots in scenes

`Assets/Editor/Build/BuildScript.cs`

* `BuildWebDevelopment()`
* `BuildWebRelease()`

`Assets/Scripts/Core/Bootstrapper.cs`

* lives in `Bootstrap`
* loads `MainMenu`

`Assets/Scripts/Core/GameSession.cs`

* persistent singleton or persistent root
* tracks:

  * current room index
  * total deaths
  * total time
  * game state

`Assets/Scripts/Core/SceneRouter.cs`

* `LoadMainMenu()`
* `LoadGameplay()`
* `ReloadGameplay()`

Acceptance:

* project opens without compile errors
* scenes exist
* menu loads
* gameplay scene can be entered

### Phase 2 — room data and runtime builder

This is the backbone.

Create:

`Assets/Scripts/Data/IntRect.cs`

```csharp
public struct IntRect { public int x, y, w, h; }
```

`Assets/Scripts/Data/HostType.cs`

```csharp
public enum HostType { None, Jumper, Flyer, Shooter }
```

`Assets/Scripts/Data/HostSpawnData.cs`

```csharp
public struct HostSpawnData { public HostType type; public Vector2Int cell; }
```

`Assets/Scripts/Data/LevelBlueprint.cs`
Fields:

* `string id`
* `string title`
* `Vector2Int roomSize` default `(32,18)`
* `Vector2Int playerStart`
* `Vector2Int exitCell`
* `IntRect[] solids`
* `IntRect[] hazards`
* `IntRect[] breakables`
* `HostSpawnData[] hosts`
* `string[] tutorialHints`

`Assets/Scripts/Data/LevelCatalog.cs`

* returns exactly 5 blueprints
* all room definitions hard-coded here for MVP

`Assets/Scripts/World/LevelManager.cs`
Responsibilities:

* current level index
* build room from blueprint
* clear old room root
* respawn current room
* advance to next room
* final victory

`Assets/Scripts/World/LevelBuilder.cs`
Responsibilities:

* create a `RuntimeLevelRoot`
* instantiate solids/hazards/breakables/hosts/exit
* return references to the spawned room content

`Assets/Scripts/Utilities/PlaceholderSpriteLibrary.cs`
Responsibilities:

* create/cache:

  * square sprite
  * circle sprite
* return sprites by shape name
* avoid external art dependency

`Assets/Scripts/World/BlockFactory.cs`
Responsibilities:

* create colored rectangles for:

  * solid floor/walls
  * hazard zones
  * breakable blocks
  * exit door

Acceptance:

* gameplay scene loads room 1 visually
* solids, hazards, exit, and hosts all appear
* room can be rebuilt cleanly with no duplicates

### Phase 3 — player input and parasite state machine

Create:

`Assets/Scripts/Player/InputFrame.cs`

```csharp
public struct InputFrame {
  public float moveX;
  public bool jumpPressed;
  public bool jumpHeld;
  public bool possessPressed;
  public bool firePressed;
  public bool restartPressed;
}
```

`Assets/Scripts/Player/InputReader.cs`

* polls `Input.GetAxisRaw` / `Input.GetKeyDown`
* produces `InputFrame`

`Assets/Scripts/Player/PlayerBrain.cs`
State:

* `ParasiteFree`
* `PossessingHost`
* `InsideHost`
* `Dead`
* `TransitioningRoom`

Fields:

* current state
* current host ref
* air timer
* input reader
* parasite body ref
* session/level refs

Methods:

* `Tick()`
* `FixedTick()`
* `TryPossessNearestHost()`
* `ReleaseHost()`
* `KillPlayer()`
* `RespawnCurrentRoom()`
* `OnLevelBuilt()`

`Assets/Scripts/Player/ParasiteBody.cs`

* Rigidbody2D
* CircleCollider2D
* SpriteRenderer
* weak movement
* optional tiny hop
* no combat

Recommended parasite values:

* move speed: `3.5`
* jump force: `0` or `2.5`
* air timer: `3.0`
* possess radius: `1.4`

`Assets/Scripts/Player/HostDetector.cs`

* `OverlapCircleNonAlloc` or `OverlapCircleAll`
* filters by `HostBase`
* picks nearest available host
* exposes current candidate for UI highlight

Acceptance:

* parasite can move
* candidate host highlights
* `E` possesses nearest host
* `E` while inside host ejects parasite
* timer kills stranded parasite
* `R` rebuilds room

### Phase 4 — host system

Create base class:

`Assets/Scripts/Hosts/HostBase.cs`
Fields:

* `HostType hostType`
* `Rigidbody2D rb`
* `Collider2D col`
* `SpriteRenderer renderer`
* `Transform releaseAnchor`
* `bool isControlled`
* `float moveSpeed`
* `string promptText`

Methods:

* `SetControlled(bool value)`
* `OnPossessed(PlayerBrain brain)`
* `OnReleased(PlayerBrain brain)`
* `TickInput(InputFrame input)`
* `FixedTickInput(InputFrame input)`
* `GetAbilityName()`
* `GetPromptText()`

Important behavior:

* when not controlled:

  * freeze motion
  * zero velocity
  * remain possessable
* when controlled:

  * enable movement logic
  * keep `FreezeRotationZ`

#### Jumper host

`Assets/Scripts/Hosts/JumperHost.cs`

* move left/right
* high jump
* no special attack

Recommended values:

* speed `5.0`
* jump force `12.0`
* coyote time `0.1`
* jump buffer `0.1`

#### Flyer host

`Assets/Scripts/Hosts/FlyerHost.cs`

* move left/right
* hold `Space` to rise
* gravity reduced
* no infinite vertical rocket feel

Recommended values:

* speed `4.0`
* ascend force/speed `4.0`
* gravity scale `0.35`
* max hover meter optional, but for MVP skip the meter

#### Shooter host

`Assets/Scripts/Hosts/ShooterHost.cs`

* basic ground movement
* low jump optional
* fires projectile on `J`

Recommended values:

* speed `4.0`
* jump force `6.5`
* fire cooldown `0.35`
* projectile speed `12.0`

#### Projectile system

`Assets/Scripts/World/Projectile.cs`

* moves in facing direction
* destroys itself after time
* on hit:

  * if `BreakableBlock`, destroy block and projectile
  * else destroy projectile

`Assets/Scripts/World/BreakableBlock.cs`

* one-hit block
* simple destroy VFX hook

Acceptance:

* jumper solves a high ledge
* flyer solves a gap/hazard crossing
* shooter destroys a wall
* no host ability leaks into another host

### Phase 5 — hazards, exit, death, progression

Create:

`Assets/Scripts/World/HazardZone.cs`

* trigger collider
* kills active body on contact

`Assets/Scripts/World/ExitDoor.cs`

* trigger collider
* if player alive and not transitioning, complete room

`Assets/Scripts/World/RespawnController.cs`

* handles brief fade
* rebuilds room
* resets timer

Death rules:

* if in parasite form, touching hazard kills immediately
* if in host form, touching hazard kills the player and respawns room
* on death:

  * increment death counter
  * short freeze `0.1–0.2s`
  * respawn same room

Progression rules:

* room 1–5 sequential
* after room 5 show victory overlay with:

  * total time
  * total deaths
  * replay button
  * main menu button

Acceptance:

* death/respawn is instant and readable
* exit always advances room
* final room reaches victory overlay

### Phase 6 — UI and feedback

Create:

`Assets/Scripts/UI/HUDController.cs`
Display:

* current room `1/5`
* current host name
* air timer bar
* context prompt
* deaths
* optional timer

`Assets/Scripts/UI/MainMenuController.cs`

* Play
* Controls panel
* Back

`Assets/Scripts/UI/OverlayController.cs`

* room intro hint text
* death flash
* final victory

HUD rules:

* top-left: host name
* top-right: air timer
* center-bottom: prompt
* very small number of UI states only

Prompt examples:

* parasite near host: `Press E to possess Jumper`
* jumper: `Space: Jump high`
* flyer: `Hold Space: Fly`
* shooter: `J: Shoot`

Visual feedback:

* candidate host gets outline/tint
* possession pulse effect
* death flash
* projectile impact puff
* exit glow

Keep it primitive and readable.

### Phase 7 — audio

Do not block MVP on imported sound assets.

Create:
`Assets/Scripts/Audio/SfxGenerator.cs`

* generates tiny tones with `AudioClip.Create`

Sounds:

* possess
* release
* jump
* fly hum burst
* shoot
* death
* win

If this becomes a time sink, cut it.

### Phase 8 — tests

Unity’s Test Framework supports both Edit Mode and Play Mode tests, so use both for smoke coverage. ([Unity Documentation][5])

EditMode tests:

* `LevelCatalog_HasExactlyFiveRooms`
* `EachRoom_HasExit`
* `EachRoom_HasAtLeastOneHost`
* `Possession_FailsWhenNoHostInRange`
* `AirTimer_KillsAtZero`
* `Projectile_DestroysBreakable`

PlayMode tests:

* `Player_CanPossessNearestHost`
* `ReleaseHost_ReturnsToParasiteState`
* `ExitDoor_AdvancesLevel`
* `Hazard_RespawnsRoom`
* `Victory_ShowsAfterLastRoom`

Don’t overengineer test coverage. Smoke tests are enough for a jam.

## 8) Exact room content plan

### Room 1 — Jump tutorial

Goal: teach possession + high jump

Layout:

* floor: full width
* low platform mid-room
* high ledge with exit
* one Jumper host near start

Blueprint intent:

* parasite starts weak
* player sees exit but cannot reach
* possesses jumper
* jumps to ledge
* reaches exit

### Room 2 — Flyer tutorial

Goal: teach gap crossing

Layout:

* left floor
* central spike pit
* right floor with exit
* one Flyer host on left side

Blueprint intent:

* impossible in parasite form
* flyer crosses safely

### Room 3 — Shooter tutorial

Goal: teach breakable obstacle

Layout:

* full floor
* breakable wall blocks right side
* shooter host on left
* exit behind wall

Blueprint intent:

* possess shooter
* fire projectile
* wall breaks
* walk to exit

### Room 4 — combination room

Goal: use two host types in sequence

Layout:

* start with Jumper host
* upper platform contains Shooter host
* shooter breaks barrier that reveals path to Flyer host
* flyer reaches elevated exit

Blueprint intent:

* jump first
* shoot second
* fly last

### Room 5 — final exam

Goal: use all three in one room

Layout:

* parasite starts left
* Jumper host reaches mid platform
* Shooter host breaks right-side block
* Flyer host crosses final hazard trench
* exit high on far right

Blueprint intent:

* no new mechanics
* just sequencing

That keeps the level count small and the object vocabulary tiny, which matches the scoping rules in your playbook. 

## 9) Concrete code file map

Use this exact runtime file set.

```text
Scripts/Core/
  Bootstrapper.cs
  GameSession.cs
  SceneRouter.cs
  GameState.cs

Scripts/Data/
  HostType.cs
  IntRect.cs
  HostSpawnData.cs
  LevelBlueprint.cs
  LevelCatalog.cs

Scripts/Player/
  InputFrame.cs
  InputReader.cs
  PlayerBrain.cs
  ParasiteBody.cs
  HostDetector.cs

Scripts/Hosts/
  HostBase.cs
  JumperHost.cs
  FlyerHost.cs
  ShooterHost.cs

Scripts/World/
  LevelManager.cs
  LevelBuilder.cs
  BlockFactory.cs
  SolidBlock.cs
  HazardZone.cs
  BreakableBlock.cs
  ExitDoor.cs
  Projectile.cs

Scripts/UI/
  HUDController.cs
  MainMenuController.cs
  OverlayController.cs

Scripts/Audio/
  SfxGenerator.cs
  AudioRouter.cs

Scripts/Utilities/
  PlaceholderSpriteLibrary.cs
  RuntimeDestroyHelper.cs
```

## 10) The exact sequence of Codex prompts to use

Do **not** give Codex one giant “build the whole game” prompt. Use these in order.

### Prompt 1

```text
Read AGENTS.md and scaffold the Parasite Player Unity repo for a jam-scale Web game.
Create the folder structure, asmdefs, README, BuildScript.cs, and either the three scenes or an editor scaffold script that can generate them safely.
Do not implement gameplay yet.
Create a git checkpoint before and after the task.
Summarize created files and list any manual Unity steps.
```

### Prompt 2

```text
Implement the runtime room system for Parasite Player.
Create LevelBlueprint, LevelCatalog, LevelBuilder, BlockFactory, PlaceholderSpriteLibrary, and LevelManager.
Use a fixed 32x18 single-screen room format and render solids, hazards, breakables, hosts, and exit from C# blueprint data.
Make room 1 render in Gameplay scene.
Add at least 2 EditMode smoke tests.
Run tests and summarize.
```

### Prompt 3

```text
Implement player input, parasite movement, host detection, possession, release, air timer, death, and room respawn.
Use the built-in Input Manager for MVP.
Create InputFrame, InputReader, PlayerBrain, ParasiteBody, and HostDetector.
Press E should possess nearest host; pressing E inside a host should release the parasite at the host release anchor.
R should restart the current room.
Add tests for no-host possession failure and air timer death.
Run tests and summarize.
```

### Prompt 4

```text
Implement HostBase, JumperHost, FlyerHost, ShooterHost, Projectile, and BreakableBlock.
Each host should provide exactly one clear ability.
Do not add enemy AI or extra mechanics.
Make room 1, 2, and 3 each fully solvable using the intended host.
Add PlayMode smoke tests for possession and exit progression.
Run tests and summarize.
```

### Prompt 5

```text
Implement room 4 and room 5 in LevelCatalog using the existing room blueprint system.
Use only the three hosts and the existing object types.
Do not expand the mechanic set.
Make the five-room campaign fully playable end to end.
Add victory flow after room 5.
Run tests and summarize.
```

### Prompt 6

```text
Implement HUD, menu, overlays, possession highlight, simple death flash, and minimal generated sound effects.
Keep UI minimal and readable.
Do not add new gameplay systems.
After implementation, run through the full game loop and report any broken feedback.
```

### Prompt 7

```text
Add final smoke coverage for room progression, hazard respawn, projectile breakables, and final victory state.
Then implement BuildScript methods for Web development and Web release builds.
Document how to create and test a local Web build in README.
Run tests and summarize remaining manual steps before shipping.
```

### Prompt 8

```text
Audit the project against AGENTS.md and the game jam rules.
List anything that violates small scope, Web playability, or one-loop design.
Fix only high-confidence issues.
Then produce a final ship checklist for Web submission.
```

## 11) Optional explicit subagent prompt

If you’re using Codex in a surface that supports subagents, this is the only safe parallel split I’d use:

```text
Spawn 3 subagents.
Subagent 1: audit AGENTS.md and repo structure for jam-scope violations.
Subagent 2: review LevelCatalog and room blueprints for progression clarity and softlocks.
Subagent 3: review tests and Web build scripts for missing coverage.
Wait for all results, then merge only high-confidence improvements into the main branch.
```

Codex only creates subagents when you explicitly ask it to. ([OpenAI Developers][6])

## 12) Web build and submission plan

Create `BuildScript.cs` with:

* `BuildWebDevelopment()`
* `BuildWebRelease()`

Output folders:

* `Builds/WebDev`
* `Builds/WebRelease`

Ship checklist:

* Web module installed
* gameplay scene in build
* five rooms complete
* no missing reference errors
* no compile warnings from your core scripts
* dev build tested locally
* release build tested locally
* zip release folder for upload

For local browser testing, Unity’s docs say not to just double-click `index.html`; use **Build and Run** or serve the build from a local web server because browser security blocks the `file://` path for Web builds. ([Unity Documentation][7])

## 13) Hard “do not let Codex do this” list

Put this in your head while reviewing its work:

* no save system
* no dialogue
* no enemy combat tree
* no inventory
* no procedural level generation
* no camera follow
* no controller remapping for MVP
* no new packages unless truly required
* no giant refactor after room 3 works
* no “architecture cleanup” that risks the playable loop

## 14) Final definition of done

The game is done when:

* boot → menu → room 1 → room 5 → victory works
* parasite timer works
* each of the 3 hosts works
* death and restart work
* five rooms are solvable
* Web build opens in browser
* smoke tests pass
* 3 fresh players can understand it without explanation and at least 2 want another try, matching your playbook’s playtest bar. 

The single most important decision here is this: **make Codex build the game mostly as C# systems and room blueprints, not as a mountain of Unity scene YAML.** That is what keeps the project reviewable, testable, and jam-safe.

# Important Review 
 After you are done , Claude Code will review the whole codebase