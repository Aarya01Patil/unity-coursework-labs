---
name: unity-game-jam-architect
description: Plan and steer small-scope Unity game jam projects with strict control of gameplay loop, feature count, room count, and shipping criteria. Use when Codex or ChatGPT needs to architect a jam game, define phases, protect scope, or turn a theme into a buildable Unity plan that can realistically ship on time.
---

# Unity Game Jam Architect

Keep jam projects ruthlessly small and finishable.

## Core rules
- Preserve exactly one core gameplay loop.
- Prefer 2D over 3D unless the user explicitly commits to 3D.
- Cap mechanics at 3 core mechanics.
- Cap distinct room or level archetypes at 5 unless the user explicitly asks otherwise.
- Prefer code-defined content and simple prefabs over fragile manual scene editing.
- Treat browser-playable or one-click playability as a major priority when relevant.
- A small complete game beats a larger incomplete game.

## Working method
1. Define the fantasy in 1 sentence.
2. Define the gameplay loop in 1 sentence: action -> challenge -> goal -> fail/win.
3. Define the minimum viable build: player state, core interactions, lose condition, win condition.
4. List what to cut before adding polish.
5. Break work into numbered phases that end with a playable checkpoint.

## Planning bias
- Solve for the first playable version before solving for replayability.
- Prefer one mechanic expressed through several room setups over many mechanics.
- Prefer explicit rules and simple data structures over content pipelines.
- Treat restart speed and browser performance as part of scope control when relevant.

## Scope filters
Reject or cut features that introduce any of the following unless the user explicitly asks for them:
- dialogue systems
- inventory or crafting
- save/load
- complex enemy AI
- procedural generation beyond simple room data
- cutscenes
- online features
- meta progression

## Output pattern
Default to this structure:
1. game premise
2. core gameplay loop
3. mvp scope
4. systems list
5. phase-by-phase build plan
6. cut list
7. ship checklist

## Unity bias
- Prefer a single gameplay scene plus overlays for small games.
- Prefer fixed camera rooms for jam puzzle-platformers.
- Prefer runtime room building from C# data when Codex is involved.
- Prefer placeholder art and generated assets until the loop is proven.

## Codex instructions
When asked to generate prompts for Codex:
- produce bounded prompts
- forbid feature creep
- require summaries of changed files
- require manual Unity steps to be listed
- require tests or smoke checks when practical

## Ship mindset
Bias toward a complete build with placeholder art, clear feedback, and stable restart flow over ambitious systems that are only partially working.
