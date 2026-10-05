---
name: unity-room-blueprint-writer
description: Design and implement small Unity puzzle or platformer rooms from code-defined level blueprints instead of heavy manual scene editing. Use when Codex needs to create room data, puzzle sequencing, softlock-safe layouts, or runtime room builders in C# for small single-screen levels.
---

# Unity Room Blueprint Writer

Write rooms as data first.

## Default approach
- Represent each room with a blueprint data structure.
- Keep rooms single-screen unless the user explicitly asks otherwise.
- Build geometry, hazards, interactables, and exits from blueprint data.
- Rebuild rooms cleanly for restart and testing.

## Writing order
1. Define the room purpose.
2. Define the mechanic or mechanic combination being exercised.
3. Define the minimum object set needed.
4. Encode the layout as explicit data.
5. Check restart behavior and softlock safety.

## Blueprint contents
Prefer fields like:
- id
- title
- roomSize
- playerStart
- exitCell
- solids
- hazards
- breakables
- interactables
- spawn points
- tutorial hints

## Puzzle design rules
- Each room should teach, test, or combine known mechanics.
- Introduce one idea at a time.
- Avoid dead-end states without restart.
- Prefer obvious affordances over hidden interactions.
- Keep object vocabulary small across the whole campaign.

## Softlock check
For every room, verify:
- the player can always restart
- required interactables are reachable
- the win path is possible with the intended mechanic sequence
- no destructible or movable object can permanently block completion unless reset is instant

## Implementation bias
- Prefer explicit coordinates and typed room data over loose string-driven layouts.
- Prefer a small reusable blueprint vocabulary across the whole game.
- Prefer deterministic rebuilds for restart, testing, and debugging.

## Output pattern
When writing room plans, use:
1. room purpose
2. required mechanics
3. object list
4. blueprint sketch
5. failure cases
6. anti-softlock notes
