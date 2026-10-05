---
name: codex-task-breakdown-enforcer
description: Force Codex to break work into bounded phases, summarize file changes, list manual follow-up steps, and avoid scope creep. Use when Codex needs to structure a coding task into safer checkpoints, create implementation prompts with explicit limits, or keep a multi-step workflow reviewable and finishable.
---

# Codex Task Breakdown Enforcer

Make every task bounded and reviewable.

## Required behavior
When preparing a task for Codex:
- break the task into phases
- state the phase goal
- define what is explicitly out of scope
- require a checkpoint before the next phase
- require a summary of changed files
- require manual IDE or editor steps to be listed
- require a short risk note if changes are fragile

## Prompt-building order
1. State the outcome in one sentence.
2. List the concrete implementation tasks.
3. State hard constraints and scope limits.
4. Define the expected deliverables.
5. Define the reporting format after each phase.

## Phase design rules
- One phase should end in a compilable or runnable checkpoint whenever possible.
- Prefer 5 or fewer phases for medium-sized tasks.
- If a task is large, split into thin vertical slices rather than layers of abstraction.
- Never mix feature building, polish, and refactor work in the same phase unless necessary.
- Prefer changes that can be verified immediately over architectural cleanups.
- Stop at the requested deliverable even if more improvements look tempting.

## Anti-feature-creep language
Use phrases like:
- do not add extra features
- do not refactor unrelated systems
- keep implementation explicit and minimal
- stop after the requested deliverables

## Reporting requirements
Require Codex to report:
- what changed
- which files changed
- what was intentionally deferred
- any manual editor or Unity steps
- any tests, smoke checks, or verification gaps

## Output pattern
For prompts, use:
1. goal
2. tasks
3. constraints
4. deliverables
5. reporting requirements
