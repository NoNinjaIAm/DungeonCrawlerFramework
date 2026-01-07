# DungeonCrawlerFramework

A lightweight **Euclidean dungeon generator and mini-framework** for creating rectangular dungeons on the **X–Z plane**, with built-in **pathfinding via A\***.

This project focuses on simplicity, extensibility, and clarity—providing a solid foundation for dungeon-based games or procedural generation experiments without locking you into a heavy engine-side solution.

---

## ✨ Features

- **Euclidean dungeon generation**
  - Generates rectangular rooms and corridors aligned to the X and Z axes
  - Deterministic, grid-based layout that’s easy to reason about and debug

- **Mini-framework architecture**
  - Designed to be extended or embedded into other projects
  - Separates dungeon structure, generation logic, and traversal logic

- **A\* Pathfinding**
  - Automatically computes the shortest path from dungeon start to end
  - Useful for validation, AI navigation, or player guidance systems

- **Compass / Directional Guidance**
  - Provides directional feedback relative to the dungeon’s goal
  - Can be used for UI hints, minimaps, or debugging visualization

---

## 🧠 Design Goals

- Keep dungeon logic **engine-agnostic**
- Prioritize **readability over cleverness**
- Make procedural generation **predictable and testable**
- Serve as a **foundation**, not a full game system

This project is intentionally scoped to rectangular, grid-aligned dungeons—if you want chaotic cave systems, this probably isn’t the tool. If you want *clean structure and control*, it is.

---

## 🗺️ Dungeon Generation Overview

1. Generate a rectangular dungeon layout on a grid
2. Enter parameters for room rendering and dungeon layout
3. Run A\* to verify and compute the shortest valid path
4. Expose dungeon data for rendering, gameplay, or analysis

The result is a dungeon that is:
- Guaranteed to be navigable
- Easy to visualize
- Friendly to AI systems

---

## 🚀 Use Cases

- Dungeon crawler prototypes
- Procedural generation research
- AI navigation testing
- Game jams or rapid prototyping
- Teaching A\* and grid-based pathfinding concepts

---

## 📦 Extending the Framework

You can easily:
- Swap out generation rules
- Add room metadata (enemies, loot, traps)
- Replace or augment A\* with custom heuristics
- Hook the dungeon data into any renderer or engine

The framework doesn’t assume how your game *looks*—only how it *works*.

---

## 🛠️ Status

This project is actively usable as a foundation. No further development is being pursued.

---

## 📜 License

MIT (or whatever you’re using — update as needed)

