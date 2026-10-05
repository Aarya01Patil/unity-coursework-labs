# Unity Coursework & Labs (`unity-coursework-labs`)

A structured collection of Unity projects, gameplay prototypes, lab exercises, and game jam submissions developed during the **Semester 6 Game Development** curriculum at **Scaler School of Technology (SST)**.

---

## 📁 Repository Structure

```text
unity-coursework-labs/
├── 01-intro-to-unity-basics/       # Lab 1: Getting started, basic C# scripting & 2D physics
├── 02-movement-and-camera/         # Lab 2: 2D player controller, smooth camera follow & scene transitions
├── 03-template-sandbox/            # URP 2D baseline template & experimental sandbox
├── 04-color-switch-2d/             # Complete 2D Color Switch mechanics & obstacle loops
├── 05-floating-damage-numbers/     # Gameplay mechanics: Dynamic floating combat text system
├── 06-soul-sucker-gamejam/         # Soul Sucker: Complete 8-level puzzle-platformer Game Jam project
├── builds/
│   └── soul-sucker-webgl/          # Playable WebGL web release of Soul Sucker
└── .gitignore                      # Universal Unity cache & build exclusion rules
```

---

## 🎮 Project Summaries

### [01-intro-to-unity-basics](./01-intro-to-unity-basics)
* **Focus:** Introduction to the Unity 2D environment, lifecycle methods, and Rigidbody2D physics.
* **Key Scripts:**
  - `HelloWorld.cs`: Component lifecycle and debug logging.
  - `BallMovement.cs`: 2D physics impulse and directional movement.
* **Target Unity Version:** Unity 6.3 LTS (URP 2D)

---

### [02-movement-and-camera](./02-movement-and-camera)
* **Focus:** 2D character controller, kinematic/dynamic motion, camera tracking, and level transitions.
* **Key Scripts & Scenes:**
  - `PlayerMove.cs` / `BallController.cs`: Input-based horizontal and vertical motion.
  - `CameraFollow.cs` / `FollowCamera.cs`: Smooth interpolated camera follow system.
  - `Scenes/Level1.unity`, `Scenes/Level2.unity`: Multi-stage level progression.

---

### [03-template-sandbox](./03-template-sandbox)
* **Focus:** Clean Universal Render Pipeline (URP) 2D project baseline configured for prototyping and experimentation.

---

### [04-color-switch-2d](./04-color-switch-2d)
* **Focus:** Mechanics recreation inspired by *Color Switch*.
* **Features:**
  - Obstacle rotation with dynamic timing (`Rotate.cs`).
  - Color-matching collision gates and color changer orbs (`ColorChangerOrb.cs`).
  - Single-tap jump physics and boundary detection (`PlayerMove.cs`).
  - Dynamic UI and text effects via TextMesh Pro.

---

### [05-floating-damage-numbers](./05-floating-damage-numbers)
* **Focus:** Reusable gameplay mechanics module integrated with the *Unity Mechanics Framework*.
* **Features:**
  - Dynamic floating damage and healing numbers spawned on hit events.
  - Smooth vertical float, fade-out animations, and pooled text instances.
  - Interactive test demo scene (`DemoScene.unity`, `FloatingDamageNumbersDemoBootstrap_UMFOSS.cs`).

---

### [06-soul-sucker-gamejam](./06-soul-sucker-gamejam)
* **Focus:** A full-featured 2D puzzle-platformer created for the SST Game Jam.
* **Premise:** You play as a fragile parasite with limited independent survival time. You must possess different host bodies, utilize their unique abilities to solve puzzle rooms, and reach the exit door.
* **Mechanics:**
  - **5 Host Types:**
    - `Jumper`: High vertical jump burst.
    - `Dasher`: Rapid horizontal dash through hazards.
    - `Shooter`: Fires ranged projectiles to trigger remote switches and break barriers.
    - `Phase`: Temporarily phase through ethereal laser barriers.
    - `Anchor`: Heavy weight to depress pressure plates and move solid blocks.
  - **Progression:** 8 hand-crafted puzzle levels, persistent HUD status card, and audio management.
  - **Entry Scene:** `Assets/Scenes/Bootstrap.unity` (Flows: Bootstrap -> MainMenu -> Gameplay -> Victory).

---

### 🌐 Playable WebGL Build: Soul Sucker
A pre-built, ready-to-run WebGL version of **Soul Sucker** is located in [`builds/soul-sucker-webgl/`](./builds/soul-sucker-webgl/).
* Can be deployed directly to **GitHub Pages** or **Itch.io**.
* Launch locally using any local web server (e.g., Python: `python -m http.server 8000`).

---

## 🛠️ How to Open Any Project in Unity

1. Open **Unity Hub**.
2. Click **Add** -> **Add project from disk**.
3. Select any of the numbered subfolders (e.g., `06-soul-sucker-gamejam` or `04-color-switch-2d`).
4. Select Unity Editor version **Unity 6.3 LTS** (or compatible 6.x release).
5. Unity will automatically download the necessary package manifests and regenerate the local `Library/` cache.

---

## 📜 License
Developed for educational coursework and game jam submission at Scaler School of Technology.
