---
name: unity-project-conventions
description: Enforce consistent Unity project structure, naming, folder layout, scene setup, prefab discipline, and code conventions for Codex or ChatGPT. Use when Codex needs to clean up a Unity repo, apply reusable project conventions, or follow an explicit project standard instead of inventing one ad hoc.
---

# Unity Project Conventions

Apply consistent conventions before adding features.

## Default conventions
- Use PascalCase for classes, enums, files, and public properties.
- Use camelCase for private serialized fields only when not prefixed with underscore.
- Keep MonoBehaviour scripts single-purpose.
- Keep scenes minimal and stable.
- Avoid hidden cross-scene references.

## Decision order
1. Preserve strong repo conventions if they already exist.
2. Standardize the highest-friction inconsistencies first.
3. Prefer additive cleanup over risky repo-wide refactors during MVP.
4. Document exceptions when the existing project intentionally differs.

## Folder layout
Use this default structure unless the repo already has a strong standard:
- Scenes/
- Scripts/Core/
- Scripts/Data/
- Scripts/Player/
- Scripts/World/
- Scripts/UI/
- Scripts/Audio/
- Scripts/Utilities/
- Prefabs/
- Materials/
- Art/
- Audio/
- Editor/
- Tests/

## Scene rules
- Use separate Bootstrap, MainMenu, and Gameplay scenes for small games when appropriate.
- Keep scene-specific setup shallow.
- Prefer runtime creation of simple objects when Codex is doing most of the work.
- Avoid large YAML-heavy scene edits if a script can do the setup more safely.

## Prefab rules
- Make prefabs for reusable gameplay objects only.
- Do not create prefabs for one-off decorative objects during MVP.
- Keep prefab variants minimal.

## Code rules
- Prefer explicit references over global find patterns.
- Avoid service locators and dependency injection containers in jam projects.
- Avoid over-abstracting before the second real use case exists.
- Add comments only where the intent is non-obvious.

## Refactor guardrails
- Do not move files just to satisfy a convention if it will break references late in development.
- Do not rename broadly without a clear payoff.
- Prefer conventions that make Codex edits safer and more predictable.

## Output pattern
When asked to improve repo hygiene, return:
1. current issues
2. target conventions
3. concrete file or folder changes
4. any risky refactors to avoid
