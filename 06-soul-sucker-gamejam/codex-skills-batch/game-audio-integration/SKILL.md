---
name: game-audio-integration
description: Integrate music and sound effects into Unity games with clean scene transitions, looping, volume balance, and lightweight audio architecture. Use when Codex needs to wire in theme songs, SFX, simple audio managers, import settings, or jam-safe audio routing without expanding gameplay scope.
---

# Game Audio Integration

Focus on clean playback and mix discipline.

## Default goals
- one title theme
- one gameplay loop
- essential sound effects only
- stable transitions across scenes

## Integration order
1. Identify which scenes or states need music.
2. Identify the minimum SFX set that supports readability.
3. Define the smallest audio architecture that can route those clips.
4. Configure import settings and looping behavior.
5. Verify transitions, duplicates, and volume balance.

## Architecture bias
- Prefer a lightweight AudioManager or AudioRouter.
- Keep music and sfx on separate channels or audio sources.
- Expose default volume multipliers in the inspector.
- Use safe looping and avoid restarting music unnecessarily.
- Prefer explicit scene hooks over overbuilt event buses.

## Minimum SFX list
- confirm or click
- jump or movement accent if relevant
- primary action
- fail or death
- win or completion
- transition or possess effect if relevant

## Music rules
- Use seamless loops when possible.
- Keep music below critical game feedback.
- Use one consistent theme family instead of many unrelated tracks.
- Avoid overproduced systems for jam projects.

## If no audio asset exists yet
Do not pretend to generate a finished song from thin air. Instead:
1. specify the needed track briefs
2. define import settings
3. prepare the Unity integration points
4. leave clear placeholders for clips

## Validation checks
- music should not restart on every scene load unless intentional
- critical gameplay feedback should stay audible over music
- looping clips should not click or gap if avoidable
- missing clips should fail safely with obvious placeholders or null-safe guards

## Output pattern
1. audio architecture
2. clip list
3. scene routing behavior
4. mix defaults
5. inspector setup notes
