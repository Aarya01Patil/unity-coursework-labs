---
name: unity-playtest-and-cut-list
description: Review Unity game builds, gameplay videos, playtest notes, or design plans for confusion, scope risk, weak feedback, and non-essential features, then produce a jam-focused cut list and next polish priorities. Use when Codex needs to decide what to cut versus polish before shipping a small Unity game, especially for game jams, student projects, MVPs, or deadline-driven builds.
---

# Unity Playtest And Cut List

Treat cuts as a feature. Optimize for a smaller, clearer game that ships.

## Gather the available evidence

Work from whatever the user provides:
- playable build
- gameplay recording or screenshots
- design brief or pitch
- level list or room flow
- control list
- playtest notes
- bug list

Do not block on missing artifacts. Use what exists, state assumptions briefly, and keep moving.

## Review in strict order
1. Confirm whether the goal and main input are understandable quickly.
2. Confirm whether the core loop is playable end to end.
3. Confirm whether fail, restart, and win states are clear and fair.
4. Confirm whether the game reaches its interesting mechanic early enough.
5. Confirm whether the player is likely to want one more try.
6. Identify everything that does not directly strengthen clarity, pacing, or completion.

## Jam-first cut rules

Cut first when a feature:
- is not required for the core loop to function
- exists only to add flavor, lore, or wrapper text
- duplicates another mechanic or screen
- appears late and is not properly taught
- creates bug risk or tuning overhead
- makes the game harder to read without increasing fun
- is only impressive in theory, not in the current build

Prefer removing one shaky system completely over keeping a broken version alive.

## Cut-first categories
- duplicate mechanics
- low-value story wrappers
- unused UI screens
- decorative systems that do not improve readability
- brittle AI or simulation layers
- any mechanic introduced late and not properly taught

## Polish-only-if-ready rule
Recommend polish only when all of these are true:
- the game is fully playable end to end
- restart flow works
- win and lose conditions are clear
- players can understand controls with minimal explanation

Keep polish recommendations cheap and high impact:
- clearer onboarding
- stronger feedback on success and failure
- faster restart
- better readability of hazards, goals, and interactables
- juice on the core action, not side systems

## Output pattern
Return exactly these sections:
1. what works
2. what confuses players
3. cut now
4. polish next
5. final ship risk

In `cut now`, rank the highest-value removals first and explain what each cut buys back.
In `polish next`, recommend only the smallest changes that improve understanding, fairness, or replay value.
In `final ship risk`, give a direct release judgment such as `safe to ship`, `ship with caution`, or `not ready`.

## Fast reference

Load [references/guide.md](references/guide.md) when a quick jam checklist is enough and a full review is unnecessary.

## Tone
Be direct. Prefer one strong recommendation over a long vague list.
