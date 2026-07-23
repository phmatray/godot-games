![dragonfall banner](.github/banner.png)

# dragonfall — Procedural dungeon generation for Godot

<!-- portfolio-badges:start -->
<!-- Identity -->
[![phmatray - dragonfall](https://img.shields.io/static/v1?label=phmatray&message=dragonfall&color=blue&logo=github)](https://github.com/phmatray/dragonfall)
![Top language](https://img.shields.io/github/languages/top/phmatray/dragonfall)
[![Stars](https://img.shields.io/github/stars/phmatray/dragonfall?style=social)](https://github.com/phmatray/dragonfall/stargazers)
[![Forks](https://img.shields.io/github/forks/phmatray/dragonfall?style=social)](https://github.com/phmatray/dragonfall/network/members)

<!-- Activity -->
[![Issues](https://img.shields.io/github/issues/phmatray/dragonfall)](https://github.com/phmatray/dragonfall/issues)
[![Pull requests](https://img.shields.io/github/issues-pr/phmatray/dragonfall)](https://github.com/phmatray/dragonfall/pulls)
[![Last commit](https://img.shields.io/github/last-commit/phmatray/dragonfall)](https://github.com/phmatray/dragonfall/commits)
<!-- portfolio-badges:end -->

<!-- portfolio-toc:start -->

## Table of Contents

- [✨ Features](#-features)
- [📦 Installation](#-installation)
- [🚀 Quick Start](#-quick-start)
- [Tech Stack](#tech-stack)
- [📄 License](#-license)
- [Contributing](#contributing)

<!-- portfolio-toc:end -->



A procedural dungeon generation engine for Godot games, implementing BSP (Binary Space Partitioning) tree algorithms to create randomized dungeons with rooms, corridors, and themed environments.

## ✨ Features
- BSP tree dungeon generation algorithm
- Corridor carving between rooms
- Multiple dungeon themes and biomes
- Seeded generation for reproducible dungeons
- Fully unit-tested generation pipeline
- Godot C# integration

## 📦 Installation
```bash
git clone https://github.com/phmatray/dragonfall
cd dragonfall
dotnet test Dragonfall.Tests
# Import into Godot as a C# project
```

## 🚀 Quick Start
```csharp
var generator = new DungeonGenerator(seed: 42);
var dungeon = generator.Generate(width: 80, height: 50);
// Dungeon contains rooms, corridors, and themed areas
foreach (var room in dungeon.Rooms)
    Console.WriteLine($"Room at {room.X},{room.Y}");
```

<!-- portfolio-techstack:start -->

## Tech Stack

- **.NET 10**
- GdUnit4
- GdUnit4.Api

<!-- portfolio-techstack:end -->

## 📄 License
MIT — see LICENSE

---

<!-- portfolio-sections:start -->

## Contributing

Contributions are welcome. Open an issue first to discuss any significant change.

1. Fork the repository and create your branch (`git checkout -b feat/my-feature`)
2. Commit your changes (`git commit -m 'feat: ...'`)
3. Push the branch and open a Pull Request

<!-- portfolio-sections:end -->
