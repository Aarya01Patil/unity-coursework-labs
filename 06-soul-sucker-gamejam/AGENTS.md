# AGENTS.md

## Project
Build "Soul Sucker", a 2D puzzle-platformer for Web (Unity).
Theme: Dependency

## Core Gameplay Loop (NON-NEGOTIABLE)
Leave host → possess host → gain ability → solve level → reach exit or die

## Game Rules (CURRENT SCOPE)
- 5 host types: Jumper, Dasher, Shooter, Phase, Anchor
- 6 levels total
- 1 gameplay loop only
- No story systems, inventory, dialogue, save/load
- No complex enemy AI
- Must be playable in Web browser
- Small complete game > big incomplete game

## Design Constraints
- Side-scrolling camera with smooth follow and bounds clamping
- Level height: 18 units (fixed)
- Level widths: 32–72 units (increasing across campaign)
- 2D only
- Each host gives exactly ONE ability:
  - Jumper → High jump (vertical burst)
  - Dasher → Horizontal dash (hazard-immune during dash)
  - Shooter → Fire projectile (breaks walls, hits switches, disables turrets)
  - Phase → Phase through spirit barriers (timed)
  - Anchor → Heavy body (pushes blocks, activates pressure plates, fan-immune)

## Player Rules
- Parasite form is weak
- Can survive outside host for max 3 seconds
- Must possess hosts to progress

## Architecture Rules
- Scenes:
  - Bootstrap
  - MainMenu
  - Gameplay

- Build levels from C# data (NO large scene editing)
- Prefer code over prefab-heavy workflow
- Keep everything simple and explicit

## Obstacle Vocabulary
- Solids (walls, floors, platforms)
- Hazards (instant death zones)
- Breakable walls (destroyed by projectile)
- Timed laser gates (cycle on/off, linked toggleable)
- Remote switches (hit by projectile to toggle targets)
- Turret nodes (hazard beam, disabled by projectile)
- Phase gates (solid barriers, passable during Phase ability)
- Pressure plates (activated by Anchor or heavy blocks)
- Heavy blocks (pushable by Anchor)
- Fan zones (wind force, Anchor is immune)
- Exit door (level completion trigger)

## Workflow
Follow phases strictly:

1. Project setup
2. Level system
3. Player + possession
4. Host abilities
5. Level content (6 levels)
6. UI + feedback
7. Tests
8. Web build

After each phase:
- Summarize changes
- Add tests if relevant
- Do NOT expand scope

## Technical Rules
- Unity 6.3 LTS (2D URP)
- Use built-in Input system
- No external packages for MVP
- No over-engineering

## Definition of Done
Game is DONE when:
- All 6 levels playable
- Possession works
- 5 host types work
- Death + restart works
- Exit progression works
- Web build runs in browser
- Game feels replayable

## Hard DO NOTs
- No feature creep
- No extra mechanics
- No new systems mid-build
- No refactoring that risks breaking gameplay

## Scope History
This project originally started as a tightly-scoped jam game with 3 hosts (Jumper,
Flyer, Shooter), 5 rooms, and a fixed single-screen camera. It was later redesigned
to 5 hosts, 6 levels, and side-scrolling camera. The codebase is fully committed to
the redesign. The original spec is preserved in IMPLEMENTATION.md for reference.
