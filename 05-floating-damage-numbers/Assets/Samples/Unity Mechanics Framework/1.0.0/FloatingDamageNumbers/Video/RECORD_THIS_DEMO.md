# Floating Damage Numbers Demo Recording Checklist

Replace the temporary README links in this repo with your final `.mp4` or hosted video URL before opening the PR.

## Recording order

1. Start in Play Mode and show the finished effect first.
2. Open the hierarchy and point out that only the manager has `FloatingDamageNumbers_UMFOSS`.
3. Show Player, Enemy, and Chest only using `HealthSystem_UMFOSS`.
4. Trigger every button once and say which event and `NumberType` each button demonstrates.
5. Compare `Rapid Fire (5 x 10)` against `Enable Combine + Rapid Fire`.
6. Switch between `Smooth`, `Bounce`, and `Elastic` presets and call out why `LerpUnclamped` matters.
7. Show the on-screen pool usage label and then verify in the Profiler that no new popup objects are instantiated after prewarm.
8. Open `FloatingDamageNumbers_UMFOSS.cs` and explain:
   - `OnEnable` / `OnDisable`
   - pooled popup creation
   - world-space canvas auto-creation
   - random spread
   - combine buffer

## Talking points

- Zero per-entity setup for floating numbers
- EventBus-driven decoupling
- Object pooling avoids repeated allocations
- Numbers billboard toward the camera
- Random horizontal spread prevents overlap
- Combine mode is optional and off by default

## Submission reminder

- Add your actual video link in `README.md`
- Replace the placeholder author details in `README.md`
- Keep the final video between 3 and 8 minutes
