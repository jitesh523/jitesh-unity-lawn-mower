# Jitesh's  Unity Project — Autonomous Lawn Mower

My first Unity project: a simulation of an **autonomous lawn-mower / robot-vacuum** that plans its own route across a grid, avoids obstacles, and drives to a target using the **A\* (A-star) pathfinding algorithm**. The project was built on top of Unity's **2D Platformer Microgame** template, which provides the scene, art, audio, and gameplay framework the experiment lives in.

---

## What it does

- Generates a **20 × 20 grid** as the "lawn".
- Places **four 2×2 obstacle blocks** (e.g. flower beds, trees, rocks) at fixed cells: `(2,2)`, `(5,5)`, `(12,12)`, `(15,15)`.
- Spawns the mower robot at cell **`(3, 7)`**.
- Runs **A\*** to compute the shortest obstacle-free route to the target cell **`(18, 18)`**.
- Animates the robot along that path one cell every **0.1 s** using a coroutine, so you can watch it navigate.

## How the pathfinding works

The logic lives in [`Assets/Character/robotvaccume.cs`](Assets/Character/robotvaccume.cs) (class `RobotVacuum`):

| Piece | Details |
|---|---|
| `GenerateGrid()` | Instantiates `obstaclePrefab` at each obstacle cell and `robotPrefab` at the start cell. |
| `AStar(start, goal)` | Classic A\*: keeps an open set sorted by `f = g + h`, tracks `cameFrom`, `gScore`, `fScore`, and rebuilds the path once the goal is reached. Returns an empty list if no route exists. |
| `Heuristic(a, b)` | **Manhattan distance** — admissible for 4-directional movement, so the path is guaranteed optimal. |
| `GetNeighbors()` | Up / down / left / right moves (no diagonals), filtered by `IsValidPosition()`. |
| `IsValidPosition()` | Rejects cells outside the grid or occupied by an obstacle. |
| `MoveRobot()` | Coroutine that steps the robot's transform through the path. |

Each move costs 1, so the result is the fewest-steps route around the obstacles.

## Built with

- **Unity 2022.3.36f1 (LTS)**
- Universal Render Pipeline (URP) 14
- Cinemachine, TextMesh Pro, Timeline, Visual Scripting
- Unity **2D Platformer Microgame** template (player controller, enemies, tokens, tilemaps, audio, simulation/event core)

## Project structure

```
Assets/
├── Character/          # robotvaccume.cs (A* mower), Cube prefab, character sprites/animations
├── Scripts/            # Platformer Microgame framework
│   ├── Core/           #   Simulation event queue, HeapQueue, fuzzy helpers
│   ├── Gameplay/       #   Events: spawn, jump, land, death, victory, token pickup
│   ├── Mechanics/      #   PlayerController, KinematicObject, PatrolPath, Health, zones
│   ├── Model/          #   PlatformerModel (shared game config)
│   ├── UI/             #   Main menu / meta-game controllers
│   └── View/           #   Parallax layers, animated tiles
├── Scenes/             # SampleScene.unity
├── Environment/, Tiles/, Mod Assets/   # Art, tiles, props, particle & powerup prefabs
├── Audio/              # Music and SFX
├── Tutorials/, Documentation/          # Template tutorials and user guide PDF
Packages/               # manifest.json / packages-lock.json
ProjectSettings/        # Unity project settings
```

## Getting started

1. Install **Unity Hub** and **Unity 2022.3.36f1** (any 2022.3 LTS should work).
2. Clone the repo:
   ```bash
   git clone https://github.com/jitesh523/jitesh-unity-lawn-mower.git
   ```
3. In Unity Hub choose **Add → Add project from disk** and select the cloned folder. The first open regenerates the `Library/` folder, so it takes a few minutes.
4. Open `Assets/Scenes/SampleScene.unity`.
5. To run the mower:
   - Create an empty GameObject and add the **RobotVacuum** component.
   - Assign an **Obstacle Prefab** (e.g. `Assets/Character/Cube.prefab`) and a **Robot Prefab**.
   - Press **Play** and watch the robot path to `(18, 18)`.

> **Note:** Unity expects a MonoBehaviour's file name to match its class name. If Unity says it can't find the script class, rename `robotvaccume.cs` to `RobotVacuum.cs`.

## Ideas for next steps

- Full **coverage path planning** (boustrophedon / spiral) so the mower cuts the whole lawn, not just one route
- Random or user-placed obstacles and a live grid visualisation
- Diagonal moves, weighted terrain, and a priority-queue open set for speed
- Battery / return-to-dock behaviour

## Credits

Built by **Jitesh** as a first Unity project. Art, audio, and the platformer framework come from Unity Technologies' Platformer Microgame template — see `Assets/ThirdPartyNotice.txt`.
