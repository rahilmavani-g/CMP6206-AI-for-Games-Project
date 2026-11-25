# CMP6206 AI for Games Project - Stalker

## 1. Project name
**Stalker** (working title). A small Unity stealth level where two AI agents play against each other: an alien that hunts and a survivor that tries to escape. Inspired by *Alien: Isolation*.

## 2. Description
- **Alien (enemy):** hunts the survivor using only its own senses. Built with a **finite state machine** (Patrol, Chase, Search). Planned: Investigate (hearing) using a stack-based FSM.
- **Survivor (player AI):** tries to reach the exit using only its own senses. Will be built with a **behaviour tree** (Flee, Hide, Escape, Get Keycard). Has a Human/AI toggle so I can control it for testing.
- **Director:** tracks a "menace" value and sends the alien into the vents when the pressure gets too high.

**AI techniques:**
- **Sensors:** vision cone using distance -> angle -> Linecast with LayerMask (cheapest check first).
- **Memory:** last known position of the player, so the alien is a model-based agent, not a simple reflex agent.
- **FSM:** enum + switch with Patrol, Chase and Search, a search timer and patrol points.
- **Grid pathfinding with A*:** the level is turned into a grid using `Physics.CheckSphere` on a Walls layer. A* uses a Euclidean heuristic because the alien can move diagonally.
- **Path improvements:** agent radius padding so paths keep away from walls, and SphereCast smoothing so the alien walks straight lines instead of a staircase.
- **Animation:** Animator with a blend tree (idle/walk/run) driven by the FSM state.

## 3. Target game engine
Unity (3D, C#) with the Unity `.gitignore` from https://github.com/github/gitignore.

## 4. How to run
1. Open the project in Unity 6 and open `Assets/Scenes/GameScene`.
2. Press Play. Move the player with WASD.
3. Select the Alien or NavGrid in the Scene view to see the debug gizmos: view cone (yellow = can't see you, red = can see you), grid (green = walkable, red = blocked) and the A* path (black).

## 5. Research
In *Alien: Isolation*, a director always knows where the player is but only gives the alien hints. The alien has to find the player with its own senses, and retreats to the vents when a "menace" gauge peaks (Thompson, 2017). The real game uses a behaviour tree, which is why my second agent uses one: I can compare an FSM (alien) and a behaviour tree (survivor) in the same project.

**Why A\*:** BFS ignores step costs and UCS searches in every direction. A* expands the node with the lowest f = g + h, so it is still optimal (because straight-line distance never overestimates, so it is admissible) but checks far fewer nodes because it is aimed at the goal.

**How I'll measure success:** nodes expanded by BFS vs UCS vs A* on the same routes, how often the survivor escapes over many AI vs AI rounds, and testing in different levels.

## 6. Methodology / planning
I use a Kanban board (To Do / In Progress / Done). Dated images of the board are in the `Docs` folder. I build one feature at a time and test it before moving on.

1. Scene, WASD player, alien moves to player ✅
2. Alien vision (view cone + line of sight) ✅
3. Alien FSM (Patrol, Chase, Search) ✅
4. Grid pathfinding (A*, agent radius, path smoothing) ✅
5. Level (sci-fi kit), alien model and animations ✅
6. Behaviour tree framework + unit tests
7. Survivor AI using a behaviour tree: keycard, exit, lockers, flee 
8. Alien hearing + stack FSM (Investigate)
9. Flocking swarm (boids)
10. Dynamic doors (path re-planning) + Director and vents
11. Playtesting and measurements (A* vs BFS/UCS, survivor escape rate)
12. Windows build, reflective video and write-up

**Quality assurance:** unit tests (Unity Test Framework), feature branches with pull requests for code review, and an `.editorconfig` for consistent code style.

## 7. Progress
**20/10/2026 - Scene and movement.** Added a test scene, player movement and an alien that moves towards the player using vectors. Had an Input System error and fixed it by setting Active Input Handling to Both. The alien cheated (always knew where the player was). Decided the player will also be an AI agent.

**24/10/2026 - Vision and FSM.** Added the vision cone (distance, angle, Linecast) with gizmos, then memory of the last known position. Built an FSM with Patrol, Chase and Search. Bug: the search timer was reset every frame so the alien searched forever; fixed by setting the timer once when entering the state. Learned the difference between "on enter" and "every frame" logic, which is a weakness of enum + switch FSMs.

**04/11/2026 - Grid.** Built a grid over the level, marking nodes blocked with `Physics.CheckSphere` on the Walls layer. Added `NodeFromWorldPoint` and `GetNeighbours`. Bug: a NullReferenceException because I used the grid before checking it existed; fixed by doing the null check first.

**11/11/2026 - A\* pathfinding.** Implemented A* with an open set, closed set and parent links to retrace the path. The alien now walks around walls. It hugged corners, so I added an agent radius to the grid, and it zig-zagged, so I added raycast smoothing (later a SphereCast so the alien's whole body fits, not just a thin line). Also fixed a crash when the path was empty.

**20/11/2026 - Level and alien model.** Replaced the grey box level with a modular sci-fi kit and checked the grid still reads it correctly. Replaced the capsule with a Mixamo character and set up an Animator blend tree driven by the FSM. Added separate patrol and chase speeds and smooth turning. A texture over 100 MB from an unused HDRP demo folder blocked my commit, so I removed the folder.

## 8. Feedback
# Milestone - 1
Tutor feedback:
1. Justify why I'm choosing the methods and strategies (e.g. are they optimal?).
2. Test the AI in different levels to find out if it's working.
3. Look for a suitable level (no need to worry about modelling).
4. Get it working first before broadening the approaches.

What I did about it:
1. I now explain my choices in section 5, e.g. why A* (optimal with an admissible heuristic, fewer nodes than UCS).
2. The grid is built from the level's colliders, so I tested it on a second, completely different level (the sci-fi kit) and it still worked without code changes.
3. I used a ready-made modular sci-fi kit instead of modelling.
4. I built one feature at a time (vision-> memory -> FSM -> grid -> A*) and only moved on once each one worked.

## 9. References
- Creative Assembly (2014) *Alien: Isolation*. [Video game]. Tokyo: SEGA.
- Thompson, T. (2017) *The Perfect Organism: The AI of Alien: Isolation*. Available at: https://www.gamedeveloper.com/design/the-perfect-organism-the-ai-of-alien-isolation (Accessed: 20 October 2026).
- Bevilacqua, F. (2013) *Finite-State Machines: Theory and Implementation*. Available at: https://code.tutsplus.com/finite-state-machines-theory-and-implementation--gamedev-11867t (Accessed: 20 October 2026).
- GeeksforGeeks (nd) *Uninformed Search Algorithms in AI*. Available at: https://www.geeksforgeeks.org/artificial-intelligence/uniformed-search-algorithms-in-ai/ (Accessed: 20 October 2026).
- Hart, P.E., Nilsson, N.J. and Raphael, B. (1968) 'A Formal Basis for the Heuristic Determination of Minimum Cost Paths', *IEEE Transactions on Systems Science and Cybernetics*, 4(2), pp. 100–107.
- Unity Technologies (2026) *Unity Scripting API*. Available at: https://docs.unity3d.com/ScriptReference/ (Accessed: 11 November 2026).
- CMP6206 lecture slides 1–8, Agents and Sensors lab and Maths for AI: Vectors lab (Birmingham City University, 2026).

**Assets**
- Creepy Cat (2026) *3D Scifi Kit Starter Kit*, Unity Asset Store. Available at: https://assetstore.unity.com/packages/3d/environments/3d-scifi-kit-starter-kit-92152 (Accessed: 20 November 2026).
- Adobe (nd) *Mixamo*: Maw J Laygo character and animations. Available at: https://www.mixamo.com (Accessed: 20 November 2026).

**Use of AI tools**
- Claude (Anthropic) was used as a tutor to explain lecture content and to review my code. It provided the starter player movement script in the first step; all later code was written by me, with Claude explaining concepts and checking my work.