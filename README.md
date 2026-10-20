# CMP6206 AI for Games Project – Stalker

## 1. Project name
**Stalker** (working title). A small Unity stealth level where two AI agents play against each other: an alien that hunts and a survivor that tries to escape. Inspired by *Alien: Isolation*.

## 2. Description
- **Alien (enemy):** finds the survivor using only sight and hearing. States: Roam, Investigate, Chase, Search, Attack, Retreat.
- **Survivor (player AI):** tries to reach the exit using only its own senses. States: GoToObjective, Sneak, Hide, Flee, Distract. Has a Human/AI toggle so I can control it for testing.
- **Director:** tracks a "menace" value and sends the alien into the vents when the pressure gets too high.

**AI techniques:**
- **Sensors:** distance + view cone + Linecast with LayerMask for vision, noise radius for hearing.
- **Stack-based FSMs** (states as classes) so interruptions can resume the previous state.
- **Grid pathfinding:** BFS, DFS and UCS compared, UCS used in game. A* later.

## 3. Target game engine
Unity (3D, C#) with the Unity `.gitignore` from https://github.com/github/gitignore.

## 4. Research
In *Alien: Isolation*, a director always knows where the player is but only gives the alien hints. The alien has to find the player with its own senses, and retreats to the vents when a "menace" gauge peaks (Thompson, 2017). The real game uses a behaviour tree. I'm starting with a stack FSM and may compare with a behaviour tree later.

I'll measure success with debug gizmos/state display, BFS vs DFS vs UCS nodes expanded, and how often the survivor escapes over many AI vs AI rounds.

## 5. Methodology / planning
Built step by step, tracked on a KANBAN board (Backlog / Up next / Doing / Done). Dated images of the board are in the `Docs` folder of this project.
1. Scene, WASD player, alien moves to player ✅
2. Alien vision
3. Alien FSM (Roam, Chase, Search)
4. Hearing + stack FSM
5. Grid pathfinding (BFS/DFS/UCS)
6. Survivor AI
7. Director + vents
8. Level, playtesting, write-up

## 6. Progress
**20/10/2026 – Step 1 done.** Added a test scene, player movement and an alien that moves towards the player using vectors. Had an Input System error and fixed it by setting Active Input Handling to Both. The alien currently cheats (always knows where the player is), so next is giving it vision. Decided the player will also be an AI agent.

## 7. References
- CMP6206 Lectures 1–4, Agents and Sensors lab, Vectors lab
- Thompson, T. (2017) *The Perfect Organism: The AI of Alien: Isolation*. Game Developer. https://www.gamedeveloper.com/design/the-perfect-organism-the-ai-of-alien-isolation
- Bevilacqua, F. *Finite-State Machines: Theory and Implementation*. Envato Tuts+. https://code.tutsplus.com/finite-state-machines-theory-and-implementation--gamedev-11867t
- GeeksforGeeks. *Uninformed Search Algorithms in AI*. https://www.geeksforgeeks.org/artificial-intelligence/uniformed-search-algorithms-in-ai/
- Creative Assembly (2014) *Alien: Isolation*. SEGA.
- Claude (Anthropic) – used to explain lecture content.
