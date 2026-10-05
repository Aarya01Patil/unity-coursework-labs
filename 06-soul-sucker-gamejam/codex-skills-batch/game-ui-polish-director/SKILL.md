---
name: game-ui-polish-director
description: Improve Unity game UI, HUD, menu clarity, visual hierarchy, feedback, and aesthetic cohesion without changing gameplay scope. Use when Codex needs to polish a jam UI, reduce prototype feel, improve readability, or add lightweight visual juice to menus, HUD, prompts, overlays, or fail and win screens.
---

# Game UI Polish Director

Polish clarity first, style second.

## Priorities
1. readability
2. feedback
3. cohesion
4. theme
5. extra flair only if safe

## Review order
1. Check whether the player always knows the current objective or state.
2. Check whether important information is visible at a glance.
3. Check whether interactions and state changes receive feedback.
4. Check whether the screens feel visually related.
5. Check whether any effect adds clutter instead of clarity.

## Jam-safe UI rules
- Keep the number of screens low.
- Use one strong visual theme and repeat it.
- Make important information readable at a glance.
- Make contextual prompts brief and consistent.
- Avoid clutter, nested panels, and tiny text.

## Required checks
For every UI pass, evaluate:
- Can a new player understand the current objective?
- Can the player read health, timer, host state, or score instantly?
- Is the current interactable highlighted?
- Are fail and win states visually distinct?

## Common upgrades
- improve spacing and alignment
- strengthen title treatment
- add color coding for player states
- add clean feedback flashes or pulses
- improve button affordance
- unify fonts and panel styling
- make room transitions and overlays consistent

## Safe implementation bias
- Prefer layout, spacing, contrast, and typography fixes before new art.
- Prefer small animations, fades, and pulses over complex shader or particle systems.
- Prefer reusable panel and text treatments over one-off styling.

## Guardrails
- Do not add new gameplay systems.
- Do not hide key information in stylized art.
- Prefer lightweight effects over shader-heavy solutions.
- Use simple palettes with one accent color family for state changes.

## Output pattern
1. ui problems
2. target visual direction
3. concrete hierarchy changes
4. feedback improvements
5. implementation notes
